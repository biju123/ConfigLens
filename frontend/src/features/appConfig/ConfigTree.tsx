import { useState } from 'react'
import type { ConfigTree as ConfigTreeData } from '../../api/types'
import { ConfigSearchBox } from './ConfigSearchBox'
import { ConfigTreeNode } from './ConfigTreeNode'

export function ConfigTree({ tree }: { tree: ConfigTreeData }) {
  const [searchTerm, setSearchTerm] = useState('')

  return (
    <div className="card">
      <h2 className="mono">{tree.rootName}</h2>
      <ConfigSearchBox value={searchTerm} onChange={setSearchTerm} />
      {tree.sections.length === 0 ? (
        <div className="muted">No configuration was found for this application, environment, tenant and session year.</div>
      ) : (
        tree.sections.map((section) => <ConfigTreeNode key={section.name} section={section} searchTerm={searchTerm} />)
      )}
    </div>
  )
}
