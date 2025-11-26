import React, {useEffect, useMemo, useState} from "react";
import ListaJezykow from "./ListaJezykow";
import {useNavigate} from "react-router-dom";

export default function FormularzAwatara({
                                             uzytkownik,
                                             staryAwatar
                                        })
{
    
    const [bladAwatara, ustawBladAwatara] = useState("");
    const [awatar, ustawAwatar] = useState(null);
    const [podgladAwatara, ustawPodgladAwatara] = useState("");
    const navigate = useNavigate();
    

    // SYNC tylko gdy props się zmienia
    useEffect(() => {
        ustawAwatar(staryAwatar ?? null);
        ustawPodgladAwatara(staryAwatar ? staryAwatar : "");
    }, [staryAwatar]);

    
    const przyWysylaniu = async() =>{

        ustawBladAwatara("");

        if (!awatar) {
            ustawBladAwatara("Wybierz plik awatara.");
            return;
        }

        const formularz = new FormData();
        formularz.append("awatar", awatar);
        

        const opcje = {
            method: "PUT",
            credentials: "include",
            body: formularz
        }
        
        const res = await fetch("http://localhost:5014/api/Profil/" + uzytkownik.id + "/awatar", opcje);
        
        // Odczyt body różni się zależnie od typu odpowiedzi
        // jeżeli to 404, to zwraca tylko tekst (nie application/json), więc res.json rzuci wyjątek. musimy to uwzlgędnić
        const ct = res.headers.get("content-type") || "";
        const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
            ? await res.json().catch(() => null)
            : await res.text().catch(() => "");

        if (!res.ok) {
            if(res.status === 400){
                let bledy = body.errors;
                ustawBladAwatara(bledy.Awatar ? bledy.Awatar[0] : "");
            }
            return;
        }

        // jak tutaj dojdziemy, wszystko jest git
        navigate("/twojProfil", {state: {message: "Pomyślnie edytowano awatar"}});
    }

    /**
     * sprawdzamy, czy jest zablokowane wyślij
     */
    const czyZablokowaneWyslij = useMemo(() => {
            return (staryAwatar === null || awatar === null || staryAwatar === awatar);
    }, [awatar]);

    
    return(<form id = "form" name= "formularz-awatara">
        <img 
            id="awatar" 
            className="awatar" 
            src={podgladAwatara || "/img/domyslny_awatar.png"}
            alt="awatar"
            style={{ width: 128, height: 128, objectFit: "cover", borderRadius: "100%" }}
        /><br/><br/>
            <input
                type="file"
                accept="image/*"
                style={{ transform: "scale(0.8)", transformOrigin: "center" }}
                onChange={async (e) => {
                    ustawBladAwatara("");

                    const file = e.target.files?.[0];
                    if (!file) return;
                    ustawPodgladAwatara(URL.createObjectURL(file));

                    ustawAwatar(file);
                }}
            /><br/><br/>
        <input type = "button" value = "Zapisz" onClick={przyWysylaniu} disabled={czyZablokowaneWyslij}/>
        <span id = "error-awatar" className="error-wiadomosc">{bladAwatara}</span><br/><br/>
    </form>)
}