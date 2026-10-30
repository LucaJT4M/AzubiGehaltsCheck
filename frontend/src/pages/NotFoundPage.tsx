import { useNavigate } from "react-router";

export function NotFoundPage() {
    const navigate = useNavigate()

    return(
    <section>
        <h1>Die Webseite oder Link wurde leider nicht gefunden</h1>
        <button type="button" className="button button-sekundaer"  onClick={() => navigate('/')}>zum Start Page</button>

    </section>
    )
    
}