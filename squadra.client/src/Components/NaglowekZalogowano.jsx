import {useEffect, useMemo, useState} from "react";
import {NavLink, useNavigate} from "react-router-dom";

export default function NaglowekZalogowano(){


    // to, co pobieramy, w prototypie nie zmieniamy
    const [nazwaAktualnegoStatusuZBazy, ustawNazweAktualnegoStatusuZBazy] = useState("Dostępny");
    const navigate = useNavigate();
    

    const przyWylogowywaniu = async () =>{
        const res = await fetch("http://localhost:5014/api/Auth/logout", {method: "POST", credentials: "include"});
        if(!res.ok) {
            console.error(res)
            console.error("Nie udało się wylogować")
            return;
        }
        navigate('/login')
    }


    // w przyszłości będzie tu na początku get status, na nagłówku select i zmiana statusu będzie oznaczała wysłanie update
    
    
    return(<div className ="menu" id = "menu">
        <span className="logo">Squadra</span>
        <ul id = "menu-na-pasku">
            {/* na razie wszystkie prowadzą do profilu, bo nie ma reszty */}
            <NavLink to = '/twojProfil' className = "nawigacja">Drużyny</NavLink>
            <NavLink to = '/twojProfil' className = "nawigacja">Gildie</NavLink>
            <NavLink to = '/twojProfil' className = "nawigacja">Znajomi</NavLink>
            <NavLink to = '/twojProfil' className = "nawigacja">Twój profil</NavLink>
            <NavLink to = '/twojProfil' className = "nawigacja">Twoje konto</NavLink>
            <span>Status: {nazwaAktualnegoStatusuZBazy}</span>
            <button onClick={przyWylogowywaniu}>Wyloguj</button>
        </ul>
    </div>)
    
    
}

