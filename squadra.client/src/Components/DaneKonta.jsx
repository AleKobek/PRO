import {useEffect, useState} from "react";
import ListaJezykow from "./ListaJezykow";

// pamiętać o tym, aby to było w nawiasach klamrowych!
export default function DaneKonta({uzytkownik}) {

    const [login, ustawLogin] = useState("");
    const [email, ustawEmail] = useState("");
    // {id, nazwa}
    const [numerTelefonu, ustawNumerTelefonu] = useState("");
    // {id, nazwa}
    const [dataUrodzenia, ustawDateUrodzenia] = useState(null);

    useEffect(() => {

        // czekamy aż się załaduje id użytkownika
        if(!uzytkownik) return;
        if(!uzytkownik.id) return;

        const ac = new AbortController();
        let alive = true;

        // taka pomocnicza funkcja dla abort controller
        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                if (!res.ok) return null;
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                return null;
            }
        };

        const podajDaneKonta = async () => {
            const idUzytkownika = uzytkownik.id;
            const data = await fetchJsonAbort(`http://localhost:5014/api/Uzytkownik/${idUzytkownika}`);

            // przerywamy działanie funkcji
            if (!alive) return;

            ustawLogin(data.login ?? "");
            ustawEmail(data.email ?? "");
            ustawNumerTelefonu(data.numerTelefonu ?? "");
            ustawDateUrodzenia(data.dataUrodzenia ?? null);
        };

        if(!login && !email) {
            podajDaneKonta();
        }
        
        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };
    }, [uzytkownik]);


    if(!uzytkownik || !uzytkownik.id) return(<><p>Ładowanie...</p></>);

    return(<div className = "dane-konta">
        <label>
            Login:
            <p id = "login" className= "pole-w-danych-konta">{login}</p>
        </label>
        <label>
            Email:
            <p id= "email" className= "pole-w-danych-konta">{email}</p>
        </label>
        <label>
            Data urodzenia:
            <p id = "data-urodzenia" className= "pole-w-danych-konta">{dataUrodzenia ? dataUrodzenia : "Nie określono"}</p>
        </label>
        <label>
            Numer telefonu:
            <p id = "numer-telefonu" className= "pole-w-danych-konta">{numerTelefonu ? numerTelefonu : "Nie podano"}</p>
        </label>
    </div>)
}