import { useState } from 'react'
import type { AksResource, ScanMetadata } from '../../api/types'
import { AksNamespaceGroup } from './AksNamespaceGroup'
import { AksResourceDrawer } from './AksResourceDrawer'

export function AksResultsView({ metadata, resources }: { metadata: ScanMetadata; resources: AksResource[] }) {
  const [selected, setSelected] = useState<AksResource | null>(null)
  const namespaces = [...new Set(resources.map((r) => r.namespace))].sort()

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <div className="section-heading">
        <span className="muted">
          Scan <span className="mono">{metadata.scanId}</span> · {resources.length} resources across {namespaces.length}{' '}
          namespace(s)
        </span>
      </div>
      {selected && <AksResourceDrawer resource={selected} onClose={() => setSelected(null)} />}
      {namespaces.length === 0 ? (
        <div className="muted">No AKS resources matched this scan's parameters.</div>
      ) : (
        namespaces.map((namespace) => (
          <AksNamespaceGroup
            key={namespace}
            namespace={namespace}
            resources={resources.filter((r) => r.namespace === namespace)}
            onSelect={setSelected}
          />
        ))
      )}
    </div>
  )
}
