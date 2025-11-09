import React, {useEffect, useMemo, useState} from 'react';
import { useNavigate, Link } from "react-router-dom";
import Naglowek from "./Naglowek";
import FormularzKonta from "./FormularzKonta";
import {useAuth} from "../Context/AuthContext";

export default function Rejestracja() {
    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const [pseudonim, ustawPseudonim] = useState("");
    const [login, ustawLogin] = useState("");
    const [email, ustawEmail] = useState("");
    const [dataUrodzenia, ustawDateUrodzenia] = useState("");
    const [numerTelefonu, ustawNumerTelefonu] = useState("");
    const [haslo1, ustawHaslo1] = useState("");
    const [haslo2, ustawHaslo2] = useState("");
    const [czySieWysyla, ustawCzySieWysyla] = useState(false);
    const [bladPseudonimu, ustawBladPseudonimu] = useState("");
    const [bladLoginu, ustawBladLoginu] = useState("");
    const [bladEmaila, ustawBladEmaila] = useState("");
    const [bladDatyUrodzenia, ustawBladDatyUrodzenia] = useState("");
    const [bladNumeruTelefonu, ustawBladNumeruTelefonu] = useState("");
    const [bladHasla, ustawBladHasla] = useState("");
    const [bladOgolny, ustawBladOgolny] = useState("");

    // jeszcze zanim wyślemy do backendu staramy się zrobić weryfikację także na froncie, aby trochę odciążyć
    function sprawdzHaslo() {
        let re = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])(?=.{8,128})");
        if(re.test(haslo1) === false) return ("Hasło musi mieć dużą literę, małą literę, cyfrę, znak specjalny i minimum 8 znaków")
        if (haslo1 !== haslo2) return "Hasła nie są zgodne.";
        return "";
    }

    useEffect(() => {
        if (!ladowanie && uzytkownik) {
            navigate("/twojProfil"); // jeśli jest zalogowany
        }
    }, [ladowanie, uzytkownik, navigate]);
    
    async function przyWysylaniu(e) {
        e.preventDefault();
        ustawBladPseudonimu("");
        ustawBladEmaila("");
        ustawBladDatyUrodzenia("");
        ustawBladNumeruTelefonu("");
        
        let bladHaslaTemp = sprawdzHaslo();
        ustawBladHasla(bladHaslaTemp);
        if(bladHaslaTemp !== "") return;
        
        ustawCzySieWysyla(true);
        try {
            const res = await fetch("http://localhost:5014/api/Auth/register", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    login: login,
                    pseudonim: pseudonim,
                    email: email,
                    dataUrodzenia: dataUrodzenia,
                    numerTelefonu: numerTelefonu,
                    haslo: haslo1
                })
            });

            if (!res.ok) {
                const body = await res.json().catch(() => ({}));
                if(res.status === 400 || res.status === 409){
                    console.log(body.errors)
                    let bledy = body.errors;
                    console.log(bledy.Pseudonim, !bledy.Pseudonim);
                    ustawBladPseudonimu(!bledy.Pseudonim ? "" : bledy.Pseudonim[0]);
                    ustawBladLoginu(!bledy.Login ? "" : bledy.Login[0]);
                    ustawBladEmaila(!bledy.Email ? "" : bledy.Email[0]);
                    ustawBladHasla(!bledy.Haslo ? "" : bledy.Haslo[0]);
                    ustawBladNumeruTelefonu(!bledy.NumerTelefonu ? "" : bledy.NumerTelefonu[0]);
                    ustawBladDatyUrodzenia(!bledy.DataUrodzenia ? "" : bledy.DataUrodzenia[0]);
                }
                console.error(body?.message || "Rejestracja nie powiodła się.");
                return;
            }

            // jeśli tu doszliśy, jest git
            
            navigate("/login", {
                replace: true,
                state: { message: "Konto utworzono. Możesz się zalogować." }
            });
        } catch (err) {
            ustawBladOgolny(err.message || "Rejestracja nie powiodła się.");
        } finally {
            ustawCzySieWysyla(false);
        }
    }

    const czyZablokowaneWyslij = useMemo(() =>{
        return(
            !pseudonim ||
            !login ||
            !email ||
            !haslo1||
            !haslo2 ||
            !dataUrodzenia ||
            czySieWysyla
        )
    },[pseudonim, email, dataUrodzenia, haslo1, haslo2, czySieWysyla]);

    if(ladowanie) return (<>
            <Naglowek/>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (
        <>
            <Naglowek />
            <div id = "glowna">
                <h1>Rejestracja</h1>
                <form id = "form" name="formularz-rejestracji">
                    <FormularzKonta
                        pseudonim = {pseudonim}
                        ustawPseudonim = {ustawPseudonim}
                        login = {login}
                        ustawLogin = {ustawLogin}
                        email = {email}
                        ustawEmail = {ustawEmail}
                        dataUrodzenia = {dataUrodzenia}
                        ustawDateUrodzenia = {ustawDateUrodzenia}
                        numerTelefonu = {numerTelefonu}
                        ustawNumerTelefonu = {ustawNumerTelefonu}
                        czySieWysyla = {czySieWysyla}
                        bladPseudonimu = {bladPseudonimu}
                        bladLoginu = {bladLoginu}
                        bladEmaila = {bladEmaila}
                        bladDatyUrodzenia = {bladDatyUrodzenia}
                        przyWysylaniu = {przyWysylaniu}
                        bladNumeruTelefonu = {bladNumeruTelefonu}
                        bladHasla = {bladHasla}
                        bladOgolny = {bladOgolny}
                    ></FormularzKonta>
                    <br/><label>
                    Hasło (Musi mieć dużą literę, małą literę, cyfrę, znak specjalny i minimum 8 znaków) <br/>
                    <input
                        type="password"
                        value={haslo1}
                        onChange={(e) => ustawHaslo1(e.target.value)}
                        placeholder="••••••••"
                        autoComplete="new-password"
                    />
                    </label><br/>
                    
                    <label>
                        Powtórz hasło <br/>
                        <input
                            type="password"
                            value={haslo2}
                            onChange={(e) => ustawHaslo2(e.target.value)}
                            placeholder="••••••••"
                            autoComplete="new-password"
                        />
                    </label><br/>
                    <span id = "error-haslo" className="error-wiadomosc">{bladHasla}</span><br/>
                    <button
                        type="button"
                        disabled={czyZablokowaneWyslij}
                        onClick={przyWysylaniu}
                    >Zarejestruj się</button>
                    <span id = "error-ogolny" className="error-wiadomosc">{bladOgolny}</span><br/>
                </form>
            </div>
        </>
    );
}