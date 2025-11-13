import React, {useEffect, useMemo, useState} from 'react';
import {useLocation, useNavigate} from "react-router-dom";
import Naglowek from "./Naglowek";
import {useAuth} from "../Context/AuthContext";

export default function Logowanie() {
    const navigate = useNavigate();
    const location = useLocation();
    const { uzytkownik, ustawUzytkownika, ladowanie } = useAuth();
    const [loginLubEmail, ustawLoginLubEmail] = useState(""); // e-mail lub nazwa użytkownika
    const [haslo, ustawHaslo] = useState("");
    const [zapamietajMnie, ustawZapamietajMnie] = useState(false);
    const [czySieWysyla, ustawCzySieWysyla] = useState(false);
    const [bladOgolny, ustawBladOgolny] = useState("");

    const czyZablokowaneWyslij = useMemo(() =>{
        return(
            loginLubEmail.trim().length === 0 ||
            haslo.trim().length === 0
        )
    },[loginLubEmail, haslo]);

    useEffect(() => {
        if (!ladowanie && uzytkownik) {
            navigate("/twojProfil"); // jeśli jest zalogowany
        }
    }, [ladowanie, uzytkownik, navigate]);

    async function przyWysylaniu(e) {
        e.preventDefault();
        ustawBladOgolny("");

        if (!loginLubEmail || !haslo) {
            ustawBladOgolny("Wprowadź login oraz hasło.");
            return;
        }

        ustawCzySieWysyla(true);
        try {
            const res = await fetch("http://localhost:5014/api/Auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify({ LoginLubEmail: loginLubEmail, Haslo: haslo, ZapamietajMnie: zapamietajMnie})
            });

            if (!res.ok) {
                const body = await res.json().catch(() => ({}));
                if(res.status === 400 || res.status === 401){
                    ustawBladOgolny(body.message);
                }
                return;
            }

            const resMe = await fetch("http://localhost:5014/api/Auth/me", { credentials: "include" });
            if (resMe.ok) {
                const tempUzytkownik = await resMe.json();
                ustawUzytkownika(tempUzytkownik);
            }
            
            navigate("/twojProfil");
        } catch (err) {
            ustawBladOgolny(err.message || "Wystąpił błąd podczas logowania.");
        } finally {
            ustawCzySieWysyla(false);
        }
    }
    
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
                <h1>Logowanie</h1>
                <h3 style={{color: "green"}}>{location.state?.message}</h3>
                <form id = "form" name="formularz-logowania">
                    <label>
                        <div>E-mail lub nazwa użytkownika</div>
                        <input
                            type="text"
                            value={loginLubEmail}
                            onChange={(e) => ustawLoginLubEmail(e.target.value)}
                            placeholder="jan.kowalski@example.com"
                            autoComplete="username"
                            
                        />
                    </label>

                    <br/><label>
                        <div>Hasło</div>
                        <input
                            type="password"
                            value={haslo}
                            onChange={(e) => ustawHaslo(e.target.value)}
                            placeholder="••••••••"
                            autoComplete="current-password"
                            style={{ width: "100%", padding: 8 }}
                        />
                    </label><br/>

                    <br/><label> Zapamiętaj mnie
                    <input
                    type="checkbox"
                    onChange={(e) => ustawZapamietajMnie(e.target.checked)}
                    /></label><br/><br/>
                    
                    <input
                        type="button"
                        disabled={czySieWysyla || czyZablokowaneWyslij}
                        onClick={przyWysylaniu}
                        value="Zaloguj się"
                    /><br/>
                    <span id="error-ogolny" className="error-wiadomosc">{bladOgolny}</span><br/>
            </form>
        </div>
</>
    )
}