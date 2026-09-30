import { useCallback, useMemo, useState, type ReactNode } from 'react'
import { AuthContext, type AuthUser } from './authContext.ts'
import { clearSession, getStoredToken, getStoredUser, storeSession } from './authStorage.ts'

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => getStoredToken())
  const [user, setUser] = useState<AuthUser | null>(() => getStoredUser<AuthUser>())

  const login = useCallback((newToken: string, newUser: AuthUser) => {
    storeSession(newToken, newUser)
    setToken(newToken)
    setUser(newUser)
  }, [])

  const logout = useCallback(() => {
    clearSession()
    setToken(null)
    setUser(null)
  }, [])

  const value = useMemo(
    () => ({ token, user, isAdmin: user?.role === 'Admin', login, logout }),
    [token, user, login, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
