import { useState } from 'react'
import { Link, NavLink, useNavigate } from 'react-router'
import { useAuth } from '../auth/useAuth.ts'

const linkClass = ({ isActive }: { isActive: boolean }) =>
  isActive ? 'nav-link active' : 'nav-link'

export default function Navbar() {
  const { user, isAdmin, logout } = useAuth()
  const navigate = useNavigate()
  const [open, setOpen] = useState(false)
  const close = () => setOpen(false)

  const handleLogout = () => {
    logout()
    close()
    navigate('/')
  }

  return (
    <header className="navbar">
      <div className="navbar-inner container">
        <Link to="/" className="brand" onClick={close}>
          <span className="brand-mark" aria-hidden>◆</span>
          Satin<span className="brand-accent">Road</span>
        </Link>

        <button
          className="nav-toggle"
          aria-label="Menu"
          aria-expanded={open}
          onClick={() => setOpen((o) => !o)}
        >
          ☰
        </button>

        <nav className={open ? 'nav-links open' : 'nav-links'}>
          <NavLink to="/listings" className={linkClass} onClick={close}>
            Listings
          </NavLink>

          {user && (
            <>
              <NavLink to="/my/listings" className={linkClass} onClick={close}>
                My shop
              </NavLink>
              <NavLink to="/my/orders" className={linkClass} onClick={close}>
                My orders
              </NavLink>
            </>
          )}

          {isAdmin && (
            <NavLink to="/admin/categories" className={linkClass} onClick={close}>
              Admin
            </NavLink>
          )}

          <div className="nav-auth">
            {user ? (
              <>
                <span className="nav-user">@{user.username}</span>
                <button className="btn btn-ghost btn-sm" onClick={handleLogout}>
                  Log out
                </button>
              </>
            ) : (
              <>
                <NavLink to="/login" className={linkClass} onClick={close}>
                  Log in
                </NavLink>
                <Link to="/register" className="btn btn-primary btn-sm" onClick={close}>
                  Sign up
                </Link>
              </>
            )}
          </div>
        </nav>
      </div>
    </header>
  )
}
