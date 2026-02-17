import '../App.css';

import React, {useEffect, useState} from 'react';
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {API_BASE_URL} from "../config/api";
import ZnajomyNaLiscieKomponent from "./ZnajomyNaLiscieKomponent";
export default function TwoiZnajomiStrona() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const [znajomi, ustawZnajomych] = useState([]);
    const [czat, ustawCzat] = useState([]);
    const [idZnajomegoZOtwartymCzatem, ustawIdZnajomegoZOtwartymCzatem] = useState(null);



    // co minutę pobieramy nową listę znajomych, aby była aktualna
    useEffect(() => {
        const ac = new AbortController();
        let alive = true;

        const interval = setInterval(async () => {
            if (!alive) return;
            await podajZnajomych(ac.signal, alive)
        }, 60000);

        // jak wychodzmy to zatrzymujemy nasz interval
        return () => {
            alive = false;
            ac.abort();
            clearInterval(interval);
        };
    }, []);

    // pobieramy listę znajomych z backendu przy załadowaniu strony
    useEffect(() => {
        if(!uzytkownik) return;
        if(!uzytkownik.id) return;

        const ac = new AbortController();
        let alive = true;

        if(!znajomi.length) podajZnajomych(ac.signal, alive);

        if(!idZnajomegoZOtwartymCzatem && znajomi.length) ustawIdZnajomegoZOtwartymCzatem(znajomi[0].id);

        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };

    }, [uzytkownik, znajomi.length]);


    // wydzielone, aby używać przy pobieraniu na początku i co minutę
    const podajZnajomych = async (signal, alive) => {

        // taka pomocnicza funkcja dla abort controller
        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: signal, credentials: "include" });
                if (!res.ok) return null;
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                return null;
            }
        };

        const znajomi = await fetchJsonAbort(`${API_BASE_URL}/Znajomi`, {signal});
        if(!alive || !znajomi) return;
        ustawZnajomych(znajomi);
    }

    // mam pobieranie znajomych, przydałoby się najpierw zrobić listę znajomych, potem wezmę się za czat




    if(ladowanie) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna" className="!p-0 !m-0">
            <div className="grid grid-cols-3 h-f">
                {/* znajomi */}
                <div className="col-span-1 flex flex-col border-r-8 border-blue-500 py-4 h-full">
                    <h1>Twoi znajomi</h1>
                    {/* dodaj znajomego */}
                    <div className="flex flex-col items-center py-4">
                        <button className="bg-green-900 text-white rounded-md px-3 py-1 my-4 hover:bg-green-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105">Dodaj znajomego (nic nie robi)</button>
                    </div>
                    <div className="border-t-2 border-gray-400">
                        {znajomi.length===0
                            ? <p className="p-4 font-light text-gray-700 text-center">
                                Lista znajomych jest pusta. <br/> Użyj przycisku "dodaj znajomego", aby to zmienić!
                              </p>
                            : <ul>
                                {znajomi.map((znajomy) => (
                                    <ZnajomyNaLiscieKomponent key={znajomy.id} znajomy={znajomy}/>
                                ))}
                                </ul>
                        }
                    </div>
                </div>
                {/* czat */}
                <div className="col-span-2 flex flex-col">
                    <h1>Czat</h1>
                    {/* wiadomości */}
                    <div>

                    </div>
                    {/* pole do wysyłania */}
                    <div>

                    </div>
                </div>
            </div>
        </div>
    </>);
}