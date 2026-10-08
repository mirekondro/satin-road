import { RouterProvider } from 'react-router'
import { AuthProvider } from './auth/AuthContext.tsx'
import { router } from './router.tsx'
import './App.css'

export default function App() {
  return (
    <AuthProvider>
      <RouterProvider router={router} />
    </AuthProvider>
  )
}
