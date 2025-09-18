import React, {useEffect, useState} from "react";
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
    const [listaJezykowUzytkownika, ustawListeJezykowUzytkownika] = useState([])
    const [pseudonim, ustawPseudonim] = useState("");
    const [zaimki, ustawZaimki] = useState("");
    const [kraj, ustawKraj] = useState({});
    const [region, ustawRegion] = useState({});
    const [opis, ustawOpis] = useState("");
    
    const [listaRegionowAktualnegoKraju, ustawListeRegionowAktualnegoKraju] = useState([]);
    
    const [czyZablokowaneWyslij, ustawCzyZablokowaneWyslij] = useState(true);

    // KLUCZOWE: zsynchronizuj lokalny stan z props, gdy props się zaktualizuje po fetchu
    useEffect(() => {
        const bezpiecznaLista = Array.isArray(staraListaJezykowUzytkownika) ? staraListaJezykowUzytkownika : [];
        ustawListeJezykowUzytkownika(bezpiecznaLista);
    }, [staraListaJezykowUzytkownika]);


    // podajemy listę krajów i regionów z bazy, używanych do select
    useEffect(() =>{
        
        const podajKrajeIRegionyZBazy = async () => {
            const opcje = {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                },
            };
            fetch("http://localhost:5014/api/Kraj", opcje)
                .then(response => response.json())
                .then(data => {
                    ustawListeKrajowZBazy(data);
                })
            
            const opcje2 = {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                },
            };
            fetch("http://localhost:5014/api/Region", opcje2)
                .then(response => response.json())
                .then(data => {
                ustawListeRegionowZBazy((poprzedniaLista) => [...poprzedniaLista, ...data]);
                ustawListeRegionowAktualnegoKraju(data.filter((obiekt) => obiekt.idKraju === kraj.id));
            })
        }
        podajKrajeIRegionyZBazy().then();
    },[])
    
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

    /**
     * sprawdzamy, czy jest zablokowane wyślij
     */
    useEffect(() => {
        ustawCzyZablokowaneWyslij(
            // jest to samo, co wcześniej
            (pseudonim === staryPseudonim &&
            zaimki === stareZaimki &&
            kraj === staryKraj &&
            region === staryRegion &&
            opis === staryOpis && czyListyJezykoweTakieSame()) ||
            // pseudonim jest pusty (zaimki i opis są opcjonalne)
            pseudonim.trim().length === 0
        )
    }, [pseudonim, zaimki, kraj, region, opis,
            staryPseudonim, stareZaimki, staryKraj, staryRegion, staryOpis, czyListyJezykoweTakieSame]);
    
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
                    ustawListeRegionowAktualnegoKraju(listaRegionowZBazy.filter((obiekt) => obiekt.krajId == id));
                }}>
                    <option value={null} key = {-1} selected={staryKraj == null}>{jezyk.brak}</option>
                    {
                        // zamiast tego powinniśmy wkładać do środka id kraju i potem przy wysyłaniu szukać
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
                            listaRegionowAktualnegoKraju.map((regionZListy, index) =>(
                                <option value={regionZListy} key={index} selected={staryRegion == null ? false : regionZListy.id === staryRegion.id}>{regionZListy.nazwa}</option>
                            ))
                        }
                    </select>
                </label>
            </div>
        
        <label htmlFor="opis">{jezyk.opis}</label><br/>
        <input type="text" id = "opis" name ="opis" value={opis} onChange={(e)=>ustawOpis(e.target.value)}></input>
        <span id = "error-opis" className="error-wiadomosc">{bledy.opis}</span><br/>
        
        {/* lista języków */}
        <ListaJezykow typ= "edycja" listaJezykowUzytkownika={listaJezykowUzytkownika} ustawListeJezykowUzytkownika={ustawListeJezykowUzytkownika}/>
        <br/>

        <input type = "button" value = {jezyk.zapisz} onClick={przyWysylaniu} disabled={czyZablokowaneWyslij}/>
    </form>)

    /**
     * porównujemy czy użytkownik coś zmienił w swoich listach języków
     */
    function czyListyJezykoweTakieSame(){
        let tempListaJezykowUzytkownika = [...listaJezykowUzytkownika].sort((a, b) => a.idJezyka - b.idJezyka);
        if(listaJezykowUzytkownika.length !== staraListaJezykowUzytkownika.length) return false;
        for(let i = 0; i < listaJezykowUzytkownika.length; i++){
            if(tempListaJezykowUzytkownika[i].idJezyka !== staraListaJezykowUzytkownika[i].idJezyka || tempListaJezykowUzytkownika[i].idStopnia !== staraListaJezykowUzytkownika[i].idStopnia){
                return false;
            }
        }
        return true;
    } 
}