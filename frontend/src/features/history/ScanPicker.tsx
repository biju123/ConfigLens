import type { ScanRecord } from '../../api/types'

export function ScanPicker({
  id,
  label,
  scans,
  value,
  onChange,
}: {
  id: string
  label: string
  scans: ScanRecord[]
  value: string
  onChange: (scanId: string) => void
}) {
  return (
    <div className="field">
      <label htmlFor={id}>{label}</label>
      <select id={id} className="input" value={value} onChange={(e) => onChange(e.target.value)}>
        <option value="">Select a scan…</option>
        {scans.map(({ metadata }) => (
          <option key={metadata.scanId} value={metadata.scanId}>
            {metadata.scanId} · {new Date(metadata.startedAtUtc).toLocaleString()}
          </option>
        ))}
      </select>
    </div>
  )
}
