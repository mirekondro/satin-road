import { Outlet, ScrollRestoration } from 'react-router'
import Navbar from '../components/Navbar.tsx'
import Footer from '../components/Footer.tsx'

export default function RootLayout() {
  return (
    <div className="app-shell">
      <Navbar />
      <main className="app-main">
        <Outlet />
      </main>
      <Footer />
      <ScrollRestoration />
    </div>
  )
}
