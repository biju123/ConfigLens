import { useAppDispatch, useAppSelector } from '../../app/hooks'
import type { ScanCategory } from '../../api/types'
import { toggleCategory } from './scanSelectionSlice'

const CATEGORIES: { value: ScanCategory; label: string; description: string }[] = [
  { value: 'AksDeployment', label: 'AKS Deployment Scan', description: 'Deployments, pods, cronjobs, HPA, KEDA, services' },
  { value: 'ApplicationConfiguration', label: 'Application Configuration Scan', description: 'Hierarchical configuration, masked sensitive values' },
  { value: 'ConfigurationCharacteristics', label: 'Configuration Characteristics Scan', description: 'Validation rules, pass/warn/fail findings' },
  { value: 'DependencyAccessibility', label: 'Dependency Accessibility Scan', description: 'Azure SQL, Storage, Service Bus, Key Vault, external APIs' },
]

export function ScanCategorySelector() {
  const dispatch = useAppDispatch()
  const selected = useAppSelector((state) => state.scanSelection.selectedCategories)

  return (
    <div className="card">
      <h2>1. Select scan categories</h2>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-2)' }}>
        {CATEGORIES.map((category) => (
          <label
            key={category.value}
            style={{ display: 'flex', gap: 'var(--space-3)', alignItems: 'flex-start', cursor: 'pointer' }}
          >
            <input
              type="checkbox"
              checked={selected.includes(category.value)}
              onChange={() => dispatch(toggleCategory(category.value))}
              style={{ marginTop: 3 }}
            />
            <span>
              <div>{category.label}</div>
              <div className="muted">{category.description}</div>
            </span>
          </label>
        ))}
      </div>
    </div>
  )
}
