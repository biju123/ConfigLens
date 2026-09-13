export function ConfigSearchBox({ value, onChange }: { value: string; onChange: (value: string) => void }) {
  return (
    <input
      className="input"
      placeholder="Search configuration keys…"
      value={value}
      onChange={(e) => onChange(e.target.value)}
      style={{ width: '100%', marginBottom: 'var(--space-3)' }}
    />
  )
}
