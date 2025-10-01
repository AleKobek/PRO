import '../App.css';

import React from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import DaneProfilu from './DaneProfilu';
import {useNavigate} from "react-router-dom";
export default function TwojProfil() {
    
    const navigate = useNavigate();
    


    return (<>
        <NaglowekZalogowano></NaglowekZalogowano>
        <div id = "glowna">
            <h1>Twój profil</h1>
            <DaneProfilu></DaneProfilu>
            <button className={"przycisk-nawigacji"} onClick={() => navigate('/edytujProfil')} style={{textAlign: "center", alignSelf: "center"}}>Edytuj profil</button>
        </div>
    </>);
}