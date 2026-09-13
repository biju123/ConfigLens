import { useState } from 'react'
import type { CharacteristicsSummaryCounts, Finding, ScanMetadata } from '../../api/types'
import { FindingDetailDrawer } from './FindingDetailDrawer'
import { FindingsTable } from './FindingsTable'
import { ValidationSummary } from './ValidationSummary'

export function CharacteristicsResultsView({
  metadata,
  findings,
  summary,
}: {
  metadata: ScanMetadata
  findings: Finding[]
  summary: CharacteristicsSummaryCounts
}) {
  const [selected, setSelected] = useState<Finding | null>(null)

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <div className="muted">
        Scan <span className="mono">{metadata.scanId}</span> · {metadata.application} · {metadata.environment} ·{' '}
        {metadata.tenant}
      </div>
      <ValidationSummary summary={summary} />
      {selected && <FindingDetailDrawer finding={selected} onClose={() => setSelected(null)} />}
      <FindingsTable findings={findings} onSelect={setSelected} />
    </div>
  )
}
