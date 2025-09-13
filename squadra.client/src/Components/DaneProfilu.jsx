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

            // najpierw pobieramy dane porfilu bez języków
            fetch("http://localhost:5014/api/profil/" + localStorage.getItem("idUzytkownika"), opcje)
                .then(response => response.json())
                .then(data => {
                    ustawPseudonim(data.pseudonim);
                    ustawZaimki(data.zaimki);
                    ustawKraj(data.kraj);
                    ustawRegion(data.region);
                    ustawOpis(data.opis);
                })
            
            // teraz pobieramy języki
            const opcje2 = {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                },
            };
            fetch("http://localhost:5014/api/jezyk/profil/" + localStorage.getItem("idUzytkownika"), opcje2)
                .then(response => response.json())
                .then(data => {
                    ustawListeJezykowUzytkownika(data.jezyki);
                })
        }
        podajDaneUzytkownika();
    }, []);
    
    return(<>
        <p>{jezyk.pseudonim}: {pseudonim}</p>
        <p>{jezyk.zaimki}: {zaimki}</p>
        <p>{jezyk.kraj}: {kraj == null ? "" : kraj.nazwa}</p>
        <p>{jezyk.region}: {region == null ? "" : region.nazwa}</p>
        <p>{jezyk.jezyki}</p>
        <ListaJezykow 
            jezyk = {jezyk} 
            typ = "wyswietlanie" 
            listaJezykowUzytkownika={listaJezykowUzytkownika} 
            ustawListeJezykowUzytkownika={ustawListeJezykowUzytkownika}/>
        <p>{jezyk.opis}: {opis}</p>
    </>)
}