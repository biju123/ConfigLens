import type { DependencyStatus, FindingStatus, ScanStatus } from '../../api/types'

type KnownStatus = FindingStatus | DependencyStatus | ScanStatus | 'Added' | 'Removed' | 'Modified' | 'Changed'

// Also used for free-form Kubernetes pod/job status strings (podStatus is
// not a backend enum), so a couple of entries here cover those states too:
// "Running" reads as healthy for a pod - the ScanStatus.Running value it
// would otherwise collide with is never actually rendered, since MVP scans
// are synchronous and always resolve straight to Completed.
const COLOR_BY_STATUS: Record<string, string> = {
  Pass: 'var(--status-pass)',
  Accessible: 'var(--status-pass)',
  Completed: 'var(--status-pass)',
  Running: 'var(--status-pass)',
  Succeeded: 'var(--status-pass)',

  Warning: 'var(--status-warning)',
  Pending: 'var(--status-warning)',
  Modified: 'var(--status-warning)',
  Changed: 'var(--status-warning)',

  Fail: 'var(--status-fail)',
  Failed: 'var(--status-fail)',
  Inaccessible: 'var(--status-fail)',
  Timeout: 'var(--status-fail)',
  ValidationError: 'var(--status-fail)',
  ConfigurationInvalid: 'var(--status-fail)',
  ConfigurationMissing: 'var(--status-fail)',
  Removed: 'var(--status-fail)',
  CrashLoopBackOff: 'var(--status-fail)',

  Error: 'var(--status-critical)',

  NotChecked: 'var(--status-neutral)',
  NotApplicable: 'var(--status-neutral)',
  Added: 'var(--status-info)',
}

export function StatusBadge({ status }: { status: KnownStatus | string }) {
  const color = COLOR_BY_STATUS[status] ?? 'var(--status-neutral)'
  return (
    <span className="badge" style={{ color, background: `color-mix(in srgb, ${color} 16%, transparent)` }}>
      <span className="badge-dot" style={{ background: color }} />
      {status}
    </span>
  )
}
