import { apiFetch } from './client';
import type { UserWithRoles } from '../types/api';

export async function getUsers() {
  const res = await apiFetch<UserWithRoles[]>('/api/UserRoles');
  if (!res.ok || !res.data?.data) return { success: false, data: [] as UserWithRoles[], status: res.status };
  return { success: true, data: res.data.data, status: res.status };
}

export async function getUserRoles(userId: string) {
  const res = await apiFetch<UserWithRoles>(`/api/UserRoles/${encodeURIComponent(userId)}/roles`);
  if (!res.ok || !res.data?.data) return { success: false, data: null, status: res.status, error: res.data?.message };
  return { success: true, data: res.data.data, status: res.status };
}

export async function assignRole(userId: string, role: string) {
  const res = await apiFetch<UserWithRoles>(`/api/UserRoles/${encodeURIComponent(userId)}/roles`, {
    method: 'POST',
    body: JSON.stringify({ role }),
  });
  if (!res.ok)
    return { success: false, error: res.data?.message ?? res.problem?.detail, status: res.status };
  return { success: true, data: res.data?.data, status: res.status };
}

export async function removeRole(userId: string, roleName: string) {
  const res = await apiFetch<UserWithRoles>(`/api/UserRoles/${encodeURIComponent(userId)}/roles/${encodeURIComponent(roleName)}`, {
    method: 'DELETE',
  });
  if (!res.ok)
    return { success: false, error: res.data?.message ?? res.problem?.detail, status: res.status };
  return { success: true, data: res.data?.data, status: res.status };
}
