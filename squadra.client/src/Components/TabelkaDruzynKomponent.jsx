import React, {useEffect, useState} from "react";

import {useNavigate} from "react-router-dom";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast} from "react-toastify";
import AwatarComponent from "./AwatarComponent";



export default function TabelkaDruzynKomponent({idDruzyn, brakDruzynWiadomosc, czySzczegolyWNowejKarcie = false, pierwszaStronaDruzyn, idUzytkownika = null}) {

    const navigate = useNavigate();
    const [druzynyNaStronie, ustawDruzynyNaStronie] = useState([])

    const [liczbaDruzynNaStronie, ustawLiczbeDruzynNaStronie] = useState(20); // domyślne ustawienie liczby drużyn na stronie to 20
    const [aktualnaStrona, ustawAktualnaStrone] = useState(0)
    const liczbaStron = Math.max(1, Math.ceil((idDruzyn?.length ?? 0) / liczbaDruzynNaStronie));

    const openInNewTab = url => {
        window.open(url, '_blank', 'noopener,noreferrer');
    };

    // po załadowaniu od razu ustawiamy pierwszą stronę drużyn
    useEffect(() => {
        if(!pierwszaStronaDruzyn || pierwszaStronaDruzyn.length === 0) return;
        ustawDruzynyNaStronie(pierwszaStronaDruzyn);
    }, [pierwszaStronaDruzyn]);

    // przy zmianie strony pobieramy drużyny o podanych id
    useEffect(() => {
        if(!idDruzyn) return;
        if(idDruzyn.length === 0) return;
        if(aktualnaStrona < 0) return;
        if(aktualnaStrona >= liczbaStron) return;
        if(liczbaDruzynNaStronie === 0) return;

        const ac = new AbortController();
        let alive = true;

        const pobierzNoweDruzyny = async () => {
            const idDruzynDoPobrania = idDruzyn.slice(aktualnaStrona * liczbaDruzynNaStronie, (aktualnaStrona + 1) * liczbaDruzynNaStronie);
            const opcje = {
                method: 'POST', // nie GET, bo to nie jest klasyczne wyszukiwanie zasobu po url
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(idDruzynDoPobrania),
                credentials: "include",
                signal: ac.signal,
            }

            let url = idUzytkownika ? `${API_BASE_URL}/Druzyna/tabelka/${idUzytkownika}` : `${API_BASE_URL}/Druzyna/tabelka`;
            const res = await fetch(url, opcje);
            const body = await res.json().catch(() => null);

            if (!res.ok) {
                toast.error(`Wystąpił błąd podczas pobierania drużyn: ${body?.message ?? res.statusText ?? "nieznany błąd"}`, {
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
            if(!alive || !body) return;
            ustawDruzynyNaStronie(body);
        }

        pobierzNoweDruzyny();

        return () => {
            alive = false;
            ac.abort();
        };
    },[aktualnaStrona, idDruzyn, idUzytkownika, liczbaDruzynNaStronie, liczbaStron])

    return (<div>
        <span className="mr-2">Maksymalna liczba drużyn na stronie:</span>
        <select
            className="border border-gray-300 rounded-md p-1 mb-2"
            value={liczbaDruzynNaStronie}
            onChange={ (e) => {
                ustawLiczbeDruzynNaStronie(parseInt(e.target.value));
                ustawAktualnaStrone(0);
            }}>
            <option value="10">10</option>
            <option value="20">20</option>
            <option value="30">30</option>
            <option value="50">50</option>
        </select>
        {Array.isArray(druzynyNaStronie) && druzynyNaStronie.length > 0 ? (
            <table className="overflow-x-auto overflow-y-auto h-full w-full border-4 border-gray-600 rounded-lg shadow-lg">
                <tbody>
                <tr className="font-bold border border-gray-600 text-3xl text-gray-800">
                    <th className="border border-gray-600 text-center" style={{width: "30%"}}>Nazwa</th>
                    <th className="border border-gray-600 text-center">Gra</th>
                    <th className="border border-gray-600 text-center text-xl" style={{width: "10%"}}>Ostatnia aktywność kapitana</th>
                    <th className="border border-gray-600 text-center">Członkowie</th>
                    <th className="border border-gray-600 text-center">Nastrój</th>
                    {czySzczegolyWNowejKarcie
                        ? <th className="text-sm" style={{width: "15%"}}></th>
                        : <th className="text-sm" style={{width: "8%"}}></th>
                    }
                </tr>

                {druzynyNaStronie.map((druzyna) => {
                    if (!druzyna || typeof druzyna !== 'object') return null;
                    const key = druzyna.id;
                    const nazwa = druzyna.nazwa;
                    const tytulGry = druzyna.tytulGry;
                    const ostatniaAktywnoscKapitana = druzyna.ostatniaAktywnoscKapitana;
                    const czlonkowie = druzyna.czlonkowie || [];
                    const nazwaNastroju = druzyna.nazwaNastroju;
                    const czySaNoweWiadomosci = druzyna.czySaNoweWiadomosci;

                    return (
                        <tr key={key} className="items-center border border-gray-600 px-2 text-xl">
                            <td className="font-semibold text-gray-900 text-center border border-gray-600">
                                <div className="flex items-center justify-center gap-2">
                                    {nazwa}
                                    {czySaNoweWiadomosci ? <img src="/img/koperta.svg" alt="koperta" className="text-red-600 h-[1.2em] w-auto align-middle"/> : null}
                                </div>
                            </td>
                            <td className="text-gray-900 text-center border border-gray-600">{tytulGry}</td>
                            <td className="text-gray-900 text-center border border-gray-600">{ostatniaAktywnoscKapitana}</td>
                            <td className="flex gap-5 items-center justify-center border-gray-600 py-5">
                                {Array.isArray(czlonkowie) && czlonkowie.map((miejsce, index) => {

                                    if (!miejsce || typeof miejsce !== 'object') return null;

                                    const rola = miejsce.rola ?? null;
                                    const czyKapitan = miejsce.czyKapitan ?? false;

                                    const czlonek = miejsce.czlonek;

                                    if (!czlonek || typeof czlonek !== 'object')
                                        return (<div key={index} className="h-10 flex flex-col items-center justify-center my-3">
                                        {rola && czlonkowie.length < 7 && <div className="text-center text-xs">{rola}</div>}
                                        <AwatarComponent status={null} obraz="puste" wysokosc={40}/>
                                        {/* jak lista członków jest za długa, to nie pokazujemy pseudonimów */}
                                        {czlonkowie.length < 7 && <div className="text-center text-xs text-blue-900 mt-1.5">Puste</div>}
                                    </div>);

                                    return (<div key={czlonek.idUzytkownika} className="h-10 flex flex-col items-center justify-center my-3">
                                        {rola && czlonkowie.length < 7 && <div className="text-center text-xs">{rola}</div>}
                                        {czyKapitan ? <img src="/img/crown.svg" alt="korona" className="w-4 h-4"/> : <div className="h-5 mt-1"/>}
                                        <AwatarComponent status={czlonek.nazwaStatusu} obraz={czlonek.awatar} wysokosc={40}/>
                                        {/* jak lista członków jest za długa, to nie pokazujemy pseudonimów */}
                                        {czlonkowie.length < 7 && <div className="text-center text-xs">{czlonek.pseudonim}</div>}
                                    </div>);
                                })}
                            </td>
                            <td className="text-gray-900 text-center border border-gray-600">{nazwaNastroju}</td>
                            <td className="items-center border border-gray-600">
                                <button
                                    className="bg-blue-600 text-white text-2xl p-2 hover:bg-blue-500 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                                    onClick={() => {
                                        czySzczegolyWNowejKarcie
                                            ? openInNewTab(druzyna.id ? `/druzyna/${druzyna.id}` : '#')
                                            : navigate(druzyna.id ? `/druzyna/${druzyna.id}` : '#')
                                    }}
                                >{czySzczegolyWNowejKarcie
                                    ? "Szczegóły (nowa karta)"
                                    : "Szczegóły"}</button>
                            </td>
                        </tr>

                    );
                })
                }
                </tbody>
            </table>
        ) : (
            <div className="p-4 text-center text-gray-800">{brakDruzynWiadomosc}</div>
        )}
        {/* paginacja */}
        <div className="flex justify-center mt-3"><span>Strona {aktualnaStrona + 1} z {liczbaStron}</span></div>
        <div className="flex justify-center gap-6">
            <button className={aktualnaStrona === 0 ? "zablokowany-przycisk" : "bg-blue-900 hover:bg-blue-600 text-white"} disabled={aktualnaStrona === 0}
                    onClick={ () => { ustawAktualnaStrone(aktualnaStrona - 1) } }
            >Poprzednia strona</button>

            {/* przycisk następnej strony*/}
            <button className={liczbaStron === 0 || aktualnaStrona === liczbaStron - 1 ? "zablokowany-przycisk" : "bg-blue-900 hover:bg-blue-600 text-white"} disabled= {liczbaStron === 0 || aktualnaStrona === liczbaStron - 1}
                    onClick={ () => { ustawAktualnaStrone(aktualnaStrona + 1) } }
            >Następna strona</button>
        </div>
    </div>);
}