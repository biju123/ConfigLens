import { useState } from 'react'
import type { ConfigSection } from '../../api/types'

function sectionMatches(section: ConfigSection, term: string): boolean {
  if (!term) return true
  const lower = term.toLowerCase()
  if (section.name.toLowerCase().includes(lower)) return true
  if (section.items.some((item) => item.key.toLowerCase().includes(lower))) return true
  return section.subSections.some((s) => sectionMatches(s, term))
}

export function ConfigTreeNode({ section, searchTerm, depth = 0 }: { section: ConfigSection; searchTerm: string; depth?: number }) {
  const [expanded, setExpanded] = useState(true)

  if (!sectionMatches(section, searchTerm)) {
    return null
  }

  const lower = searchTerm.toLowerCase()
  const visibleItems = section.items.filter((item) => !searchTerm || item.key.toLowerCase().includes(lower))

  return (
    <div style={{ marginLeft: depth === 0 ? 0 : 16 }}>
      <div
        onClick={() => setExpanded((e) => !e)}
        style={{ cursor: 'pointer', display: 'flex', alignItems: 'center', gap: 6, padding: '4px 0' }}
      >
        <span className="muted" style={{ width: 10, display: 'inline-block' }}>
          {expanded ? '▾' : '▸'}
        </span>
        <strong>{section.name}</strong>
      </div>
      {expanded && (
        <div style={{ marginLeft: 16 }}>
          {visibleItems.map((item) => (
            <div key={item.key} style={{ display: 'flex', gap: 8, padding: '3px 0' }}>
              <span className="muted mono" style={{ minWidth: 160 }}>
                {item.key}
              </span>
              <span className="mono">{item.isSensitive ? '••••••••' : (item.value ?? '(null)')}</span>
            </div>
          ))}
          {section.subSections.map((sub) => (
            <ConfigTreeNode key={sub.name} section={sub} searchTerm={searchTerm} depth={depth + 1} />
          ))}
        </div>
      )}
    </div>
  )
}
