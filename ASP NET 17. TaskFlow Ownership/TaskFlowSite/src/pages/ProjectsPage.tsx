import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getProjects, createProject } from '../api/projects';
import { useToast } from '../context/ToastContext';
import type { Project } from '../types/api';

const FORBIDDEN_CREATE_PROJECT = 'Creating projects is available only to Managers and Admins.';

export default function ProjectsPage() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [newName, setNewName] = useState('');
  const [newDesc, setNewDesc] = useState('');
  const [creating, setCreating] = useState(false);
  const [error, setError] = useState('');
  const { toastError, toastSuccess } = useToast();

  const load = async () => {
    setLoading(true);
    const res = await getProjects();
    setLoading(false);
    if (res.success && res.data) setProjects(res.data);
  };

  useEffect(() => { load(); }, []);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setCreating(true);
    const res = await createProject(newName.trim(), newDesc.trim() || undefined);
    setCreating(false);
    if (res.success && res.data) {
      setShowModal(false);
      setNewName('');
      setNewDesc('');
      load();
      toastSuccess('Project created.');
    } else {
      const msg = res.status === 403 ? FORBIDDEN_CREATE_PROJECT : (res.error ?? 'Failed to create');
      setError(msg);
      toastError(msg);
    }
  };

  return (
    <>
      <div className="flex justify-between items-center mb-5">
        <h1 className="text-2xl font-semibold text-[#172b4d]">Projects</h1>
        <button
          type="button"
          onClick={() => setShowModal(true)}
          className="inline-flex items-center justify-center py-2 px-4 rounded font-medium bg-[#0052cc] text-white hover:bg-[#0747a6] transition-colors cursor-pointer"
        >
          Create project
        </button>
      </div>
      {loading ? (
        <p className="text-[#5e6c84]">Loading…</p>
      ) : (
        <div className="grid grid-cols-[repeat(auto-fill,minmax(280px,1fr))] gap-4">
          {projects.map((p) => (
            <Link
              key={p.id}
              to={`/project/${p.id}`}
              className="block p-5 bg-white border border-[#ebecf0] rounded shadow-sm text-[#172b4d] no-underline transition-shadow hover:shadow-md"
            >
              <div className="text-base font-semibold mb-2">{p.name}</div>
              <div className="text-[13px] text-[#5e6c84]">
                {p.tasksCount} {p.tasksCount === 1 ? 'task' : 'tasks'}
                {p.description && ` · ${p.description.slice(0, 50)}${p.description.length > 50 ? '…' : ''}`}
              </div>
            </Link>
          ))}
        </div>
      )}

      {showModal && (
        <div
          className="fixed inset-0 bg-[#091e42]/54 flex items-center justify-center z-[100]"
          onClick={() => setShowModal(false)}
          role="dialog"
          aria-modal="true"
          aria-labelledby="modal-title"
        >
          <div
            className="bg-white p-6 rounded-lg w-full max-w-[400px] shadow-xl"
            onClick={(e) => e.stopPropagation()}
          >
            <h2 id="modal-title" className="mt-0 mb-5 text-lg font-semibold text-[#172b4d]">New project</h2>
            <form onSubmit={handleCreate} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-[#172b4d] mb-1">Name</label>
                <input
                  type="text"
                  className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] placeholder:text-[#97a0af] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  placeholder="Project name"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-[#172b4d] mb-1">Description (optional)</label>
                <textarea
                  className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] placeholder:text-[#97a0af] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff] resize-y"
                  value={newDesc}
                  onChange={(e) => setNewDesc(e.target.value)}
                  placeholder="Description"
                  rows={3}
                />
              </div>
              {error && <p className="text-xs text-red-600">{error}</p>}
              <div className="flex gap-2 justify-end mt-5">
                <button
                  type="button"
                  onClick={() => setShowModal(false)}
                  className="py-2 px-4 rounded font-medium border border-[#ebecf0] bg-white text-[#172b4d] hover:bg-[#ebecf0] transition-colors cursor-pointer"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={creating}
                  className="py-2 px-4 rounded font-medium bg-[#0052cc] text-white hover:bg-[#0747a6] transition-colors disabled:opacity-60 disabled:cursor-not-allowed cursor-pointer"
                >
                  {creating ? 'Creating…' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
