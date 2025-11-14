import '../App.css';

import React, {useEffect, useMemo, useState} from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import FormularzProfilu from './FormularzProfilu';
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import Naglowek from "./Naglowek";
import FormularzKonta from "./FormularzKonta";
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
    const[bladOgolny, ustawBladOgolny] = useState("");
    
    
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
            const data = await fetchJsonAbort(`http://localhost:5014/api/Uzytkownik/${idUzytkownika}`);

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
    }, [uzytkownik]);

    const czyZablokowaneWyslij = useMemo(() =>{
        return(
            (login === staryLogin && 
            email === staryEmail && 
            numerTelefonu === staryNumerTelefonu && 
            dataUrodzenia === staraDataUrodzenia) 
            ||
            login.trim().length === 0 || 
            email.trim().length === 0
        )
    },[login, email, numerTelefonu, dataUrodzenia]);

    const przyWysylaniuZmianyKonta = async() => {
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
        
        console.log("Co wysyłamy:", kontoDoWyslania);

        const res = await fetch("http://localhost:5014/api/Uzytkownik/" + uzytkownik.id, opcje);
        
        console.log("Odpowiedź z bazy:", res);
        
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
                ustawBladOgolny(body.message);
            }
            return;
        }

        // jak tutaj dojdziemy, wszystko jest git
        navigate("/twojeKonto");
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
            <form id = "form" name = "formularz-konta">
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
                <input type = "button" value = "Zapisz" onClick={przyWysylaniuZmianyKonta} disabled={czyZablokowaneWyslij}/>
                <span id = "error-zapisz" className="error-wiadomosc">{bladOgolny}</span><br/>
            </form>
            <br></br>
        </div>

    {/* tutaj będzie jeszcze zmiana hasła, na razie testujemy tylko edycję konta bez tego */}
    </>);
}