import { createContext } from 'react'

export type Role = 'User' | 'Admin'

export interface AuthUser {
  id: number
  username: string
  role: Role
}

export interface AuthState {
  user: AuthUser | null
  token: string | null
  isAdmin: boolean
  /** Call after a successful login/register with what the backend returns. */
  login: (token: string, user: AuthUser) => void
  logout: () => void
}

export const AuthContext = createContext<AuthState | null>(null)
