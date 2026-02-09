import { Link, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';

function navLinkClass(active: boolean) {
  return `block py-2.5 px-5 text-sm font-medium transition-colors ${
    active ? 'bg-[#253858] text-white' : 'text-white/85 hover:bg-[#253858]/50 hover:text-white'
  }`;
}

export default function Layout() {
  const location = useLocation();
  const { user, logout } = useAuth();
  const isActive = (path: string) => location.pathname === path || location.pathname.startsWith(path + '/');

  return (
    <div className="flex min-h-screen">
      <aside className="w-60 min-h-screen bg-[#172b4d] text-white py-3 shrink-0">
        <Link to="/" className="block py-3 px-5 text-xl font-bold text-white no-underline tracking-tight">
          TaskFlow
        </Link>
        <nav>
          <Link to="/" className={navLinkClass(isActive('/') && location.pathname === '/')}>
            Projects
          </Link>
          {location.pathname.startsWith('/project/') && (
            <Link to={location.pathname} className={navLinkClass(isActive(location.pathname))}>
              Board
            </Link>
          )}
        </nav>
      </aside>
      <div className="flex-1 flex flex-col min-w-0">
        <header className="h-14 flex items-center justify-between px-6 bg-white border-b border-[#ebecf0] shrink-0">
          <span className="text-sm text-[#5e6c84]">Task management</span>
          <div className="flex items-center gap-4">
            <span className="text-sm text-[#172b4d]">{user?.email}</span>
            <button
              type="button"
              onClick={() => logout()}
              className="py-1.5 px-3 text-sm font-medium rounded border border-[#ebecf0] bg-white text-[#172b4d] hover:bg-[#ebecf0] transition-colors cursor-pointer"
            >
              Sign out
            </button>
          </div>
        </header>
        <main className="flex-1 overflow-auto p-6 bg-[#f4f5f7]">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
