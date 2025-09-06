import {useState} from "react";
import ListaJezykow from "./ListaJezykow";

export default function DaneProfilu({jezyk}) {
    
    const [pseudonim, ustawPseudonim] = useState("");
    const [zaimki, ustawZaimki] = useState("");
    const [kraj, ustawKraj] = useState("");
    const [region, ustawRegion] = useState("");
    const [opis, ustawOpis] = useState("");
    
    // tutaj wysyłamy get do bazy
    
    return(<>
        <p>{jezyk.pseudonim}: {pseudonim}</p>
        <p>{jezyk.zaimki}: {zaimki}</p>
        <p>{jezyk.kraj}: {kraj}</p>
        <p>{jezyk.region}: {region}</p>
        <p>{jezyk.jezyki}</p><ListaJezykow jezyk = {jezyk} typ = "językiWProfilu"></ListaJezykow>
        <p>{jezyk.opis}: {opis}</p>
    </>)
}