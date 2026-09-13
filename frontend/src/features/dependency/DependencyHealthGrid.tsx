import type { DependencyCheckResult } from '../../api/types'
import { StatusBadge } from '../../components/common/StatusBadge'

function groupByApplication(checks: DependencyCheckResult[]): Map<string, DependencyCheckResult[]> {
  const map = new Map<string, DependencyCheckResult[]>()
  for (const check of checks) {
    const group = map.get(check.application)
    if (group) group.push(check)
    else map.set(check.application, [check])
  }
  return map
}

export function DependencyHealthGrid({
  checks,
  onSelect,
}: {
  checks: DependencyCheckResult[]
  onSelect: (check: DependencyCheckResult) => void
}) {
  const byApplication = groupByApplication(checks)

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      {[...byApplication.entries()].map(([application, appChecks]) => (
        <div key={application} className="card">
          <h2>{application}</h2>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(220px, 1fr))', gap: 'var(--space-2)' }}>
            {appChecks.map((check) => (
              <div
                key={check.dependencyType}
                className="card"
                onClick={() => onSelect(check)}
                style={{ cursor: 'pointer', padding: 'var(--space-3)' }}
              >
                <div style={{ fontWeight: 600, marginBottom: 4 }}>{check.dependencyType}</div>
                <StatusBadge status={check.status} />
                {check.failureReason && (
                  <div className="muted" style={{ marginTop: 6, fontSize: 12 }}>
                    {check.failureReason}
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      ))}
    </div>
  )
}
