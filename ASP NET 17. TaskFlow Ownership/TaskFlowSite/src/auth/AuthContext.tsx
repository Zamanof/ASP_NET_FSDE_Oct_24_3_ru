import React, { createContext, useCallback, useContext, useEffect, useState } from 'react';
import * as authApi from '../api/auth';
import { setAuthTokens, clearAuthTokens, setOnTokensUpdated } from '../api/client';
import type { AuthResponse } from '../types/api';

const STORAGE_KEY = 'taskflow_auth';

interface StoredAuth {
  token: string;
  refreshToken: string;
  email: string;
  roles: string[];
}

interface User {
  email: string;
  roles: string[];
}

interface AuthContextValue {
  user: User | null;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<{ success: boolean; error?: string }>;
  register: (payload: {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
  }) => Promise<{ success: boolean; error?: string }>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

function loadStored(): StoredAuth | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const data = JSON.parse(raw) as StoredAuth;
    if (data.token && data.refreshToken && data.email) return data;
  } catch {
    /* ignore */
  }
  return null;
}

function saveStored(data: StoredAuth | null) {
  if (data) localStorage.setItem(STORAGE_KEY, JSON.stringify(data));
  else localStorage.removeItem(STORAGE_KEY);
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const applyAuth = useCallback((data: AuthResponse) => {
    // AuthResponse приходит с полем accessToken, а не token
    setAuthTokens(data.accessToken, data.refreshToken);
    setUser({ email: data.email, roles: [...(data.roles ?? [])] });
    saveStored({
      token: data.accessToken,
      refreshToken: data.refreshToken,
      email: data.email,
      roles: [...(data.roles ?? [])],
    });
  }, []);

  useEffect(() => {
    setOnTokensUpdated((tokens) => {
      const stored = loadStored();
      if (stored) {
        saveStored({
          ...stored,
          token: tokens.token,
          refreshToken: tokens.refreshToken,
        });
      }
    });
    return () => setOnTokensUpdated(null);
  }, []);

  useEffect(() => {
    const stored = loadStored();
    if (!stored) {
      setIsLoading(false);
      return;
    }
    setAuthTokens(stored.token, stored.refreshToken);
    setUser({ email: stored.email, roles: stored.roles ?? [] });
    setIsLoading(false);
  }, []);

  const login = useCallback(
    async (email: string, password: string) => {
      const result = await authApi.login(email, password);
      if (result.success && result.data) {
        applyAuth(result.data);
        return { success: true };
      }
      return { success: false, error: result.error };
    },
    [applyAuth]
  );

  const register = useCallback(
    async (payload: {
      firstName: string;
      lastName: string;
      email: string;
      password: string;
      confirmPassword: string;
    }) => {
      const result = await authApi.register(payload);
      if (result.success && result.data) {
        applyAuth(result.data);
        return { success: true };
      }
      return { success: false, error: result.error };
    },
    [applyAuth]
  );

  const logout = useCallback(async () => {
    const stored = loadStored();
    if (stored?.refreshToken) await authApi.revokeRefresh(stored.refreshToken);
    clearAuthTokens();
    saveStored(null);
    setUser(null);
  }, []);

  const value: AuthContextValue = { user, isLoading, login, register, logout };
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
