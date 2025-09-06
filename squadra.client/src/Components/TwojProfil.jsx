import '../App.css';

import React from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import DaneProfilu from './DaneProfilu';
import {useNavigate} from "react-router-dom";
export default function TwojProfil({jezyk}) {
    
    const navigate = useNavigate();
    
    return (<>
        <NaglowekZalogowano></NaglowekZalogowano>
        <div id = "glowna">
            <h1>{jezyk.twojProfil}</h1>
            <DaneProfilu></DaneProfilu>
        </div>
        <button onClick={() => navigate('/edytujProfil')}>{jezyk.edytujProfil}</button>
    </>);
}