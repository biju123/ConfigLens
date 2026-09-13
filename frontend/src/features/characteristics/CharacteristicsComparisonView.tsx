import type { CharacteristicsComparisonResult, Finding } from '../../api/types'
import { StatusBadge } from '../../components/common/StatusBadge'

function FindingList({ findings }: { findings: Finding[] }) {
  if (findings.length === 0) return <div className="muted">None</div>
  return (
    <ul>
      {findings.map((f) => (
        <li key={f.findingId} className="mono">
          {f.ruleId} · {f.resource}
        </li>
      ))}
    </ul>
  )
}

export function CharacteristicsComparisonView({ result }: { result: CharacteristicsComparisonResult }) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <div className="card">
        <h2>
          New failures <span className="muted">({result.newFailures.length})</span>
        </h2>
        <FindingList findings={result.newFailures} />
      </div>
      <div className="card">
        <h2>
          Resolved failures <span className="muted">({result.resolvedFailures.length})</span>
        </h2>
        <FindingList findings={result.resolvedFailures} />
      </div>
      <div className="card">
        <h2>
          New warnings <span className="muted">({result.newWarnings.length})</span>
        </h2>
        <FindingList findings={result.newWarnings} />
      </div>
      <div className="card">
        <h2>
          Resolved warnings <span className="muted">({result.resolvedWarnings.length})</span>
        </h2>
        <FindingList findings={result.resolvedWarnings} />
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
                <th>Rule</th>
                <th>Resource</th>
                <th>Baseline</th>
                <th>Current</th>
              </tr>
            </thead>
            <tbody>
              {result.statusChanges.map((c) => (
                <tr key={`${c.ruleId}-${c.resource}`}>
                  <td>{c.ruleId}</td>
                  <td className="mono">{c.resource}</td>
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
      <div className="card">
        <h2>
          Rule changes <span className="muted">({result.ruleChanges.length})</span>
        </h2>
        {result.ruleChanges.length === 0 ? (
          <div className="muted">None</div>
        ) : (
          <ul>
            {result.ruleChanges.map((c) => (
              <li key={c.ruleId}>
                <StatusBadge status={c.changeType} /> {c.ruleName} ({c.ruleId})
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  )
}
