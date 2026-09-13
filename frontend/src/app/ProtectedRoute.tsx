import type { ReactNode } from 'react'
import { Navigate } from 'react-router-dom'
import { useAppSelector } from './hooks'
import { AppShell } from '../components/common/AppShell'

export function ProtectedRoute({ children }: { children: ReactNode }) {
  const token = useAppSelector((state) => state.auth.token)

  if (!token) {
    return <Navigate to="/login" replace />
  }

  return <AppShell>{children}</AppShell>
}
