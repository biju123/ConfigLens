import type { AksResource } from '../../api/types'

const FIELD_LABELS: [keyof AksResource, string][] = [
  ['resourceType', 'Resource type'],
  ['resourceName', 'Resource name'],
  ['namespace', 'Namespace'],
  ['appOrService', 'Application / service'],
  ['desiredReplicas', 'Desired replicas'],
  ['runningReplicas', 'Running replicas'],
  ['readyReplicas', 'Ready replicas'],
  ['cpuRequest', 'CPU request'],
  ['cpuLimit', 'CPU limit'],
  ['memoryRequest', 'Memory request'],
  ['memoryLimit', 'Memory limit'],
  ['containerCount', 'Container count'],
  ['image', 'Image / version'],
  ['podStatus', 'Pod status'],
  ['restartCount', 'Restart count'],
  ['schedule', 'Schedule'],
  ['lastScheduleTimeUtc', 'Last scheduled (UTC)'],
  ['minReplicas', 'Min replicas'],
  ['maxReplicas', 'Max replicas'],
  ['currentMetricValue', 'Current metric'],
  ['clusterIp', 'Cluster IP'],
  ['ports', 'Ports'],
]

export function AksResourceDrawer({ resource, onClose }: { resource: AksResource; onClose: () => void }) {
  return (
    <div className="card" style={{ position: 'sticky', top: 0 }}>
      <div className="section-heading">
        <h2>{resource.resourceName}</h2>
        <button className="btn" onClick={onClose}>
          Close
        </button>
      </div>
      <dl style={{ display: 'grid', gridTemplateColumns: '160px 1fr', rowGap: 8, columnGap: 12, margin: 0 }}>
        {FIELD_LABELS.filter(([key]) => resource[key] !== null && resource[key] !== undefined).map(([key, label]) => (
          <div key={key} style={{ display: 'contents' }}>
            <dt className="muted">{label}</dt>
            <dd className="mono" style={{ margin: 0 }}>
              {String(resource[key])}
            </dd>
          </div>
        ))}
      </dl>
    </div>
  )
}
