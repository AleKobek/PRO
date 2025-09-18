import {useEffect, useState} from "react";
import ListaJezykow from "./ListaJezykow";
import {useJezyk} from "../LanguageContext.";

export default function DaneProfilu() {
    
    const { jezyk } = useJezyk();

    const [pseudonim, ustawPseudonim] = useState("");
    const [zaimki, ustawZaimki] = useState("");
    // {idRegionu, nazwaRegionu, idKraju, nazwaKraju}
    const [regionIKraj, ustawRegionIKraj] = useState({});
    const [opis, ustawOpis] = useState("");

    
    
    const [czyZaladowano, ustawCzyZaladowano] = useState(false);
    

    useEffect(() => {
        const podajDaneUzytkownika = async () => {
            const opcje = {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                },
            };
            try {
                const response = await fetch("http://localhost:5014/api/Profil/" + localStorage.getItem("idUzytkownika"), opcje);
                const data = await response.json();
                ustawPseudonim(data.pseudonim ?? "");
                ustawZaimki(data.zaimki ?? "");
                ustawRegionIKraj(data.region ?? null);
                ustawOpis(data.opis ?? "");
            } finally {
                // ustawiamy dopiero po zakończeniu próby pobrania
                ustawCzyZaladowano(true);
            }

        }
        if(!pseudonim && !zaimki && (!regionIKraj || Object.keys(regionIKraj).length === 0) && !opis) {
            podajDaneUzytkownika().then();
        }
    }, []);
    
    //if(!czyZaladowano) return(<><p>{jezyk.ladowanie}</p></>);
    
    return(<>
        <p>{jezyk.pseudonim}: {pseudonim}</p>
        <p>{jezyk.zaimki}: {zaimki}</p>
        <p>{jezyk.kraj}: {regionIKraj?.nazwaKraju || ""}</p>
        <p>{jezyk.region}: {regionIKraj?.nazwaRegionu || ""}</p>
        <p>{jezyk.jezyki}</p>
        <ListaJezykow typ = "wyswietlanie"/>
        <p>{jezyk.opis}: {opis}</p>
    </>)
}