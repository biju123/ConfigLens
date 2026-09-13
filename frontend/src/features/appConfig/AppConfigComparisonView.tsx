import type { AppConfigComparisonResult } from '../../api/types'

function displayValue(value: string | null, isSensitive: boolean): string {
  if (isSensitive) return '••••••••'
  return value ?? '(null)'
}

export function AppConfigComparisonView({ result }: { result: AppConfigComparisonResult }) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <div className="card">
        <h2>
          Added <span className="muted">({result.added.length})</span>
        </h2>
        {result.added.length === 0 ? (
          <div className="muted">None</div>
        ) : (
          <ul>
            {result.added.map((e) => (
              <li key={e.path} className="mono">
                {e.path} = {displayValue(e.value, e.isSensitive)}
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="card">
        <h2>
          Removed <span className="muted">({result.removed.length})</span>
        </h2>
        {result.removed.length === 0 ? (
          <div className="muted">None</div>
        ) : (
          <ul>
            {result.removed.map((e) => (
              <li key={e.path} className="mono">
                {e.path} = {displayValue(e.value, e.isSensitive)}
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="card">
        <h2>
          Changed <span className="muted">({result.changed.length})</span>
        </h2>
        {result.changed.length === 0 ? (
          <div className="muted">None</div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Path</th>
                <th>Baseline</th>
                <th>Current</th>
              </tr>
            </thead>
            <tbody>
              {result.changed.map((c) => (
                <tr key={c.path}>
                  <td className="mono">{c.path}</td>
                  <td className="mono">{displayValue(c.oldValue, c.isSensitive)}</td>
                  <td className="mono">{displayValue(c.newValue, c.isSensitive)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <div className="muted">{result.unchangedCount} unchanged value(s)</div>
    </div>
  )
}
