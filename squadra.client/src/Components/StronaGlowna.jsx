import {useNavigate} from "react-router-dom";
import React, {useEffect} from "react";
import Naglowek from "./Naglowek";
import {useAuth} from "../Context/AuthContext";

export default function StronaGlowna() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();

    useEffect(() => {
        document.title = `Squadra`;
    }, []);
    
    useEffect(() => {
        if (!ladowanie && uzytkownik) {
            navigate("/twojProfil"); // jeśli jest zalogowany
        }
    }, [ladowanie, uzytkownik, navigate]);

    if(ladowanie) return (<>
            <Naglowek/>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <Naglowek></Naglowek>
        <div id = "glowna" className="flex flex-col items-center">
            <h1>Witamy w Squadra - twojej platformie do jednoczenia się z innymi graczami i komunikacji!</h1>
            <p className="text-center my-3">
                W naszej aplikacji możesz znaleźć towarzyszy do grania w gry wieloosobowe i komunikować się z nimi. 
                Oferujemy możliwość tworzenia drużyn, dołączania do nich oraz wysyłania wiadomości do innych graczy.
                Po połączeniu konta z serwisem XYZ dostępne jest wyświetlanie swojej biblioteki gier wraz ze statystykami 
                w niej oraz ustawianie wymagań na dołączenie do drużyny, aby mieć pewność, że znajdziesz osoby odpowiadające 
                oczekiwanemu przez Ciebie poziomie umiejętności.
            </p>
            <p>Zaloguj się, aby skorzystać z usług naszego serwisu.</p>
        </div>
    </>);
}