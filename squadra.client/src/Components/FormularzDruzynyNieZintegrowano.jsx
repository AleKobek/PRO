import React, {useEffect, useMemo, useState} from "react";
import {Bounce, toast} from "react-toastify";
import {API_BASE_URL} from "../config/api";
import {useNavigate} from "react-router-dom";

export default function FormularzDruzynyNieZintegrowano({
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
      "jezyki": [
        {
          "id": 1,
          "nazwa": "polski"
        },
        {
          "id": 2,
          "nazwa": "angielski"
        },
        {
          "id": 3,
          "nazwa": "niemiecki"
        },
        {
          "id": 4,
          "nazwa": "francuski"
        },
        {
          "id": 5,
          "nazwa": "hiszpański"
        },
        {
          "id": 6,
          "nazwa": "japoński"
        },
        {
          "id": 7,
          "nazwa": "rosyjski"
        }
      ],
      "stopnieBieglosciJezyka": [
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
      ]
    }
    */

    const navigate = useNavigate();

    // pobrane dane z bazy
    const [nastrojeRozgrywki, ustawNastrojeRozgrywki] = useState([]);
    const [platformy, ustawPlatformy] = useState([]);
    const [wszystkieJezyki, ustawWszystkieJezyki] = useState([]);
    const [wszystkieStopnie, ustawWszystkieStopnie] = useState([]);
    const [role, ustawRole] = useState([]);

    // dane z formularza
    const [nazwa, ustawNazwa] = useState("");
    const [opis, ustawOpis] = useState("");
    const [idWybranegoNastroju, ustawIdWybranegoNastroju] = useState(1);
    const [idWymaganegoJezyka, ustawIdWymaganegoJezyka] = useState(null);
    const [idMinimalnegoStopniaJezyka, ustawIdMinimalnegoStopniaJezyka] = useState(null);
    const [czyPubliczna, ustawCzyPubliczna] = useState(true);
    const [idWybranejPlatformy, ustawIdWybranejPlatformy] = useState(null);

    // {idRoli, nazwaRoli}
    const [miejscaWDruzynie, ustawMiejscaWDruzynie] = useState([]); // tyle elementów ile miejsc
    const [idRoliKapitana, ustawIdRoliKapitana] = useState(null);
    const [idRoliDoDodania, ustawIdRoliDoDodania] = useState(null);

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

            const dane = await fetchJsonAbort(`${API_BASE_URL}/Druzyna/formularz/bez-statystyk/${idGryDruzyny}`);
            if (!alive || !dane) return;

            ustawNastrojeRozgrywki(dane.nastrojeRozgrywki);
            ustawIdWybranegoNastroju(dane.nastrojeRozgrywki[0].id)
            ustawPlatformy(dane.platformy);
            ustawWszystkieJezyki(dane.jezyki);
            ustawWszystkieStopnie(dane.stopnieBieglosciJezyka);
            ustawRole(dane.role ?? []);
            ustawIdRoliDoDodania(dane.role[0]?.id ?? null);
        };

        podajNieZintegrowaneDane();

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
            int? IdPlatformy,
            int? IdRoliKapitana,
            ICollection<WymaganaStatystykaDruzyny>? WymaganeStatystyki, (u nas null)
            ICollection<CreateMiejsceWDruzynieReq> MiejscaWDruzynie,
            bool CzyZintegrowana

        CreateMiejsceWDruzynieReq wygląda tak:
            int? IdRoli,
            WartoscStatystykiDTO? WymaganaStatystyka (u nas null)
        */

        const dane = {
            nazwa: nazwa,
            idGry: idGryDruzyny,
            czyPubliczna: czyPubliczna,
            opis: opis,
            idNastrojuRozgrywki: idWybranegoNastroju,
            idWymaganegoJezyka: idWymaganegoJezyka,
            idWymaganegoStopniaBieglosciJezyka: idMinimalnegoStopniaJezyka,
            idPlatformy: idWybranejPlatformy,
            idRoliKapitana: idRoliKapitana,
            WymaganeStatystyki: null,
            MiejscaWDruzynie: miejscaWDruzynie.map((miejsce) => {
                return {
                    idRoli: miejsce.idRoli,
                    wymaganaStatystyka: null,
                }
            }),
            czyZintegrowana: false
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
                ustawBladOgolny(bledy.Ogolne ?? body.message);
            }
            toast.error(`${body.errors.Ogolne ?? body.message ?? "Wystąpił błąd podczas tworzenia drużyny"}`, {
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



    // w środku będzie generowana tabelka jeśli jest zintegrowane
    return (<div className="flex flex-col items-center justify-center gap-5">
        <button className="przycisk-nawigacji" onClick={() => ustawIdGryDruzyny(0)}>Zmień grę</button>
        <form className="flex flex-col items-center justify-center gap-5 border-4 border-gray-500 rounded-lg p-5 shadow-lg">
            <div className="flex-col items-center justify-center gap-2 w-[700px]">
                <label>
                    Nazwa:
                    <input
                        type="text"
                        value={nazwa}
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
                                    }}>
                                    <option value = "" key = {-1}>Brak</option>
                                    {
                                        wszystkieJezyki.map((jezyk) =>
                                            <option value={jezyk.id} key={jezyk.id}>{jezyk.nazwa}</option>
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
                                    disabled={idWymaganegoJezyka === null}
                                >
                                    <option value = "" key = {-1}>Brak</option>
                                    {
                                        wszystkieStopnie.map((stopien) =>
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
                    </label><br/>
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
            </div>
        </form>
        <h3>Konfiguracja miejsc w drużynie</h3>
        <table className="overflow-x-auto overflow-y-auto h-full w-1/4 border-4 border-gray-600 rounded-lg shadow-lg">
            <thead className="bg-gray-200">
                <tr className="font-semibold border border-gray-600 text-3xl text-gray-800">
                    <th className="border border-gray-600 text-center">Rola</th>
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
                    <th className="text-gray-700 p-2">Ty - kapitan (zablokowane)</th>
                </tr>
            {/* tutaj mapujemy po miejscach w drużynie */}
                {
                    miejscaWDruzynie.map((miejsce, index) =>
                        <>
                            <tr className="border border-gray-600 bg-amber-50" key = {index}>
                                <th className="border border-gray-600 text-center">{miejsce.nazwaRoli.length === 0 ? "brak" : miejsce.nazwaRoli}</th>
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
