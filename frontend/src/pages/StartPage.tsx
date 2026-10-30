import { Link } from "react-router"

export function StartPage() {
    return (
        <section>
            <h1>Azubi-Gehalt-Check</h1>
                <p>
                Finde in wenigen Schritten heraus, welche Leistungen dir als
                Auszubildende*r zustehen könnten.
                </p>
                <p className="hinweis">
                    Hinweis: Das Ergebnis ist eine Orientierung und keine Rechtsberatung.
                </p>
                <Link to="/check">Jetzt prüfen</Link>
        </section>
    )
}
 