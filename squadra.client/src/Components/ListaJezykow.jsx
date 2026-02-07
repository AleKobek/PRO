import {useEffect, useMemo, useState} from "react";
import JezykNaLiscieKomponent from "./JezykNaLiscieKomponent";

export default function ListaJezykow({
                                         typ,
                                         // lista języków użytkownika to {idJezyka, nazwaJezyka, idStopnia, nazwaStopnia, wartoscStopnia}
                                         listaJezykowUzytkownika = [],
                                         ustawListeJezykowUzytkownika = () => {}
                                     }){



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


    useEffect(() => {

        const ac = new AbortController();
        let alive = true;

        // taka pomocnicza funkcja dla abort controller
        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                if (!res.ok) return null;
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                return null;
            }
        };
        
        const pobierzJezykiIStopnie = async () => {
            const jezyki = await fetchJsonAbort("http://localhost:5014/api/Jezyk");
            if (!alive || !jezyki) return;
            ustawListeJezykowZBazy(jezyki);
            
            const stopnie = await fetchJsonAbort("http://localhost:5014/api/StopienBieglosciJezyka");
            if (!alive || !stopnie) return;
            ustawListeStopniZBazy(stopnie);
        };
        
        pobierzJezykiIStopnie();

        // funkcja czyszcząca
        return () => {
            alive = false;
            ac.abort(); // przerywamy
        };
    }, []);

    // gdy dane są pobrane, ustawiamy domyślne wybory i aktualizujemy dostępne języki
    useEffect(() => {
        if (listaStopniZBazy.length > 0 && !wybranyStopienDoDodania) {
            ustawWybranyStopienDoDodania(listaStopniZBazy[0]);
        }
    }, [listaStopniZBazy, wybranyStopienDoDodania]);


    useEffect(() => {
        if (listaJezykowDostepnychDoDodania.length > 0 && !wybranyJezykDoDodania) {
            ustawWybranyJezykDoDodania(listaJezykowDostepnychDoDodania[0]);
        }
    }, [listaJezykowDostepnychDoDodania, wybranyJezykDoDodania]);




    // adres backendu to "http://localhost:5014"


    const przyKliknieciuUsun = (e) => {
        const index = Number(e?.target?.id);
        if (Number.isNaN(index)) return;
        // używamy filter zwracając nową tablicę:
        ustawListeJezykowUzytkownika((prev) => prev.filter((_, i) => i !== index));
    }

    const przyKliknieciuDodaj = (e) => {
        e.preventDefault();

        // język który dodajemy. jeżeli jest nic nie wybrane, to bierzemy pierwszy, bo tak jest w UI
        const jezyk = wybranyJezykDoDodania?.id
            ? wybranyJezykDoDodania
            : listaJezykowDostepnychDoDodania[0];

        // to samo co wyżej
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
    };

    // przyciski do paginacji stron listy
    let przyciski = <>
        {/* strona 1 z 5 itp */}
        <p className="mt-3">Strona {aktualnaStrona + 1} z {liczbaStron}</p><br/>

        {/* przycisk poprzedniej strony */}
        <button className={aktualnaStrona === 0 ? "zablokowany-przycisk" : "bg-blue-900 hover:bg-blue-600 text-white"} disabled={aktualnaStrona === 0}
                onClick={ () => { ustawAktualnaStrone(aktualnaStrona - 1) } }
        >Poprzednia strona</button>

        {/* przycisk następnej strony*/}
        <button className={liczbaStron === 0 || aktualnaStrona === liczbaStron - 1 ? "zablokowany-przycisk" : "bg-blue-900 hover:bg-blue-600 text-white"} disabled= {liczbaStron === 0 || aktualnaStrona === liczbaStron - 1}
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
                <select className="border border-gray-600 pl-2 mr-1 rounded rounded-l" onChange={(e) => {
                    let id = e.target.value;
                    let tempJezyk = listaJezykowZBazy.find(jezyk => jezyk.id == id);
                    ustawWybranyJezykDoDodania(tempJezyk);
                }}>
                    {listaJezykowDostepnychDoDodania.map((element) => (
                        // na każdy język {id, nazwa}
                        <option key={element.id} value={element.id}>{element.nazwa}</option>
                    ))}
                </select>
                {/* select stopnia */}
                <select className="border border-gray-600 pl-2 mr-1 rounded rounded-l" onChange={(e) => {
                    let id = e.target.value;
                    let tempStopien = listaStopniZBazy.find(stopien => stopien.id == id);
                    ustawWybranyStopienDoDodania(tempStopien);
                }}>
                    {listaStopniZBazy.map((element) => (
                        // elementem jest stopnień {id, nazwa}
                        <option key={element.id} value={element.id}>{element.nazwa}</option>
                    )).sort(
                        // sortujemy elementy listy względem pola wartosc
                        (stopien1, stopien2) => {
                            return stopien1.wartosc - stopien2.wartosc
                        }
                    )}
                </select>
                {/* przycisk obok selectów */}
                <button className={listaJezykowDostepnychDoDodania.length === 0 ? "zablokowany-przycisk" :"p-2 bg-green-900 text-white rounded-md px-3 py-1 my-4 hover:bg-green-600"} onClick={przyKliknieciuDodaj} disabled={listaJezykowDostepnychDoDodania.length === 0}>Dodaj</button>
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