import {useEffect, useState} from "react";
import {NavLink, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";

export default function NaglowekZalogowano(){

    const {uzytkownik, ustawUzytkownika} = useAuth();
    
    // to, co pobieramy, w prototypie nie zmieniamy
    const [aktualnyStatus, ustawAktualnyStatusZBazy] = useState("Dostępny");
    const [listaStatusow, ustawListeStatusow] = useState([]);
    const [awatarUrl, ustawAwatarUrl] = useState("");
    const navigate = useNavigate();


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
    </>)


}
