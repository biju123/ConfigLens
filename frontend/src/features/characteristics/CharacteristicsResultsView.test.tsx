import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { characteristicsScanResponseFixture } from '../../test/fixtures'
import { CharacteristicsResultsView } from './CharacteristicsResultsView'

describe('CharacteristicsResultsView (flow 9: view findings)', () => {
  it('shows the validation summary counts and every finding', () => {
    render(
      <CharacteristicsResultsView
        metadata={characteristicsScanResponseFixture.metadata}
        findings={characteristicsScanResponseFixture.findings}
        summary={characteristicsScanResponseFixture.summary}
      />,
    )

    expect(screen.getAllByText('1', { selector: 'div' })).toHaveLength(2) // pass count and fail count
    expect(screen.getByText('Database retry count is required')).toBeInTheDocument()
    expect(screen.getByText('Database connection string is required')).toBeInTheDocument()
  })
})
