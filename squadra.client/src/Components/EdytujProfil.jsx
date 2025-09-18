import '../App.css';

import React, {useEffect, useState} from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import FormularzProfilu from './FormularzProfilu';
import {useNavigate} from "react-router-dom";
import {useJezyk} from "../LanguageContext.";
    export default function EdytujProfil() {

    const navigate = useNavigate();
    const {jezyk} = useJezyk();

    const [pseudonim, ustawPseudonim] = useState("");
    const [zaimki, ustawZaimki] = useState("");
    const [kraj, ustawKraj] = useState("");
    const [region, ustawRegion] = useState("");
    const [opis, ustawOpis] = useState("");

    // lista języków użytkownika to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}
    const [listaJezykowUzytkownika, ustawListeJezykowUzytkownika] = useState([])


    useEffect(() => {
        // pobieramy dane użytkownika do automatycznego wypełnienia i sprawdzania czy coś się zmieniło
        const podajDaneProfilu = async () => {
            const opcje = {
                method: "GET",
                headers: { 'Content-Type': 'application/json' },
            };

            const res = await fetch("http://localhost:5014/api/Profil/" + localStorage.getItem("idUzytkownika"), opcje);
            const data = await res.json();

            // defensywnie upewnij się, że to tablica
            ustawListeJezykowUzytkownika(Array.isArray(data.jezyki) ? data.jezyki : []);
            ustawPseudonim(data.pseudonim ?? "");
            ustawZaimki(data.zaimki ?? "");
            ustawKraj(data.kraj ?? "");
            ustawRegion(data.region ?? "");
            ustawOpis(data.opis ?? "");
        };

        const podajJezykiIStopnieUzytkownika = async (id) => {
            const opcje2 = {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                },
            };
            const res = await fetch("http://localhost:5014/api/Jezyk/profil/" + id, opcje2);
            const data = await res.json();
            ustawListeJezykowUzytkownika(data.map(item => ({
                        idJezyka: item.jezyk.id,
                        nazwaJezyka: item.jezyk.nazwa,
                        idStopnia: item.stopien.id,
                        nazwaStopnia: item.stopien.nazwa
            })));
        }
        if(listaJezykowUzytkownika.length === 0) podajJezykiIStopnieUzytkownika(localStorage.getItem("idUzytkownika")).then();
        if(pseudonim === "" && zaimki === "" && kraj === "" && region === "" && opis === "") podajDaneProfilu().then();
    }, []);
    
    return (<>
        <NaglowekZalogowano></NaglowekZalogowano>
        <div id = "glowna">
            <h1>{jezyk.edytujProfil}</h1>
            <button onClick={() => {navigate('/')}}>{jezyk.powrotDoProfilu}</button>
            <FormularzProfilu czyEdytuj = {true} staraListaJezykowUzytkownika = {listaJezykowUzytkownika} staryPseudonim = {pseudonim} 
                              stareZaimki = {zaimki} staryOpis = {opis} staryRegion = {region} staryKraj = {kraj}/>
            <br></br>
        </div>
    
    </>);
}