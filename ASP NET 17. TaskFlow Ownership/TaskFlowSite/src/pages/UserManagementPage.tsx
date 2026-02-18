import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getUsers, assignRole, removeRole } from '../api/users';
import { useAuth } from '../auth/AuthContext';
import { useToast } from '../context/ToastContext';
import type { UserWithRoles } from '../types/api';

const ROLES = ['Admin', 'Manager', 'User'] as const;

export default function UserManagementPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { toastError, toastSuccess } = useToast();
  const [users, setUsers] = useState<UserWithRoles[]>([]);
  const [loading, setLoading] = useState(true);
  const [assigning, setAssigning] = useState<string | null>(null);
  const [newRole, setNewRole] = useState<Record<string, string>>({});

  const isAdmin = user?.roles?.includes('Admin') ?? false;

  const load = async () => {
    setLoading(true);
    const res = await getUsers();
    setLoading(false);
    if (res.success && res.data) setUsers(res.data);
    else if (res.status === 403) {
      toastError('Only Admins can manage users.');
      navigate('/');
    }
  };

  useEffect(() => {
    if (!isAdmin) {
      navigate('/');
      return;
    }
    load();
  }, [isAdmin]);

  const handleAssign = async (userId: string) => {
    const role = newRole[userId]?.trim();
    if (!role) {
      toastError('Select a role.');
      return;
    }
    setAssigning(userId);
    const res = await assignRole(userId, role);
    setAssigning(null);
    if (res.success) {
      load();
      toastSuccess(`Role "${role}" assigned.`);
      setNewRole((prev) => ({ ...prev, [userId]: '' }));
    } else {
      toastError(res.error ?? 'Failed to assign role.');
    }
  };

  const handleRemove = async (userId: string, roleName: string) => {
    setAssigning(userId);
    const res = await removeRole(userId, roleName);
    setAssigning(null);
    if (res.success) {
      load();
      toastSuccess(`Role "${roleName}" removed.`);
    } else {
      toastError(res.error ?? 'Failed to remove role.');
    }
  };

  if (!isAdmin) return null;

  return (
    <>
      <h1 className="text-2xl font-semibold text-[#172b4d] mb-5">Users &amp; roles</h1>
      {loading ? (
        <p className="text-[#5e6c84]">Loading…</p>
      ) : (
        <div className="bg-white border border-[#ebecf0] rounded-lg shadow-sm overflow-hidden">
          <table className="w-full text-left text-sm">
            <thead className="bg-[#f4f5f7] border-b border-[#ebecf0]">
              <tr>
                <th className="py-3 px-4 font-semibold text-[#172b4d]">Email</th>
                <th className="py-3 px-4 font-semibold text-[#172b4d]">Name</th>
                <th className="py-3 px-4 font-semibold text-[#172b4d]">Roles</th>
                <th className="py-3 px-4 font-semibold text-[#172b4d]">Actions</th>
              </tr>
            </thead>
            <tbody>
              {users.map((u) => (
                <tr key={u.id} className="border-b border-[#ebecf0] last:border-0">
                  <td className="py-3 px-4 text-[#172b4d]">{u.email}</td>
                  <td className="py-3 px-4 text-[#5e6c84]">
                    {[u.firstName, u.lastName].filter(Boolean).join(' ') || '—'}
                  </td>
                  <td className="py-3 px-4">
                    <div className="flex flex-wrap gap-1">
                      {u.roles.map((r) => (
                        <span
                          key={r}
                          className="inline-flex items-center gap-1 py-0.5 px-2 rounded bg-[#deebff] text-[#0052cc] text-xs font-medium"
                        >
                          {r}
                          <button
                            type="button"
                            onClick={() => handleRemove(u.id, r)}
                            disabled={assigning === u.id}
                            className="text-[#0052cc] hover:text-red-600 disabled:opacity-50"
                            aria-label={`Remove ${r}`}
                          >
                            ×
                          </button>
                        </span>
                      ))}
                    </div>
                  </td>
                  <td className="py-3 px-4">
                    <div className="flex items-center gap-2">
                      <select
                        className="py-1.5 px-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc]"
                        value={newRole[u.id] ?? ''}
                        onChange={(e) => setNewRole((prev) => ({ ...prev, [u.id]: e.target.value }))}
                      >
                        <option value="">Add role…</option>
                        {ROLES.filter((r) => !u.roles.includes(r)).map((r) => (
                          <option key={r} value={r}>{r}</option>
                        ))}
                      </select>
                      <button
                        type="button"
                        onClick={() => handleAssign(u.id)}
                        disabled={assigning === u.id || !newRole[u.id]?.trim()}
                        className="py-1.5 px-3 rounded font-medium bg-[#0052cc] text-white hover:bg-[#0747a6] disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer text-sm"
                      >
                        {assigning === u.id ? '…' : 'Assign'}
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  );
}
