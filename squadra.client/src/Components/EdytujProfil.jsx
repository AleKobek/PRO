import '../App.css';

import React, {useEffect, useState} from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import FormularzProfilu from './FormularzProfilu';
import {useNavigate} from "react-router-dom";
export default function EdytujProfil({jezyk}) {

    const navigate = useNavigate();

    const [pseudonim, ustawPseudonim] = useState("");
    const [zaimki, ustawZaimki] = useState("");
    const [kraj, ustawKraj] = useState("");
    const [region, ustawRegion] = useState("");
    const [opis, ustawOpis] = useState("");

    // lista języków użytkownika to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}
    const [listaJezykowUzytkownika, ustawListeJezykowUzytkownika] = useState([])


    useEffect(() => {
        // pobieramy dane użytkownika do automatycznego wypełnienia i sprawdzania czy coś się zmieniło
        const podajDaneProfilu = () => {
            const opcje = {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                },
            };

            fetch("http://localhost:5014/api/uzytkownik/" + localStorage.getItem("idUzytkownika"), opcje)
                .then(response => response.json())
                // profil ma (chyba) podawany też awatar, ale nam do prototypu nie potrzebny
                .then(data => {
                    ustawListeJezykowUzytkownika(data.jezyki);
                    ustawPseudonim(data.pseudonim);
                    ustawZaimki(data.zaimki);
                    ustawKraj(data.kraj);
                    ustawRegion(data.region);
                    ustawOpis(data.opis);
                })
        }
        podajDaneProfilu();
    }, []);
    
    return (<>
        <NaglowekZalogowano></NaglowekZalogowano>
        <div id = "glowna">
            <h1>{jezyk.edytujProfil}</h1>
            <button onClick={() => {navigate('/')}}>{jezyk.powrotDoProfilu}</button>
            <FormularzProfilu jezyk = {jezyk} czyEdytuj = {true} staraListaJezykowUzytkownika = {listaJezykowUzytkownika} staryPseudonim = {pseudonim} 
                              stareZaimki = {zaimki} staryOpis = {opis} staryRegion = {region} staryKraj = {kraj}/>
            <br></br>
        </div>
    
    </>);
}