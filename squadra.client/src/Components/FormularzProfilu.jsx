import React, {useEffect, useState} from "react";
import ListaJezykow from "./ListaJezykow";
import {useNavigate} from "react-router-dom";

export default function FormularzProfilu(jezyk, czyEdytuj, staraListaJezykowUzytkownika, staryPseudonim, stareZaimki, staryOpis, staryRegion, staryKraj) {
    
    const navigate = useNavigate();
    
    // {id, nazwa}
    const [listaKrajowZBazy, ustawListeKrajowZBazy] = useState([])
    // {id, nazwa, idKraju}
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
    
    const [czyZablokowaneWyslij, ustawCzyZablokowaneWyslij] = useState(true);
    
    
    useEffect(() =>{
        
        // ######################################### tylko do prototypu! ###############################################
        localStorage.setItem("idUzytkownika", "1");
        
        // podajemy listę krajów i regionów z bazy, używanych do select
        const podajKrajeIRegionyZBazy = () => {
            const opcje = {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                },
            };
            fetch("http://localhost:5014/api/kraje", opcje)
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
            fetch("http://localhost:5014/api/regiony", opcje2)
                .then(response => response.json())
                .then(data => {
                ustawListeRegionowZBazy(data);
            })
        }
        podajKrajeIRegionyZBazy();
    })
    
    const przyWysylaniu = async() =>{
        
        const uzytkownik = {listaJezykow: listaJezykowUzytkownika, pseudonim: pseudonim, zaimki: zaimki, kraj: kraj, region: region, opis: opis};
        
        const opcje = {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(uzytkownik)
        }
        
        const resPost = await fetch("http://localhost:5014/api/uzytkownik/" + localStorage.getItem("idUzytkownika"), opcje);
        
        ustawBledy(resPost.bledy);
        
        if(resPost.czyPoprawny === true){
            navigate('/twojProfil');
        }
    }

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
            staryPseudonim, stareZaimki, staryKraj, staryRegion, staryOpis]);
    
    return(<form id = "form" name= "formularz-profilu">
        <label htmlFor="pseudonim">{jezyk.pseudonim}</label>
        <input type="text" id = "pseudonim" name ="pseudonim" value={pseudonim} onChange={()=>ustawPseudonim(e.target.value)}></input><br/>
        <span id = "error-pseudonim" className="error-wiadomosc">{bledy.pseudonim}</span><br/>
        
        <label htmlFor="zaimki">{jezyk.zaimki}</label>
        <input type="text" id = "zaimki" name ="zaimki" value={zaimki} onChange={()=>ustawZaimki(e.target.value)}></input><br/>
        <span id = "error-zaimki" className="error-wiadomosc">{bledy.zaimki}</span><br/>
        
        {/* kraj */}
        <label> {jezyk.kraj} <br/>
            <select onChange={(e) =>{ustawKraj(e.target.value)}}>
        {
            listaKrajowZBazy.map((kraj, index) => (
                <option value={kraj} key = {index} selected = {kraj.id === staryKraj.id}>{kraj.nazwa}</option>
            ))
        }
        </select>
        </label>
        
        {/* region */}
        <label>{jezyk.region}<br/>
            <select onChange={(e) =>{ustawRegion(e.target.value)}}>
                {
                    listaRegionowZBazy.filter((regionZListy) =>{
                        // na liście regionów mają być tylko te z aktualnego kraju
                        return regionZListy.idKraju !== kraj.id
                    }).map((regionZListy, index) =>(
                        <option value={regionZListy} key={index} selected={regionZListy.id === staryRegion.id}>{regionZListy.nazwa}</option>
                    ))
                }
            </select>
        </label>
        
        <label htmlFor="opis">{jezyk.opis}</label>
        <input type="text" id = "opis" name ="opis" value={opis} onChange={()=>ustawOpis(e.target.value)}></input>
        <span id = "error-opis" className="error-wiadomosc">{bledy.opis}</span><br/>
        
        {/* lista języków */}
        <ListaJezykow jezyk={jezyk} typ= "edycja" listaJezykowUzytkownika={listaJezykowUzytkownika} ustawListeJezykowUzytkownika={ustawListeJezykowUzytkownika}/>
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