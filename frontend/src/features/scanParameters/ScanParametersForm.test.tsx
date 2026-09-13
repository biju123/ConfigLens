import { screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { makeTestStore, renderWithProviders } from '../../test/renderWithProviders'
import { referenceDataFixture } from '../../test/fixtures'
import type { ScanCategory } from '../../api/types'
import { ScanParametersForm } from './ScanParametersForm'

function storeWithSelection(categories: ScanCategory[]) {
  return makeTestStore({ scanSelection: { selectedCategories: categories } })
}

describe('ScanParametersForm (flow 3: enter scan parameters)', () => {
  it('renders nothing when no category is selected', () => {
    renderWithProviders(<ScanParametersForm referenceData={referenceDataFixture} onRun={vi.fn()} running={false} />, {
      store: storeWithSelection([]),
    })

    expect(screen.queryByText(/Enter scan parameters/i)).not.toBeInTheDocument()
  })

  it('renders only the field group for the selected category', () => {
    renderWithProviders(<ScanParametersForm referenceData={referenceDataFixture} onRun={vi.fn()} running={false} />, {
      store: storeWithSelection(['AksDeployment']),
    })

    expect(screen.getByText('AKS Deployment Scan')).toBeInTheDocument()
    expect(screen.queryByText('Application Configuration Scan')).not.toBeInTheDocument()
    expect(screen.queryByText('Configuration Characteristics Scan')).not.toBeInTheDocument()
    expect(screen.queryByText('Dependency Accessibility Scan')).not.toBeInTheDocument()
  })

  it('renders field groups for every selected category', () => {
    renderWithProviders(<ScanParametersForm referenceData={referenceDataFixture} onRun={vi.fn()} running={false} />, {
      store: storeWithSelection(['AksDeployment', 'DependencyAccessibility']),
    })

    expect(screen.getByText('AKS Deployment Scan')).toBeInTheDocument()
    expect(screen.getByText('Dependency Accessibility Scan')).toBeInTheDocument()
    expect(screen.queryByText('Application Configuration Scan')).not.toBeInTheDocument()
  })
})
