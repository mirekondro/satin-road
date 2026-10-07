import type { ComponentType } from 'react'
import { createBrowserRouter } from 'react-router'
import RootLayout from './layouts/RootLayout.tsx'
import { RequireAdmin, RequireAuth } from './auth/guards.tsx'
import HomePage from './pages/HomePage.tsx'
import ListingsPage from './pages/ListingsPage.tsx'
import ErrorPage from './pages/ErrorPage.tsx'

// Stránky, které nejsou potřeba hned po otevření webu, se načítají až při první návštěvě
// (code splitting). Úvodní stránka a výpis listingů jsou v hlavním balíčku, ať jsou okamžitě.
const page = (load: () => Promise<{ default: ComponentType }>) => () =>
    load().then((m) => ({ Component: m.default }))

export const router = createBrowserRouter([
  {
    path: '/',
    element: <RootLayout />,
    errorElement: <ErrorPage />,
    children: [
      { index: true, element: <HomePage /> },
      { path: 'listings', element: <ListingsPage /> },
      { path: 'listings/:id', lazy: page(() => import('./pages/ListingDetailPage.tsx')) },
      { path: 'login', lazy: page(() => import('./pages/LoginPage.tsx')) },
      { path: 'register', lazy: page(() => import('./pages/RegisterPage.tsx')) },

      // Logged-in users only
      {
        element: <RequireAuth />,
        children: [
          { path: 'my/listings', lazy: page(() => import('./pages/MyListingsPage.tsx')) },
          { path: 'my/orders', lazy: page(() => import('./pages/MyOrdersPage.tsx')) },
        ],
      },

      // Admins only
      {
        element: <RequireAdmin />,
        children: [
          { path: 'admin/categories', lazy: page(() => import('./pages/AdminCategoriesPage.tsx')) },
        ],
      },

      { path: '*', element: <ErrorPage /> },
    ],
  },
])