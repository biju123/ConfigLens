import type { DependencyCheckResult, DependencyComparisonResult } from '../../api/types'
import { StatusBadge } from '../../components/common/StatusBadge'

function CheckList({ checks }: { checks: DependencyCheckResult[] }) {
  if (checks.length === 0) return <div className="muted">None</div>
  return (
    <ul>
      {checks.map((c) => (
        <li key={`${c.application}-${c.dependencyType}`}>
          {c.application} · {c.dependencyType}
        </li>
      ))}
    </ul>
  )
}

export function DependencyComparisonView({ result }: { result: DependencyComparisonResult }) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <div className="card">
        <h2>
          New failures <span className="muted">({result.newFailures.length})</span>
        </h2>
        <CheckList checks={result.newFailures} />
      </div>
      <div className="card">
        <h2>
          Resolved failures <span className="muted">({result.resolvedFailures.length})</span>
        </h2>
        <CheckList checks={result.resolvedFailures} />
      </div>
      <div className="card">
        <h2>
          Status changes <span className="muted">({result.statusChanges.length})</span>
        </h2>
        {result.statusChanges.length === 0 ? (
          <div className="muted">None</div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Application</th>
                <th>Dependency</th>
                <th>Baseline</th>
                <th>Current</th>
              </tr>
            </thead>
            <tbody>
              {result.statusChanges.map((c) => (
                <tr key={`${c.application}-${c.dependencyType}`}>
                  <td>{c.application}</td>
                  <td>{c.dependencyType}</td>
                  <td>
                    <StatusBadge status={c.oldStatus} />
                  </td>
                  <td>
                    <StatusBadge status={c.newStatus} />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}
