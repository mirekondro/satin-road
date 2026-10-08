import { useState, type FormEvent } from 'react'
import { Link, Navigate, useLocation, useNavigate } from 'react-router'
import { api } from '../../api/client.ts'
import { getErrorMessage } from '../../api/errors.ts'
import type { AuthResponse } from '../../api/generated/Api.ts'
import type { Role } from '../../auth/authContext.ts'
import { useAuth } from '../../auth/useAuth.ts'

const MIN_PASSWORD_LENGTH = 6 // same as AuthService.MinPasswordLength

interface Props {
  mode: 'login' | 'register'
}

// One form for both login and registration – they only differ in the endpoint
// and in the extra "confirm password" field.
export default function AuthForm({ mode }: Props) {
  const isRegister = mode === 'register'
  const { user, login } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  // Where the user wanted to go before being sent to login (set by RequireAuth / BuyBox)
  const from = (location.state as { from?: string } | null)?.from ?? '/'

  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [confirm, setConfirm] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  // Already logged in → nothing to do here
  if (user) return <Navigate to={from} replace />

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError(null)

    // Only a quick check in the browser – the real rules live in AuthService
    if (isRegister && password !== confirm) {
      setError('Passwords do not match.')
      return
    }

    setSubmitting(true)
    try {
      const body = { username, password }
      const res: AuthResponse = isRegister
        ? await api.auth.authRegisterCreate(body)
        : await api.auth.authLoginCreate(body)

      // Backend returns the JWT right away (also after registration) → user is logged in
      login(res.token, { id: res.userId, username: res.username, role: res.role as Role })
      navigate(from, { replace: true })
    } catch (err) {
      // 401 wrong password, 403 shut down by FBI, 409 username taken, 400 validation…
      setError(getErrorMessage(err))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <section className="container page auth-page">
      <form className="card form auth-form" onSubmit={handleSubmit}>
        <h1 className="page-title">{isRegister ? 'Create account' : 'Log in'}</h1>
        <p className="muted">
          {isRegister ? 'Become a buyer or a vendor in seconds.' : 'Welcome back to Satin Road.'}
        </p>

        <div className="field">
          <label htmlFor="auth-username">Username</label>
          <input
            id="auth-username"
            className="input"
            autoComplete="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            autoFocus
          />
        </div>

        <div className="field">
          <label htmlFor="auth-password">Password</label>
          <input
            id="auth-password"
            className="input"
            type="password"
            autoComplete={isRegister ? 'new-password' : 'current-password'}
            minLength={isRegister ? MIN_PASSWORD_LENGTH : undefined}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          {isRegister && <small className="muted">At least {MIN_PASSWORD_LENGTH} characters.</small>}
        </div>

        {isRegister && (
          <div className="field">
            <label htmlFor="auth-confirm">Confirm password</label>
            <input
              id="auth-confirm"
              className="input"
              type="password"
              autoComplete="new-password"
              value={confirm}
              onChange={(e) => setConfirm(e.target.value)}
              required
            />
          </div>
        )}

        {error && <p className="alert alert-error" role="alert">{error}</p>}

        <button className="btn btn-primary" disabled={submitting}>
          {submitting ? 'Please wait…' : isRegister ? 'Create account' : 'Log in'}
        </button>

        <p className="muted auth-switch">
          {isRegister ? (
            <>Already have an account? <Link to="/login" state={{ from }}>Log in</Link></>
          ) : (
            <>New here? <Link to="/register" state={{ from }}>Create an account</Link></>
          )}
        </p>
      </form>
    </section>
  )
}
