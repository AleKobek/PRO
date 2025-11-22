import '../App.css';

import React, {useEffect} from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import DaneProfilu from './DaneProfilu';
import {useLocation, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import Naglowek from "./Naglowek";
export default function TwojProfil() {
    
    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const location = useLocation();


    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate("/login"); // jeśli jest niezalogowany
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
        <div id = "glowna">
            <h1>Twój profil</h1>
            <h3 className="success-widomosc">{location.state?.message}</h3>
            <DaneProfilu uzytkownik = {uzytkownik}></DaneProfilu>
            <button className={"przycisk-nawigacji"} onClick={() => navigate('/edytujProfil')}>Edytuj profil</button>
        </div>
    </>);
}