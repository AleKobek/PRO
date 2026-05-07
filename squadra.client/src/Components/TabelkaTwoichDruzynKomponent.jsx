import React, {useEffect, useMemo, useState} from "react";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast} from "react-toastify";



export default function TabelkaTwoichDruzynKomponent({idUzytkownika}) {

    const [druzyny, ustawDruzyny] = useState([]);

    const [pokazPanelSzczegolow, ustawPokazPanelSzczegolow] = useState(false);
    const statRef = React.useRef(null);


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
                                {Array.isArray(czlonkowie) && czlonkowie.map((miejsce) => {
                                    if (!miejsce || typeof miejsce !== 'object') return null;

                                    const czlonek = miejsce.czlonek;
                                    if (!czlonek || typeof czlonek !== 'object') return null;

                                    const rola = miejsce.rola ?? null;
                                    const czyKapitan = miejsce.czyKapitan ?? false;

                                    return (<div key={czlonek.idUzytkownika} className="h-10 flex flex-col items-center justify-center my-3">
                                        {rola && czlonkowie.length < 7 && <div className="text-center text-xs">{rola}</div>}
                                        {czyKapitan ? <img src="/img/crown.svg" alt="korona" className="w-4 h-4"/> : <div className="h-5 mt-1"/>}
                                        <MiniAwatarComponent status={czlonek.nazwaStatusu} obraz={czlonek.awatar}/>
                                        {/* jak lista członków jest za długa, to nie pokazujemy pseudonimów */}
                                        {czlonkowie.length < 7 && <div className="text-center text-xs">{czlonek.pseudonim}</div>}
                                    </div>);
                                })}
                            </td>
                            <td className="text-gray-900 text-center border border-gray-600">{nazwaNastroju}</td>
                            <td className="items-center border border-gray-600">
                                <button className="bg-blue-600 text-white text-2xl p-2 hover:bg-blue-500 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105">Szczegóły</button>
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

    </div>);
}

function MiniAwatarComponent({obraz, status}) {

    const imgSrc = obraz === "" ? "/img/domyslny_awatar.png" : "data:image/jpeg;base64," + obraz;

    return (
        <span className="relative inline-block h-10 w-10 mb-2">
            <img
                src={imgSrc}
                alt="awatar"
                className="awatar block h-full w-full object-cover rounded-full border border-3 border-black"
            />
            <MiniKropkaStatusuKomponent status={status}/>
        </span>
    );
}

function MiniKropkaStatusuKomponent({status}){

    const classNameStatusu = useMemo(() => {
        const Status = {
            1: "DOSTĘPNY",
            2: "ZARAZ WRACAM",
            3: "NIE PRZESZKADZAĆ",
            5: "OFFLINE"
        }
        const tempStatus = Status[status]
        // dostajemy nazwę (w awatar komponent)
        if(!tempStatus)
            switch(status){
                case "Dostępny": return "bg-green-500";
                case "Zaraz wracam": return "bg-yellow-400";
                case "Nie przeszkadzać": return "bg-red-500";
                case "Offline": return "bg-gray-400";
                default : return null; // jak nie ma statusu, nic nie rysujemy
            }
        // dostajemy id
        switch(tempStatus){
            case "DOSTĘPNY": return "bg-green-500";
            case "ZARAZ WRACAM": return "bg-yellow-400";
            case "NIE PRZESZKADZAĆ": return "bg-red-500";
            case "OFFLINE": return "bg-gray-400";
            default : return null; // jak coś się popsuje
        }
    },[status]);

    const kropkaClass = `absolute bottom-0 right-0 translate-x-1/4 translate-y-1/4 h-3 w-3 rounded-full border border-black ${classNameStatusu}`;

    return(<span className={status && kropkaClass}/>)
}