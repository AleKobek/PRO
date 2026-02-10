import '../App.css';

import React, {useEffect, useMemo, useState} from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import FormularzKonta from "./FormularzKonta";
import {API_BASE_URL} from "../config/api";
export default function EdytujKonto() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();

    const [login, ustawLogin] = useState("");
    const [staryLogin, ustawStaryLogin] = useState("");
    
    const [email, ustawEmail] = useState("");
    const [staryEmail, ustawStaryEmail] = useState("");
    
    const [numerTelefonu, ustawNumerTelefonu] = useState("");
    const [staryNumerTelefonu, ustawStaryNumerTelefonu] = useState("");
    
    const [dataUrodzenia, ustawDateUrodzenia] = useState("");
    const [staraDataUrodzenia, ustawStaraDateUrodzenia] = useState("");
    
    const[bladLoginu, ustawBladLoginu] = useState("");
    const[bladEmaila, ustawBladEmaila] = useState("");
    const[bladNumeruTelefonu, ustawBladNumeruTelefonu] = useState("");
    const[bladDatyUrodzenia, ustawBladDatyUrodzenia] = useState("");
    const[bladOgolnyKonta, ustawBladOgolnyKonta] = useState("");
    const[sukcesKonta, ustawSukcesKonta] = useState("");
    
    // do zmiany hasła
    
    const [stareHaslo, ustawStareHaslo] = useState("");
    const [noweHaslo, ustawNoweHaslo] = useState("");
    const [powtorzHaslo, ustawPowtorzHaslo] = useState("");
    const [bladOgolnyHasla, ustawbladOgolnyHasla] = useState("");
    const [sukcesHasla, ustawSukcesHasla] = useState("");
    
    
    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate("/login"); // jeśli jest niezalogowany
        }
    }, [ladowanie, uzytkownik, navigate]);


    useEffect(() => {

        // czekamy aż się załaduje id użytkownika
        if(!uzytkownik) return;
        if(!uzytkownik.id) return;

        const ac = new AbortController();
        let alive = true;

        // taka pomocnicza funkcja dla abort controller
        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                if (!res.ok) return null;
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                return null;
            }
        };

        const podajDaneKonta = async () => {
            const idUzytkownika = uzytkownik.id;
            const data = await fetchJsonAbort(`${API_BASE_URL}/Uzytkownik/${idUzytkownika}`);

            // przerywamy działanie funkcji
            if (!alive) return;

            ustawStaryLogin(data.login ?? "");
            ustawLogin(data.login ?? "");
            
            ustawStaryEmail(data.email ?? "");
            ustawEmail(data.email ?? "");
            
            ustawStaryNumerTelefonu(data.numerTelefonu ?? "");
            ustawNumerTelefonu(data.numerTelefonu ?? "");
            
            ustawStaraDateUrodzenia(data.dataUrodzenia ?? "");
            ustawDateUrodzenia(data.dataUrodzenia ?? "");
        };

        if(!login && !staryEmail) {
            podajDaneKonta();
        }

        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };
    }, [login, staryEmail, uzytkownik]);

    const czyZablokowaneWyslijKonta = useMemo(() =>{
        return(
            (login === staryLogin && 
            email === staryEmail && 
            numerTelefonu === staryNumerTelefonu && 
            dataUrodzenia === staraDataUrodzenia) 
            ||
            login.trim().length === 0 || 
            email.trim().length === 0
        )
    },[login, staryLogin, email, staryEmail, numerTelefonu, staryNumerTelefonu, dataUrodzenia, staraDataUrodzenia]);

    const przyWysylaniuZmianyKonta = async() => {
        ustawBladOgolnyKonta("");
        ustawBladLoginu("");
        ustawBladEmaila("");
        ustawBladNumeruTelefonu("");
        ustawBladDatyUrodzenia("");
        ustawSukcesKonta("");
        
        const kontoDoWyslania = {
            login: login.trim(),
            email: email.trim(),
            numerTelefonu: numerTelefonu.trim(),
            dataUrodzenia: dataUrodzenia,
        };

        const opcje = {
            method: "PUT",
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: "include",
            body: JSON.stringify(kontoDoWyslania)
        }
        
        const res = await fetch(`${API_BASE_URL}/Uzytkownik/` + uzytkownik.id, opcje);
        
        // Odczyt body różni się zależnie od typu odpowiedzi
        // jeżeli to 404, to zwraca tylko tekst (nie application/json), więc res.json rzuci wyjątek. musimy to uwzlgędnić
        const ct = res.headers.get("content-type") || "";
        const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
            ? await res.json().catch(() => null)
            : await res.text().catch(() => "");

        if (!res.ok) {
            if(res.status === 400){
                let bledy = body.errors;
                ustawBladLoginu(bledy.Login ? bledy.Login[0] : "");
                ustawBladEmaila(bledy.Email ? bledy.Email[0] : "");
                ustawBladNumeruTelefonu(bledy.NumerTelefonu ? bledy.NumerTelefonu[0] : "");
                ustawBladDatyUrodzenia(bledy.DataUrodzenia ? bledy.DataUrodzenia[0] : "");
                ustawBladOgolnyKonta(body.message);
            }
            return;
        }

        // jak tutaj dojdziemy, wszystko jest git
        ustawSukcesKonta("Konto zmienione pomyślnie!")
    }

    const czyZablokowaneWyslijHasla = useMemo(() =>{
        let tempNoweHaslo = noweHaslo.trim();
        let tempPowtorzHaslo = powtorzHaslo.trim();
        return(
            stareHaslo.trim().length === 0 ||
            tempNoweHaslo.length === 0 || 
            tempPowtorzHaslo.length === 0 || 
            tempNoweHaslo !== tempPowtorzHaslo
        )
    },[stareHaslo, noweHaslo, powtorzHaslo]);

    const przyWysylaniuZmianyHasla = async() => {
        ustawbladOgolnyHasla("");
        ustawSukcesHasla("");
        
        const hasloDoWyslania = {
            stareHaslo: stareHaslo.trim(),
            noweHaslo: noweHaslo.trim(),
        };

        const opcje = {
            method: "PUT",
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: "include",
            body: JSON.stringify(hasloDoWyslania)
        }
        
        const res = await fetch(`${API_BASE_URL}/Uzytkownik/` + uzytkownik.id +"/haslo", opcje);


        // Odczyt body różni się zależnie od typu odpowiedzi
        // jeżeli to 404, to zwraca tylko tekst (nie application/json), więc res.json rzuci wyjątek. musimy to uwzlgędnić
        const ct = res.headers.get("content-type") || "";
        const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
            ? await res.json().catch(() => null)
            : await res.text().catch(() => "");

        if (!res.ok) {
            if(res.status === 400){
                ustawbladOgolnyHasla(body[0].message);
            }
            return;
        }

        // jak tutaj dojdziemy, wszystko jest git
        ustawSukcesHasla("Hasło zmienione pomyślnie!")
    }
    
    if(ladowanie) return (<>
            <NaglowekZalogowano/>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Edytuj konto</h1>
            <button className={"przycisk-nawigacji"} onClick={() => {navigate('/twojeKonto')}}>Powrót do konta</button>
            <br/><br/>
            <form id = "formularz-konta" name = "formularz-konta">
                <FormularzKonta
                    login={login}
                    ustawLogin={ustawLogin}
                    email={email}
                    ustawEmail={ustawEmail}
                    dataUrodzenia={dataUrodzenia}
                    ustawDateUrodzenia={ustawDateUrodzenia}
                    numerTelefonu={numerTelefonu}
                    ustawNumerTelefonu={ustawNumerTelefonu}
                    bladLoginu={bladLoginu}
                    bladEmaila={bladEmaila}
                    bladNumeruTelefonu={bladNumeruTelefonu}
                    bladDatyUrodzenia={bladDatyUrodzenia}
                />
                <input className="wyslij-formularz-przycisk" type = "button" value = "Zapisz" onClick={przyWysylaniuZmianyKonta} disabled={czyZablokowaneWyslijKonta}/>
                <span id = "error-zapisz" className="error-wiadomosc">{bladOgolnyKonta}</span><br/>
            </form>
            <span className="success-widomosc">{sukcesKonta}</span>
            <br></br>
            <div className="box-zmiany-hasla text-center my-10 border-2 border-red-900 rounded-md p-2.5 bg-[#fcb7ab]">
                <h2>Zmień hasło</h2>
                <form id= "formularz-hasła" name = "formularz-hasła" className="border-0">
                    <label>
                        Stare hasło<br/>
                        <input
                        type="password" value={stareHaslo} onChange={(e) => ustawStareHaslo(e.target.value)}/><br/>
                    </label>
                    <label>
                        Nowe hasło<br/>
                        <input
                            type="password" value={noweHaslo} onChange={(e) => ustawNoweHaslo(e.target.value)}/><br/>
                    </label>
                    <label>
                        Powtórz nowe hasło<br/>
                        <input
                            type="password" value={powtorzHaslo} onChange={(e) => ustawPowtorzHaslo(e.target.value)}/><br/>
                    </label>
                    <input className="wyslij-formularz-przycisk" type="button" value="Zapisz" onClick={przyWysylaniuZmianyHasla} disabled={czyZablokowaneWyslijHasla}/><br/>
                    <span id = "error-ogolny-hasla" className="error-wiadomosc">{bladOgolnyHasla}</span><br/>
                </form>
            </div>
            <span className="success-widomosc">{sukcesHasla}</span>
        </div>
    </>);
}