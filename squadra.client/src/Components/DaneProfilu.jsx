﻿import {useEffect, useState} from "react";
import ListaJezykow from "./ListaJezykow";

export default function DaneProfilu() {
    

    const [pseudonim, ustawPseudonim] = useState("");
    const [zaimki, ustawZaimki] = useState("");
    // {id, nazwa}
    const [region, ustawRegion] = useState({});
    // {id, nazwa}
    const [kraj, ustawKraj] = useState({});
    const [opis, ustawOpis] = useState("");

    // lista języków użytkownika to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}
    const [listaJezykowUzytkownika, ustawListeJezykowUzytkownika] = useState([])



    const [czyZaladowano, ustawCzyZaladowano] = useState(false);

    
    useEffect(() => {
        const ac = new AbortController();
        let alive = true;

        const fetchJson = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal });
                console.log("odpowiedź na fetch "+url, res);
                if (!res.ok) return null;
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                return null;
            }
        };

        const podajDaneProfilu = async () => {
            const idUzytkownika = localStorage.getItem("idUzytkownika");
            const data = await fetchJson(`http://localhost:5014/api/Profil/${idUzytkownika}`);

            if (!alive) return;

            ustawPseudonim(data.pseudonim ?? "");
            ustawZaimki(data.zaimki ?? "");
            // {idRegionu, nazwaRegionu, idKraju, nazwaKraju}
            const regionIKraj = data.regionIKraj;
            if(!regionIKraj){
                ustawRegion(null);
                ustawKraj(null);
            }else{
                ustawRegion({id: regionIKraj.idRegionu, nazwa: regionIKraj.nazwaRegionu})
                ustawKraj(regionIKraj.idKraju ? {id: regionIKraj.idKraju, nazwa: regionIKraj.nazwaKraju} : null);
            }
            ustawOpis(data.opis ?? "");
        };

        const podajJezykiIStopnieUzytkownika = async () => {
            const idUzytkownika = localStorage.getItem("idUzytkownika");
            const data = await fetchJson(`http://localhost:5014/api/Jezyk/profil/${idUzytkownika}`);

            if (!alive) return;

            if (!data) {
                ustawListeJezykowUzytkownika([]);
                return;
            }

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
        };

        (async () => {
            try {
                if(!pseudonim && !zaimki && !opis) {
                    await podajDaneProfilu();
                }
                if(listaJezykowUzytkownika.length === 0) await podajJezykiIStopnieUzytkownika();
            } finally {
                if (alive) ustawCzyZaladowano(true);
            }
        })();
        return () => {
            alive = false;
            ac.abort(); // przerywamy
        };
    }, []);


    if(!czyZaladowano) return(<><p>Ładowanie...</p></>);
    
    return(<div className = "dane-profilu">
        <label>
            Pseudonim:
            <p id = "pseudonim" className= "pole-w-danych-profilu">{pseudonim}</p>
        </label>
        <label>
            Zaimki:
            <p id= "zaimki" className= "pole-w-danych-profilu">{zaimki}</p>
        </label>
        <label>
            Kraj:
            <p id = "kraj" className= "pole-w-danych-profilu">{kraj ? kraj.nazwa : "Nie określono"}</p>
        </label>
        <label>
            Region:
            <p className= "pole-w-danych-profilu">{region ? region.nazwa : "Nie określono"}</p>
        </label>
        <div id = "lista-jezykow-dane-profilu" className = "lista-jezykow-dane-profilu">
            <p style={{fontWeight: "bold"}}>Języki</p>
            <ListaJezykow typ = "wyswietlanie" listaJezykowUzytkownika = {listaJezykowUzytkownika} ustawListeJezykowUzytkownika={ustawListeJezykowUzytkownika}/>
        </div><br/>
        <label>
            Opis:
            <p id = "opis" className= "pole-w-danych-profilu">{opis}</p>
        </label>
    </div>)
}