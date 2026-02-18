import type { ApiResponse } from '../types/api';

const API_BASE = import.meta.env.VITE_API_URL ?? 'http://localhost:5034';

let accessToken: string | null = null;
let refreshToken: string | null = null;
let onTokensUpdated: ((tokens: { token: string; refreshToken: string }) => void) | null = null;

export function setAuthTokens(token: string, refresh: string) {
  accessToken = token;
  refreshToken = refresh;
}

export function clearAuthTokens() {
  accessToken = null;
  refreshToken = null;
}

export function setOnTokensUpdated(cb: ((tokens: { token: string; refreshToken: string }) => void) | null) {
  onTokensUpdated = cb;
}

export function getAccessToken() {
  return accessToken;
}

async function refreshAccessToken(): Promise<boolean> {
  if (!refreshToken) return false;
  try {
    const res = await fetch(`${API_BASE}/api/auth/refresh`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken }),
    });
    if (!res.ok) return false;
    const json = (await res.json()) as ApiResponse<{
      accessToken: string;
      refreshToken: string;
      expiresAt: string;
      refreshTokenExpiresAt: string;
      email: string;
      roles: string[];
    }>;
    if (!json.success || !json.data) return false;
    accessToken = json.data.accessToken;
    refreshToken = json.data.refreshToken;
    onTokensUpdated?.({ token: json.data.accessToken, refreshToken: json.data.refreshToken });
    return true;
  } catch {
    return false;
  }
}

export async function apiFetch<T>(
  path: string,
  options: RequestInit = {}
): Promise<{ ok: boolean; status: number; data?: ApiResponse<T>; problem?: ProblemDetails }> {
  const url = path.startsWith('http') ? path : `${API_BASE}${path}`;
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    ...options.headers,
  };
  if (accessToken) (headers as Record<string, string>)['Authorization'] = `Bearer ${accessToken}`;

  let res = await fetch(url, { ...options, headers });

  if (res.status === 401 && refreshToken) {
    const refreshed = await refreshAccessToken();
    if (refreshed && accessToken) {
      (headers as Record<string, string>)['Authorization'] = `Bearer ${accessToken}`;
      res = await fetch(url, { ...options, headers });
    }
  }

  if (res.status === 204) {
    return { ok: true, status: 204 };
  }

  const contentType = res.headers.get('content-type');
  const isJson = contentType?.includes('application/json') || contentType?.includes('application/problem+json');

  if (!isJson) {
    return { ok: res.ok, status: res.status };
  }

  const json = await res.json();

  if (res.ok) {
    return { ok: true, status: res.status, data: json as ApiResponse<T> };
  }

  return {
    ok: false,
    status: res.status,
    problem: json as ProblemDetails,
  };
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}
