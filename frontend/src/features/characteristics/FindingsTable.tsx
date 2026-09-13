import { useMemo, useState } from 'react'
import type { Finding, FindingStatus } from '../../api/types'
import { SeverityBadge } from '../../components/common/SeverityBadge'
import { StatusBadge } from '../../components/common/StatusBadge'

const STATUS_FILTERS: (FindingStatus | 'All')[] = ['All', 'Fail', 'Warning', 'Pass', 'NotChecked', 'NotApplicable', 'Error']

export function FindingsTable({ findings, onSelect }: { findings: Finding[]; onSelect: (finding: Finding) => void }) {
  const [statusFilter, setStatusFilter] = useState<FindingStatus | 'All'>('All')

  const filtered = useMemo(
    () => (statusFilter === 'All' ? findings : findings.filter((f) => f.status === statusFilter)),
    [findings, statusFilter],
  )

  return (
    <div className="card">
      <div className="section-heading">
        <h2>Findings</h2>
        <select className="input" value={statusFilter} onChange={(e) => setStatusFilter(e.target.value as FindingStatus | 'All')}>
          {STATUS_FILTERS.map((status) => (
            <option key={status} value={status}>
              {status}
            </option>
          ))}
        </select>
      </div>
      <table className="data-table">
        <thead>
          <tr>
            <th>Status</th>
            <th>Severity</th>
            <th>Rule</th>
            <th>Resource</th>
            <th>Description</th>
          </tr>
        </thead>
        <tbody>
          {filtered.map((finding) => (
            <tr key={finding.findingId} onClick={() => onSelect(finding)}>
              <td>
                <StatusBadge status={finding.status} />
              </td>
              <td>
                <SeverityBadge severity={finding.severity} />
              </td>
              <td>{finding.ruleName}</td>
              <td className="mono">{finding.resource}</td>
              <td>{finding.description}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
