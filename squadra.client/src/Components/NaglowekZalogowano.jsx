import {useState} from "react";

export default function NaglowekZalogowano({jezyk}){
    
    
    // to, co pobieramy
    const [nazwaAktualnegoStatusuZBazy, ustawNazweAktualnegoStatusuZBazy] = useState("Online");
    
    // to, co wyświetlamy
    const [statusDoWyswietlenia, ustawStatusDoWyswietlenia] = useState("");
    
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
    
    
    // w przyszłości będzie tu na początku get status, potem select i zmiana statusu będzie oznaczała wysłanie update
    
    
    return(<div className ="menu" id = "menu">
        <span style={}>Squadra</span>
        <ul id = "menu-na-pasku">
            <li>{jezyk.druzyny}</li>
            <li>{jezyk.gildie}</li>
            <li>{jezyk.znajomi}</li>
            <li>{jezyk.twojProfil}</li>
            <li>{jezyk.status}: {statusDoWyswietlenia}</li>
            <li>{jezyk.wyloguj}</li>
        </ul>
    </div>)
    
    
}

