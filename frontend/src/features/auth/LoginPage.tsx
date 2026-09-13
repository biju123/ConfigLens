import { useNavigate } from 'react-router-dom'
import { LoginForm } from './LoginForm'

export function LoginPage() {
  const navigate = useNavigate()

  return (
    <div style={{ minHeight: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <div className="card" style={{ width: 340 }}>
        <h1>ConfigLens</h1>
        <p className="muted" style={{ marginTop: 0, marginBottom: 'var(--space-4)' }}>
          Deployment configuration inspection and validation
        </p>
        <LoginForm onSuccess={() => navigate('/new-scan', { replace: true })} />
      </div>
    </div>
  )
}
