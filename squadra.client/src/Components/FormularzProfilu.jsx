import React, {useEffect, useMemo, useState} from "react";
import ListaJezykow from "./ListaJezykow";
import {useNavigate} from "react-router-dom";
import {useJezyk} from "../LanguageContext.";

export default function FormularzProfilu({
                                             staraListaJezykowUzytkownika,
                                             staryPseudonim,
                                             stareZaimki,
                                             staryOpis,
                                             staryRegion,
                                             staryKraj,
                                         }) 
{
    
    const navigate = useNavigate();
    const { jezyk } = useJezyk();


    // {id, nazwa}
    const [listaKrajowZBazy, ustawListeKrajowZBazy] = useState([])
    // {id, nazwa, ?idKraju}
    const [listaRegionowZBazy, ustawListeRegionowZBazy] = useState([])
    
    
    const [bledy, ustawBledy] = useState({pseudonim: "", zaimki: "", opis: ""});

    // to, co jest aktualnie w formularzu
    // lista języków użytkownika to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}
    // Uwaga: inicjalizacja lokalnego stanu z propsa:
    const [listaJezykowUzytkownika, ustawListeJezykowUzytkownika] = useState(() =>
        Array.isArray(staraListaJezykowUzytkownika)
            ? staraListaJezykowUzytkownika
            : []

    )
    const [pseudonim, ustawPseudonim] = useState("");
    const [zaimki, ustawZaimki] = useState("");
    const [kraj, ustawKraj] = useState({});
    const [region, ustawRegion] = useState({});
    const [opis, ustawOpis] = useState("");

    // Uwaga: SYNC tylko gdy props się zmienia – bez lokalnych stanów w depsach
    useEffect(() => {
        ustawListeJezykowUzytkownika(staraListaJezykowUzytkownika ?? []);
        ustawPseudonim(staryPseudonim ?? "");
        ustawZaimki(stareZaimki ?? "");
        ustawOpis(staryOpis ?? "");

        ustawKraj(staryKraj && typeof staryKraj === "object" ? staryKraj : {});
        ustawRegion(staryRegion && typeof staryRegion === "object" ? staryRegion : {});
    }, [staraListaJezykowUzytkownika, staryPseudonim, stareZaimki, staryOpis, staryKraj, staryRegion]);



    // Pobranie list krajów/regionów (z cleanup)
    useEffect(() => {
        const ac = new AbortController();

        const podajKrajeIRegionyZBazy = async () => {
            try {
                const [krajeRes, regionyRes] = await Promise.all([
                    fetch("http://localhost:5014/api/Kraj", { signal: ac.signal }),
                    fetch("http://localhost:5014/api/Region", { signal: ac.signal }),
                ]);

                if (!krajeRes.ok) throw new Error(`GET /api/Kraj -> ${krajeRes.status}`);
                if (!regionyRes.ok) throw new Error(`GET /api/Region -> ${regionyRes.status}`);

                const [kraje, regiony] = await Promise.all([krajeRes.json(), regionyRes.json()]);

                ustawListeKrajowZBazy(kraje);
                ustawListeRegionowZBazy(regiony);
            } catch (err) {
                if (err.name !== "AbortError") {
                    console.error("Błąd podczas pobierania krajów/regionów:", err);
                }
            }
        };

        podajKrajeIRegionyZBazy();
        return () => ac.abort();
    }, []);



    const przyWysylaniu = async() =>{
        // sprawdzić wszystkie pola!
        const profil = {
            IdUzytkownika: localStorage.getItem("idUzytkownika"),
            RegionId: region.id,
            Zaimki: zaimki,
            Opis: opis,
            Jezyki: listaJezykowUzytkownika,
            Pseudonim: pseudonim,
            // ############# tylko do prototypu ################
            Awatar: null};
        
        const opcje = {
            method: "PUT",
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(profil)
        }
        
        const resPost = await fetch("http://localhost:5014/api/profil/" + localStorage.getItem("idUzytkownika"), opcje);
        
        ustawBledy(resPost.bledy);
        
        if(resPost.czyPoprawny === true){
            navigate('/twojProfil');
        }
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
        //console.log("Posortowana lista języków użytkownika z formularza:", posortowanaListaJezykowUzytkownika);
        //console.log("Stara lista języków użytkownika:", staraListaJezykowUzytkownika);
        if(listaJezykowUzytkownika.length !== staraListaJezykowUzytkownika.length) return false;
        for(let i = 0; i < listaJezykowUzytkownika.length; i++){
            //console.log("Teraz porównujemy języki:", posortowanaListaJezykowUzytkownika[i], "z", staraListaJezykowUzytkownika[i])
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
            return (
                (pseudonim === staryPseudonim &&
                zaimki === stareZaimki &&
                kraj?.id === staryKraj?.id &&
                region?.id === staryRegion?.id &&
                opis === staryOpis && czyListyJezykoweTakieSame) 
                ||
                pseudonim.trim().length === 0) // pseudonim nie może być pusty (reszta jest opcjonalna)
        }, [
            pseudonim, zaimki, kraj, region, opis,
            czyListyJezykoweTakieSame]);
    
    
    
    return(<form id = "form" name= "formularz-profilu">
        <label htmlFor="pseudonim">{jezyk.pseudonim}</label><br/>
        <input type="text" id = "pseudonim" name ="pseudonim" value={pseudonim} onChange={(e)=>ustawPseudonim(e.target.value)}></input><br/>
        <span id = "error-pseudonim" className="error-wiadomosc">{bledy.pseudonim}</span><br/>
        
        <label htmlFor="zaimki">{jezyk.zaimki}</label><br/>
        <input type="text" id = "zaimki" name ="zaimki" value={zaimki} onChange={(e)=>ustawZaimki(e.target.value)}></input><br/>
        <span id = "error-zaimki" className="error-wiadomosc">{bledy.zaimki}</span><br/>
        
        {/* kraj */}
        
            <div id = "kraj">
                <label> {jezyk.kraj} <br/>
                <select onChange={(e) =>{
                    let id = e.target.value;
                    let tempKraj = listaKrajowZBazy.find((kraj) => kraj.id == id);
                    ustawKraj(tempKraj);
                }}>
                    <option value={null} key = {-1} selected={staryKraj == null}>{jezyk.brak}</option>
                    {
                        listaKrajowZBazy.map((kraj, index) => (
                            <option value={kraj.id} key = {index} selected = {staryKraj == null ? false : kraj.id === staryKraj.id}>{kraj.nazwa}</option>
                        ))
                    }
                </select>
                </label>
            </div>
    
            {/* region */}
            <div id = "region">
                <label>{jezyk.region}<br/>
                    <select onChange={(e) =>{ustawRegion(e.target.value)}}>
                        <option value={null} key = {-1} selected = {staryRegion == null}>{jezyk.brak}</option>
                        {
                            regionyDoWyboru.map((regionZListy, index) =>(
                                <option value={regionZListy} key={index} selected={staryRegion == null ? false : regionZListy.id === staryRegion.id}>{regionZListy.nazwa}</option>
                            ))
                        }
                    </select>
                </label>
            </div>
        
        <label htmlFor="opis">{jezyk.opis}</label><br/>
        <textarea id = "opis" name ="opis" maxLength={100} value={opis} onChange={(e)=>ustawOpis(e.target.value)}></textarea>
        <span id = "error-opis" className="error-wiadomosc">{bledy.opis}</span><br/>
        
        {/* lista języków */}
        <ListaJezykow typ= "edycja" listaJezykowUzytkownika={listaJezykowUzytkownika} ustawListeJezykowUzytkownika={ustawListeJezykowUzytkownika}/>
        <br/>

        <input type = "button" value = {jezyk.zapisz} onClick={przyWysylaniu} disabled={czyZablokowaneWyslij}/>
    </form>)

    
}