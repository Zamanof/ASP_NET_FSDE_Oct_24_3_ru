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
import { getProject } from '../api/projects';
import { getTasksByProject, createTask, updateTask, deleteTask } from '../api/tasks';
import { useToast } from '../context/ToastContext';
import type { Project } from '../types/api';
import type { TaskItem } from '../types/api';

const FORBIDDEN_TASKS = 'Creating and editing tasks is available only to Managers and Admins.';

const COLUMNS = ['ToDo', 'InProgress', 'Done'] as const;
const PRIORITIES = ['Low', 'Medium', 'High'] as const;

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
  const id = projectId ? parseInt(projectId, 10) : NaN;
  const [project, setProject] = useState<Project | null>(null);
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTask, setActiveTask] = useState<TaskItem | null>(null);
  const [modalTask, setModalTask] = useState<TaskItem | null>(null);
  const [showCreate, setShowCreate] = useState(false);
  const [editTitle, setEditTitle] = useState('');
  const [editDesc, setEditDesc] = useState('');
  const [editStatus, setEditStatus] = useState('');
  const [editPriority, setEditPriority] = useState('');
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
    const newStatus = String(over.id);
    if (!COLUMNS.includes(newStatus as (typeof COLUMNS)[number]) || newStatus === task.status) return;

    const prevTasks = [...tasks];
    setTasks((prev) =>
      prev.map((t) => (t.id === task.id ? { ...t, status: newStatus } : t))
    );

    const res = await updateTask(task.id, {
      title: task.title,
      description: task.description ?? undefined,
      status: newStatus,
      priority: task.priority,
    });
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
    const res = await updateTask(modalTask.id, {
      title: editTitle.trim(),
      description: editDesc || undefined,
      status: editStatus,
      priority: editPriority,
    });
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

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!id) return;
    setError('');
    setSaving(true);
    const res = await createTask(id, editTitle.trim(), editDesc || undefined, editPriority);
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
      <div className="flex items-center gap-4 mb-6">
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
        <button
          type="button"
          onClick={() => {
            setShowCreate(true);
            setEditTitle('');
            setEditDesc('');
            setEditPriority('Medium');
            setError('');
          }}
          className="ml-auto py-2 px-4 rounded font-medium bg-[#0052cc] text-white hover:bg-[#0747a6] cursor-pointer"
        >
          Create task
        </button>
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
              <div>
                <label className="block text-sm font-medium text-[#172b4d] mb-1">Status</label>
                <select
                  className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
                  value={editStatus}
                  onChange={(e) => setEditStatus(e.target.value)}
                >
                  {COLUMNS.map((s) => (
                    <option key={s} value={s}>{s}</option>
                  ))}
                </select>
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
            </div>
            {error && <p className="text-xs text-red-600 mt-2">{error}</p>}
            <div className="flex justify-between gap-2 mt-5">
              <button
                type="button"
                onClick={handleDelete}
                className="py-2 px-4 rounded font-medium bg-red-600 text-white hover:bg-red-700 cursor-pointer"
              >
                Delete
              </button>
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
