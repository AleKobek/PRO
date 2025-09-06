import {useState} from "react";
import JezykNaLiscieKomponent from "./JezykNaLiscieKomponent";

export default function Tabelka({jezyk, typ}){

    const [aktualnaStrona, ustawAktualnaStrone] = useState(0);
    // {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}
    const [listaJezykowUzytkownika, ustawListeJezykowUzytkownika] = useState([])
    // {id, nazwa}
    const [listaJezykowZBazy, ustawListeJezykowZBazy] = useState([])
    // {id, nazwa}
    const [listaStopniZBazy, ustawListeStopniZBazy] = useState([])
    // to, co aktualnie jest wybrane w select język, w postaci {id, nazwa}
    const [wybranyJezykDoDodania, ustawWybranyJezykDoDodania] = useState({})
    // to, co aktualnie jest wybrane w select stopień, w postaci {id, nazwa, wartosc}
    const [wybranyStopienDoDodania, ustawWybranyStopienDoDodania] = useState({})
    
    // trzeba podać listę języków i dostępnych stopni z bazy
    // do bazy wystarczy wysłać samo id każdego
    
    
    const przyKliknieciuUsun = (e) => {
        let temp = [...listaJezykowUzytkownika];
        temp.filter((element, index) => index !== e.target.id);
        ustawListeJezykowUzytkownika(temp);
    }
    
    const przyKliknieciuDodaj = () => {
        // dodajemy do listy języków użytkownika
        let temp = [...listaJezykowUzytkownika];
        temp.push({
            idJezyka: wybranyJezykDoDodania.id,
            nazwaJezyka: wybranyJezykDoDodania.nazwa,
            idStopnia: wybranyStopienDoDodania.id,
            nazwaStopnia: wybranyStopienDoDodania.nazwa
        });
    }

    // przyciski do paginacji stron listy
    let przyciski = <>
        {/* strona 1 z 5 itp */}
        <p>{jezyk.ktoraStrona1}{aktualnaStrona + 1}{jezyk.ktoraStrona2}{listaJezykowUzytkownika.length}</p>

        {/* przycisk poprzedniej strony */}
        <button disabled={aktualnaStrona === 0}
                onClick={ () => { ustawAktualnaStrone(aktualnaStrona - 1) } }
        >{jezyk.poprzedniaStrona}</button>

        {/* przycisk następnej strony */}
        <button disabled={aktualnaStrona === listaJezykowUzytkownika.length - 1}
                onClick={ () => { ustawAktualnaStrone(aktualnaStrona + 1) } }
        >{jezyk.nastepnaStrona}</button>
    </>
    
    // lista języków bez edycji
    if (typ === "językiWProfilu") {
        if (listaJezykowUzytkownika.length === 0) {
            return (<>
                <p style={{fontFamily: "Comic Sans MS"}}>
                    {jezyk.pustaTabelka}
                </p>
            </>);
        }
        return (
            <>
                <ul>
                    {listaJezykowUzytkownika.map((element, index) => {
                        if (index >= aktualnaStrona * 5 && index < (aktualnaStrona + 1) * 5) {
                            return <JezykNaLiscieKomponent jezyk={jezyk} jezykDoKomponentu={element} idZListy={index} czyEdytuj={false}/>
                        }
                    })}
                </ul>
                {przyciski}
            </>
        )
    }
    
    if(typ === "językiEdycja"){
        // jest lista języków {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}. obok każdego elementu jest usuwanie, pod spodem jest pusty z przyciskiem dodaj
        return(<>
            <ul>
                {/*lista języków nad selectem z dodaniem nowego*/}
                {listaJezykowUzytkownika.map((element, index) => {
                    if (index >= aktualnaStrona * 5 && index < (aktualnaStrona + 1) * 5) {
                        return (<>
                            <JezykNaLiscieKomponent jezyk={jezyk} jezykDoKomponentu={element} idZListy={index} coPrzyKlikaniu={przyKliknieciuUsun} czyEdytuj={true}/>
                        </>)
                    }
                })}
                {/* select języka */}
                <select onChange={(e) =>{
                    ustawWybranyJezykDoDodania(e.target.value)
                }}>
                    {listaJezykowZBazy.filter(
                        // na liście opcji mają być do wyboru tylko te, które nie są już wśród listy języków użytkownika
                        (jezyk) => {
                            // w predykacie zwracamy false jeśli trzeba odfiltrować
                            if(jezyk.id === wybranyJezykDoDodania.id) return false;
                            let czyJest = false;
                            for(let i = 0; i < listaJezykowUzytkownika.length; i++){
                                // jeżeli język w opcjach miałby być jednym z elementów które są już w liście
                                if(listaJezykowUzytkownika[i].id === jezyk.id) czyJest = true;
                            }
                            return !czyJest;
                        }
                    ).map((element, index) => (
                        // na każdy język {id, nazwa}
                        <option key={index} value={element}>{element.nazwa}</option>
                    ))}
                </select>
                {/* select stopnia */}
                <select onChange={(e) =>{
                    ustawWybranyStopienDoDodania(e.target.value)
                }}>
                    {listaStopniZBazy.map((element, index) => (
                        // elementem jest stopnień {id, nazwa}
                        <option key={index} value={element}>{element.nazwa}</option>
                    )).sort(
                        // sortujemy elementy listy względem pola wartosc
                        (stopien1, stopien2) =>{
                            if(stopien1.wartosc < stopien2.wartosc) return -1;
                            if(stopien1.wartosc > stopien2.wartosc) return 1;
                            return 0;
                        }
                    )}
                </select>
                {/* przycisk obok selectów */}
                <button onClick={przyKliknieciuDodaj}>{jezyk.dodaj}</button>
            </ul>
            {przyciski}
        </>)
    }
    
    return(
        <>
            {jezyk.typTabelkiNieIstnieje}
        </>
    )
}