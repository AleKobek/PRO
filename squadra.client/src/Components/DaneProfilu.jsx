import {useEffect, useState} from "react";
import ListaJezykow from "./ListaJezykow";

export default function DaneProfilu({jezyk}) {
    
    const [pseudonim, ustawPseudonim] = useState("");
    const [zaimki, ustawZaimki] = useState("");
    // {id, nazwa}
    const [kraj, ustawKraj] = useState("");
    // {id, nazwa}
    const [region, ustawRegion] = useState("");
    const [opis, ustawOpis] = useState("");

    // lista języków użytkownika to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}
    const [listaJezykowUzytkownika, ustawListeJezykowUzytkownika] = useState([])
    

    useEffect(() => {
        const podajDaneUzytkownika = () => {
            const opcje = {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                },
            };

            fetch("http://localhost:5014/api/uzytkownik/" + localStorage.getItem("idUzytkownika"), opcje)
                .then(response => response.json())
                .then(data => {
                    ustawListeJezykowUzytkownika(data.jezyki);
                    ustawPseudonim(data.pseudonim);
                    ustawZaimki(data.zaimki);
                    ustawKraj(data.kraj);
                    ustawRegion(data.region);
                    ustawOpis(data.opis);
                })
        }
        podajDaneUzytkownika();
    }, []);
    
    return(<>
        <p>{jezyk.pseudonim}: {pseudonim}</p>
        <p>{jezyk.zaimki}: {zaimki}</p>
        <p>{jezyk.kraj}: {kraj}</p>
        <p>{jezyk.region}: {region}</p>
        <p>{jezyk.jezyki}</p>
        <ListaJezykow 
            jezyk = {jezyk} 
            typ = "wyswietlanie" 
            listaJezykowUzytkownika={listaJezykowUzytkownika} 
            ustawListeJezykowUzytkownika={ustawListeJezykowUzytkownika}/>
        <p>{jezyk.opis}: {opis}</p>
    </>)
}