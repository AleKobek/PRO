import {useEffect, useMemo, useRef, useState} from "react";
import JezykNaLiscieKomponent from "./JezykNaLiscieKomponent";
import {useJezyk} from "../LanguageContext.";

export default function ListaJezykow({
                                         typ,
                                         listaJezykowUzytkownika = [],              // domyślnie pusta tablica zamiast undefined
                                         ustawListeJezykowUzytkownika = () => {}     // domyślna pusta funkcja
                                     }){


    const { jezyk } = useJezyk();

    const startedRef = useRef(false);

    // na razie nie chce mi się bawić z paginacją
    const liczbaJezykowNaStronie = 100;

    const [aktualnaStrona, ustawAktualnaStrone] = useState(0);
    const liczbaStron = Math.max(1, Math.ceil((listaJezykowUzytkownika?.length ?? 0) / liczbaJezykowNaStronie));

    // {id, nazwa}
    const [listaJezykowZBazy, ustawListeJezykowZBazy] = useState([])
    // {id, nazwa}
    const [listaStopniZBazy, ustawListeStopniZBazy] = useState([])
    // to, co aktualnie jest wybrane w select język, w postaci {id, nazwa}
    const [wybranyJezykDoDodania, ustawWybranyJezykDoDodania] = useState({})
    // to, co aktualnie jest wybrane w select stopień, w postaci {id, nazwa, wartosc}
    const [wybranyStopienDoDodania, ustawWybranyStopienDoDodania] = useState({})

    // Pochodna – bez setState w efekcie
    const listaJezykowDostepnychDoDodania = useMemo(() => {
        const zajete = new Set((listaJezykowUzytkownika ?? []).map(x => x.idJezyka ?? x.id));
        return (listaJezykowZBazy ?? []).filter(j => !zajete.has(j.id));
    }, [listaJezykowZBazy, listaJezykowUzytkownika]);
    
    // lista języków użytkownika to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}


    useEffect(() => {
        if (startedRef.current) return; // tylko dla inicjalnego pobrania
        startedRef.current = true;
        let alive = true;

        (async () => {
            try {
                const [jezykiRes, stopnieRes] = await Promise.all([
                    fetch("http://localhost:5014/api/Jezyk"),
                    fetch("http://localhost:5014/api/StopienBieglosciJezyka"),
                ]);
                if (!jezykiRes.ok) throw new Error(`GET /api/Jezyk -> ${jezykiRes.status}`);
                if (!stopnieRes.ok) throw new Error(`GET /api/StopienBieglosciJezyka -> ${stopnieRes.status}`);

                const [jezyki, stopnie] = await Promise.all([jezykiRes.json(), stopnieRes.json()]);
                ustawListeJezykowZBazy(jezyki);
                ustawListeStopniZBazy(stopnie);

            } catch (e) {
                console.error(e);
            }
        })();

        return () => { alive = false; };
    }, []);

    // 2) Gdy słowniki gotowe -> ustaw domyślne wybory i zaktualizuj dostępne języki
    useEffect(() => {
        if (listaStopniZBazy.length > 0) {
            ustawWybranyStopienDoDodania(listaStopniZBazy[0]);
        }
    }, [listaStopniZBazy]);


    useEffect(() => {
        if (listaJezykowDostepnychDoDodania.length > 0) {
            ustawWybranyJezykDoDodania(listaJezykowDostepnychDoDodania[0]);
        }
    }, [listaJezykowDostepnychDoDodania]);

    


    // adres backendu to "http://localhost:5014"


    const przyKliknieciuUsun = (e) => {
        const index = Number(e?.target?.id);
        if (Number.isNaN(index)) return;
        // używamy filter zwracając nową tablicę:
        ustawListeJezykowUzytkownika((prev) => prev.filter((_, i) => i !== index));
        //zaktualizujDostepneJezykiDoDodania();
    }

    const przyKliknieciuDodaj = (e) => {
        e.preventDefault();

        // Nie ustawiaj domyślnej wartości przez setState i od razu z niej korzystaj (setState jest async).
        // Zrób lokalny fallback:
        const jezyk = wybranyJezykDoDodania?.id
            ? wybranyJezykDoDodania
            : listaJezykowDostepnychDoDodania[0];

        const stopien = wybranyStopienDoDodania?.id
            ? wybranyStopienDoDodania
            : listaStopniZBazy[0];

        if (!jezyk?.id || !stopien?.id) return;

        ustawListeJezykowUzytkownika((prev) => ([
            ...prev,
            {
                idJezyka: jezyk.id,
                nazwaJezyka: jezyk.nazwa,
                idStopnia: stopien.id,
                nazwaStopnia: stopien.nazwa,
                wartosc: stopien.wartosc
            }
        ]));

        // Opcjonalnie: zresetuj wybór, żeby select pokazał następną dostępną opcję
        ustawWybranyJezykDoDodania(undefined);
        ustawWybranyStopienDoDodania(undefined);

        // NIE wywołujemy ręcznie "zaktualizujDostepneJezykiDoDodania"
    };

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
                        {(listaJezykowUzytkownika ?? [])
                            .slice(aktualnaStrona * liczbaJezykowNaStronie, (aktualnaStrona + 1) * liczbaJezykowNaStronie)
                            .sort((a, b) => b.wartosc - a.wartosc)
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
                    {listaJezykowUzytkownika
                        .sort((a, b) => b.wartosc - a.wartosc)
                        .map((element, index) => {
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
    
    return(
        <>
            {jezyk.typTabelkiNieIstnieje}
        </>
    )
}