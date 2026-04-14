import React, {useMemo, useState} from "react";
import {useNavigate} from "react-router-dom";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast} from "react-toastify";

// zerżnięte z formularza awatara
export default function FormularzPlatformy()
{
    const [idPlatformy, ustawIdPlatformy] = useState(0);
    const [bladIdPlatformy, ustawBladIdPlatformy] = useState("");
    const [nazwaPlatformy, ustawNazwaPlatformy] = useState("");
    const [bladNazwyPlatformy, ustawBladNazwyPlatformy] = useState("");
    const [bladLogo, ustawBladLogo] = useState("");
    const [logo, ustawlogo] = useState(null);
    const [podgladLogo, ustawPodgladLogo] = useState("");
    const navigate = useNavigate();


    
    const przyWysylaniu = async() =>{

        console.log("Wysyłamy!")
        console.log("idPlatformy", idPlatformy);
        console.log("nazwa", nazwaPlatformy);

        ustawBladLogo("");
        ustawBladNazwyPlatformy("");
        ustawBladIdPlatformy("");

        if (!logo) {
            ustawBladLogo("Wybierz plik logo.");
            return;
        }
        if(nazwaPlatformy.trim() === ""){
            ustawBladNazwyPlatformy("Nazwa platformy jest wymagana.");
            return;
        }
        if(idPlatformy <= 0){
            ustawBladIdPlatformy("Nieprawodłowe id platformy.");
        }

        const formularz = new FormData();
        formularz.append("logo", logo);
        formularz.append("nazwa", nazwaPlatformy);

        const opcje = {
            method: "POST",
            credentials: "include",
            body: formularz
        }
        
        const res = await fetch(`${API_BASE_URL}/Platforma/`+idPlatformy, opcje);
        
        // Odczyt body różni się zależnie od typu odpowiedzi
        // jeżeli to 404, to zwraca tylko tekst (nie application/json), więc res.json rzuci wyjątek. musimy to uwzlgędnić
        const ct = res.headers.get("content-type") || "";
        const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
            ? await res.json().catch(() => null)
            : await res.text().catch(() => "");

        if (!res.ok) {
            if(res.status === 400){
                let bledy = body.errors;
                ustawBladLogo(bledy.logo ? bledy.logo[0] : "");
                toast.error('Wystąpił błąd podczas dodawania platformy', {
                    position: "top-center",
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: false,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                    theme: "light",
                    transition: Bounce,
                });
            }
            return;
        }

        // jak tutaj dojdziemy, wszystko jest git
        navigate("/twojProfil", {
            state: { pomyslnieEdytowanoProfil: true }
        });
    }

    /**
     * sprawdzamy, czy jest zablokowane wyślij
     */
    const czyZablokowaneWyslij = useMemo(() => {
        return (idPlatformy === null || nazwaPlatformy.trim().length === 0 || logo === null);
    }, [idPlatformy, nazwaPlatformy, logo]);

    
    return(<form id = "form" name= "formularz-platformy">
        <label>Id<br/>
            <input
                type="number"
                id = "id" name ="id"
                value={idPlatformy}
                onChange={(e)=>ustawIdPlatformy(e.target.value)}>
            </input></label><br/>
        <span className="error-wiadomosc">{bladIdPlatformy}</span><br/>
        <label>Nazwa<br/>
            <input
                type="text"
                id = "nazwa" name ="nazwa"
                value={nazwaPlatformy}
                onChange={(e)=>ustawNazwaPlatformy(e.target.value)}>
            </input></label><br/>
        <span className="error-wiadomosc">{bladNazwyPlatformy}</span><br/>
        <img
            className="awatar block mx-auto w-[128px] h-[128px] object-cover rounded-full border-4 border-black mt-4"
            src={podgladLogo || "/img/domyslny_awatar.png"}
            alt="awatar"
            // style={{ width: 128, height: 128, objectFit: "cover", borderRadius: "100%" }}
        /><br/><br/>
            <input
                type="file"
                accept="image/*"
                className="p-2"
                onChange={async (e) => {
                    ustawBladLogo("");

                    const file = e.target.files?.[0];
                    if (!file) return;
                    ustawPodgladLogo(URL.createObjectURL(file));

                    ustawlogo(file);
                }}
            /><br/><br/>
        <input className={czyZablokowaneWyslij ? "zablokowany-przycisk" :"wyslij-formularz-przycisk"} type = "button" value = "Zapisz" onClick={przyWysylaniu} disabled={czyZablokowaneWyslij}/>
        <span id = "error-awatar" className="error-wiadomosc">{bladLogo}</span><br/><br/>
    </form>)
}