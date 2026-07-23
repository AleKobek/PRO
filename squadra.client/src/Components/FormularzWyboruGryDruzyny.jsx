// wybierasz, czy gra ma mieć dane z zewnętrznego serwisu i wybierasz grę.
// Pobierane są zarówno gry użytkownika, jak i gry. Zależnie od wybranej opcji, select ma inną listę do renderowania
import React, {useEffect, useState} from "react";
import {Bounce, toast} from "react-toastify";
import {API_BASE_URL} from "../config/api";

export default function FormularzWyboruGryDruzyny({
                                                      uzytkownik, 
                                                      ustawIdGryDruzyny, 
                                                      czyZintegrowano, 
                                                      ustawCzyZintegrowano,
                                                      ustawPokazOkienkoTlumaczenia
                                                  }) {


    const [idWybranejGry, ustawIdWybranejGry] = useState(0);
    const [wszystkieGry, ustawWszystkieGry] = useState([]);
    const [gryUzytkownika, ustawGryUzytkownika] = useState([]);
    const [ladowanie, ustawLadowanie] = useState(true);


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

            // pobieramy wszystkie gry w bazie
            const wszystkieGryPobrane = await fetchJsonAbort(`${API_BASE_URL}/WspieraneGry`);
            if (!alive || !wszystkieGryPobrane || !Array.isArray(wszystkieGryPobrane)) return;
            ustawWszystkieGry(wszystkieGryPobrane);

            // pobieramy gry z biblioteki użytkownika
            const gryUzytkownikaPobrane = await fetchJsonAbort(`${API_BASE_URL}/BibliotekaGier/${uzytkownik.id}`);
            if(!alive || !gryUzytkownikaPobrane || !Array.isArray(gryUzytkownikaPobrane)) return;
            ustawGryUzytkownika(gryUzytkownikaPobrane);

            if(gryUzytkownikaPobrane.length === 0) ustawIdWybranejGry(wszystkieGryPobrane[0].id);
            else ustawIdWybranejGry(gryUzytkownikaPobrane[0].idGry);
        };

        podajGry().then(() => {
            ustawLadowanie(false);
        });

        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            ac.abort();
            alive = false; // przerywamy fetch
        }
    }, [uzytkownik, uzytkownik.id]);

    if(ladowanie) return (<div className="flex flex-col items-center justify-center gap-5 border-4 border-gray-500 rounded-lg p-5 shadow-lg">
        <h3>Formularz wyboru gry</h3>
        <h1>Ładowanie...</h1>
    </div>)

    return (<div className="flex flex-col items-center justify-center gap-5 border-4 border-gray-500 rounded-lg p-5 shadow-lg">
        <h3>Formularz wyboru gry</h3>
        <select
            onChange={(e) => ustawIdWybranejGry(e.target.value)}
            className="border-2 border-gray-300 rounded-md p-2"
        >
            {czyZintegrowano
                ? gryUzytkownika.map((gra) => <option value={gra.idGry} key={gra.idGry}>{gra.tytul}</option>)
                : wszystkieGry.map((gra) => <option value={gra.id} key={gra.id}>{gra.tytul}</option>)
            }
        </select>
        <div className="flex items-center justify-center">
            <input
            type="checkbox"
            checked={czyZintegrowano}
            onChange={() => {
                // gdy zmieniamy z niezintegrowanego na zintegrowane
                if(!czyZintegrowano){
                    ustawIdWybranejGry(gryUzytkownika[0] ? gryUzytkownika[0].idGry : null);
                }
                ustawCzyZintegrowano(!czyZintegrowano)
            }}
            disabled={gryUzytkownika.length === 0}
            className="mr-2"
            />
            Użyj zintegrowanych danych
            <img
                src="/img/znak-zapytania.svg"
                alt="znak zapytania"
                className="h-[1em] w-auto align-middle ml-2 cursor-pointer"
                onClick={() => ustawPokazOkienkoTlumaczenia(true)}
            />
        </div>
        <button
            className="bg-green-600 text-white text-2xl p-2 hover:bg-green-500 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
            onClick={() => {
                ustawIdGryDruzyny(idWybranejGry)
            }}>Dalej</button>
    </div>);
}