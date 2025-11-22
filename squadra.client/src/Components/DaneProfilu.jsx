import {useEffect, useState} from "react";
import ListaJezykow from "./ListaJezykow";

// pamiętać o tym, aby to było w nawiasach klamrowych!
export default function DaneProfilu({uzytkownik}) {
    
    const [pseudonim, ustawPseudonim] = useState("");
    const [zaimki, ustawZaimki] = useState("");
    // {id, nazwa}
    const [region, ustawRegion] = useState({});
    // {id, nazwa}
    const [kraj, ustawKraj] = useState({});
    const [opis, ustawOpis] = useState("");
    const [nazwaStatusu, ustawNazweStatusu] = useState("");
    const [awatar, ustawAwatar] = useState("");
    
    // lista języków użytkownika to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}
    const [listaJezykowUzytkownika, ustawListeJezykowUzytkownika] = useState([])
    
    
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

        const podajDaneProfilu = async () => {
            const idUzytkownika = uzytkownik.id;
            const data = await fetchJsonAbort(`http://localhost:5014/api/Profil/${idUzytkownika}`);

            // przerywamy działanie funkcji
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
            ustawNazweStatusu(data.nazwaStatusu ?? "");
            ustawAwatar(data.awatar ? "data:image/jpeg;base64,"+data.awatar : "");
        };

        const podajJezykiIStopnieUzytkownika = async () => {
            const idUzytkownika = uzytkownik.id;
            const data = await fetchJsonAbort(`http://localhost:5014/api/Jezyk/profil/${idUzytkownika}`);

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

        if(!pseudonim && !zaimki && !opis) {
            podajDaneProfilu();
        }
        if(listaJezykowUzytkownika.length === 0) podajJezykiIStopnieUzytkownika();
        
        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };
    }, []);


    if(!uzytkownik || !uzytkownik.id) return(<><p>Ładowanie...</p></>);
    
    return(<div className = "dane-profilu">
                                      {/* v dzięki "||" bierze też jak jest pusty string  */}
        <img id = "awatar" src = {awatar || "/img/domyslny_awatar.png"} alt = "awatar" className = "awatar"/><br/>
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
        <label>
            Status:
        <p id = "status" className= "pole-w-danych-profilu">{nazwaStatusu}</p>
        </label>
    </div>)
}