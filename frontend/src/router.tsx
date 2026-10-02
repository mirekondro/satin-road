import { createBrowserRouter } from 'react-router'
import RootLayout from './layouts/RootLayout.tsx'
import { RequireAdmin, RequireAuth } from './auth/guards.tsx'
import HomePage from './pages/HomePage.tsx'
import ListingsPage from './pages/ListingsPage.tsx'
import ListingDetailPage from './pages/ListingDetailPage.tsx'
import MyListingsPage from './pages/MyListingsPage.tsx'
import MyOrdersPage from './pages/MyOrdersPage.tsx'
import LoginPage from './pages/LoginPage.tsx'
import RegisterPage from './pages/RegisterPage.tsx'
import AdminCategoriesPage from './pages/AdminCategoriesPage.tsx'
import ErrorPage from './pages/ErrorPage.tsx'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <RootLayout />,
    errorElement: <ErrorPage />,
    children: [
      { index: true, element: <HomePage /> },
      { path: 'listings', element: <ListingsPage /> },
      { path: 'listings/:id', element: <ListingDetailPage /> },
      { path: 'login', element: <LoginPage /> },
      { path: 'register', element: <RegisterPage /> },

      // Logged-in users only
      {
        element: <RequireAuth />,
        children: [
          { path: 'my/listings', element: <MyListingsPage /> },
          { path: 'my/orders', element: <MyOrdersPage /> },
        ],
      },

      // Admins only
      {
        element: <RequireAdmin />,
        children: [{ path: 'admin/categories', element: <AdminCategoriesPage /> }],
      },

      { path: '*', element: <ErrorPage /> },
    ],
  },
])
