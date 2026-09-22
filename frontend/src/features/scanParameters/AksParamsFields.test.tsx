import { useState } from 'react'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { referenceDataFixture } from '../../test/fixtures'
import { AksParamsFields } from './AksParamsFields'
import type { AksParamsState } from './paramTypes'

function Harness({ initial }: { initial: AksParamsState }) {
  const [value, setValue] = useState(initial)
  return <AksParamsFields value={value} onChange={setValue} referenceData={referenceDataFixture} />
}

function datalistValues(datalistId: string): string[] {
  return Array.from(document.querySelectorAll(`#${datalistId} option`)).map((option) => option.getAttribute('value') ?? '')
}

describe('AksParamsFields (flow 3: enter scan parameters for the AKS Deployment Scan)', () => {
  it('populates the AKS cluster dropdown after the user enters a subscription id', async () => {
    render(
      <Harness
        initial={{ subscriptionId: '', clusterName: '', environment: 'Production', tenant: 'ContosoCounty', namespace: '' }}
      />,
    )

    const subscriptionInput = screen.getByLabelText(/Azure subscription/i)
    await userEvent.type(subscriptionInput, 'sub-assessor-prod-01')
    await userEvent.tab()

    const clusterSelect = await screen.findByLabelText(/AKS cluster/i)
    await waitFor(() => expect(clusterSelect).toHaveValue('aks-assessor-prod-eastus'))
    expect(screen.getByRole('option', { name: 'aks-assessor-prod-westus' })).toBeInTheDocument()
  })

  it('re-populates the namespace suggestions when the user picks a different cluster', async () => {
    render(
      <Harness
        initial={{ subscriptionId: '', clusterName: '', environment: 'Production', tenant: 'ContosoCounty', namespace: '' }}
      />,
    )

    // Entering the subscription auto-selects the first cluster (aks-assessor-prod-eastus) and loads its namespaces.
    await userEvent.type(screen.getByLabelText(/Azure subscription/i), 'sub-assessor-prod-01')
    await userEvent.tab()
    const clusterSelect = await screen.findByLabelText(/AKS cluster/i)
    await waitFor(() => expect(clusterSelect).toHaveValue('aks-assessor-prod-eastus'))
    await waitFor(() => expect(datalistValues('aks-namespace-options')).toContain('assessor-prod-riverbend'))

    // Switching to a cluster with no configured namespaces (per the mock handler) clears the suggestions.
    await userEvent.selectOptions(clusterSelect, 'aks-assessor-prod-westus')

    await waitFor(() => expect(datalistValues('aks-namespace-options')).not.toContain('assessor-prod-riverbend'))
  })

  it('lets the subscription field accept a value that is not in the reference data', async () => {
    render(
      <Harness
        initial={{ subscriptionId: '', clusterName: '', environment: 'Production', tenant: 'ContosoCounty', namespace: '' }}
      />,
    )

    const subscriptionInput = screen.getByLabelText(/Azure subscription/i)
    await userEvent.type(subscriptionInput, 'sub-not-in-the-list')

    expect(subscriptionInput).toHaveValue('sub-not-in-the-list')
  })
})
