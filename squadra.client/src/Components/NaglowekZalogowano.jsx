import {useEffect, useState} from "react";
import {NavLink} from "react-router-dom";
import {useJezyk} from "../LanguageContext.";

export default function NaglowekZalogowano(){

    const { jezyk } = useJezyk();

    // to, co pobieramy, w prototypie nie zmieniamy
    const [nazwaAktualnegoStatusuZBazy, ustawNazweAktualnegoStatusuZBazy] = useState("Online");
    
    // to, co wyświetlamy
    const [statusDoWyswietlenia, ustawStatusDoWyswietlenia] = useState("");

    useEffect(() => {
        switch(nazwaAktualnegoStatusuZBazy){
            case "Online":{
                ustawStatusDoWyswietlenia(jezyk.dostepny);
                break;
            }
            case "Away":{
                ustawStatusDoWyswietlenia(jezyk.zarazWracam);
                break;
            }
            case "Do not disturb":{
                ustawStatusDoWyswietlenia(jezyk.niePrzeszkadzac);
                break;
            }
            case "Offline" | "Invisible":{
                ustawStatusDoWyswietlenia(jezyk.niedostepny);
            }
        }
    }, [statusDoWyswietlenia]);
    
    
    
    // w przyszłości będzie tu na początku get status, na nagłówku select i zmiana statusu będzie oznaczała wysłanie update
    
    
    return(<div className ="menu" id = "menu">
        <span style={{fontSize: 90}}>Squadra</span>
        <ul id = "menu-na-pasku">
            {/* na razie wszystkie prowadzą do profilu, bo nie ma reszty */}
            <NavLink to = '/' className = "nawigacja">{jezyk.druzyny}</NavLink>
            <NavLink to = '/' className = "nawigacja">{jezyk.gildie}</NavLink>
            <NavLink to = '/' className = "nawigacja">{jezyk.znajomi}</NavLink>
            <NavLink to = '/' className = "nawigacja">{jezyk.twojProfil}</NavLink>
            <NavLink to = '/' className = "nawigacja">{jezyk.status}: {statusDoWyswietlenia}</NavLink>
            <NavLink to = '/' className = "nawigacja">{jezyk.wyloguj}</NavLink>
        </ul>
    </div>)
    
    
}

