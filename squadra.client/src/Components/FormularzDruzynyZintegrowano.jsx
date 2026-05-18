import React, {useEffect, useMemo, useState} from "react";
import {Bounce, toast} from "react-toastify";
import {API_BASE_URL} from "../config/api";
import {useNavigate} from "react-router-dom";

export default function FormularzDruzynyZintegrowano({
                                                            uzytkownik,
                                                            idGryDruzyny,
                                                            ustawIdGryDruzyny,
                                                            ustawBladOgolny
                                                        }) {
    /*
    dane przychodzą w formie: (tutaj dla ow)
        {
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
      "platformy": [
        {
          "id": 1,
          "nazwa": "PC",
          "logo": "tutaj mnóstwo znaków"
        },
        {
          "id": 2,
          "nazwa": "Play Station",
          "logo": "tutaj mnóstwo znaków"
        },
        {
          "id": 3,
          "nazwa": "Xbox",
          "logo": "tutaj mnóstwo znaków"
        },
        {
          "id": 4,
          "nazwa": "Nintendo Switch",
          "logo": "tutaj mnóstwo znaków"
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
            },
            {
              "jezyk": {
                "id": 1,
                "nazwa": "polski"
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
                },
                {
                  "id": 3,
                  "nazwa": "Zaawansowany",
                  "wartosc": 3
                }
              ]
            }
          ],
      "role": [
        {
          "id": 1,
          "nazwa": "tank",
          "idGry": 1
        },
        {
          "id": 2,
          "nazwa": "DPS",
          "idGry": 1
        },
        {
          "id": 3,
          "nazwa": "support",
          "idGry": 1
        }
      ],
      "statystyki": {
            "statystykiNieBedaceRangami": [
              {
                "id": 1,
                "nazwa": "Ogólne: Czas rozgrywki"
              },
              {
                "id": 2,
                "nazwa": "Tryb rankingowy: Czas rozgrywki"
              },
              {
                "id": 3,
                "nazwa": "Tryb nierankingowy: Czas rozgrywki"
              },
              itd.
            ],
            "rangi": [
              {
                "id": 12,
                "nazwa": "Tryb rankingowy: Ranga(tank)",
                "rangi": [
                      {
                        "nazwaRangi": "Bronze V",
                        "wartoscLiczbowa": 1
                      },
                      {
                        "nazwaRangi": "Bronze IV",
                        "wartoscLiczbowa": 2
                      },
                     itd
                  ]
              },
          }
    }
    */

    const navigate = useNavigate();
    const [ladowanie, ustawLadowanie] = useState(true);

    // pobrane dane z bazy
    const [nastrojeRozgrywki, ustawNastrojeRozgrywki] = useState([]);
    const [platformy, ustawPlatformy] = useState([]);
    const [jezykiIStopnie, ustawJezykiIStopnie] = useState([]);
    const [role, ustawRole] = useState([]);
    const [statystykiZBazy, ustawStatystykiZBazy] = useState([]);
    const [rangiZBazy, ustawRangiZBazy] = useState([]);

    // dane z formularza
    const [nazwa, ustawNazwa] = useState("");
    const [opis, ustawOpis] = useState("");
    const [idWybranegoNastroju, ustawIdWybranegoNastroju] = useState(1);
    const [idWymaganegoJezyka, ustawIdWymaganegoJezyka] = useState(null);
    const [idMinimalnegoStopniaJezyka, ustawIdMinimalnegoStopniaJezyka] = useState(null);
    const [czyPubliczna, ustawCzyPubliczna] = useState(true);
    const [idWybranejPlatformy, ustawIdWybranejPlatformy] = useState(null);
    const [czy18Plus, ustawCzy18Plus] = useState(false);

    // {int? idRoli, string? nazwaRoli, string? nazwaStatystyki, int? idStatystyki, string? wartoscStatystyki, int? porownywalnaWartoscLiczbowa}
    const [miejscaWDruzynie, ustawMiejscaWDruzynie] = useState([]); // tyle elementów ile miejsc
    //{int? idStatystyki, string? wartosc, int? porownywalnaWartoscLiczbowa}
    const [wymaganiaDruzynowe, ustawWymaganiaDruzynowe] = useState([]);
    const [idRoliKapitana, ustawIdRoliKapitana] = useState(null);

    // dodawanie wymagań drużyny
    const [idStatystykiDoDodania, ustawIdStatystykiDoDodania] = useState(null);
    const [nazwaStatystykiDoDodania, ustawNazweStatystykiDoDodania] = useState(null);
    const [wartoscStatystykiDoDodania, ustawWartoscStatystykiDoDodania] = useState(null);
    const [wartoscLiczbowaStatystykiDoDodania, ustawWartoscLiczbowaStatystykiDoDodania] = useState(null);
    const [idStatystykiRangiDoDodania, ustawIdStatystykiRangiDoDodania] = useState(null);
    const [nazwaStatystykiRangiDoDodania, ustawNazweStatystykiRangiDoDodania] = useState(null);
    const [wartoscStatystykiRangiDoDodania, ustawWartoscStatystykiRangiDoDodania] = useState(null);
    const [wartoscLiczbowaStatystykiRangiDoDodania, ustawWartoscLiczbowaStatystykiRangiDoDodania] = useState(null);

    // dodawanie miejsca
    const [idRoliDoDodania, ustawIdRoliDoDodania] = useState(null);
    const [idStatystykiMiejscaDoDodania, ustawIdStatystykiMiejscaDoDodania] = useState(null);
    const [nazwaStatystykiMiejscaDoDodania, ustawNazweStatystykiMiejscaDoDodania] = useState(null);
    const [wartoscStatystykiMiejscaDoDodania, ustawWartoscStatystykiMiejscaDoDodania] = useState(null);
    const [wartoscLiczbowaStatystykiMiejscaDoDodania, ustawWartoscLiczbowaStatystykiMiejscaDoDodania] = useState(null);
    const [idStatystykiRangiMiejscaDoDodania, ustawIdStatystykiRangiMiejscaDoDodania] = useState(null);
    const [nazwaStatystykiRangiMiejscaDoDodania, ustawNazweStatystykiRangiMiejscaDoDodania] = useState(null);
    const [wartoscRangiMiejscaDoDodania, ustawWartoscRangiMiejscaDoDodania] = useState(null);
    const [wartoscLiczbowaRangiMiejscaDoDodania, ustawWartoscLiczbowaRangiMiejscaDoDodania] = useState(null);


    const [bladNazwy, ustawBladNazwy] = useState("");
    const [bladOpisu, ustawBladOpisu] = useState("");

    useEffect(() => {
        const ac = new AbortController();
        let alive = true;
        if(!uzytkownik) return;
        if(!idGryDruzyny) return;

        // taka pomocnicza funkcja dla abort controller
        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                if (!res.ok) return null;
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                toast.error('Wystąpił błąd podczas pobierania danych do formularza', {
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

        const podajNieZintegrowaneDane = async () => {

            const dane = await fetchJsonAbort(`${API_BASE_URL}/Druzyna/formularz/ze-statystykami/${idGryDruzyny}`);
            if (!alive || !dane) return;
            console.log(dane);

            ustawNastrojeRozgrywki(dane.nastrojeRozgrywki);
            ustawIdWybranegoNastroju(dane.nastrojeRozgrywki[0].id)
            ustawPlatformy(dane.platformy);
            ustawJezykiIStopnie(dane.jezykiOrazStopnie);
            ustawRole(dane.role ?? []);
            ustawIdRoliDoDodania(dane.role[0]?.id ?? null);
            ustawStatystykiZBazy(dane.statystyki.statystykiNieBedaceRangami);
            console.log(dane.statystyki.statystykiNieBedaceRangami)
            ustawRangiZBazy(dane.statystyki.rangi)
        };

        podajNieZintegrowaneDane();
        ustawLadowanie(false);

        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            ac.abort();
            alive = false; // przerywamy fetch
        }
    }, [idGryDruzyny, uzytkownik]);



    const przyWysylaniu = async () => {
        if(!uzytkownik) return;
        if(!idGryDruzyny) return;
        if(czyZablokowane) return; // na wszeeeelki wypadek

        ustawBladNazwy("");
        ustawBladOpisu("");
        ustawBladOgolny("");

        /*
        Dane mają mieć format:
            string Nazwa,
            int IdGry,
            bool CzyPubliczna,
            string? Opis,
            int? IdNastrojuRozgrywki,
            int? IdWymaganegoJezyka,
            int? IdWymaganegoStopniaBieglosciJezyka,
            bool Czy18Plus,
            int? IdPlatformy,
            int? IdRoliKapitana,
            ICollection<WartoscStatystykiDTO>? WymaganeStatystyki -> {int IdStatystyki, string? Wartosc, double PorownywalnaWartoscLiczbowa}
            ICollection<CreateMiejsceWDruzynieReq> MiejscaWDruzynie

        CreateMiejsceWDruzynieReq wygląda tak:
            int? IdRoli,
            WartoscStatystykiDTO? WymaganaStatystyka -> {int IdStatystyki, string? Wartosc, double PorownywalnaWartoscLiczbowa}
        */

        const dane = {
            nazwa: nazwa,
            idGry: idGryDruzyny,
            czyPubliczna: czyPubliczna,
            opis: opis,
            idNastrojuRozgrywki: idWybranegoNastroju,
            idWymaganegoJezyka: idWymaganegoJezyka,
            idWymaganegoStopniaBieglosciJezyka: idMinimalnegoStopniaJezyka,
            czy18Plus: czy18Plus,
            idPlatformy: idWybranejPlatformy,
            idRoliKapitana: idRoliKapitana,
            WymaganeStatystyki: wymaganiaDruzynowe.map((wymaganie) => {
                //TODO ###################################################################################################################
            }),
            MiejscaWDruzynie: miejscaWDruzynie.map((miejsce) => {
                return {
                    idRoli: miejsce.idRoli,
                    wymaganaStatystyka: null,
                }
            })
        }

        // pakujemy i wysyłamy
        const opcje = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(dane),
            credentials: "include"
        }

        const res = await fetch(`${API_BASE_URL}/Druzyna`, opcje);

        // Odczyt body różni się zależnie od typu odpowiedzi
        // jeżeli to 404, to zwraca tylko tekst (nie application/json), więc res.json rzuci wyjątek. musimy to uwzlgędnić
        const ct = res.headers.get("content-type") || "";
        const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
            ? await res.json().catch(() => null)
            : await res.text().catch(() => "");

        if (!res.ok) {
            if(res.status === 400){
                let bledy = body.errors;
                ustawBladNazwy(bledy.Nazwa ? bledy.Nazwa[0] : "");
                ustawBladOpisu(bledy.Opis ? bledy.Opis[0] : "");
                ustawBladOgolny(body.message);
            }
            toast.error('Wystąpił błąd podczas tworzenia drużyny', {
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

        // jak tutaj dojdziemy, wszystko jest git
        navigate("/twojeDruzyny", {
            state: { pomyslnieStworzonoDruzyne: true }
        });
    }

    const czyZablokowane = useMemo(() =>{
        return (!uzytkownik || !idGryDruzyny
            || miejscaWDruzynie.length === 0 || miejscaWDruzynie.length > 8
            || nazwa.trim().length === 0)
    }, [idGryDruzyny, miejscaWDruzynie, nazwa, uzytkownik]);

    const aktualnaListaStopni = useMemo(() =>{
        if(idWymaganegoJezyka === null) return [];
        return jezykiIStopnie.find((x) => x.jezyk.id === idWymaganegoJezyka)?.stopnie ?? [];
    }, [idWymaganegoJezyka, jezykiIStopnie]);

    const aktualnaListaStatystyk = useMemo(() =>{
        const zajete = new Set(wymaganiaDruzynowe.map((w => w.idStatystyki))); // id wszystkich statystyk, które są w wymaganiach
       return (statystykiZBazy ?? []).filter(s => !zajete.has(s.idStatystyki))  // zostawiamy tylko te, których id nie jest zajęte
    }, [statystykiZBazy, wymaganiaDruzynowe]);

    const aktualnaListaRang = useMemo(() =>{
        const zajete = new Set(wymaganiaDruzynowe.map((w => w.idStatystyki))); // id wszystkich statystyk, które są w wymaganiach
        return (rangiZBazy ?? []).filter(s => !zajete.has(s.id))  // zostawiamy tylko te, których id nie jest zajęte
    }, [rangiZBazy, wymaganiaDruzynowe]);

    const aktualnaListaWartosciRang = useMemo(() =>{
        let aktualnaRanga = (rangiZBazy ?? []).find(x => x.id === idStatystykiRangiDoDodania);
        if (aktualnaRanga === undefined) return [];
        if(wartoscStatystykiRangiDoDodania === null){
            ustawWartoscStatystykiRangiDoDodania(aktualnaRanga.rangi[0]?.nazwaRangi)
            ustawWartoscLiczbowaStatystykiRangiDoDodania(aktualnaRanga.rangi[0]?.wartoscLiczbowa);
        }
        if(aktualnaRanga.rangi.length === 0){
            ustawWartoscStatystykiRangiDoDodania(null);
            ustawWartoscLiczbowaStatystykiRangiDoDodania(null);
        }
        return aktualnaRanga.rangi;
    },[rangiZBazy, wartoscStatystykiRangiDoDodania, idStatystykiRangiDoDodania]);

    // to samo co wyżej?
    const aktualnaListaWartosciRangMiejsca = useMemo(() =>{
        let aktualnaRanga = (rangiZBazy ?? []).find(x => x.id === idStatystykiRangiMiejscaDoDodania);
        if (aktualnaRanga === undefined) return [];
        return aktualnaRanga.rangi;
    },[rangiZBazy, idStatystykiRangiMiejscaDoDodania]);

    // na samym początku, gdy się załaduje strona, wybieramy pierwsze rangi z listy
    useEffect(() => {
        if(rangiZBazy === null) return;
        const pierwszaRanga = rangiZBazy[0];
        if(pierwszaRanga === undefined) return;

        ustawIdStatystykiRangiDoDodania(pierwszaRanga.id);
        ustawNazweStatystykiRangiDoDodania(pierwszaRanga.nazwa)
        ustawWartoscStatystykiDoDodania(pierwszaRanga.rangi[0]?.nazwaRangi)
        ustawWartoscLiczbowaStatystykiDoDodania(pierwszaRanga.rangi[0]?.wartoscLiczbowa)

        ustawIdStatystykiRangiMiejscaDoDodania(pierwszaRanga.id);
        ustawNazweStatystykiRangiMiejscaDoDodania(pierwszaRanga.nazwa)
        ustawWartoscStatystykiMiejscaDoDodania(pierwszaRanga.rangi[0]?.nazwaRangi)
        ustawWartoscLiczbowaStatystykiMiejscaDoDodania(pierwszaRanga.rangi[0]?.wartoscLiczbowa)
    }, [rangiZBazy]);

    if(!uzytkownik || ladowanie) return <div className="flex flex-col justify-center items-center h-screen">
        <div className="animate-spin rounded-full h-32 w-32 border-t-2 border-b-2 border-gray-900">
        </div>
        <span className="text-4xl mt-10">Ładowanie</span>
    </div>




    return (<div className="flex flex-col items-center justify-center gap-5">
        <button className="przycisk-nawigacji" onClick={() => ustawIdGryDruzyny(0)}>Zmień grę</button>
        <form className="flex flex-col items-center justify-center gap-5 border-4 border-gray-500 rounded-lg p-5 shadow-lg">
            <div className="flex-col items-center justify-center gap-2 w-[700px]">
                <label>
                    Nazwa:
                    <input
                        type="text"
                        value={nazwa ?? 0}
                        onChange={(e) => ustawNazwa(e.target.value)}
                        className="border-2 border-gray-300 rounded-md p-2 w-full"
                        maxLength={40}
                    />
                </label>
                <span className="error-wiadomosc">{bladNazwy}</span>
            </div>
            <div className="flex items-center justify-center gap-5">
                <label>
                    Nastrój rozgrywki: <br/>
                    <select
                        className="border-2 border-gray-300 rounded-md p-2"
                        onChange={(e) => ustawIdWybranegoNastroju(parseInt(e.target.value))}
                        value={idWybranegoNastroju}
                    >
                        {
                            nastrojeRozgrywki.map((nastroj) =>
                                <option value={nastroj.id} key={nastroj.id}>{nastroj.nazwa}</option>
                            )
                        }
                    </select>
                </label>
                <label>
                    Wymagany język i stopień biegłości:
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
                                    <option value = "" key = {-1}>Brak</option>
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
                                    {aktualnaListaStopni.length === 0 && <option value="" key={-1}>Brak</option>}
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
            <div className="flex items-center justify-center gap-5">
                <label>
                    Platforma: <br/>
                    <select
                        className="border-2 border-gray-300 rounded-md p-2"
                        onChange={(e) => {
                            if(e.target.value === "") ustawIdWybranejPlatformy(null);
                            else ustawIdWybranejPlatformy(parseInt(e.target.value))
                        }}
                        disabled={platformy.length === 0}
                        value={idWybranejPlatformy ?? ""}
                    >
                        <option value = "" key = {-1}>Brak</option>
                        {
                            platformy.map((platforma) =>
                                <option value={platforma.id} key={platforma.id}>{platforma.nazwa}</option>
                            )
                        }
                    </select>
                </label>
                <div className="flex-col items-center justify-center gap-2">
                    <label>
                        Opis: <br/>
                        <textarea
                            value={opis}
                            onChange={(e) => ustawOpis(e.target.value)}
                            className="border-2 border-gray-300 rounded-md p-2 w-[500px]"
                            maxLength={300}
                        />
                    </label>
                    <span className="error-wiadomosc">{bladOpisu}</span>
                </div>
            </div>
            <div className="flex items-center justify-center gap-5">
                <label>
                    Czy publiczna:
                    <input
                        type="checkbox"
                        className="mx-2"
                        checked={czyPubliczna}
                        onChange={() => ustawCzyPubliczna(!czyPubliczna)}
                    />
                </label>
                <label>
                    Czy 18 plus:
                    <input
                        type="checkbox"
                        className="mx-2"
                        checked={czy18Plus}
                        onChange={() => ustawCzy18Plus(!czy18Plus)}
                    />
                </label>
            </div>
        </form>
        <h3>Konfiguracja minimalnych wymaganych statystyk</h3>
        {/* tabelka w wymaganiami */}
        <table className="overflow-x-auto overflow-y-auto h-full w-1/4 border-4 border-gray-600 rounded-lg shadow-lg">
            <thead className="bg-gray-200">
            <tr className="font-semibold border border-gray-600 text-3xl text-gray-800">
                <th className="border border-gray-600 text-center p-2">Nazwa</th>
                <th className="border border-gray-600 text-center p-2">Wartość</th>
                <th className="p-2">Zarządzanie</th>
            </tr>
            </thead>
            <tbody>
            {wymaganiaDruzynowe.map((wymaganie, index) =>{
                return (
                    <tr className="border border-gray-600 bg-amber-50" key = {wymaganie.idStatystyki ?? index}>
                        <th className="border border-gray-600 text-center">{wymaganie.nazwa ?? "brak"}</th>
                        <th className="border border-gray-600 text-center">{wymaganie.wartosc ?? "brak"}</th>
                        <th>
                            <button
                                className="bg-red-900 text-white rounded-md px-5 py-3.5 hover:bg-red-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                                onClick={() => {
                                    ustawWymaganiaDruzynowe(prev => prev.filter((_, i) => i !== index))
                                }}
                            >Usuń</button>
                        </th>
                    </tr>
                )
            })}
            </tbody>
        </table>
        {/* dodawanie statystyk */}
        <div className="flex flex-col items-center justify-center gap-5">
            <div>
                <label>
                    Wymaganie liczbowe: <br/>
                    <select
                        className="border-2 border-gray-300 rounded-md p-2"
                        value = {idStatystykiDoDodania ?? ""}
                        onChange={(e) => {
                            if(e.target.value === "") {
                                ustawIdStatystykiDoDodania(null);
                                ustawNazweStatystykiDoDodania(null);
                            }
                            else {
                                let id = parseInt(e.target.value);
                                let statystyka = aktualnaListaStatystyk.find(x => x.id === id);
                                ustawIdStatystykiDoDodania(id);
                                ustawNazweStatystykiDoDodania(statystyka.nazwa);
                            }
                        }}
                    >
                        <option value = "" key = {-1}>Brak</option>
                        {
                            aktualnaListaStatystyk.map((statystyka) =>
                                <option value={statystyka.id} key={statystyka.id}>{statystyka.nazwa}</option>)
                        }
                    </select>
                    <input
                        type="number"
                        className="border-2 border-gray-300 rounded-md p-2 ml-2"
                        value={wartoscStatystykiDoDodania}
                        onChange={(e) => ustawWartoscStatystykiDoDodania(e.target.value)}
                        disabled={!idStatystykiRangiDoDodania || idStatystykiRangiDoDodania < 0}
                    />
                </label>
                <button
                    className="bg-green-900 text-white rounded-md px-5 py-3.5 hover:bg-green-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                    onClick={() => {
                        ustawWymaganiaDruzynowe(prev => [...prev, {
                            idStatystyki: idStatystykiDoDodania,
                            nazwa: nazwaStatystykiDoDodania,
                            wartosc: wartoscStatystykiDoDodania,
                            porownywalnaWartoscLiczbowa: wartoscLiczbowaStatystykiDoDodania,
                        }])
                    }}
                    disabled={!idStatystykiDoDodania || idStatystykiDoDodania < 0 || !wartoscStatystykiDoDodania}
                >Dodaj</button>
            </div>
            <div>
                <label>
                    Wymaganie w postaci rangi: <br/>
                    <div className="flex gap-2">
                        <select
                            className="border-2 border-gray-300 rounded-md p-2"
                            value = {idStatystykiRangiDoDodania ?? ""}
                            onChange={(e) => {
                                if(e.target.value === "") ustawIdStatystykiRangiDoDodania(null);
                                else {
                                    let id = parseInt(e.target.value);
                                    let statystyka = aktualnaListaRang.find(x => x.id === id);
                                    ustawIdStatystykiRangiDoDodania(id)
                                    ustawNazweStatystykiRangiDoDodania(statystyka.nazwa)
                                }
                            }}
                        >
                            <option value = "" key = {-1}>Brak</option>
                        {
                            aktualnaListaRang.map((ranga) =>
                                <option value={ranga.id} key={ranga.id}>{ranga.nazwa}</option>)
                        }
                        </select>
                        <select
                            className="border-2 border-gray-300 rounded-md p-2"
                            value={wartoscLiczbowaStatystykiRangiDoDodania ?? ""}
                            onChange={(e) => {
                                // na wszelki wypadek
                                if(e.target.value === "") {
                                    ustawWartoscStatystykiRangiDoDodania(null);
                                    ustawWartoscLiczbowaStatystykiRangiDoDodania(null);
                                }
                                else {
                                    let wartoscLiczbowa = parseInt(e.target.value);
                                    let ranga = aktualnaListaWartosciRang.find(x => x.wartoscLiczbowa === wartoscLiczbowa);
                                    ustawWartoscStatystykiRangiDoDodania(ranga.nazwaRangi);
                                    ustawWartoscLiczbowaStatystykiRangiDoDodania(wartoscLiczbowa);
                                }
                            }}
                            disabled={idStatystykiRangiDoDodania === null || aktualnaListaWartosciRang.length === 0}
                        >
                            {aktualnaListaWartosciRang.length === 0 && <option value="" key={-1}>Brak</option>}
                            {aktualnaListaWartosciRang.map((wartoscRangi) =>
                            <option
                                key = {wartoscRangi.wartoscLiczbowa}
                                value = {wartoscRangi.wartoscLiczbowa}
                            >
                                {wartoscRangi.nazwaRangi}
                            </option>)}
                        </select>
                    </div>
                </label>
                <button
                    className="bg-green-900 text-white rounded-md px-5 py-3.5 hover:bg-green-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                    onClick={() => {
                        console.log("dodajemy rangę do wymagań drużynowych", {
                            idStatystykiRangiDoDodania,
                            nazwaStatystykiRangiDoDodania,
                            wartoscStatystykiRangiDoDodania,
                            wartoscLiczbowaStatystykiRangiDoDodania,
                        })
                        ustawWymaganiaDruzynowe(prev => [...prev, {
                            idStatystyki: idStatystykiRangiDoDodania,
                            nazwa: nazwaStatystykiRangiDoDodania,
                            wartosc: wartoscStatystykiRangiDoDodania,
                            porownywalnaWartoscLiczbowa: wartoscLiczbowaStatystykiRangiDoDodania,
                        }])
                        ustawIdStatystykiRangiDoDodania(null);
                        ustawNazweStatystykiRangiDoDodania(null);
                        ustawWartoscStatystykiRangiDoDodania(null);
                        ustawWartoscLiczbowaStatystykiRangiDoDodania(null);
                    }}
                    disabled={!idStatystykiRangiDoDodania || !wartoscStatystykiRangiDoDodania || !wartoscLiczbowaStatystykiRangiDoDodania || wartoscLiczbowaStatystykiRangiDoDodania < 0}
                >Dodaj</button>
            </div>
        </div>
        <h3>Konfiguracja miejsc w drużynie</h3>
        <table className="overflow-x-auto overflow-y-auto h-full w-1/4 border-4 border-gray-600 rounded-lg shadow-lg">
            <thead className="bg-gray-200">
            <tr className="font-semibold border border-gray-600 text-3xl text-gray-800">
                <th className="border border-gray-600 text-center">Rola</th>
                <th className="border border-gray-600 text-center">Wymaganie</th>
                <th className="border border-gray-600 text-center">Wartość</th>
                <th className="text-xl">Zarządzanie</th>
            </tr>
            </thead>
            <tbody>
            <tr className="border border-gray-700 bg-amber-50">
                <th className="border border-gray-600 text-center">
                    <select
                        onChange={(e) => {
                            if(e.target.value === "") ustawIdRoliKapitana(null);
                            else ustawIdRoliKapitana(parseInt(e.target.value))
                        }}
                        value={idRoliKapitana ?? ""}
                        className="border-2 border-gray-300 rounded-md p-2"
                        disabled={role.length === 0}
                    >
                        <option value = "" key = {-1}>Brak</option>
                        {

                            role.map((rola) =>
                                <option value={rola.id} key={rola.id}>{rola.nazwa}</option>
                            )
                        }
                    </select>
                </th>
                <th className="border border-gray-600 text-center">brak</th>
                <th className="border border-gray-600 text-center">brak</th>
                <th className="text-gray-700 p-2">Ty - kapitan (zablokowane)</th>
            </tr>
            {/* tutaj mapujemy po miejscach w drużynie */}
            {
                miejscaWDruzynie.map((miejsce, index) =>
                    <>
                        <tr className="border border-gray-600 bg-amber-50" key = {index}>
                            <th className="border border-gray-600 text-center">{miejsce.nazwaRoli.length === 0 ? "brak" : miejsce.nazwaRoli}</th>
                            <th className="border border-gray-600 text-center">{miejsce.nazwaStatystyki.length === 0 ? "brak" : miejsce.nazwaStatystyki}</th>
                            <th className="border border-gray-600 text-center">{miejsce.wartoscStatystyki.length === 0 ? "brak" : miejsce.wartoscStatystyki}</th>
                            <th>
                                <button
                                    className="bg-red-900 text-white rounded-md px-5 py-3.5 hover:bg-red-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                                    onClick={() => {
                                        ustawMiejscaWDruzynie(prev => prev.filter((_, i) => i !== index))
                                    }}
                                >Usuń</button>
                            </th>
                        </tr>
                    </>
                )
            }
            </tbody>
        </table>
        {/* dodawanie miejsca */}
        <div className="flex items-center justify-center gap-5">
            <select
                value={idRoliDoDodania ?? ""}
                onChange={(e) => {
                    if(e.target.value === "") ustawIdRoliDoDodania(null);
                    else ustawIdRoliDoDodania(parseInt(e.target.value))
                }}
                disabled={role.length === 0}
                className="border-2 border-gray-300 rounded-md p-2"
            >
                <option value = "" key = {-1}>Brak</option>
                {
                    role.map((rola) =>
                        <option value={rola.id} key={rola.id}>{rola.nazwa}</option>
                    )
                }
            </select>
            <button
                className={ miejscaWDruzynie.length > 8
                    ? "zablokowany-przycisk"
                    : "p-2 bg-green-900 text-white rounded-md px-3 py-1 my-4 hover:bg-green-600"}
                onClick={() => {
                    let miejsceDoDodania;
                    if(idRoliDoDodania === null || idRoliDoDodania === "") miejsceDoDodania = {idRoli: null, nazwaRoli: ""};
                    else miejsceDoDodania = {idRoli: idRoliDoDodania, nazwaRoli: role.find(r => r.id === idRoliDoDodania).nazwa};
                    ustawMiejscaWDruzynie(prev => [...prev, miejsceDoDodania]);
                }}
                disabled={miejscaWDruzynie.length > 8}
            >Dodaj</button>
        </div>
        <button
            className={ czyZablokowane
                ? "zablokowany-przycisk"
                : "p-2 bg-green-900 text-white rounded-md px-3 py-1 mt-10 hover:bg-green-600"}
            disabled={czyZablokowane}
            onClick={przyWysylaniu}>
            Stwórz
        </button>
    </div>)
}
