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

    // lista języków użytkownika to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}
    const [listaJezykowUzytkownika, ustawListeJezykowUzytkownika] = useState([])

    
    
    const [czyZaladowano, ustawCzyZaladowano] = useState(false);
    

    useEffect(() => {
        const podajDaneProfilu = async () => {
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
                ustawRegionIKraj(data.regionIKraj ?? null);
                ustawOpis(data.opis ?? "");
            } finally {
                // ustawiamy dopiero po zakończeniu próby pobrania
                ustawCzyZaladowano(true);
            }

        }
        const podajJezykiIStopnieUzytkownika = async () => {
            try {
                const r = await fetch(`http://localhost:5014/api/Jezyk/profil/${localStorage.getItem("idUzytkownika")}`, {
                    headers: { "Content-Type": "application/json" },
                });
                const data = await r.json();
                const temp = data
                    .map(item => ({
                        idJezyka: item.jezyk.id,
                        nazwaJezyka: item.jezyk.nazwa,
                        idStopnia: item.stopien.id,
                        nazwaStopnia: item.stopien.nazwa,
                        wartosc: item.stopien.wartosc,
                    }))
                    .sort((a, b) => b.wartosc - a.wartosc);
                ustawListeJezykowUzytkownika(temp);
            } catch (e) {
                console.error(e);
            }
        }
        if(!pseudonim && !zaimki && (!regionIKraj || Object.keys(regionIKraj).length === 0) && !opis) {
            podajDaneProfilu().then();
        }
        if(listaJezykowUzytkownika.length === 0) podajJezykiIStopnieUzytkownika(localStorage.getItem("idUzytkownika")).then();
    }, []);
    
    //if(!czyZaladowano) return(<><p>{jezyk.ladowanie}</p></>);
    
    return(<>
        <p>{jezyk.pseudonim}: {pseudonim}</p>
        <p>{jezyk.zaimki}: {zaimki}</p>
        <p>{jezyk.kraj}: {regionIKraj?.nazwaKraju || "Unknown"}</p>
        <p>{jezyk.region}: {regionIKraj?.nazwaRegionu || "Unknown"}</p>
        <p>{jezyk.jezyki}</p>
        <ListaJezykow typ = "wyswietlanie" listaJezykowUzytkownika = {listaJezykowUzytkownika} ustawListeJezykowUzytkownika={ustawListeJezykowUzytkownika}/>
        <p>{jezyk.opis}: {opis}</p>
    </>)
}