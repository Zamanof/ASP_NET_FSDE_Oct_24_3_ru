export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  executionTimeMs?: number;
}

export interface AuthResponse {
  token: string;
  accessToken?: string;
  expiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
  email: string;
  roles: string[];
}

export interface Project {
  id: number;
  name: string;
  description?: string;
  taskCount: number;
  ownerId?: string;
}

export interface UserWithRoles {
  id: string;
  email: string;
  firstName?: string;
  lastName?: string;
  roles: string[];
}

export interface ProjectMemberResponse {
  userId: string;
  email: string;
  firstName?: string;
  lastName?: string;
  joinedAt: string;
}

export interface AvailableUser {
  id: string;
  email: string;
  firstName?: string;
  lastName?: string;
}

export type TaskStatus = 'ToDo' | 'InProgress' | 'Done';
export type TaskPriority = 'Low' | 'Medium' | 'High';

export interface TaskItem {
  id: number;
  title: string;
  description?: string;
  status: TaskStatus;
  priority: TaskPriority;
  projectId: number;
  projectName: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
