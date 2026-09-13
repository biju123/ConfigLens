import { createBrowserRouter, Navigate } from 'react-router-dom'
import { LoginPage } from '../features/auth/LoginPage'
import { NewScanPage } from '../features/newScan/NewScanPage'
import { ScanHistoryPage } from '../features/history/ScanHistoryPage'
import { ScanDetailPage } from '../features/scanDetail/ScanDetailPage'
import { ComparisonSetupPage } from '../features/history/ComparisonSetupPage'
import { CompareResultPage } from '../features/history/CompareResultPage'
import { ProtectedRoute } from './ProtectedRoute'

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  { path: '/', element: <Navigate to="/new-scan" replace /> },
  {
    path: '/new-scan',
    element: (
      <ProtectedRoute>
        <NewScanPage />
      </ProtectedRoute>
    ),
  },
  {
    path: '/history',
    element: (
      <ProtectedRoute>
        <ScanHistoryPage />
      </ProtectedRoute>
    ),
  },
  {
    path: '/results/:scanId',
    element: (
      <ProtectedRoute>
        <ScanDetailPage />
      </ProtectedRoute>
    ),
  },
  {
    path: '/compare',
    element: (
      <ProtectedRoute>
        <ComparisonSetupPage />
      </ProtectedRoute>
    ),
  },
  {
    path: '/compare/:currentScanId/:baselineScanId',
    element: (
      <ProtectedRoute>
        <CompareResultPage />
      </ProtectedRoute>
    ),
  },
  { path: '*', element: <Navigate to="/new-scan" replace /> },
])
