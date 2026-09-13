import type { AksComparisonResult } from '../../api/types'
import { StatusBadge } from '../../components/common/StatusBadge'

export function AksComparisonView({ result }: { result: AksComparisonResult }) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <div className="card">
        <h2>
          Added resources <span className="muted">({result.addedResources.length})</span>
        </h2>
        {result.addedResources.length === 0 ? (
          <div className="muted">None</div>
        ) : (
          <ul>
            {result.addedResources.map((r) => (
              <li key={`${r.resourceType}-${r.resourceName}`} className="mono">
                {r.namespace} / {r.resourceType} / {r.resourceName}
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="card">
        <h2>
          Removed resources <span className="muted">({result.removedResources.length})</span>
        </h2>
        {result.removedResources.length === 0 ? (
          <div className="muted">None</div>
        ) : (
          <ul>
            {result.removedResources.map((r) => (
              <li key={`${r.resourceType}-${r.resourceName}`} className="mono">
                {r.namespace} / {r.resourceType} / {r.resourceName}
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="card">
        <h2>
          Changed resources <span className="muted">({result.changedResources.length})</span>
        </h2>
        {result.changedResources.length === 0 ? (
          <div className="muted">None</div>
        ) : (
          result.changedResources.map((change) => (
            <div key={`${change.resourceType}-${change.resourceName}`} style={{ marginBottom: 'var(--space-3)' }}>
              <div className="mono">
                {change.namespace} / {change.resourceType} / {change.resourceName}
              </div>
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Field</th>
                    <th>Baseline</th>
                    <th>Current</th>
                  </tr>
                </thead>
                <tbody>
                  {change.fieldChanges.map((f) => (
                    <tr key={f.fieldName}>
                      <td>{f.fieldName}</td>
                      <td className="mono">{f.oldValue ?? '—'}</td>
                      <td className="mono">{f.newValue ?? '—'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ))
        )}
      </div>

      {result.addedResources.length === 0 && result.removedResources.length === 0 && result.changedResources.length === 0 && (
        <StatusBadge status="Pass" />
      )}
    </div>
  )
}
