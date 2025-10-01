import {useEffect, useMemo, useState} from "react";
import {NavLink} from "react-router-dom";

export default function NaglowekZalogowano(){


    // to, co pobieramy, w prototypie nie zmieniamy
    const [nazwaAktualnegoStatusuZBazy, ustawNazweAktualnegoStatusuZBazy] = useState("Dostępny");
    




    // w przyszłości będzie tu na początku get status, na nagłówku select i zmiana statusu będzie oznaczała wysłanie update
    
    
    return(<div className ="menu" id = "menu">
        <span style={{fontSize: 90}}>Squadra</span>
        <ul id = "menu-na-pasku">
            {/* na razie wszystkie prowadzą do profilu, bo nie ma reszty */}
            <NavLink to = '/' className = "nawigacja">Drużyny</NavLink>
            <NavLink to = '/' className = "nawigacja">Gildie</NavLink>
            <NavLink to = '/' className = "nawigacja">Znajomi</NavLink>
            <NavLink to = '/' className = "nawigacja">Twój profil</NavLink>
            <NavLink to = '/' className = "nawigacja">Status: {nazwaAktualnegoStatusuZBazy}</NavLink>
            <NavLink to = '/' className = "nawigacja">Wyloguj</NavLink>
        </ul>
    </div>)
    
    
}

