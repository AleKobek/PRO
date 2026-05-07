// wybierasz, czy gra ma mieć dane z zewnętrznego serwisu i wybierasz grę.
// Pobierane są zarówno gry użytkownika, jak i gry. Zależnie od wybranej opcji, select ma inną listę do renderowania
import {useEffect, useState} from "react";
import {Bounce, toast} from "react-toastify";
import {API_BASE_URL} from "../config/api";

export default function FormularzWyboruGryDruzyny({
                                                      uzytkownik, 
                                                      ustawIdGryDruzyny, 
                                                      czyZintegrowano, 
                                                      ustawCzyZintegrowano,
                                                  }) {


    const [idWybranejGry, ustawIdWybranejGry] = useState(0);
    // do formularza wyboru gry drużyny (i integracji)
    const [wszystkieGry, ustawWszystkieGry] = useState([]);
    const [gryUzytkownika, ustawGryUzytkownika] = useState([]);


    // Pobranie list gier
    useEffect(() => {
        if (!uzytkownik) return;

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
                toast.error('Wystąpił błąd podczas pobierania danych gier', {
                    position: "top-center",
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: false,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                    theme: "light",
                    transition: Bounce,
                });
                return null;
            }
        };

        const podajGry = async () => {

            // podajemy kraje
            const wszystkieGryPobrane = await fetchJsonAbort(`${API_BASE_URL}/WspieranaGra`);
            if (!alive || !wszystkieGryPobrane || !Array.isArray(wszystkieGryPobrane)) return;
            ustawWszystkieGry(wszystkieGryPobrane);

            // podajemy regiony
            const gryUzytkownikaPobrane = await fetchJsonAbort(`${API_BASE_URL}/BibliotekaGier/${uzytkownik.id}`);
            if(!alive || !gryUzytkownikaPobrane || !Array.isArray(gryUzytkownikaPobrane)) return;
            ustawGryUzytkownika(gryUzytkownikaPobrane);

        };

        podajGry();

        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            ac.abort();
            alive = false; // przerywamy fetch
        }
    }, [uzytkownik, uzytkownik.id]);

    return (<div className="flex flex-col items-center justify-center gap-5 border-4 border-gray-500 rounded-lg p-5 shadow-lg">
        <h3>Formularz wyboru gry</h3>
        <select onChange={(e) => ustawIdWybranejGry(e.target.value)}>
            {czyZintegrowano
                ? gryUzytkownika.map((gra) => <option value={gra.id} key={gra.id}>{gra.tytul}</option>)
                : wszystkieGry.map((gra) => <option value={gra.id} key={gra.id}>{gra.tytul}</option>)
            }
        </select>
        <div>
            <input
            type="checkbox"
            checked={czyZintegrowano}
            onChange={() => ustawCzyZintegrowano(!czyZintegrowano)}
            disabled={gryUzytkownika.length === 0}
            className="mr-2"
            />
            Użyj zintegrowanych danych
        </div>
        <button
            className="bg-green-600 text-white text-2xl p-2 hover:bg-green-500 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
            onClick={() => ustawIdGryDruzyny(idWybranejGry)}>Dalej</button>
    </div>);
}