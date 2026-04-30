import React, {useEffect, useState} from "react";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast} from "react-toastify";


export default function TabelkaBibliotekiGierKomponent({idUzytkownika}){

    const [bibliotekaGier, ustawBibliotekaGier] = useState([]);

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

        return () => { alive = false; ac.abort(); };
    }, [idUzytkownika]);


    return (
        <div className="overflow-x-auto overflow-y-auto h-full border-4 border-gray-400 rounded-lg mt-5">
            <div className="w-full">
                {/* nagłówek */}
                <div className="hidden md:grid grid-cols-10 gap-4 font-semibold border-b-4 px-2 my-2 text-2xl text-gray-700">
                    <div className="border-r-2 col-span-3 text-center">Tytuł</div>
                    <div className="border-r-2 col-span-2 text-center">Gatunek</div>
                    <div className="border-r-2 col-span-2 text-center">Czas rozgrywki</div>
                    <div className="flex justify-center border-r-2 col-span-2 text-center">Platformy</div>
                    <div className="col-span-1"></div>
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
                                <div key={key} className="grid grid-cols-10 gap-4 items-center border-b px-2 py-3 text-lg border-x-2">
                                    <div className="font-medium text-gray-900 text-center border-r-2 col-span-3">{tytul || "-"}</div>
                                    <div className="text-gray-500 text-center border-r-2 col-span-2">{gatunek || "-"}</div>
                                    <div className="text-gray-500 text-center border-r-2 col-span-2">{godzinyGrania}</div>
                                    <div className="flex gap-2 items-center justify-center border-r-2 col-span-2">
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
                                        <button className="text-white bg-blue-900 block mx-auto text-[10px] px-2 py-1 hover:bg-blue-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105">Szczegóły</button>
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
    );
}