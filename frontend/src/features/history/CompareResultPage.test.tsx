import { render, screen } from '@testing-library/react'
import { Provider } from 'react-redux'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { describe, expect, it } from 'vitest'
import { makeTestStore } from '../../test/renderWithProviders'
import { CompareResultPage } from './CompareResultPage'

describe('CompareResultPage (flow 12: view category-specific comparison)', () => {
  it('renders the AKS-specific comparison view for the compared category', async () => {
    render(
      <Provider store={makeTestStore()}>
        <MemoryRouter initialEntries={['/compare/SCAN-20260912-00001/SCAN-20260911-00001']}>
          <Routes>
            <Route path="/compare/:currentScanId/:baselineScanId" element={<CompareResultPage />} />
          </Routes>
        </MemoryRouter>
      </Provider>,
    )

    expect(await screen.findByText('Comparison: AksDeployment')).toBeInTheDocument()
    expect(screen.getByText(/Added resources/)).toBeInTheDocument()
    expect(screen.getByText(/assessor-prod \/ Deployment \/ assessor-api/)).toBeInTheDocument()
  })
})
