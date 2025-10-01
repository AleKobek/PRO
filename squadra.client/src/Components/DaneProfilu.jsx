import {useEffect, useState} from "react";
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
            } 
            catch (e) {
                console.error(e);
            }
            finally {
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
        if(!pseudonim && !zaimki && !opis) {
            podajDaneProfilu().then();
        }
        if(listaJezykowUzytkownika.length === 0) podajJezykiIStopnieUzytkownika(localStorage.getItem("idUzytkownika")).then();
    }, []);
    
    if(!czyZaladowano) return(<><p>Ładowanie...</p></>);
    
    return(<div className = "dane-profilu">
        <label htmlFor = "pseudonim">Pseudonim:</label>
        <p id = "pseudonim" className= "pole-w-danych-profilu">{pseudonim}</p>
        <label htmlFor = "zaimki">Zaimki:</label>
        <p id= "zaimki" className= "pole-w-danych-profilu">{zaimki}</p>
        <label htmlFor = "kraj">Kraj:</label>
        <p id = "kraj" className= "pole-w-danych-profilu">{kraj ? kraj.nazwa : "Nie określono"}</p>
        <label htmlFor = "region">Region:</label>
        <p className= "pole-w-danych-profilu">{region ? region.nazwa : "Nie określono"}</p>
        <div id = "lista-jezykow-dane-profilu" className = "lista-jezykow-dane-profilu">
            <p style={{fontWeight: "bold"}}>Języki</p>
            <ListaJezykow typ = "wyswietlanie" listaJezykowUzytkownika = {listaJezykowUzytkownika} ustawListeJezykowUzytkownika={ustawListeJezykowUzytkownika}/>
        </div><br/>
        <label htmlFor = "opis">Opis:</label>
        <p id = "opis" className= "pole-w-danych-profilu">{opis}</p>
    </div>)
}