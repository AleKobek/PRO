import {useEffect, useRef, useState} from "react";
import {NavLink, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";

export default function NaglowekZalogowano(){

    const {uzytkownik, ustawUzytkownika} = useAuth();
    
    // to, co pobieramy, w prototypie nie zmieniamy
    const [aktualnyStatus, ustawAktualnyStatusZBazy] = useState("Dostępny");
    const [listaStatusow, ustawListeStatusow] = useState([]);
    const [awatarUrl, ustawAwatarUrl] = useState("");
    const navigate = useNavigate();

    // powiadomienia
    const [pokazPowiadomienia, ustawPokazPowiadomienia] = useState(false);
    const [powiadomienia, ustawPowiadomienia] = useState([]);
    const [ladowaniePowiadomien, ustawLadowaniePowiadomien] = useState(false);
    const [maPowiadomienia, ustawMaPowiadomienia] = useState(false);

    const notyRef = useRef(null);



    // ping
    useEffect(() => {
        const interval = setInterval(async () => {
            await fetch("http://localhost:5014/api/Uzytkownik/ping", {credentials: "include" });
        }, 60000); // co minutę

        // jak wychodzimy to usuwamy nasz interval
        return () => clearInterval(interval);
    }, []);
    
    useEffect(() => {
        ustawAwatarUrl(uzytkownik?.awatar ? "data:image/jpeg;base64," + uzytkownik?.awatar : "");
    },[uzytkownik]);


    // pobieramy powiadomienia z serwera co minutę
    useEffect(() => {
        const ac = new AbortController();
        let alive = true;

        const interval = setInterval(async () => {
            if (!alive) return;
            await pobierzPowiadomienia(ac.signal)
        }, 60000);

        // jak wychodzmy to zatrzymujemy nasz interval
        return () => {
            alive = false;
            ac.abort();
            clearInterval(interval);
        };
    }, []);

    // przy załadowaniu strony także jednorazowo pobieramy powiadomienia
    useEffect(() => {
        const ac = new AbortController();
        let alive = true;

        async function fetchNow(signal) {
            if (!alive) return;
            await pobierzPowiadomienia(signal);
        }

        fetchNow(ac.signal);

        return () => {
            alive = false;
            ac.abort();
        };
    },[])


    // pobieramy powiadomienia
    const pobierzPowiadomienia = async (signal) => {
        ustawLadowaniePowiadomien(true);
        try {
            const res = await fetch("http://localhost:5014/api/Powiadomienie", { credentials: "include", signal });
            console.log("res: ",res);
            if (!res.ok) {
                console.error("Błąd pobierania powiadomień", res.status);
                ustawPowiadomienia([]);
                ustawMaPowiadomienia(false);
                return;
            }
            const data = await res.json();
            console.log("data: ", data);
            const list = Array.isArray(data) ? data : [];
            ustawPowiadomienia(list);
            ustawMaPowiadomienia(list.length > 0);
        } catch (err) {
            if (err && err.name === 'AbortError') return;
            console.error(err);
            ustawPowiadomienia([]);
            ustawMaPowiadomienia(false);
        } finally {
            ustawLadowaniePowiadomien(false);
        }
    };
    

    const przyWylogowywaniu = async () =>{
        
        const res = await fetch("http://localhost:5014/api/Auth/logout", {method: "POST", credentials: "include"});
        if(!res.ok) {
            console.error(res);
            console.error("Nie udało się wylogować")
            return;
        }
        ustawUzytkownika(null);
        navigate('/login')
    }
    
    const przyZmianieStatusu = async (idStatusu) => {
        const opcje = {
            method: "PUT",
            headers: {"Content-Type": "application/json"},
            credentials: "include",
            body: idStatusu
        }

        const res = await fetch("http://localhost:5014/api/Profil/" + uzytkownik.id + "/status", opcje);
        if(!res.ok){
            console.error(res);
            return
        }

        // jeżeli git
        let tempStatus = listaStatusow.find(item => item.id === idStatusu);
        ustawAktualnyStatusZBazy(tempStatus);
    }
    
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
        const podajStatusy = async () => {
            const statusy = await fetchJsonAbort("http://localhost:5014/api/Status");
            if(!alive || !statusy || !Array.isArray(statusy)) return;
            ustawListeStatusow(statusy);
        }
        
        const podajAktualnyStatus = async () => {
            const aktualnyStatus = await fetchJsonAbort("http://localhost:5014/api/Profil/" + uzytkownik.id + "/status/baza");
            if(!alive || !aktualnyStatus) return;
            ustawAktualnyStatusZBazy(aktualnyStatus);
        }
        
        podajStatusy();
        podajAktualnyStatus();

        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };
        
    },[uzytkownik])
    
    // nawigujemy do strony "twój profil"
    const przyKliknieciuWAwatar = () =>{
        navigate('/twojProfil');
    }

    // render powiadomień
    const PanelPowiadomien = () => (
        <div
            ref={notyRef}
            className="fixed top-[70px] right-5 w-80 max-h-[60vh] overflow-y-auto bg-white rounded-lg shadow-lg p-3"
            style={{ zIndex: 2000 }}
        >
            <div className="flex justify-between items-center mb-2">
                <strong>Powiadomienia</strong>
                <button onClick={() => ustawPokazPowiadomienia(false)} className="cursor-pointer">Zamknij</button>
            </div>
            {ladowaniePowiadomien && <div>Ładowanie...</div>}
            {!ladowaniePowiadomien && powiadomienia.length === 0 && <div>Brak powiadomień</div>}
            {/*
                powiadomienia przyjdą w formacie:
                [
                  {
                    "id": 0,
                    "idTypuPowiadomienia": 0,
                    "uzytkownikId": 0,
                    "idPowiazanegoObiektu": 0,
                    "nazwaPowiazanegoObiektu": "string",
                    "tresc": "string",
                    "dataWyslania": "2026-02-07T17:12:02.518Z"
                  }
                ]

            */}
            <ul className="list-none p-0 m-0">
                {powiadomienia.map((p) => (
                    <li key={p.id} className="p-2 border-b border-gray-200">
                        <div className="font-semibold">{p.nazwaPowiazanegoObiektu || 'Powiadomienie systemowe'}</div>
                        <div className="text-sm text-gray-600">{p.tresc}</div>
                        <div className="text-xs text-gray-400 mt-1.5">{p.dataWyslania}</div>
                    </li>
                ))}
            </ul>
        </div>
    );
    
    
    return(<>
        <header>
            <title>Squadra</title>
        </header>
        <div className ="menu" id = "menu">
            <span className="logo">Squadra</span>
            <div id = "menu-na-pasku">
                <div className="nawigacja-na-pasku">
                    {/* na razie wszystkie prowadzą do profilu, bo nie ma reszty */}
                    <NavLink to = '/twojProfil' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Drużyny</NavLink>
                    <NavLink to = '/twojProfil' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Gildie</NavLink>
                    <NavLink to = '/twoiZnajomi' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Znajomi</NavLink>
                    <NavLink to = '/twojProfil' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Twój profil</NavLink>
                    <NavLink to = '/twojeKonto' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Twoje konto</NavLink>
                </div>
                <span className="bg-none">Status:</span>
                <select
                    value={aktualnyStatus?.id || ''}
                    onChange={(e) => {
                        const id = Number(e.target.value);
                        przyZmianieStatusu(id);
                    }}
                    >
                    {listaStatusow.map(item =>
                        <option key = {item.id} value={item.id}>{item.nazwa}</option>
                    )}
                </select>
                {/* przycisk powiadomień */}
                <button
                    onClick={() => ustawPokazPowiadomienia(v => !v)}
                    className={`relative ml-3 w-9 h-9 p-0 border-none flex items-center justify-center cursor-pointer rounded-lg transition-colors ${
                        maPowiadomienia ? 'bg-red-400' : 'bg-transparent'
                    }`}
                    aria-expanded={pokazPowiadomienia}
                    aria-controls="panel-powiadomien"
                    title="Powiadomienia"
                >
                    <img
                        src={maPowiadomienia ? "/img/bell-dot.svg" : "/img/bell.svg"}
                        alt="powiadomienia"
                        className="w-6 h-6"
                    />
                </button>
                <img id = "awatar"
                     src = {awatarUrl || "/img/domyslny_awatar.png"}
                     alt = "awatar"
                     className = "awatar cursor-pointer ml-3"
                     onClick={przyKliknieciuWAwatar}
                />
                <button className="text-[20px] text-white bg-red-900 rounded-lg px-3 py-2 mx-2 hover:bg-red-600" onClick={przyWylogowywaniu}>
                    Wyloguj
                </button>
            </div>
        </div>
        {/* overlay powiadomień, renderujemy na wierzchu tej samej strony */}
        {pokazPowiadomienia && <PanelPowiadomien />}
    </>)


}
