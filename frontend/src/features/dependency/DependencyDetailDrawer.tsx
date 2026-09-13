import type { DependencyCheckResult } from '../../api/types'
import { StatusBadge } from '../../components/common/StatusBadge'

export function DependencyDetailDrawer({ check, onClose }: { check: DependencyCheckResult; onClose: () => void }) {
  return (
    <div className="card">
      <div className="section-heading">
        <h2>{check.dependencyType}</h2>
        <button className="btn" onClick={onClose}>
          Close
        </button>
      </div>
      <div style={{ marginBottom: 'var(--space-3)' }}>
        <StatusBadge status={check.status} />
      </div>
      <dl style={{ display: 'grid', gridTemplateColumns: '140px 1fr', rowGap: 8, columnGap: 12, margin: 0 }}>
        <dt className="muted">Application</dt>
        <dd style={{ margin: 0 }}>{check.application}</dd>
        <dt className="muted">Target</dt>
        <dd className="mono" style={{ margin: 0 }}>
          {check.targetDescription}
        </dd>
        {check.failureReason && (
          <>
            <dt className="muted">Failure reason</dt>
            <dd style={{ margin: 0 }}>{check.failureReason}</dd>
          </>
        )}
        {check.latencyMs != null && (
          <>
            <dt className="muted">Latency</dt>
            <dd style={{ margin: 0 }}>{check.latencyMs} ms</dd>
          </>
        )}
        <dt className="muted">Checked at</dt>
        <dd style={{ margin: 0 }}>{new Date(check.checkedAtUtc).toLocaleString()}</dd>
      </dl>
    </div>
  )
}
