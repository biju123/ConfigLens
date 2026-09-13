import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import type { AppErrorKind } from '../../api/errors'
import { ErrorBanner } from './ErrorBanner'

const ALL_KINDS: AppErrorKind[] = [
  'invalid-input',
  'authentication-failure',
  'authorization-failure',
  'infrastructure-failure',
  'application-failure',
  'dependency-failure',
  'timeout',
  'validation-failure',
]

describe('ErrorBanner', () => {
  it.each(ALL_KINDS)('renders a distinct, readable label for kind "%s"', (kind) => {
    render(<ErrorBanner error={{ kind, message: `message for ${kind}` }} />)

    expect(screen.getByRole('alert')).toHaveTextContent(`message for ${kind}`)
  })

  it('renders field-level validation errors when present', () => {
    render(
      <ErrorBanner
        error={{
          kind: 'invalid-input',
          message: 'One or more validation errors occurred.',
          fieldErrors: { Tenant: ["'Tenant' must not be empty."] },
        }}
      />,
    )

    expect(screen.getByText(/Tenant:/)).toBeInTheDocument()
  })
})
