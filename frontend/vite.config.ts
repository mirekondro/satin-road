import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // The backend runs locally on http://localhost:5273 (see launchSettings.json).
    // The frontend calls relative /api/... and Vite forwards it to the backend in dev.
    proxy: {
      '/api': {
        target: 'http://localhost:5273',
        changeOrigin: true,
      },
    },
  },
})
