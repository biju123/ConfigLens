import { useState } from 'react'
import type { DependencyCheckResult, ScanMetadata } from '../../api/types'
import { DependencyDetailDrawer } from './DependencyDetailDrawer'
import { DependencyHealthGrid } from './DependencyHealthGrid'

export function DependencyResultsView({ metadata, checks }: { metadata: ScanMetadata; checks: DependencyCheckResult[] }) {
  const [selected, setSelected] = useState<DependencyCheckResult | null>(null)

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <div className="muted">
        Scan <span className="mono">{metadata.scanId}</span> · {metadata.environment} · {metadata.namespace}
      </div>
      {selected && <DependencyDetailDrawer check={selected} onClose={() => setSelected(null)} />}
      <DependencyHealthGrid checks={checks} onSelect={setSelected} />
    </div>
  )
}
