import {useEffect, useState} from "react";
import JezykNaLiscieKomponent from "./JezykNaLiscieKomponent";
import {useJezyk} from "../LanguageContext.";

export default function ListaJezykow({typ}){

    const { jezyk } = useJezyk();
    
    // na razie nie chce mi się bawić z paginacją
    const liczbaJezykowNaStronie = 100;

    const [aktualnaStrona, ustawAktualnaStrone] = useState(0);
    const [liczbaStron, ustawLiczbeStron] = useState(0);
    
    // {id, nazwa}
    const [listaJezykowZBazy, ustawListeJezykowZBazy] = useState([])
    // {id, nazwa}
    const [listaStopniZBazy, ustawListeStopniZBazy] = useState([])
    // to, co aktualnie jest wybrane w select język, w postaci {id, nazwa}
    const [wybranyJezykDoDodania, ustawWybranyJezykDoDodania] = useState({})
    // to, co aktualnie jest wybrane w select stopień, w postaci {id, nazwa, wartosc}
    const [wybranyStopienDoDodania, ustawWybranyStopienDoDodania] = useState({})
    
    const [listaJezykowDostepnychDoDodania, ustawListeJezykowDostepnychDoDodania] = useState([])
    
    // lista języków użytkownika to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}
    const [listaJezykowUzytkownika, ustawListeJezykowUzytkownika] = useState([])
    
    const [czyZaladowanoJezykiIStopnieZBazy, ustawCzyZaladowanoJezykiIStopnieZBazy] = useState(false);
    const [czyZaladowanoJezykiIStopnieUzytkownika, ustawCzyZaladowanoJezykiIStopnieUzytkownika] = useState(false);

    useEffect(() => {
        
        const podajJezykiIStopnieZBazy = async () => {

            const opcje = {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                },
            };
            fetch("http://localhost:5014/api/Jezyk", opcje)
                .then(response => response.json())
                .then(data => ustawListeJezykowZBazy(data))
                .catch(error => {
                    console.log(error);
                });

            const opcje2 = {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                },
            };
            fetch("http://localhost:5014/api/StopienBieglosciJezyka", opcje2)
                .then(response => response.json())
                .then(data => ustawListeStopniZBazy(data))
                .catch(console.error);
        }
        
        //TODO jak to wyeksportować?
        /**
         * Ładujemy języki i stopnie użytkownika z bazy do listy
         * @param id id użytkownika
         * @returns {Promise<*>} lista języków użytkownika do ustawienia
         */
         const podajJezykiIStopnieUzytkownika = async (id) => {
            const opcje2 = {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                },
            };
            return fetch("http://localhost:5014/api/Jezyk/profil/" + id, opcje2)
                .then(response => response.json())
                .then(data => {
                    ustawLiczbeStron(Math.ceil(data.length / liczbaJezykowNaStronie));
                    let temp = data.map(item => ({
                        idJezyka: item.jezyk.id,
                        nazwaJezyka: item.jezyk.nazwa,
                        idStopnia: item.stopien.id,
                        nazwaStopnia: item.stopien.nazwa,
                        wartosc: item.stopien.wartosc
                    }));
                    temp.sort((a, b) => {
                        return b.wartosc - a.wartosc
                    });
                    return temp;
                });
        }
        if(!czyZaladowanoJezykiIStopnieZBazy) {
            podajJezykiIStopnieZBazy().then();
            ustawWybranyStopienDoDodania(listaStopniZBazy[0]);
            zaktualizujDostepneJezykiDoDodania();
            ustawWybranyJezykDoDodania(listaJezykowDostepnychDoDodania[0]);
            ustawCzyZaladowanoJezykiIStopnieZBazy(true);
        }
        if(!czyZaladowanoJezykiIStopnieUzytkownika && listaJezykowUzytkownika.length === 0 && listaJezykowZBazy.length > 0 && listaStopniZBazy.length > 0) {
            podajJezykiIStopnieUzytkownika(localStorage.getItem("idUzytkownika")).then(r => ustawListeJezykowUzytkownika(r));
            ustawCzyZaladowanoJezykiIStopnieUzytkownika(true);
        }
    },[listaStopniZBazy.length, listaJezykowZBazy.length, listaJezykowUzytkownika.length]);



    // do bazy wystarczy wysłać samo id każdego
    // adres backendu to "http://localhost:5014"


    const przyKliknieciuUsun = (e) => {
        const index = Number(e?.target?.id);
        if (Number.isNaN(index)) return;
        // używamy filter zwracając nową tablicę:
        ustawListeJezykowUzytkownika((prev) => prev.filter((_, i) => i !== index));
        zaktualizujDostepneJezykiDoDodania();
    }

    const przyKliknieciuDodaj = (e) => {
        e.preventDefault();
        console.log("wybrany język do dodania: ");
        console.log(wybranyJezykDoDodania);
        console.log("wybrany stopień do dodania: ");
        console.log(wybranyStopienDoDodania);
        if (!wybranyJezykDoDodania?.id || !wybranyStopienDoDodania?.id) return;
        ustawListeJezykowUzytkownika((prev) => ([
            ...prev,
            {
                idJezyka: wybranyJezykDoDodania.id,
                nazwaJezyka: wybranyJezykDoDodania.nazwa,
                idStopnia: wybranyStopienDoDodania.id,
                nazwaStopnia: wybranyStopienDoDodania.nazwa,
                wartosc: wybranyStopienDoDodania.wartosc
            }
        ]));
        zaktualizujDostepneJezykiDoDodania();
    }
    
    const zaktualizujDostepneJezykiDoDodania = () => {
        //TODO z jakiegoś powodu to się kurczy bardzo szybko i nie ma wszystkich języków. gdzieś musi być ucinane, tylko gdzie?
        //TODO w dodatku filtrowanie nie działa. przefiltrowało dopiero na początku i potem już nie filtruje
        let temp = listaJezykowZBazy;
        console.log("lista języków z bazy: ", temp)
        for (let i = 0; i < temp.length; i++) {
            let czyJest = listaJezykowUzytkownika.some(jezyk => {
                return jezyk.idJezyka == temp[i].id
            });
            if (czyJest) {
                temp.splice(i, 1);
                i--;
            }
        }
        console.log("lista dostępnych języków do ustawienia: ", temp)
        ustawListeJezykowDostepnychDoDodania(temp);
    }


    // przyciski do paginacji stron listy
    let przyciski = <>
        {/* strona 1 z 5 itp */}
        <p>{jezyk.ktoraStrona1}{aktualnaStrona + 1}{jezyk.ktoraStrona2}{liczbaStron}</p>

        {/* przycisk poprzedniej strony */}
        <button disabled={aktualnaStrona === 0}
                onClick={ () => { ustawAktualnaStrone(aktualnaStrona - 1) } }
        >{jezyk.poprzedniaStrona}</button>

        {/* przycisk następnej strony*/}
        <button disabled= {liczbaStron === 0 || aktualnaStrona === liczbaStron - 1}
                onClick={ () => { ustawAktualnaStrone(aktualnaStrona + 1) } }
        >{jezyk.nastepnaStrona}</button>
    </>
    
    if(!(czyZaladowanoJezykiIStopnieZBazy && czyZaladowanoJezykiIStopnieUzytkownika)) return(
        <>
            <p>{jezyk.ladowanie}</p>
        </>
    )
    else {

        // lista języków bez edycji
        if (typ === "wyswietlanie") {
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
                        {/*{listaJezykowUzytkownika.map((element, index) => {*/}
                        {/*    if (index >= aktualnaStrona * liczbaJezykowNaStronie && index < (aktualnaStrona + 1) * liczbaJezykowNaStronie) {*/}
                        {/*        return <JezykNaLiscieKomponent key={element.idJezyka - element.idStopnia} jezykDoKomponentu={element}*/}
                        {/*                                       idZListy={index} czyEdytuj={false}/>*/}
                        {/*    } else return <></>*/}
                        {/*})}*/}
                        {(listaJezykowUzytkownika ?? [])
                            .slice(aktualnaStrona * liczbaJezykowNaStronie, (aktualnaStrona + 1) * liczbaJezykowNaStronie)
                            .map((pozycja, index) => (
                                <JezykNaLiscieKomponent
                                    key={`${pozycja.idJezyka}-${pozycja.idStopnia}`} // użyj stabilnego, unikalnego klucza
                                    jezykDoKomponentu={pozycja} idZListy={index} czyEdytuj={false}
                                />
                            ))}
                    </ul>
                    {przyciski}
                </>
            )
        }

        if (typ === "edycja") {
            // jest lista języków {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}. obok każdego elementu jest usuwanie, pod spodem jest pusty z przyciskiem dodaj
            return (<>
                <ul>
                    {/*lista języków nad selectem z dodaniem nowego*/}
                    {listaJezykowUzytkownika.map((element, index) => {
                        if (index >= aktualnaStrona * 5 && index < (aktualnaStrona + 1) * 5) {
                            return (<>
                                <JezykNaLiscieKomponent jezyk={jezyk} jezykDoKomponentu={element} key={index} idZListy={index}
                                                        coPrzyKlikaniu={przyKliknieciuUsun} czyEdytuj={true}/>
                            </>)
                        }
                    })}
                    {/* select języka */}
                    <select onChange={(e) => {
                        let id = e.target.value;
                        let tempJezyk = listaJezykowZBazy.find(jezyk => jezyk.id == id);
                        // console.log("język do dodania: ",tempJezyk);
                        ustawWybranyJezykDoDodania(tempJezyk);
                    }}>
                        {listaJezykowDostepnychDoDodania.map((element, index) => (
                            // na każdy język {id, nazwa}
                            <option key={index} value={element.id}>{element.nazwa}</option>
                        ))}
                    </select>
                    {/* select stopnia */}
                    <select onChange={(e) => {
                        let id = e.target.value;
                        let tempStopien = listaStopniZBazy.find(stopien => stopien.id == id);
                        console.log("stopień do dodania: ",tempStopien);
                        ustawWybranyStopienDoDodania(tempStopien);
                    }}>
                        {listaStopniZBazy.map((element, index) => (
                            // elementem jest stopnień {id, nazwa}
                            <option key={index} value={element.id}>{element.nazwa}</option>
                        )).sort(
                            // sortujemy elementy listy względem pola wartosc
                            (stopien1, stopien2) => {
                                return stopien1.wartosc - stopien2.wartosc
                            }
                        )}
                    </select>
                    {/* przycisk obok selectów */}
                    <button onClick={przyKliknieciuDodaj}>{jezyk.dodaj}</button>
                </ul>
                {przyciski}
            </>)
        }
    }
    
    return(
        <>
            {jezyk.typTabelkiNieIstnieje}
        </>
    )
}