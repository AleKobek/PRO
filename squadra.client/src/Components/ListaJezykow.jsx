import {useEffect, useMemo, useRef, useState} from "react";
import JezykNaLiscieKomponent from "./JezykNaLiscieKomponent";

export default function ListaJezykow({
                                         typ,
                                         listaJezykowUzytkownika = [],
                                         ustawListeJezykowUzytkownika = () => {}
                                     }){



    const startedRef = useRef(false);

    const liczbaJezykowNaStronie = 3;

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
                    fetch("http://localhost:5014/api/Jezyk", {credentials: "include"}),
                    fetch("http://localhost:5014/api/StopienBieglosciJezyka", {credentials: "include"}),
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

    // gdy dane są pobrane, ustawiamy domyślne wybory i aktualizujemy dostępne języki
    useEffect(() => {
        if (listaStopniZBazy.length > 0 && !wybranyStopienDoDodania) {
            ustawWybranyStopienDoDodania(listaStopniZBazy[0]);
        }
    }, [listaStopniZBazy]);


    useEffect(() => {
        if (listaJezykowDostepnychDoDodania.length > 0 && !wybranyJezykDoDodania) {
            ustawWybranyJezykDoDodania(listaJezykowDostepnychDoDodania[0]);
        }
    }, [listaJezykowDostepnychDoDodania]);




    // adres backendu to "http://localhost:5014"


    const przyKliknieciuUsun = (e) => {
        const index = Number(e?.target?.id);
        if (Number.isNaN(index)) return;
        // używamy filter zwracając nową tablicę:
        ustawListeJezykowUzytkownika((prev) => prev.filter((_, i) => i !== index));
    }

    const przyKliknieciuDodaj = (e) => {
        e.preventDefault();

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

        ustawWybranyJezykDoDodania(undefined);
        ustawWybranyStopienDoDodania(undefined);

    };

    // przyciski do paginacji stron listy
    let przyciski = <>
        {/* strona 1 z 5 itp */}
        <p>Strona {aktualnaStrona + 1} z {liczbaStron}</p>

        {/* przycisk poprzedniej strony */}
        <button disabled={aktualnaStrona === 0}
                onClick={ () => { ustawAktualnaStrone(aktualnaStrona - 1) } }
        >Poprzednia strona</button>

        {/* przycisk następnej strony*/}
        <button disabled= {liczbaStron === 0 || aktualnaStrona === liczbaStron - 1}
                onClick={ () => { ustawAktualnaStrone(aktualnaStrona + 1) } }
        >Następna strona</button>
    </>


    // lista języków bez edycji
    if (typ === "wyswietlanie") {
        if (listaJezykowUzytkownika.length === 0) {
            return (<>
                <p style={{fontFamily: "Comic Sans MS"}}>
                    Lista jest pusta.
                </p>
            </>);
        }
        return (<div className="lista-jezykow">
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
            </div>
        )
    }

    if (typ === "edycja") {
        // lista języków {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia}
        return (<div className={"lista-jezykow"}>
            <ul>
                {/*lista języków nad selectem z dodaniem nowego*/}
                {listaJezykowUzytkownika
                    .sort((a, b) => b.wartosc - a.wartosc)
                    .map((element, index) => {
                        if (index >= aktualnaStrona * liczbaJezykowNaStronie && index < (aktualnaStrona + 1) * liczbaJezykowNaStronie) {
                            return (<>
                                <JezykNaLiscieKomponent jezykDoKomponentu={element} key={`${element.idJezyka}-${element.idStopnia}`} idZListy={index}
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
                <button onClick={przyKliknieciuDodaj}>Dodaj</button>
            </ul>
            {przyciski}
        </div>)
    }

    // jeżeli tu doszliśmy, to nie pasuje do żadnego typu
    return(
        <>
            Coś poszło nie tak, typ tabelki {typ} nie istnieje.
        </>
    )
}