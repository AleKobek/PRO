import React, {useEffect, useMemo, useState} from "react";
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {OkienkoTlumaczaceZintegrowanie} from "./OkienkoTlumaczaceZintegrowanie";
import {Bounce, toast} from "react-toastify";
import {API_BASE_URL} from "../config/api";

export default function WyszukajDruzyne() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();

    const [pokazOkienkoTlumaczenia, ustawPokazOkienkoTlumaczenia] = useState(false);
    const ref = React.useRef(null);

    const [wszystkieGryzPlatformami, ustawWszystkieGryzPlatformami] = useState([]);
    const [gryUzytkownikaZPlatformami, ustawGryUzytkownikaZPlatformami] = useState([]);
    const [nastrojeRozgrywki, ustawNastrojeRozgrywki] = useState([]);
    const [jezykiIStopnie, ustawJezykiIStopnie] = useState([]);
    const [role, ustawRole] = useState([]);
    
    const [nazwa, ustawNazwe] = useState("");
    const [idWybranejGry, ustawIdWybranejGry] = useState(0);
    const [idWybranejPlatformy, ustawIdWybranejPlatformy] = useState(null);
    const [idWybranegoNastroju, ustawIdWybranegoNastroju] = useState(null);
    const [czyZintegrowano, ustawCzyZintegrowano] = useState(false);
    const [czyTylkoZintegrowane, ustawCzyTylkoZintegrowane] = useState(false);
    const [wybraneRole, ustawWybraneRole] = useState([]);
    const [idWymaganegoJezyka, ustawIdWymaganegoJezyka] = useState(null);
    const [idMinimalnegoStopniaJezyka, ustawIdMinimalnegoStopniaJezyka] = useState(null);

    const aktualnaListaRol = useMemo(() => {
        return role.filter((rola) => rola.idGry === idWybranejGry) ?? [];
    }, [role, idWybranejGry]);

    const aktualnaListaStopni = useMemo(() =>{
        if(idWymaganegoJezyka === null) return [];
        return jezykiIStopnie.find((x) => x.jezyk.id === idWymaganegoJezyka)?.stopnie ?? [];
    }, [idWymaganegoJezyka, jezykiIStopnie]);
    
    const aktualnaListaPlatform = useMemo(() =>{
        const idGry = parseInt(idWybranejGry);
        if(isNaN(idGry) || idGry < 1) return [];

        if(czyZintegrowano) {
            return gryUzytkownikaZPlatformami.find((x) => x.id === idGry)?.platformy ?? []
        }
        return wszystkieGryzPlatformami.find((x) => x.id === idGry)?.platformy ?? [];
    }, [czyZintegrowano, gryUzytkownikaZPlatformami, idWybranejGry, wszystkieGryzPlatformami]);
    
    const czyZablokowane  = useMemo(() =>{
        return (
            idWybranejGry  < 1 || 
            (wybraneRole.length === 0 && aktualnaListaRol.length > 0) // są role, ale nie wybrał żadnej
        )
    }, [aktualnaListaRol, idWybranejGry, wybraneRole.length]);

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    /*
     Dane przychodzą w formie:
     {
          "wszystkieGryzPlatformami": [
            {
              "id": 1,
              "tytul": "Overwatch",
              "platformy": [
                {
                  "id": 1,
                  "nazwa": "PC"
                }, ...
              ]
            },...
          ],
          "gryUzytkownikaZPlatformami": [
            {
              "id": 1,
              "tytul": "Overwatch",
              "platformy": [
                {
                  "id": 1,
                  "nazwa": "PC"
                },...
              ]
            },..
          ],
          "nastrojeRozgrywki": [
            {
              "id": 1,
              "nazwa": "Zwykły"
            },
            {
              "id": 2,
              "nazwa": "Casual"
            },
            {
              "id": 3,
              "nazwa": "Rywalizacja"
            }
          ],
          "jezykiOrazStopnie": [
            {
              "jezyk": {
                "id": 2,
                "nazwa": "angielski"
              },
              "stopnie": [
                {
                  "id": 1,
                  "nazwa": "Podstawowy",
                  "wartosc": 1
                },
                {
                  "id": 2,
                  "nazwa": "Średnio-zaawansowany",
                  "wartosc": 2
                }
              ]
            }, ...
          ],
          "role": [
            {
              "id": 1,
              "nazwa": "tank",
              "idGry": 1
            },...
          ]
        }
    */

    useEffect(() => {
        if (!uzytkownik) return;

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
                toast.error('Wystąpił błąd podczas pobierania danych gier', {
                    position: "top-center",
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: false,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                    theme: "light",
                    transition: Bounce,
                });
                return null;
            }
        };

        const podajDane = async () => {
            const dane = await fetchJsonAbort(`${API_BASE_URL}/Druzyny/formularz/wyszukiwanie`);
            if(!alive || !dane) return;
            ustawWszystkieGryzPlatformami(dane.wszystkieGryzPlatformami);
            ustawGryUzytkownikaZPlatformami(dane.gryUzytkownikaZPlatformami);
            ustawNastrojeRozgrywki(dane.nastrojeRozgrywki);
            ustawJezykiIStopnie(dane.jezykiOrazStopnie);
            ustawRole(dane.role);
            
            ustawIdWybranejGry(dane.wszystkieGryzPlatformami[0].id)
        };

        if(wszystkieGryzPlatformami.length === 0) podajDane();

        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            ac.abort();
            alive = false; // przerywamy fetch
        }
    }, [uzytkownik, wszystkieGryzPlatformami]);

    /*
     Dane mają zostać wysłane w formie:
        int IdGry,
        int IdPlatformy,
        int IdNastrojuRozgrywki,
        int IdJezyka,
        int IdStopnia,
        string PreferencjeZintegrowania,
        string Nazwa,
        ICollection<int> IdRol
    */
    const przyWysylaniu = async () => {
        if(!uzytkownik) return;
        if(czyZablokowane) return;
        if(idWybranejGry < 1) return;


        const daneDoWyslania = {
            idGry: idWybranejGry,
            idPlatformy: idWybranejPlatformy,
            idNastrojuRozgrywki: idWybranegoNastroju,
            idJezyka: idWymaganegoJezyka,
            idStopnia: idMinimalnegoStopniaJezyka,
            PreferencjeZintegrowania: czyZintegrowano
                ? czyTylkoZintegrowane
                    ? "zintegrowane"
                    : "wszystkie"
                : "niezintegrowane",
            nazwa: nazwa,
            idRol: wybraneRole
        }


        // pakujemy i wysyłamy
        const opcje = {
            method: 'POST', // nie GET, bo to nie jest klasyczne wyszukiwanie zasobu po url
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(daneDoWyslania),
            credentials: "include",
        }

        const res = await fetch(`${API_BASE_URL}/Druzyny/wyszukaj`, opcje);


        const body = await res.json().catch(() => null);

        if (!res.ok) {
            toast.error(`Wystąpił błąd podczas wyszukiwania drużyny: ${body?.message ?? res.statusText}`, {
                position: "top-center",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: false,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
                theme: "light",
                transition: Bounce,
            });
            return;
        }

        navigate('/wyszukaneDruzyny', { state: { dane: body } });
    }

    if(ladowanie || !uzytkownik || !wszystkieGryzPlatformami) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna" className="flex flex-col items-center justify-center gap-5">
            <h1>Wyszukaj drużynę</h1>
            <button className={"przycisk-nawigacji"} onClick={() => {navigate('/twojeDruzyny')}}>Powrót do twoich drużyn</button>
            <br/><br/>
            <form className="flex flex-col items-center justify-center gap-5">
                {/* czy zintegrowano */}
                <div>
                    <div className="flex items-center justify-center">
                        <input
                            type="checkbox"
                            checked={czyZintegrowano}
                            onChange={() => {
                                // gdy zmieniamy z niezintegrowanego na zintegrowane
                                if(!czyZintegrowano){
                                    ustawIdWybranejGry(gryUzytkownikaZPlatformami[0] ? gryUzytkownikaZPlatformami[0].id : null);
                                }
                                ustawCzyZintegrowano(!czyZintegrowano)
                            }}
                            disabled={gryUzytkownikaZPlatformami.length === 0}
                            className="mr-2"
                        />
                        Użyj zintegrowanych danych
                        <img
                            src="/img/znak-zapytania.svg"
                            alt="znak zapytania"
                            className="h-[1em] w-auto align-middle ml-2 cursor-pointer"
                            onClick={() => ustawPokazOkienkoTlumaczenia(true)}
                        />
                    </div>
                    {czyZintegrowano && <div>
                        <input
                            type="checkbox"
                            checked={czyTylkoZintegrowane}
                            onChange={() => {
                                ustawCzyTylkoZintegrowane(!czyTylkoZintegrowane)
                            }}
                            disabled={gryUzytkownikaZPlatformami.length === 0 || !czyZintegrowano}
                            className="mr-2"
                        />
                        Wyszukaj TYLKO drużyny ze zintegrowanymi danymi
                    </div>}
                </div>
                {/* wybór gry i platformy */}
                <label>Tytuł gry i platforma
                <div className="flex items-center justify-center gap-5">
                    <select
                        onChange={(e) => ustawIdWybranejGry(parseInt(e.target.value))}
                        value={idWybranejGry}
                        className="border-2 border-gray-300 rounded-md p-2"

                    >
                        {czyZintegrowano
                            ? gryUzytkownikaZPlatformami.map((gra) => <option value={gra.id} key={gra.id}>{gra.tytul}</option>)
                            : wszystkieGryzPlatformami.map((gra) => <option value={gra.id} key={gra.id}>{gra.tytul}</option>)
                        }
                    </select>
                    <select
                        onChange={(e) => {
                            if(e.target.value === "") ustawIdWybranejPlatformy(null);
                            ustawIdWybranejPlatformy(parseInt(e.target.value))
                        }}
                        value={idWybranejPlatformy}
                        disabled={idWybranejGry === null}
                        className="border-2 border-gray-300 rounded-md p-2 w-full"

                    >
                        <option value = "" key = {-1}>Brak</option>
                        {aktualnaListaPlatform.map((platforma) => <option value={platforma.id} key={platforma.id}>{platforma.nazwa}</option>)}
                    </select>
                </div></label>
                <div className="flex-col items-center justify-center gap-2 w-[700px]">
                    <label>
                        Nazwa (lub jej część):
                        <input
                            type="text"
                            value={nazwa ?? 0}
                            onChange={(e) => ustawNazwe(e.target.value)}
                            className="border-2 border-gray-300 rounded-md p-2 w-full"
                            maxLength={40}
                        />
                    </label>
                </div>
                <div className="flex items-center justify-center gap-5">
                    <label>
                        Nastrój rozgrywki: <br/>
                        <select
                            className="border-2 border-gray-300 rounded-md p-2"
                            onChange={(e) => {
                                if(e.target.value === "") ustawIdWybranegoNastroju(null);
                                else ustawIdWybranegoNastroju(parseInt(e.target.value))
                            }}
                            value={idWybranegoNastroju}
                        >
                            <option value = "" key = {-1}>Dowolny</option>
                            {
                                nastrojeRozgrywki.map((nastroj) =>
                                    <option value={nastroj.id} key={nastroj.id}>{nastroj.nazwa}</option>
                                )
                            }
                        </select>
                    </label>
                    <label>
                        Wymagany język i stopień biegłości (jeśli jest ustalony):
                        <div>
                            {
                                <div className="flex gap-2">
                                    <select
                                        className="border-2 border-gray-300 rounded-md p-2"
                                        value={idWymaganegoJezyka ?? ""}
                                        onChange={(e) => {
                                            if(e.target.value === "") ustawIdWymaganegoJezyka(null);
                                            else ustawIdWymaganegoJezyka(parseInt(e.target.value))
                                        }}
                                        disabled={jezykiIStopnie.length === 0}
                                    >
                                        <option value = "" key = {-1}>Dowolny posiadany</option>
                                        {
                                            jezykiIStopnie.map((jezykIStopien) =>
                                                <option value={jezykIStopien.jezyk.id} key={jezykIStopien.jezyk.id}>{jezykIStopien.jezyk.nazwa}</option>
                                            )
                                        }
                                    </select>
                                    <select
                                        className="border-2 border-gray-300 rounded-md p-2"
                                        value={idMinimalnegoStopniaJezyka ?? ""}
                                        onChange={(e) => {
                                            if(e.target.value === "") ustawIdMinimalnegoStopniaJezyka(null);
                                            else ustawIdMinimalnegoStopniaJezyka(parseInt(e.target.value))
                                        }}
                                        disabled={idWymaganegoJezyka === null || aktualnaListaStopni.length === 0}
                                    >
                                        {aktualnaListaStopni.length === 0 && <option value="" key={-1}>Dowolny posiadany</option>}
                                        {
                                            aktualnaListaStopni.map((stopien) =>
                                                <option value={stopien.id} key={stopien.id}>{stopien.nazwa}</option>)
                                        }
                                    </select>
                                </div>
                            }
                        </div>
                    </label>
                </div>
                {/* tabelka ról */}
                {aktualnaListaRol.length > 0 && <div className="flex flex-col items-center justify-center gap-5 mt-5">
                    <h3>Akceptowane role przyporządkowane do miejsc</h3>
                    <div className="flex flex-col items-center justify-center gap-2 w-1/2">
                        {
                            aktualnaListaRol.map((rola) =>
                                <div className="flex items-center justify-center gap-2" key={rola.id}>
                                    <input
                                        type="checkbox"
                                        checked={wybraneRole.includes(rola.id)}
                                        onChange={() => {
                                            if (wybraneRole.includes(rola.id)) ustawWybraneRole(wybraneRole.filter((x) => x !== rola.id))
                                            else ustawWybraneRole([...wybraneRole, rola.id])
                                        }}
                                    />
                                    <span>{rola.nazwa}</span>
                                </div>
                            )
                        }
                    </div>
                </div>}
            </form>
            <button
                className={czyZablokowane
                    ? "zablokowany-przycisk"
                    : "p-2 bg-green-900 text-white rounded-md px-3 py-1 mt-10 hover:bg-green-600"}
                disabled={czyZablokowane}
                onClick={przyWysylaniu}>
                Wyszukaj
            </button>
            {pokazOkienkoTlumaczenia && OkienkoTlumaczaceZintegrowanie(ref, ustawPokazOkienkoTlumaczenia)}
        </div>
    </>);
}