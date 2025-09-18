import '../App.css';

import React from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import DaneProfilu from './DaneProfilu';
import {useNavigate} from "react-router-dom";
import {useJezyk} from "../LanguageContext.";
export default function TwojProfil() {
    
    const navigate = useNavigate();

    const { jezyk } = useJezyk();


    return (<>
        <NaglowekZalogowano></NaglowekZalogowano>
        <div id = "glowna">
            <h1>{jezyk.twojProfil}</h1>
            <DaneProfilu jezyk={jezyk}></DaneProfilu>
            <button onClick={() => navigate('/edytujProfil')}>{jezyk.edytujProfil}</button>
        </div>
    </>);
}