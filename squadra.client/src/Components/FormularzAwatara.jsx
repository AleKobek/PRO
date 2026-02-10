import React, {useEffect, useMemo, useState} from "react";
import {useNavigate} from "react-router-dom";
import {API_BASE_URL} from "../config/api";

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
        
        const res = await fetch(`${API_BASE_URL}/Profil/` + uzytkownik.id + "/awatar", opcje);
        
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
    }, [awatar, staryAwatar]);

    
    return(<form id = "form" name= "formularz-awatara">
        <img
            className="awatar block mx-auto w-[128px] h-[128px] object-cover rounded-full border-4 border-black mt-4"
            src={podgladAwatara || "/img/domyslny_awatar.png"}
            alt="awatar"
            // style={{ width: 128, height: 128, objectFit: "cover", borderRadius: "100%" }}
        /><br/><br/>
            <input
                type="file"
                accept="image/*"
                className="p-2"
                onChange={async (e) => {
                    ustawBladAwatara("");

                    const file = e.target.files?.[0];
                    if (!file) return;
                    ustawPodgladAwatara(URL.createObjectURL(file));

                    ustawAwatar(file);
                }}
            /><br/><br/>
        <input className={czyZablokowaneWyslij ? "zablokowany-przycisk" :"wyslij-formularz-przycisk"} type = "button" value = "Zapisz" onClick={przyWysylaniu} disabled={czyZablokowaneWyslij}/>
        <span id = "error-awatar" className="error-wiadomosc">{bladAwatara}</span><br/><br/>
    </form>)
}