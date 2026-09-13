import type { Finding } from '../../api/types'
import { SeverityBadge } from '../../components/common/SeverityBadge'
import { StatusBadge } from '../../components/common/StatusBadge'

export function FindingDetailDrawer({ finding, onClose }: { finding: Finding; onClose: () => void }) {
  return (
    <div className="card">
      <div className="section-heading">
        <h2>{finding.ruleName}</h2>
        <button className="btn" onClick={onClose}>
          Close
        </button>
      </div>
      <div style={{ display: 'flex', gap: 8, marginBottom: 'var(--space-3)' }}>
        <StatusBadge status={finding.status} />
        <SeverityBadge severity={finding.severity} />
      </div>
      <dl style={{ display: 'grid', gridTemplateColumns: '140px 1fr', rowGap: 8, columnGap: 12, margin: 0 }}>
        <dt className="muted">Resource</dt>
        <dd className="mono" style={{ margin: 0 }}>
          {finding.resource}
        </dd>
        <dt className="muted">Description</dt>
        <dd style={{ margin: 0 }}>{finding.description}</dd>
        <dt className="muted">Expected</dt>
        <dd style={{ margin: 0 }}>{finding.expectedCondition}</dd>
        <dt className="muted">Actual</dt>
        <dd style={{ margin: 0 }}>{finding.actualCondition}</dd>
        {finding.recommendation && (
          <>
            <dt className="muted">Recommendation</dt>
            <dd style={{ margin: 0 }}>{finding.recommendation}</dd>
          </>
        )}
      </dl>
    </div>
  )
}
