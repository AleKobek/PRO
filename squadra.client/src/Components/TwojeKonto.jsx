import '../App.css';

import React, {useEffect} from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import DaneProfilu from './DaneProfilu';
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import Naglowek from "./Naglowek";
import DaneKonta from "./DaneKonta";
export default function TwojeKonto() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();


    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate("/login"); // jeśli jest niezalogowany
        }
    }, [ladowanie, uzytkownik, navigate]);

    if(ladowanie) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Twoje konto</h1>
            <DaneKonta uzytkownik = {uzytkownik}></DaneKonta>
            <button className={"przycisk-nawigacji"} onClick={() => navigate('/edytujKonto')} style={{textAlign: "center", alignSelf: "center"}}>Edytuj konto</button>
        </div>
    </>);
}