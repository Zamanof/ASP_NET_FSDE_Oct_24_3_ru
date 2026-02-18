import { useCallback, useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  DndContext,
  DragEndEvent,
  DragOverlay,
  DragStartEvent,
  PointerSensor,
  useSensor,
  useSensors,
} from '@dnd-kit/core';
import { useDraggable } from '@dnd-kit/core';
import { useDroppable } from '@dnd-kit/core';
import { getProject, getProjectMembers, getAvailableUsersToAdd, addProjectMember, removeProjectMember } from '../api/projects';
import { getTasksByProject, createTask, updateTask, updateTaskStatus, deleteTask } from '../api/tasks';
import { useAuth } from '../auth/AuthContext';
import { useToast } from '../context/ToastContext';
import type { Project } from '../types/api';
import type { TaskItem, TaskPriority, TaskStatus } from '../types/api';
import type { ProjectMemberResponse, AvailableUser } from '../types/api';

const FORBIDDEN_TASKS = 'Creating and editing tasks is available only to Managers and Admins.';

const COLUMNS: TaskStatus[] = ['ToDo', 'InProgress', 'Done'];
const PRIORITIES: TaskPriority[] = ['Low', 'Medium', 'High'];

const STATUS_TO_ENUM: Record<TaskStatus, number> = {
  ToDo: 0,
  InProgress: 1,
  Done: 2,
};
const PRIORITY_TO_ENUM: Record<TaskPriority, number> = {
  Low: 0,
  Medium: 1,
  High: 2,
};

function statusForApi(status: TaskStatus): number {
  return STATUS_TO_ENUM[status];
}

function priorityForApi(priority: TaskPriority): number {
  return PRIORITY_TO_ENUM[priority];
}

function columnLabel(col: string) {
  if (col === 'ToDo') return 'To do';
  if (col === 'InProgress') return 'In progress';
  return 'Done';
}

function priorityClass(p: string) {
  if (p === 'High') return 'text-red-600';
  if (p === 'Medium') return 'text-amber-600';
  return 'text-[#5e6c84]';
}

function TaskCardPreview({ task }: { task: TaskItem }) {
  return (
    <div className="p-3 bg-white rounded shadow-lg cursor-grabbing rotate-2 scale-105">
      <div className="font-medium text-sm mb-1">{task.title}</div>
      <div className={`text-xs ${priorityClass(task.priority)}`}>{task.priority}</div>
    </div>
  );
}

function DraggableTaskCard({ task, onClick }: { task: TaskItem; onClick: () => void }) {
  const { attributes, listeners, setNodeRef, isDragging } = useDraggable({
    id: String(task.id),
    data: { task },
  });

  return (
    <div
      ref={setNodeRef}
      {...attributes}
      {...listeners}
      onClick={onClick}
      className={`p-3 bg-white rounded shadow-sm mb-2 cursor-grab active:cursor-grabbing hover:shadow transition-shadow ${isDragging ? 'opacity-50' : ''}`}
    >
      <div className="font-medium text-sm mb-1">{task.title}</div>
      <div className={`text-xs ${priorityClass(task.priority)}`}>{task.priority}</div>
    </div>
  );
}

function Column({
  status,
  tasks,
  onTaskClick,
}: {
  status: string;
  tasks: TaskItem[];
  onTaskClick: (t: TaskItem) => void;
}) {
  const { setNodeRef, isOver } = useDroppable({ id: status });

  return (
    <div
      ref={setNodeRef}
      className={`min-w-[280px] rounded-lg p-3 transition-colors ${
        isOver ? 'bg-[#deebff] ring-2 ring-[#0052cc] ring-inset' : 'bg-[#ebecf0]'
      }`}
    >
      <div className="text-xs font-semibold text-[#5e6c84] uppercase mb-3">{columnLabel(status)}</div>
      <div className="space-y-2">
        {tasks.map((t) => (
          <DraggableTaskCard key={t.id} task={t} onClick={() => onTaskClick(t)} />
        ))}
      </div>
    </div>
  );
}

const modalOverlayClass = 'fixed inset-0 bg-[#091e42]/54 flex items-center justify-center z-[100]';
const modalBoxClass = 'bg-white p-6 rounded-lg w-full max-w-[480px] max-h-[90vh] overflow-auto shadow-xl';

export default function ProjectBoardPage() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();
  const id = projectId ? parseInt(projectId, 10) : NaN;
  const canManageTasks = (user?.roles?.includes('Manager') || user?.roles?.includes('Admin')) ?? false;
  const canManageMembers = canManageTasks;
  const [project, setProject] = useState<Project | null>(null);
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTask, setActiveTask] = useState<TaskItem | null>(null);
  const [modalTask, setModalTask] = useState<TaskItem | null>(null);
  const [showCreate, setShowCreate] = useState(false);
  const [showMembers, setShowMembers] = useState(false);
  const [members, setMembers] = useState<ProjectMemberResponse[]>([]);
  const [availableUsers, setAvailableUsers] = useState<AvailableUser[]>([]);
  const [selectedUserId, setSelectedUserId] = useState('');
  const [memberEmail, setMemberEmail] = useState('');
  const [editTitle, setEditTitle] = useState('');
  const [editDesc, setEditDesc] = useState('');
  const [editStatus, setEditStatus] = useState<TaskStatus | ''>('');
  const [editPriority, setEditPriority] = useState<TaskPriority | ''>('');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const { toastError, toastSuccess } = useToast();

  const load = useCallback(async () => {
    if (!id || isNaN(id)) return;
    setLoading(true);
    const [projRes, tasksRes] = await Promise.all([getProject(id), getTasksByProject(id)]);
    setLoading(false);
    if (projRes.success && projRes.data) setProject(projRes.data);
    if (tasksRes.success && tasksRes.data) setTasks(tasksRes.data);
  }, [id]);

  useEffect(() => { load(); }, [load]);

  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: { distance: 8 },
    })
  );

  const handleDragStart = (event: DragStartEvent) => {
    const task = event.active.data.current?.task as TaskItem | undefined;
    if (task) setActiveTask(task);
  };

  const handleDragEnd = async (event: DragEndEvent) => {
    setActiveTask(null);
    const { active, over } = event;
    if (!over || over.id === active.id) return;
    const task = active.data.current?.task as TaskItem | undefined;
    if (!task) return;
    const newStatus = String(over.id) as TaskStatus;
    if (!COLUMNS.includes(newStatus) || newStatus === task.status) return;

    const prevTasks = [...tasks];
    setTasks((prev) =>
      prev.map((t) => (t.id === task.id ? { ...t, status: newStatus } : t))
    );

    const res = canManageTasks
      ? await updateTask(task.id, {
          title: task.title,
          description: task.description ?? undefined,
          status: statusForApi(newStatus),
          priority: priorityForApi(task.priority),
        })
      : await updateTaskStatus(task.id, statusForApi(newStatus));
    if (!res.success) {
      setTasks(prevTasks);
      const msg = res.status === 403 ? FORBIDDEN_TASKS : (res.error ?? 'Failed to update task');
      toastError(msg);
    } else {
      toastSuccess('Task status updated.');
    }
  };

  const openTask = (t: TaskItem) => {
    setModalTask(t);
    setEditTitle(t.title);
    setEditDesc(t.description ?? '');
    setEditStatus(t.status);
    setEditPriority(t.priority);
    setError('');
  };

  const closeModal = () => {
    setModalTask(null);
    setShowCreate(false);
  };

  const saveTask = async () => {
    if (!modalTask) return;
    setError('');
    setSaving(true);
    const status = (editStatus || modalTask.status) as TaskStatus;
    const priority = (editPriority || modalTask.priority) as TaskPriority;
    const res = canManageTasks
      ? await updateTask(modalTask.id, {
          title: editTitle.trim(),
          description: editDesc || undefined,
          status: statusForApi(status),
          priority: priorityForApi(priority),
        })
      : await updateTaskStatus(modalTask.id, statusForApi(status));
    setSaving(false);
    if (res.success) {
      load();
      closeModal();
      toastSuccess('Task saved.');
    } else {
      const msg = res.status === 403 ? FORBIDDEN_TASKS : (res.error ?? 'Failed to save');
      setError(msg);
      toastError(msg);
    }
  };

  const handleDelete = async () => {
    if (!modalTask) return;
    if (!canManageTasks) {
      toastError(FORBIDDEN_TASKS);
      return;
    }
    const result = await deleteTask(modalTask.id);
    if (result.ok) {
      load();
      closeModal();
      toastSuccess('Task deleted.');
    } else if (result.status === 403) {
      toastError(FORBIDDEN_TASKS);
    } else {
      toastError('Failed to delete task.');
    }
  };

  const loadMembers = useCallback(async () => {
    if (!id || !canManageMembers) return;
    const res = await getProjectMembers(id);
    if (res.success && res.data) setMembers(res.data);
  }, [id, canManageMembers]);

  const loadAvailableUsers = useCallback(async () => {
    if (!id || !canManageMembers) return;
    const res = await getAvailableUsersToAdd(id);
    if (res.success && res.data) setAvailableUsers(res.data);
  }, [id, canManageMembers]);

  const openMembersModal = () => {
    setShowMembers(true);
    setMemberEmail('');
    setSelectedUserId('');
    loadMembers();
    loadAvailableUsers();
  };

  const handleAddByEmail = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!id || !memberEmail.trim()) return;
    const res = await addProjectMember(id, memberEmail.trim());
    if (res.success) {
      loadMembers();
      loadAvailableUsers();
      setMemberEmail('');
      toastSuccess('Member added.');
    } else {
      toastError(res.error ?? 'Failed to add member.');
    }
  };

  const handleAddSelected = async () => {
    if (!id || !selectedUserId) return;
    const res = await addProjectMember(id, selectedUserId);
    if (res.success) {
      loadMembers();
      loadAvailableUsers();
      setSelectedUserId('');
      toastSuccess('Member added.');
    } else {
      toastError(res.error ?? 'Failed to add member.');
    }
  };

  const handleRemoveMember = async (userId: string) => {
    if (!id) return;
    const res = await removeProjectMember(id, userId);
    if (res.ok) {
      loadMembers();
      loadAvailableUsers();
      toastSuccess('Member removed.');
    } else {
      toastError('Failed to remove member.');
    }
  };

  const displayUser = (u: AvailableUser) => {
    const name = [u.firstName, u.lastName].filter(Boolean).join(' ');
    return name ? `${u.email} (${name})` : u.email;
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!id) return;
    setError('');
    setSaving(true);
    const priority = (editPriority || 'Medium') as TaskPriority;
    const res = await createTask(
      id,
      editTitle.trim(),
      editDesc || undefined,
      priorityForApi(priority)
    );
    setSaving(false);
    if (res.success && res.data) {
      load();
      setShowCreate(false);
      setEditTitle('');
      setEditDesc('');
      setEditPriority('Medium');
      toastSuccess('Task created.');
    } else {
      const msg = res.status === 403 ? FORBIDDEN_TASKS : (res.error ?? 'Failed to create');
      setError(msg);
      toastError(msg);
    }
  };

  const tasksByStatus = (status: string) => tasks.filter((t) => t.status === status);

  if (loading && !project) {
    return <p className="text-[#5e6c84]">Loading…</p>;
  }
  if (!project) {
    return (
      <div>
        <button
          type="button"
          onClick={() => navigate('/')}
          className="py-1.5 px-3 text-sm font-medium rounded border border-[#ebecf0] bg-white text-[#172b4d] hover:bg-[#ebecf0] cursor-pointer"
        >
          ← Back
        </button>
        <p className="mt-4 text-[#5e6c84]">Project not found.</p>
      </div>
    );
  }

  return (
    <>
      <div className="flex items-center gap-4 mb-6 flex-wrap">
        <button
          type="button"
          onClick={() => navigate('/')}
          className="py-1.5 px-3 text-sm font-medium rounded border border-[#ebecf0] bg-white text-[#172b4d] hover:bg-[#ebecf0] cursor-pointer"
        >
          ← Back
        </button>
        <h1 className="text-2xl font-semibold text-[#172b4d]">{project.name}</h1>
        {project.description && (
          <span className="text-sm text-[#5e6c84]">{project.description}</span>
        )}
        <div className="ml-auto flex items-center gap-2">
          {canManageMembers && (
            <button
              type="button"
              onClick={openMembersModal}
              className="py-2 px-4 rounded font-medium border border-[#ebecf0] bg-white text-[#172b4d] hover:bg-[#ebecf0] cursor-pointer"
            >
              Members
            </button>
          )}
          {canManageTasks && (
            <button
              type="button"
              onClick={() => {
                setShowCreate(true);
                setEditTitle('');
                setEditDesc('');
                setEditPriority('Medium');
                setError('');
              }}
              className="py-2 px-4 rounded font-medium bg-[#0052cc] text-white hover:bg-[#0747a6] cursor-pointer"
            >
              Create task
            </button>
          )}
        </div>
      </div>

      <DndContext sensors={sensors} onDragStart={handleDragStart} onDragEnd={handleDragEnd}>
        <div className="flex gap-4 overflow-x-auto pb-4">
          {COLUMNS.map((col) => (
            <Column
              key={col}
              status={col}
              tasks={tasksByStatus(col)}
              onTaskClick={openTask}
            />
          ))}
        </div>

        <DragOverlay>
          {activeTask ? <TaskCardPreview task={activeTask} /> : null}
        </DragOverlay>
      </DndContext>

      {modalTask && (
        <div className={modalOverlayClass} onClick={closeModal} role="dialog" aria-modal="true">
          <div className={modalBoxClass} onClick={(e) => e.stopPropagation()}>
            <h2 className="mt-0 mb-4 text-lg font-semibold text-[#172b4d]">Task</h2>
            <div className="space-y-4">
              {canManageTasks ? (
                <>
                  <div>
                    <label className="block text-sm font-medium text-[#172b4d] mb-1">Title</label>
                    <input
                      type="text"
                      className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
                      value={editTitle}
                      onChange={(e) => setEditTitle(e.target.value)}
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-[#172b4d] mb-1">Description</label>
                    <textarea
                      className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff] resize-y"
                      value={editDesc}
                      onChange={(e) => setEditDesc(e.target.value)}
                      rows={3}
                    />
                  </div>
                </>
              ) : (
                <div>
                  <div className="text-sm font-medium text-[#172b4d] mb-1">{modalTask.title}</div>
                  {modalTask.description && <p className="text-sm text-[#5e6c84]">{modalTask.description}</p>}
                </div>
              )}
              <div>
                <label className="block text-sm font-medium text-[#172b4d] mb-1">Status</label>
                <select
                  className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
                  value={editStatus}
                  onChange={(e) => setEditStatus(e.target.value)}
                >
                  {COLUMNS.map((s) => (
                    <option key={s} value={s}>{columnLabel(s)}</option>
                  ))}
                </select>
              </div>
              {canManageTasks && (
                <div>
                  <label className="block text-sm font-medium text-[#172b4d] mb-1">Priority</label>
                  <select
                    className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
                    value={editPriority}
                    onChange={(e) => setEditPriority(e.target.value)}
                  >
                    {PRIORITIES.map((p) => (
                      <option key={p} value={p}>{p}</option>
                    ))}
                  </select>
                </div>
              )}
            </div>
            {error && <p className="text-xs text-red-600 mt-2">{error}</p>}
            <div className="flex justify-between gap-2 mt-5">
              {canManageTasks ? (
                <button
                  type="button"
                  onClick={handleDelete}
                  className="py-2 px-4 rounded font-medium bg-red-600 text-white hover:bg-red-700 cursor-pointer"
                >
                  Delete
                </button>
              ) : <span />}
              <div className="flex gap-2">
                <button type="button" onClick={closeModal} className="py-2 px-4 rounded font-medium border border-[#ebecf0] bg-white text-[#172b4d] hover:bg-[#ebecf0] cursor-pointer">
                  Cancel
                </button>
                <button type="button" onClick={saveTask} disabled={saving} className="py-2 px-4 rounded font-medium bg-[#0052cc] text-white hover:bg-[#0747a6] disabled:opacity-60 disabled:cursor-not-allowed cursor-pointer">
                  {saving ? 'Saving…' : 'Save'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {showMembers && (
        <div className={modalOverlayClass} onClick={() => setShowMembers(false)} role="dialog" aria-modal="true">
          <div className={modalBoxClass} onClick={(e) => e.stopPropagation()}>
            <h2 className="mt-0 mb-4 text-lg font-semibold text-[#172b4d]">Project members</h2>
            <div className="space-y-4 mb-4">
              <div>
                <label className="block text-sm font-medium text-[#172b4d] mb-1">Select from list</label>
                <div className="flex gap-2">
                  <select
                    className="flex-1 px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
                    value={selectedUserId}
                    onChange={(e) => setSelectedUserId(e.target.value)}
                  >
                    <option value="">Choose user…</option>
                    {availableUsers.map((u) => (
                      <option key={u.id} value={u.id}>{displayUser(u)}</option>
                    ))}
                  </select>
                  <button
                    type="button"
                    onClick={handleAddSelected}
                    disabled={!selectedUserId}
                    className="py-2 px-4 rounded font-medium bg-[#0052cc] text-white hover:bg-[#0747a6] disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer"
                  >
                    Add
                  </button>
                </div>
                {availableUsers.length === 0 && <p className="text-xs text-[#5e6c84] mt-1">All users are already in the project.</p>}
              </div>
              <div>
                <label className="block text-sm font-medium text-[#172b4d] mb-1">Or add by email</label>
                <form onSubmit={handleAddByEmail} className="flex gap-2">
                  <input
                    type="text"
                    className="flex-1 px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] placeholder:text-[#97a0af] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
                    value={memberEmail}
                    onChange={(e) => setMemberEmail(e.target.value)}
                    placeholder="Email"
                  />
                  <button type="submit" className="py-2 px-4 rounded font-medium bg-[#0052cc] text-white hover:bg-[#0747a6] cursor-pointer">
                    Add
                  </button>
                </form>
              </div>
            </div>
            <ul className="space-y-2 max-h-60 overflow-auto">
              {members.map((m) => (
                <li key={m.userId} className="flex items-center justify-between py-2 px-3 bg-[#f4f5f7] rounded">
                  <span className="text-sm text-[#172b4d]">{m.email}</span>
                  <button
                    type="button"
                    onClick={() => handleRemoveMember(m.userId)}
                    className="text-sm text-red-600 hover:underline"
                  >
                    Remove
                  </button>
                </li>
              ))}
              {members.length === 0 && <p className="text-sm text-[#5e6c84]">No members yet.</p>}
            </ul>
            <div className="mt-4 flex justify-end">
              <button type="button" onClick={() => setShowMembers(false)} className="py-2 px-4 rounded font-medium border border-[#ebecf0] bg-white text-[#172b4d] hover:bg-[#ebecf0] cursor-pointer">
                Close
              </button>
            </div>
          </div>
        </div>
      )}

      {showCreate && (
        <div className={modalOverlayClass} onClick={() => setShowCreate(false)} role="dialog" aria-modal="true">
          <div className={modalBoxClass} onClick={(e) => e.stopPropagation()}>
            <h2 className="mt-0 mb-4 text-lg font-semibold text-[#172b4d]">New task</h2>
            <form onSubmit={handleCreate} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-[#172b4d] mb-1">Title</label>
                <input
                  type="text"
                  className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
                  value={editTitle}
                  onChange={(e) => setEditTitle(e.target.value)}
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-[#172b4d] mb-1">Description</label>
                <textarea
                  className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff] resize-y"
                  value={editDesc}
                  onChange={(e) => setEditDesc(e.target.value)}
                  rows={3}
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-[#172b4d] mb-1">Priority</label>
                <select
                  className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
                  value={editPriority}
                  onChange={(e) => setEditPriority(e.target.value)}
                >
                  {PRIORITIES.map((p) => (
                    <option key={p} value={p}>{p}</option>
                  ))}
                </select>
              </div>
              {error && <p className="text-xs text-red-600">{error}</p>}
              <div className="flex gap-2 justify-end mt-5">
                <button type="button" onClick={() => setShowCreate(false)} className="py-2 px-4 rounded font-medium border border-[#ebecf0] bg-white text-[#172b4d] hover:bg-[#ebecf0] cursor-pointer">
                  Cancel
                </button>
                <button type="submit" disabled={saving} className="py-2 px-4 rounded font-medium bg-[#0052cc] text-white hover:bg-[#0747a6] disabled:opacity-60 disabled:cursor-not-allowed cursor-pointer">
                  {saving ? 'Creating…' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
