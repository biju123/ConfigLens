import type { ConfigTree as ConfigTreeData, ScanMetadata } from '../../api/types'
import { ConfigTree } from './ConfigTree'

export function AppConfigResultsView({ metadata, tree }: { metadata: ScanMetadata; tree: ConfigTreeData }) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <div className="muted">
        Scan <span className="mono">{metadata.scanId}</span> · {metadata.application} · {metadata.environment} ·{' '}
        {metadata.tenant} · session {metadata.sessionYear}
      </div>
      <ConfigTree tree={tree} />
    </div>
  )
}
