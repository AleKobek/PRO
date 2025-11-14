import React, {useEffect, useMemo, useState} from "react";
import ListaJezykow from "./ListaJezykow";
import {useNavigate} from "react-router-dom";

export default function FormularzProfilu({
                                             staraListaJezykowUzytkownika,
                                             staryPseudonim,
                                             stareZaimki,
                                             staryOpis,
                                             staryRegion,
                                             staryKraj,
                                             uzytkownik
                                         }) 
{
    
    const navigate = useNavigate();


    // {id, nazwa}
    const [listaKrajowZBazy, ustawListeKrajowZBazy] = useState([])
    // {id, nazwa, ?idKraju}
    const [listaRegionowZBazy, ustawListeRegionowZBazy] = useState([])
    
    
    const [bledy, ustawBledy] = useState({pseudonim: "", zaimki: "", opis: "", zapisz: ""});

    // to, co jest aktualnie w formularzu
    // lista języków użytkownika to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia, wartoscStopnia}
    // inicjalizacja lokalnego stanu ze starej listy, też ubezpieczamy się jakby nie istniała
    const [listaJezykowUzytkownika, ustawListeJezykowUzytkownika] = useState(() =>
        Array.isArray(staraListaJezykowUzytkownika)
            ? staraListaJezykowUzytkownika
            : []

    )
    const [pseudonim, ustawPseudonim] = useState("");
    const [zaimki, ustawZaimki] = useState("");
    const [kraj, ustawKraj] = useState({});
    const [region, ustawRegion] = useState({});
    // przychodzi w postaci {idRegionu, nazwaRegionu, idKraju, nazwaKraju}
    const [opis, ustawOpis] = useState("");

    // SYNC tylko gdy props się zmienia
    useEffect(() => {
        ustawListeJezykowUzytkownika(staraListaJezykowUzytkownika ?? []);
        ustawPseudonim(staryPseudonim ?? "");
        ustawZaimki(stareZaimki ?? "");
        ustawOpis(staryOpis ?? "");

        ustawKraj(staryKraj && typeof staryKraj === "object" ? staryKraj : {});
        ustawRegion(staryRegion && typeof staryRegion === "object" ? staryRegion : {});
    }, [staraListaJezykowUzytkownika, staryPseudonim, stareZaimki, staryOpis, staryKraj, staryRegion]);



    // Pobranie list krajów/regionów
    useEffect(() => {
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

        const podajKrajeIRegionyZBazy = async () => {
            
            // podajemy kraje
            const kraje = await fetchJsonAbort("http://localhost:5014/api/Kraj");
            if (!alive || !kraje || !Array.isArray(kraje)) return;
            ustawListeKrajowZBazy(kraje);
            
            // podajemy regiony
            const regiony = await fetchJsonAbort("http://localhost:5014/api/Region");
            if(!alive || !regiony || !Array.isArray(regiony)) return;
            ustawListeRegionowZBazy(regiony);
            
        };

        podajKrajeIRegionyZBazy();

        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            ac.abort();
            alive = false; // przerywamy fetch
        }
    }, []);



    const przyWysylaniu = async() =>{
        const profilDoWyslania = {
            regionId: region.id ?? null,
            zaimki: zaimki.trim(),
            opis: opis.trim(),
            jezyki: (listaJezykowUzytkownika ?? []).map(j => ({
                jezykId: j.idJezyka,
                stopienId: j.idStopnia
            })),
            pseudonim: pseudonim,
            awatar: null
            
        };
        
        const opcje = {
            method: "PUT",
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: "include",
            body: JSON.stringify(profilDoWyslania)
        }
        
        const res = await fetch("http://localhost:5014/api/Profil/" + uzytkownik.id, opcje);
        // Odczyt body różni się zależnie od typu odpowiedzi
        // jeżeli to 404, to zwraca tylko tekst (nie application/json), więc res.json rzuci wyjątek. musimy to uwzlgędnić
        const ct = res.headers.get("content-type") || "";       
        const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
            ? await res.json().catch(() => null)
            : await res.text().catch(() => "");

        if (!res.ok) {
            if(res.status === 400){
                let bledy = body.errors;
                ustawBledy({
                    pseudonim: bledy.Pseudonim ? bledy.Pseudonim[0] : "",
                    zaimki: bledy.Zaimki ? bledy.Zaimki[0] : "",
                    opis: bledy.Opis ? bledy.Opis[0] : "",
                    zapisz: body.message,
                });
            }
            return;
        }

        // jak tutaj dojdziemy, wszystko jest git
        navigate("/twojProfil");
    }

    // ustawiamy nową listę dostępnych regionów, jeśli kraj się zmieni
    const regionyDoWyboru = useMemo(() => {
        if (!kraj?.id) return [];
        return listaRegionowZBazy.filter(r => r.krajId == kraj.id);
    }, [kraj?.id, listaRegionowZBazy]);

    //
    useEffect(() => {
        if (region?.id && !regionyDoWyboru.some(r => r.id == region.id)) {
            ustawRegion({});
        }
    }, [regionyDoWyboru, region?.id]);

    /**
     * porównujemy czy użytkownik coś zmienił w swoich listach języków
     */
    const czyListyJezykoweTakieSame = useMemo(()=>{
        if(listaJezykowUzytkownika.length === 0 && staraListaJezykowUzytkownika.length === 0) return false;
        let posortowanaListaJezykowUzytkownika = [...listaJezykowUzytkownika].sort((a, b) => a.idJezyka - b.idJezyka);
        let posortowanaStaraListaJezykowUzytkownika = [...staraListaJezykowUzytkownika].sort((a, b) => a.idJezyka - b.idJezyka);
        if(listaJezykowUzytkownika.length !== staraListaJezykowUzytkownika.length) return false;
        for(let i = 0; i < listaJezykowUzytkownika.length; i++){
            if(posortowanaListaJezykowUzytkownika[i].idJezyka !== posortowanaStaraListaJezykowUzytkownika[i].idJezyka || posortowanaListaJezykowUzytkownika[i].idStopnia !== posortowanaStaraListaJezykowUzytkownika[i].idStopnia){
                return false;
            }
        }
        return true;
    }, [listaJezykowUzytkownika, staraListaJezykowUzytkownika]);
    


    /**
     * sprawdzamy, czy jest zablokowane wyślij
     */
    const czyZablokowaneWyslij = useMemo(() => {
        //console.log("Stary region id:", staryRegion.id, ", nowy region id:", region ? region.id : null);
            return (
                (pseudonim === staryPseudonim &&
                zaimki === stareZaimki &&
                region?.id === staryRegion?.id &&
                opis === staryOpis && czyListyJezykoweTakieSame) 
                ||
                pseudonim.trim().length === 0) // pseudonim nie może być pusty (reszta jest opcjonalna)
        }, [
            pseudonim, zaimki, kraj, region, opis,
            czyListyJezykoweTakieSame]);
    
    
    
    return(<form id = "form" name= "formularz-profilu">
        <label>Pseudonim<br/>
        <input 
            type="text" 
            id = "pseudonim" name ="pseudonim"
            value={pseudonim}
            // maxLength={20}
            onChange={(e)=>ustawPseudonim(e.target.value)}>
        </input></label><br/>
        <span id = "error-pseudonim" className="error-wiadomosc">{bledy.pseudonim}</span><br/>
        
        <label>Zaimki<br/>
        <input 
            type="text" 
            id = "zaimki" name ="zaimki" 
            maxLength={10} 
            value={zaimki} 
            onChange={(e)=>ustawZaimki(e.target.value)}>
        </input></label><br/>
        <span id = "error-zaimki" className="error-wiadomosc">{bledy.zaimki}</span><br/>
        
        {/* kraj */}
        
            <div id = "kraj">
                <label> Kraj <br/>
                <select
                    value={kraj?.id ?? ""}
                    onChange={(e) =>{
                        let id = e.target.value;
                        let tempKraj = listaKrajowZBazy.find((kraj) => kraj.id == id);
                        ustawKraj(tempKraj);
                    }}
                >
                    <option value = "" key = {-1}>Brak</option>
                    {
                        listaKrajowZBazy.map((kraj) => (
                            <option value={kraj.id} key = {kraj.id}>{kraj.nazwa}</option>
                        ))
                    }
                </select>
                </label>
            </div>
    
            {/* region */}
            <div id = "region">
                <label>Region<br/>
                    <select
                        value={region?.id ?? ""}
                        disabled={!kraj?.id}
                        onChange={(e) =>{
                            let id = e.target.value;
                            let tempRegion = listaRegionowZBazy.find((region) => region.id == id);
                            ustawRegion(tempRegion)
                        }}
                    >
                        {!kraj?.id ? (
                            // placeholder, gdy nic nie jest wybrane
                            <option value="">Brak</option>
                        ) : (
                            // gdy kraj wybrany - lista regionów dla kraju
                            <>
                                {regionyDoWyboru.map((region) =>(
                                    <option value={region.id} key={region.id}>{region.nazwa}</option>
                                ))}
                            </>
                        )}

                    </select>
                </label>
            </div>
        <br/>
        <label htmlFor="opis">Opis<br/>
        <textarea id = "opis" name ="opis" maxLength={100} value={opis} onChange={(e)=>ustawOpis(e.target.value)}></textarea></label>
        <span id = "error-opis" className="error-wiadomosc">{bledy.opis}</span><br/>
        
        <br/>
        
        {/* lista języków */}
        <label htmlFor="lista-jezykow">
            Lista języków
            <div id = "lista-jezykow" className="lista-jezykow">
            <ListaJezykow typ= "edycja" listaJezykowUzytkownika={listaJezykowUzytkownika} ustawListeJezykowUzytkownika={ustawListeJezykowUzytkownika}/>
            </div>
        </label>
        <br/>

        <input type = "button" value = "Zapisz" onClick={przyWysylaniu} disabled={czyZablokowaneWyslij}/>
        <span id = "error-zapisz" className="error-wiadomosc">{bledy.zapisz}</span><br/>
    </form>)

    
}