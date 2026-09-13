import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { scanApi } from '../../api/scanApi'
import type { ScanCategory, ScanRecord } from '../../api/types'
import { ScanPicker } from './ScanPicker'

const CATEGORIES: ScanCategory[] = [
  'AksDeployment',
  'ApplicationConfiguration',
  'ConfigurationCharacteristics',
  'DependencyAccessibility',
]

export function ComparisonSetupPage() {
  const navigate = useNavigate()
  const [category, setCategory] = useState<ScanCategory>('AksDeployment')
  const [scans, setScans] = useState<ScanRecord[]>([])
  const [currentScanId, setCurrentScanId] = useState('')
  const [baselineScanId, setBaselineScanId] = useState('')

  useEffect(() => {
    setCurrentScanId('')
    setBaselineScanId('')
    scanApi.queryScans(category).then(setScans)
  }, [category])

  const canCompare = currentScanId && baselineScanId && currentScanId !== baselineScanId

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
      <h1>Compare Scans</h1>
      <p className="muted">
        Comparison is a separate, explicit action. Select the current scan and a baseline scan of the same category,
        then select Compare.
      </p>
      <div className="card" style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-3)', maxWidth: 480 }}>
        <div className="field">
          <label htmlFor="compare-category">Scan category</label>
          <select id="compare-category" className="input" value={category} onChange={(e) => setCategory(e.target.value as ScanCategory)}>
            {CATEGORIES.map((c) => (
              <option key={c} value={c}>
                {c}
              </option>
            ))}
          </select>
        </div>
        <ScanPicker id="compare-current" label="Current scan" scans={scans} value={currentScanId} onChange={setCurrentScanId} />
        <ScanPicker id="compare-baseline" label="Baseline scan" scans={scans} value={baselineScanId} onChange={setBaselineScanId} />
        <button
          className="btn btn-primary"
          disabled={!canCompare}
          onClick={() => navigate(`/compare/${currentScanId}/${baselineScanId}`)}
        >
          Compare
        </button>
      </div>
    </div>
  )
}
