import React, {useEffect, useState} from "react";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast} from "react-toastify";


export default function TabelkaBibliotekiGierKomponent({idUzytkownika}) {

    const [bibliotekaGier, ustawBibliotekaGier] = useState([]);

    const [pokazPanelStatystyk, ustawPokazPanelStatystyk] = useState(false);
    const statRef = React.useRef(null);

    const [statystykiGry, ustawStatystykiGry] = useState([]);
    const [idWybranejGry, ustawIdWybranejGry] = useState(null);

    useEffect(() => {

        if(!idUzytkownika) return;

        const ac = new AbortController();
        let alive = true;

        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                if (!res.ok) {
                    toast.error('Wystąpił błąd podczas pobierania biblioteki gier', {
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
                toast.error('Wystąpił błąd podczas pobierania biblioteki gier', {
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

        const podajDaneGier = async () => {
            const data = await fetchJsonAbort(`${API_BASE_URL}/BibliotekaGier/${idUzytkownika}`);
            if (!alive) return;

            let normalized = [];
            if (Array.isArray(data)) normalized = data;
            else if (data) normalized = [data];

            ustawBibliotekaGier(normalized);
        };

        podajDaneGier();

        return () => {
            alive = false;
            ac.abort();
        };
    }, [idUzytkownika]);

    const pobierzStatystykiGry = async (idGry) => {
        if (!idGry) return;
        if (!idUzytkownika) return;
        if (idWybranejGry === idGry) return; // nie musimy pobierać drugi raz

        ustawIdWybranejGry(idGry);

        const ac = new AbortController();
        let alive = true;
        // pobieramy statystyki danego użytkownika dla danej gry

        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                if (!res.ok) {
                    toast.error('Wystąpił błąd podczas pobierania danych profilu', {
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
                toast.error('Wystąpił błąd podczas pobierania danych profilu', {
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

        const data = await fetchJsonAbort(`${API_BASE_URL}/Statystyki/${idUzytkownika}/${idGry}`);

        // przerywamy działanie funkcji
        if (!alive) return;
        if (!data) return;

        ustawStatystykiGry(data);

    }

    const przyKliknieciuSzczegoly = async (idGry) => {
        await pobierzStatystykiGry(idGry);
        ustawPokazPanelStatystyk(true);
    }

    const PanelStatystyk = () =>{
        if(!statystykiGry || statystykiGry.length === 0) {
            return(<div
                ref={statRef}
                className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] p-10 overflow-y-auto
                rounded-md shadow-lg justify-center items-center bg-amber-50 border-2 border-amber-400"
                style={{zIndex: 2000}}
            >
                <div className="flex justify-end">
                    <button onClick={() => ustawPokazPanelStatystyk(false)} className="cursor-pointer">Zamknij</button>
                </div>
                <div className="flex flex-col">
                    <h2 className="text-2xl font-bold mb-4">Statystyki gier</h2>
                    <p>Brak statystyk dla tej gry.</p>
                </div>
            </div>);
        }
        return(<div
                ref={statRef}
                className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[700px] pt-2 p-10 overflow-y-auto
                rounded-md shadow-lg justify-center items-center bg-amber-50 border-2 border-amber-400"
                style={{zIndex: 2000}}
            >
            <div className="flex justify-end">
                <button onClick={() => ustawPokazPanelStatystyk(false)} className="cursor-pointer">Zamknij</button>
            </div>
            <div className="flex flex-col">
                <h2 className="text-2xl font-bold mb-4">Statystyki gier</h2>
            {/*  wyświetlamy statystyki jako tabelka, dzieląc na kategorie  */}
            {
                statystykiGry.map((kategoria) => (
                    <div id={kategoria.idKategorii} className="flex flex-col overflow-y-auto">
                        <h3>{kategoria.nazwaKategorii}</h3>
                        <table className="w-full border-collapse">
                            <thead>
                            <tr>
                                <th className="border border-gray-500 p-2">Nazwa</th>
                                <th className="border border-gray-500 p-2">Wartość</th>
                            </tr>
                            </thead>
                            <tbody>{kategoria.statystyki.map((stat) => (
                                <tr key={stat.id}>
                                    <td className="border border-gray-500 p-2">{stat.nazwa}</td>
                                    <td className="border border-gray-500 p-2">{stat.wartosc.length === 0 ? "-" : stat.wartosc}</td>
                                </tr>
                            ))}</tbody>
                        </table>
                    </div>
                ))
            }
            </div>
        </div>);
    }



    return (<div>
        <div className="overflow-x-auto overflow-y-auto h-full border-4 border-gray-600 rounded-lg shadow-lg">
            <div className="w-full">
                {/* nagłówek */}
                <div className="hidden md:grid grid-cols-10 gap-4 font-semibold border-b-2 border-gray-600 px-2 py-2 text-2xl text-gray-800">
                    <div className="border-r-2 border-gray-600 col-span-3 text-center">Tytuł</div>
                    <div className="border-r-2 border-gray-600 col-span-2 text-center">Gatunek</div>
                    <div className="border-r-2 border-gray-600 col-span-2 text-center">Czas rozgrywki</div>
                    <div className="flex justify-center border-r-2 border-gray-600 col-span-2 text-center">Platformy</div>
                    <div className="col-span-1 "></div>
                </div>

                {/* wiersze */}
                <div>
                    {Array.isArray(bibliotekaGier) && bibliotekaGier.length > 0 ? (
                        bibliotekaGier.map((gra, index) => {
                            if (!gra || typeof gra !== 'object') return null;

                            const tytul = gra.tytul;
                            const gatunek = gra.gatunek;
                            const godzinyGrania = Number(gra.godzinyGrania) || 0;
                            const platformy = gra.platformy ?? [];
                            const key = gra.idGry;

                            return (
                                <div key={key} className="grid grid-cols-10 gap-4 items-center border-b border-gray-600 px-2  text-lg border-x-2">
                                    <div className="font-medium text-gray-900 text-center border-r-2 border-gray-600 col-span-3">{tytul || "-"}</div>
                                    <div className="text-gray-500 text-center border-r-2 border-gray-600 col-span-2">{gatunek || "-"}</div>
                                    <div className="text-gray-500 text-center border-r-2 border-gray-600 col-span-2">{godzinyGrania}</div>
                                    <div className="flex gap-2 items-center justify-center border-r-2 border-gray-600 col-span-2">
                                        {Array.isArray(platformy) && platformy.map((platforma) => {
                                            const pKey = platforma?.idPlatformy;
                                            if (platforma?.logo) {
                                                const src = typeof platforma.logo === 'string'
                                                    ? (platforma.logo.startsWith('data:') ? platforma.logo : "data:image/jpeg;base64," + platforma.logo)
                                                    : undefined;
                                                return src ? (
                                                    <img key={pKey} src={src} alt={platforma?.nazwa ?? "logo"} className="h-10 w-10 my-1 rounded-full border-2 border-black" />
                                                ) : (
                                                    <div key={pKey} className="h-10 w-10 flex items-center justify-center bg-gray-200 rounded-full border-2 border-black text-xs">{platforma?.nazwa ?? "?"}</div>
                                                );
                                            }
                                            return (<div key={pKey} className="h-10 w-10 flex items-center justify-center bg-gray-200 rounded-full border-2 border-black text-xs">{platforma?.nazwa ?? "?"}</div>);
                                        })}
                                    </div>
                                    <div className="flex justify-center col-span-1">
                                        <button
                                            className="text-white bg-blue-900 block mx-auto text-[10px] px-2 py-1 hover:bg-blue-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                                            onClick={() => przyKliknieciuSzczegoly(gra.idGry)}
                                        >Szczegóły</button>
                                    </div>
                                </div>
                            );
                        })
                    ) : (
                        <div className="p-4 text-center text-gray-500">Brak gier w bibliotece</div>
                    )}
                </div>
            </div>
        </div>
        {pokazPanelStatystyk && <PanelStatystyk />}
    </div>);
}