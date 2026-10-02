import { Navigate, Outlet, useLocation } from 'react-router'
import { useAuth } from './useAuth.ts'

/** Only lets logged-in users through; otherwise redirects to /login and remembers where they were going. */
export function RequireAuth() {
  const { user } = useAuth()
  const location = useLocation()
  if (!user) return <Navigate to="/login" replace state={{ from: location.pathname }} />
  return <Outlet />
}

/** Only lets admins through. */
export function RequireAdmin() {
  const { user, isAdmin } = useAuth()
  if (!user) return <Navigate to="/login" replace />
  if (!isAdmin) return <Navigate to="/" replace />
  return <Outlet />
}
