import type { CharacteristicsSummaryCounts } from '../../api/types'
import { StatusBadge } from '../../components/common/StatusBadge'

const ROWS: [keyof CharacteristicsSummaryCounts, string][] = [
  ['pass', 'Pass'],
  ['warning', 'Warning'],
  ['fail', 'Fail'],
  ['notChecked', 'NotChecked'],
  ['notApplicable', 'NotApplicable'],
  ['error', 'Error'],
]

export function ValidationSummary({ summary }: { summary: CharacteristicsSummaryCounts }) {
  return (
    <div className="card" style={{ display: 'flex', gap: 'var(--space-5)', flexWrap: 'wrap' }}>
      {ROWS.map(([key, status]) => (
        <div key={key}>
          <StatusBadge status={status} />
          <div style={{ fontSize: 22, fontWeight: 700, marginTop: 4 }}>{summary[key]}</div>
        </div>
      ))}
    </div>
  )
}
