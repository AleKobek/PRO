import '../App.css';

import React, {useEffect, useState} from 'react';
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {API_BASE_URL} from "../config/api";
export default function TwoiZnajomiStrona() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const [znajomi, ustawZnajomych] = useState([]);


    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate("/login"); // jeśli jest niezalogowany
        }
    }, [ladowanie, uzytkownik, navigate]);
    

    // pobieramy listę znajomych z backendu
    useEffect(() => {
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

        const podajZnajomych = async () => {
            const id = uzytkownik.id;
            const znajomi = await fetchJsonAbort(`${API_BASE_URL}/Znajomi/${id}`);
            if(!alive || !znajomi) return;
            ustawZnajomych(znajomi);
        }

        if(!znajomi.length) podajZnajomych();

        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };

    }, [uzytkownik, znajomi.length]);

    if(ladowanie) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Twoi znajomi</h1>
            <div className="grid grid-cols-2">
                {/* znajomi */}
                <div>

                </div>
                {/* czat */}
                <div>

                </div>
            </div>
        </div>
    </>);
}