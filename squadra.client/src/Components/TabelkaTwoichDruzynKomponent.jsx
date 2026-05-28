import React, {useEffect, useState} from "react";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast} from "react-toastify";
import PanelSzczegolowDruzyny from "./PanelSzczegolowDruzyny";
import MiniAwatarKomponent from "./MiniAwatarKomponent";



export default function TabelkaTwoichDruzynKomponent({idUzytkownika}) {

    const [druzyny, ustawDruzyny] = useState([]);

    const [pokazPanelSzczegolow, ustawPokazPanelSzczegolow] = useState(false);
    const [pokazPanelEdycji, ustawPokazPanelEdycji] = useState(false);
    const [idWybranejDruzyny, ustawIdWybranejDruzyny] = useState(null);
    const [nazwaWybranejDruzyny, ustawNazwaWybranejDruzyny] = useState("");
    const [szczegolyWybranejDruzyny, ustawSzczegolyWybranejDruzyny] = useState(null);
    const szczegolyRef = React.useRef(null); // powinien być tutaj, czy w komponencie?


    // pobieramy tabelkę drużyn
    useEffect(() => {

        if(!idUzytkownika) return;

        const ac = new AbortController();
        let alive = true;

        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                if (!res.ok) {
                    toast.error('Wystąpił błąd podczas pobierania twoich drużyn', {
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
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                toast.error('Wystąpił błąd podczas pobierania twoich drużyn', {
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

        const podajTwojeDruzyny = async () => {
            const data = await fetchJsonAbort(`${API_BASE_URL}/Druzyna/tabelka/${idUzytkownika}`);
            if (!alive) return;

            let normalized = [];
            if (Array.isArray(data)) normalized = data;
            else if (data) normalized = [data];

            ustawDruzyny(normalized);
        };

        podajTwojeDruzyny();

        return () => {
            alive = false;
            ac.abort();
        };
    }, [idUzytkownika]);

    const pobierzStatystykiDruzyny = async (idDruzyny) => {
        if (!idDruzyny) return;
        if (!idUzytkownika) return;
        if (idDruzyny === idWybranejDruzyny) return; // nie musimy pobierać drugi raz

        ustawIdWybranejDruzyny(idDruzyny);


        const ac = new AbortController();
        let alive = true;

        // pobieramy szczegóły danej drużyny
        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                if (!res.ok) {
                    toast.error('Wystąpił błąd podczas pobierania danych drużyny', {
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
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                toast.error('Wystąpił błąd podczas pobierania danych drużyny', {
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

        const data = await fetchJsonAbort(`${API_BASE_URL}/Druzyna/szczegoly/${idDruzyny}`);

        // przerywamy działanie funkcji
        if (!alive) return;
        if (!data) return;

        ustawSzczegolyWybranejDruzyny(data);
    }

    const przyKliknieciuSzczegoly = async (idDruzyny, nazwaDruzyny) => {
        await pobierzStatystykiDruzyny(idDruzyny);
        ustawNazwaWybranejDruzyny(nazwaDruzyny);
        ustawPokazPanelSzczegolow(!pokazPanelSzczegolow);
    }

    const usunDruzyneZTabelki = (idDruzyny) => {
        if (!idDruzyny) return;
        if (!idUzytkownika) return;
        ustawIdWybranejDruzyny(null);
        ustawNazwaWybranejDruzyny("");
        ustawSzczegolyWybranejDruzyny(null);
        let druzynyTemp = druzyny.filter(druzyna => druzyna.id !== idDruzyny);
        ustawDruzyny(druzynyTemp);
    }


    return (<div>
        {Array.isArray(druzyny) && druzyny.length > 0 ? (
            <table className="overflow-x-auto overflow-y-auto h-full w-full border-4 border-gray-600 rounded-lg shadow-lg">
                <tbody>
                <tr className="font-bold border border-gray-600 text-3xl text-gray-800">
                    <th className="border border-gray-600 text-center" style={{width: "30%"}}>Nazwa</th>
                    <th className="border border-gray-600 text-center">Gra</th>
                    <th className="border border-gray-600 text-center text-xl" style={{width: "10%"}}>Ostatnia aktywność kapitana</th>
                    <th className="border border-gray-600 text-center">Członkowie</th>
                    <th className="border border-gray-600 text-center">Nastrój</th>
                    <th style={{width: "8%"}}></th>
                </tr>

                {druzyny.map((druzyna) => {
                    if (!druzyna || typeof druzyna !== 'object') return null;

                    const key = druzyna.id;
                    const nazwa = druzyna.nazwa;
                    const tytulGry = druzyna.tytulGry;
                    const ostatniaAktywnoscKapitana = druzyna.ostatniaAktywnoscKapitana;
                    const czlonkowie = druzyna.czlonkowie || [];
                    const nazwaNastroju = druzyna.nazwaNastroju;

                    return (
                        <tr key={key} className="items-center border border-gray-600 px-2 text-xl">
                            <td className="font-semibold text-gray-900 text-center border border-gray-600">{nazwa}</td>
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
                                        <MiniAwatarKomponent status={null} obraz="puste"/>
                                        {/* jak lista członków jest za długa, to nie pokazujemy pseudonimów */}
                                        {czlonkowie.length < 7 && <div className="text-center text-xs text-blue-900 mt-1.5">Puste</div>}
                                    </div>);

                                    return (<div key={czlonek.idUzytkownika} className="h-10 flex flex-col items-center justify-center my-3">
                                        {rola && czlonkowie.length < 7 && <div className="text-center text-xs">{rola}</div>}
                                        {czyKapitan ? <img src="/img/crown.svg" alt="korona" className="w-4 h-4"/> : <div className="h-5 mt-1"/>}
                                        <MiniAwatarKomponent status={czlonek.nazwaStatusu} obraz={czlonek.awatar}/>
                                        {/* jak lista członków jest za długa, to nie pokazujemy pseudonimów */}
                                        {czlonkowie.length < 7 && <div className="text-center text-xs">{czlonek.pseudonim}</div>}
                                    </div>);
                                })}
                            </td>
                            <td className="text-gray-900 text-center border border-gray-600">{nazwaNastroju}</td>
                            <td className="items-center border border-gray-600">
                                <button
                                    className="bg-blue-600 text-white text-2xl p-2 hover:bg-blue-500 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                                    onClick={() => przyKliknieciuSzczegoly(druzyna.id, druzyna.nazwa)}
                                >Szczegóły</button>
                            </td>
                        </tr>

                    );
                })
                }
                </tbody>
            </table>
        ) : (
            <div className="p-4 text-center text-gray-800">Brak drużyn. Kliknij przycisk na górze, aby do jakiejś dołączyć!</div>
        )}
        {pokazPanelSzczegolow &&
            <PanelSzczegolowDruzyny
                idDruzyny={idWybranejDruzyny}
                nazwaDruzyny={nazwaWybranejDruzyny}
                szczegolyDruzyny={szczegolyWybranejDruzyny}
                idUzytkownika={idUzytkownika}
                daneDruzyny={szczegolyWybranejDruzyny}
                ref={szczegolyRef}
                ustawPokazPanelSzczegolow={ustawPokazPanelSzczegolow}
                ustawPokazPanelEdycji={ustawPokazPanelEdycji}
                usunDruzyne = {usunDruzyneZTabelki}
                ustawSzczegolyDruzyny={ustawSzczegolyWybranejDruzyny}
            />}
    </div>);
}