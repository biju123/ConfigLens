import type { AppError, AppErrorKind } from '../../api/errors'

const LABEL_BY_KIND: Record<AppErrorKind, string> = {
  'invalid-input': 'Invalid input',
  'authentication-failure': 'Authentication failure',
  'authorization-failure': 'Authorization failure',
  'infrastructure-failure': 'Infrastructure failure',
  'application-failure': 'Application failure',
  'dependency-failure': 'Dependency failure',
  timeout: 'Timeout',
  'validation-failure': 'Validation failure',
}

export function ErrorBanner({ error }: { error: AppError }) {
  return (
    <div className="error-banner" role="alert">
      <strong>{LABEL_BY_KIND[error.kind]}:</strong> {error.message}
      {error.fieldErrors && (
        <ul style={{ margin: '8px 0 0', paddingLeft: 20 }}>
          {Object.entries(error.fieldErrors).map(([field, messages]) => (
            <li key={field}>
              {field}: {messages.join(' ')}
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
