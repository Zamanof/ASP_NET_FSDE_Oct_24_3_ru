import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';

export default function RegisterPage() {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { register } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }
    setLoading(true);
    const result = await register({ firstName, lastName, email, password, confirmPassword });
    setLoading(false);
    if (result.success) navigate('/', { replace: true });
    else setError(result.error ?? 'Registration failed');
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-[#172b4d] to-[#0052cc] p-6">
      <div className="w-full max-w-[420px] p-10 bg-white rounded-lg shadow-xl">
        <div className="text-2xl font-bold text-[#0052cc] mb-2">TaskFlow</div>
        <p className="text-[#5e6c84] text-sm mb-6">Project and task management</p>
        <h1 className="text-lg font-semibold text-[#172b4d] mb-5">Sign up</h1>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-[#172b4d] mb-1">First name</label>
              <input
                type="text"
                className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-[#172b4d] mb-1">Last name</label>
              <input
                type="text"
                className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                required
              />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-[#172b4d] mb-1">Email</label>
            <input
              type="email"
              className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              autoComplete="email"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-[#172b4d] mb-1">Password</label>
            <input
              type="password"
              className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              minLength={6}
              autoComplete="new-password"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-[#172b4d] mb-1">Confirm password</label>
            <input
              type="password"
              className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
              autoComplete="new-password"
            />
          </div>
          {error && <p className="text-xs text-red-600">{error}</p>}
          <button
            type="submit"
            className="w-full mt-2 py-2.5 px-4 font-medium rounded bg-[#0052cc] text-white hover:bg-[#0747a6] transition-colors disabled:opacity-60 disabled:cursor-not-allowed"
            disabled={loading}
          >
            {loading ? 'Signing up…' : 'Sign up'}
          </button>
        </form>
        <p className="text-center mt-5">
          <Link to="/login" className="text-[#0052cc] hover:underline">Already have an account? Sign in</Link>
        </p>
      </div>
    </div>
  );
}
