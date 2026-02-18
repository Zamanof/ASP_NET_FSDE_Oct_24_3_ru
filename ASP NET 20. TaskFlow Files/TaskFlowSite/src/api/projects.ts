import { apiFetch } from './client';
import type { Project, ProjectMemberResponse, AvailableUser } from '../types/api';

export async function getProjects() {
  const res = await apiFetch<Project[]>('/api/projects');
  if (!res.ok || !res.data?.data) return { success: false, data: [] as Project[] };
  return { success: true, data: res.data.data };
}

export async function getProject(id: number) {
  const res = await apiFetch<Project>(`/api/projects/${id}`);
  if (!res.ok || !res.data?.data) return { success: false, data: null };
  return { success: true, data: res.data.data };
}

export async function createProject(name: string, description?: string) {
  const res = await apiFetch<Project>('/api/projects', {
    method: 'POST',
    body: JSON.stringify({ name, description: description ?? '' }),
  });
  if (!res.ok || !res.data?.data)
    return { success: false, error: res.problem?.detail ?? res.data?.message, status: res.status };
  return { success: true, data: res.data.data };
}

export async function updateProject(id: number, name: string, description?: string) {
  const res = await apiFetch<Project>(`/api/projects/${id}`, {
    method: 'PUT',
    body: JSON.stringify({ name, description: description ?? '' }),
  });
  if (!res.ok || !res.data?.data)
    return { success: false, error: res.problem?.detail ?? res.data?.message, status: res.status };
  return { success: true, data: res.data.data };
}

export async function deleteProject(id: number) {
  const res = await apiFetch(`/api/projects/${id}`, { method: 'DELETE' });
  return { ok: res.ok, status: res.status };
}

export async function getProjectMembers(projectId: number) {
  const res = await apiFetch<ProjectMemberResponse[]>(`/api/projects/${projectId}/members`);
  if (!res.ok || !res.data?.data) return { success: false, data: [] as ProjectMemberResponse[], status: res.status };
  return { success: true, data: res.data.data, status: res.status };
}

export async function getAvailableUsersToAdd(projectId: number) {
  const res = await apiFetch<AvailableUser[]>(`/api/projects/${projectId}/available-users`);
  if (!res.ok || !res.data?.data) return { success: false, data: [] as AvailableUser[], status: res.status };
  return { success: true, data: res.data.data, status: res.status };
}

export async function addProjectMember(projectId: number, emailOrUserId: string) {
  const res = await apiFetch(`/api/projects/${projectId}/members`, {
    method: 'POST',
    body: JSON.stringify(emailOrUserId.includes('@') ? { email: emailOrUserId } : { userId: emailOrUserId }),
  });
  if (!res.ok) return { success: false, error: res.data?.message ?? res.problem?.detail, status: res.status };
  return { success: true, status: res.status };
}

export async function removeProjectMember(projectId: number, userId: string) {
  const res = await apiFetch(`/api/projects/${projectId}/members/${encodeURIComponent(userId)}`, {
    method: 'DELETE',
  });
  return { ok: res.ok, status: res.status };
}
