import { NavLink, Route, Routes } from 'react-router'
import { CheckPage } from './pages/CheckPage'
import { NotFoundPage } from './pages/NotFoundPage'
import { StartPage } from './pages/StartPage'

// Wurzelkomponente: Navigation und Routen der Anwendung.
export default function App() {
  return (
    <>
      <header>
        <nav aria-label="Hauptnavigation">
          <NavLink to="/" end>Start</NavLink>
          <NavLink to="/check">Anspruch prüfen</NavLink>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<StartPage />} />
          <Route path="/check" element={<CheckPage />} />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </main>
    </>
  )
}