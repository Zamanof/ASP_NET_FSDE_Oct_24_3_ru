export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  executionTimeMs?: number;
}

export interface AuthResponse {
  accessToken: string;
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
  createdAt: string;
  updatedAt?: string;
  tasksCount: number;
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
  createdAt: string;
  updatedAt?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
