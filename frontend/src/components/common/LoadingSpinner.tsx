export function LoadingSpinner({ label = 'Loading…' }: { label?: string }) {
  return (
    <div className="muted" role="status" style={{ padding: 'var(--space-4)' }}>
      {label}
    </div>
  )
}
