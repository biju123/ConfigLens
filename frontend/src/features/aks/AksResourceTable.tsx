import type { AksResource } from '../../api/types'
import { StatusBadge } from '../../components/common/StatusBadge'

export function AksResourceTable({
  resources,
  onSelect,
}: {
  resources: AksResource[]
  onSelect: (resource: AksResource) => void
}) {
  return (
    <table className="data-table">
      <thead>
        <tr>
          <th>Type</th>
          <th>Name</th>
          <th>Replicas</th>
          <th>CPU</th>
          <th>Memory</th>
          <th>Image</th>
          <th>Status</th>
          <th>Restarts</th>
        </tr>
      </thead>
      <tbody>
        {resources.map((resource) => (
          <tr key={`${resource.resourceType}-${resource.resourceName}`} onClick={() => onSelect(resource)}>
            <td>{resource.resourceType}</td>
            <td className="mono">{resource.resourceName}</td>
            <td>
              {resource.desiredReplicas != null
                ? `${resource.runningReplicas ?? 0}/${resource.desiredReplicas} (ready ${resource.readyReplicas ?? 0})`
                : '—'}
            </td>
            <td>{resource.cpuRequest ? `${resource.cpuRequest} / ${resource.cpuLimit}` : '—'}</td>
            <td>{resource.memoryRequest ? `${resource.memoryRequest} / ${resource.memoryLimit}` : '—'}</td>
            <td className="mono">{resource.image ?? '—'}</td>
            <td>{resource.podStatus ? <StatusBadge status={resource.podStatus} /> : '—'}</td>
            <td>{resource.restartCount ?? '—'}</td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
