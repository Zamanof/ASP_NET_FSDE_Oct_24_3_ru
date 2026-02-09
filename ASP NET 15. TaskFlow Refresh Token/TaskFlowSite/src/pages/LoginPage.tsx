import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    const result = await login(email, password);
    setLoading(false);
    if (result.success) navigate('/', { replace: true });
    else setError(result.error ?? 'Login failed');
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-[#172b4d] to-[#0052cc]">
      <div className="w-full max-w-[400px] p-10 bg-white rounded-lg shadow-xl">
        <div className="text-2xl font-bold text-[#0052cc] tracking-tight mb-2">TaskFlow</div>
        <p className="text-[#5e6c84] text-sm mb-8">Project and task management</p>
        <h1 className="text-lg font-semibold text-[#172b4d] mb-6">Sign in</h1>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-[#172b4d] mb-1">Email</label>
            <input
              type="email"
              className="w-full px-3 py-2 border border-[#ebecf0] rounded bg-white text-[#172b4d] placeholder:text-[#97a0af] focus:outline-none focus:border-[#0052cc] focus:ring-2 focus:ring-[#deebff]"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="your@email.com"
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
              autoComplete="current-password"
            />
          </div>
          {error && <p className="text-xs text-red-600">{error}</p>}
          <button
            type="submit"
            className="w-full mt-2 py-2.5 px-4 font-medium rounded bg-[#0052cc] text-white hover:bg-[#0747a6] transition-colors disabled:opacity-60 disabled:cursor-not-allowed"
            disabled={loading}
          >
            {loading ? 'Signing in…' : 'Sign in'}
          </button>
        </form>
        <p className="text-center mt-6">
          <Link to="/register" className="text-[#0052cc] hover:underline">Don&apos;t have an account? Sign up</Link>
        </p>
      </div>
    </div>
  );
}
