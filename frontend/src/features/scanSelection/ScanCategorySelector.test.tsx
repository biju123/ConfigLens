import { screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { renderWithProviders } from '../../test/renderWithProviders'
import { ScanCategorySelector } from './ScanCategorySelector'

describe('ScanCategorySelector (flow 2: select scan category)', () => {
  it('toggles a category on and off when clicked', async () => {
    const { store } = renderWithProviders(<ScanCategorySelector />)
    const checkbox = screen.getByRole('checkbox', { name: /AKS Deployment Scan/i })

    await userEvent.click(checkbox)
    expect(store.getState().scanSelection.selectedCategories).toEqual(['AksDeployment'])

    await userEvent.click(checkbox)
    expect(store.getState().scanSelection.selectedCategories).toEqual([])
  })

  it('supports selecting multiple categories independently', async () => {
    const { store } = renderWithProviders(<ScanCategorySelector />)

    await userEvent.click(screen.getByRole('checkbox', { name: /AKS Deployment Scan/i }))
    await userEvent.click(screen.getByRole('checkbox', { name: /Dependency Accessibility Scan/i }))

    expect(store.getState().scanSelection.selectedCategories).toEqual(['AksDeployment', 'DependencyAccessibility'])
  })
})
