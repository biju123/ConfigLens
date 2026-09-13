import type { FindingSeverity } from '../../api/types'

const COLOR_BY_SEVERITY: Record<FindingSeverity, string> = {
  Info: 'var(--status-info)',
  Low: 'var(--status-neutral)',
  Medium: 'var(--status-warning)',
  High: 'var(--status-fail)',
  Critical: 'var(--status-critical)',
}

export function SeverityBadge({ severity }: { severity: FindingSeverity }) {
  const color = COLOR_BY_SEVERITY[severity]
  return (
    <span className="badge" style={{ color, background: `color-mix(in srgb, ${color} 16%, transparent)` }}>
      {severity}
    </span>
  )
}
