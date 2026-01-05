import {useNavigate} from "react-router-dom";
import React, {useEffect} from "react";
import Naglowek from "./Naglowek";
import {useAuth} from "../Context/AuthContext";

export default function StronaGlowna() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();

    
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
        <div id = "glowna">
            <h1>Witamy w Squadra - twojej platformie do jednoczenia się z innymi graczami i komunikacji!</h1>
            <p>Zaloguj się, aby skorzystać z usług.</p>
        </div>
    </>);
}