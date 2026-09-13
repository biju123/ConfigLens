import { Link } from 'react-router-dom'
import type { ScanRecord } from '../../api/types'
import { StatusBadge } from '../../components/common/StatusBadge'

export function ScanHistoryTable({ scans }: { scans: ScanRecord[] }) {
  return (
    <table className="data-table">
      <thead>
        <tr>
          <th>Scan ID</th>
          <th>Category</th>
          <th>Started (UTC)</th>
          <th>Environment</th>
          <th>Tenant / Application</th>
          <th>Status</th>
          <th>Initiated by</th>
        </tr>
      </thead>
      <tbody>
        {scans.map(({ metadata }) => (
          <tr key={metadata.scanId}>
            <td className="mono">
              <Link to={`/results/${metadata.scanId}`}>{metadata.scanId}</Link>
            </td>
            <td>{metadata.category}</td>
            <td>{new Date(metadata.startedAtUtc).toLocaleString()}</td>
            <td>{metadata.environment ?? '—'}</td>
            <td>{metadata.tenant ?? metadata.application ?? '—'}</td>
            <td>
              <StatusBadge status={metadata.status} />
            </td>
            <td>{metadata.initiatingUser}</td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
