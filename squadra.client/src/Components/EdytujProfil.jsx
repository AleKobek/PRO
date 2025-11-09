import '../App.css';

import React, {useEffect, useState} from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import FormularzProfilu from './FormularzProfilu';
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import Naglowek from "./Naglowek";
    export default function EdytujProfil() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();

    const [pseudonim, ustawPseudonim] = useState("");
    const [zaimki, ustawZaimki] = useState("");
    const [kraj, ustawKraj] = useState({});
    const [region, ustawRegion] = useState({});
    const [opis, ustawOpis] = useState("");

    // lista języków użytkownika przed zmianami, jej postać to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}
    const [staraListaJezykowUzytkownika, ustawStaraListeJezykowUzytkownika] = useState([])

    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate("/login"); // jeśli jest niezalogowany
        }
    }, [ladowanie, uzytkownik, navigate]);
        

    useEffect(() => {
        if(!uzytkownik) return;
        const ac = new AbortController();
        let alive = true; // nie aktualizuj stanu po unmount

        const fetchJson = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include"});
                if (!res.ok) return null;
                return await res.json();
            } catch (err) {
                // To zdarza się przy czyszczeniu efektu / Hot Reload / StrictMode – ignorujemy
                if (err && err.name === 'AbortError') return null;
                // Inne błędy warto zalogować
                console.error('Błąd pobierania:', err);
                return null;
            }
        };

        (async () => {
            const id = uzytkownik.id;

            const profil = await fetchJson(`http://localhost:5014/api/Profil/${id}`, {credentials: "include"});
            if (alive && profil) {
                // console.log("Pobrano profil:", profil);
                ustawStaraListeJezykowUzytkownika(Array.isArray(profil.jezyki) ? profil.jezyki : []);
                ustawPseudonim(profil.pseudonim ?? '');
                ustawZaimki(profil.zaimki ?? '');
                if(profil.regionIKraj){
                    ustawKraj({id: profil.regionIKraj.idKraju, nazwa: profil.regionIKraj.nazwaKraju});
                    ustawRegion({id: profil.regionIKraj.idRegionu, nazwa: profil.regionIKraj.nazwaRegionu});
                }else {
                    ustawKraj({id: null, nazwa: null});
                    ustawRegion({id: null, nazwa: null});   
                }
                ustawOpis(profil.opis ?? '');
            }

            const jezyki = await fetchJson(`http://localhost:5014/api/Jezyk/profil/${id}`);
            if (alive && Array.isArray(jezyki)) {
                ustawStaraListeJezykowUzytkownika(jezyki.map(item => ({
                    idJezyka: item.jezyk.id,
                    nazwaJezyka: item.jezyk.nazwa,
                    idStopnia: item.stopien.id,
                    nazwaStopnia: item.stopien.nazwa,
                    wartoscStopnia: item.stopien.wartosc
                })));
            }
        })();

        return () => {
            alive = false;
            ac.abort(); // przerywamy
        };
    }, [uzytkownik]);

    if(ladowanie) return (<>
            <Naglowek/>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Edytuj profil</h1>
            <button className={"przycisk-nawigacji"} onClick={() => {navigate('/')}}>Powrót do profilu</button>
            <br/><br/>
            <FormularzProfilu czyEdytuj = {true} staraListaJezykowUzytkownika = {staraListaJezykowUzytkownika} staryPseudonim = {pseudonim} 
                              stareZaimki = {zaimki} staryOpis = {opis} staryRegion = {region} staryKraj = {kraj} uzytkownik={uzytkownik}/>
            <br></br>
        </div>

    </>);
}