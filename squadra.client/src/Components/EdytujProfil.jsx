import '../App.css';

import React from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import FormularzProfilu from './FormularzProfilu';
import {useNavigate} from "react-router-dom";
export default function EdytujProfil({jezyk}) {

    const navigate = useNavigate();
    
    const przyZapisywaniu = () =>{
        // tutaj wysyłamy update do bazy i cofamy
    }
    
    return (<>
        <NaglowekZalogowano></NaglowekZalogowano>
        <div id = "glowna">
            <h1>{jezyk.edytujProfil}</h1>
            <button onClick={() => {navigate('/')}}>{jezyk.powrotDoProfilu}</button>
            <FormularzProfilu></FormularzProfilu>
            <br></br>
            <button onClick={przyZapisywaniu}>{jezyk.zapisz}</button>
        </div>
    {/* tutaj przycisk zapisz, wysyłający update i cofający do profilu */}
    </>);
}