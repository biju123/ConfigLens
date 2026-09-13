export function SelectField({
  id,
  label,
  value,
  onChange,
  options,
  allowEmpty,
  emptyLabel = 'All',
}: {
  id: string
  label: string
  value: string
  onChange: (value: string) => void
  options: string[]
  allowEmpty?: boolean
  emptyLabel?: string
}) {
  return (
    <div className="field">
      <label htmlFor={id}>{label}</label>
      <select id={id} className="input" value={value} onChange={(e) => onChange(e.target.value)}>
        {allowEmpty && <option value="">{emptyLabel}</option>}
        {options.map((option) => (
          <option key={option} value={option}>
            {option}
          </option>
        ))}
      </select>
    </div>
  )
}
