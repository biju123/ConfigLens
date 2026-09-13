import type { AksResource } from '../../api/types'
import { AksResourceTable } from './AksResourceTable'

function groupBy<T>(items: T[], keyFn: (item: T) => string): Map<string, T[]> {
  const map = new Map<string, T[]>()
  for (const item of items) {
    const key = keyFn(item)
    const group = map.get(key)
    if (group) group.push(item)
    else map.set(key, [item])
  }
  return map
}

export function AksNamespaceGroup({
  namespace,
  resources,
  onSelect,
}: {
  namespace: string
  resources: AksResource[]
  onSelect: (resource: AksResource) => void
}) {
  const byApp = groupBy(resources, (r) => r.appOrService)

  return (
    <div className="card">
      <h2 className="mono">{namespace}</h2>
      {[...byApp.entries()].map(([app, appResources]) => (
        <div key={app} style={{ marginBottom: 'var(--space-4)' }}>
          <h3>{app}</h3>
          <AksResourceTable resources={appResources} onSelect={onSelect} />
        </div>
      ))}
    </div>
  )
}
