/** An editable dropdown: a text input backed by a native <datalist> of suggestions, so the
 *  user can either pick a known value or type one that isn't in the list yet (e.g. a
 *  subscription id or namespace the reference data doesn't know about). */
export function ComboBoxField({
  id,
  label,
  value,
  onChange,
  onCommit,
  options,
  placeholder,
  loading,
}: {
  id: string
  label: string
  value: string
  onChange: (value: string) => void
  /** Fires on blur and on Enter - the "the user is done editing this field" moment. */
  onCommit?: (value: string) => void
  options: string[]
  placeholder?: string
  loading?: boolean
}) {
  const listId = `${id}-options`
  return (
    <div className="field">
      <label htmlFor={id}>
        {label}
        {loading ? ' (loading…)' : ''}
      </label>
      <input
        id={id}
        className="input"
        list={listId}
        value={value}
        placeholder={placeholder}
        autoComplete="off"
        onChange={(e) => onChange(e.target.value)}
        onBlur={(e) => onCommit?.(e.target.value)}
        onKeyDown={(e) => {
          if (e.key === 'Enter') onCommit?.(value)
        }}
      />
      <datalist id={listId}>
        {options.map((option) => (
          <option key={option} value={option} />
        ))}
      </datalist>
    </div>
  )
}
