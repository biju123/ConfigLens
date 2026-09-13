import type { ReactNode } from 'react'
import { NavLink } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '../../app/hooks'
import { loggedOut } from '../../features/auth/authSlice'
import styles from './AppShell.module.css'

const navLinkClassName = ({ isActive }: { isActive: boolean }) =>
  isActive ? `${styles.navLink} ${styles.navLinkActive}` : styles.navLink

export function AppShell({ children }: { children: ReactNode }) {
  const dispatch = useAppDispatch()
  const username = useAppSelector((state) => state.auth.username)

  return (
    <div className={styles.shell}>
      <header className={styles.topBar}>
        <div className={styles.brand}>
          <span className="dot" style={{ width: 8, height: 8, borderRadius: '50%', background: 'var(--accent)' }} />
          ConfigLens
        </div>
        <nav className={styles.nav}>
          <NavLink to="/new-scan" className={navLinkClassName}>
            New Scan
          </NavLink>
          <NavLink to="/history" className={navLinkClassName}>
            Scan History
          </NavLink>
          <NavLink to="/compare" className={navLinkClassName}>
            Compare
          </NavLink>
        </nav>
        <div className={styles.userArea}>
          <span>{username}</span>
          <button className="btn" onClick={() => dispatch(loggedOut())}>
            Sign out
          </button>
        </div>
      </header>
      <main className={styles.main}>{children}</main>
    </div>
  )
}
