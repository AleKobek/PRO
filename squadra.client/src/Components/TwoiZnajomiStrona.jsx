import '../App.css';

import React, {useEffect, useState} from 'react';
import {useAuth} from "../Context/AuthContext";
import {API_BASE_URL} from "../config/api";
import ZnajomyNaLiscieKomponent from "./ZnajomyNaLiscieKomponent";
import {Bounce, toast, ToastContainer} from "react-toastify";
import CzatZeZnajomymKomponent from "./CzatZeZnajomymKomponent";

const TOAST_CONTAINER_ID = "twoi-znajomi-toast";
export default function TwoiZnajomiStrona({ustawCzySaNoweWiadomosci}) {

    const { uzytkownik, ladowanie } = useAuth();
    const userId = uzytkownik?.id ?? null;
    const [znajomi, ustawZnajomych] = useState([]);
    const [idZnajomegoZOtwartymCzatem, ustawIdZnajomegoZOtwartymCzatem] = useState(null);
    const [pseudonimZnajomegoZOtwartymCzatem, ustawPseudonimZnajomegoZOtwartymCzatem] = useState("");
    const [awatarZnajomegoZOtwartymCzatem, ustawAwatarZnajomegoZOtwartymCzatem] = useState("");
    const [pokazDodajZnajomego, ustawPokazDodajZnajomego] = useState(false);
    const frReqRef = React.useRef(null);
    const [loginDoZaproszenia, ustawLoginDoZaproszenia] = useState("");
    const [czySieWysylaZaproszenie, ustawCzySieWysylaZaproszenie] = useState(false);

    // aktualizujemy wybranemu znajomemu, czy ma nowe wiadomości
    const przyWyborzeZnajomego = (idZnajomego) => {
        ustawIdZnajomegoZOtwartymCzatem(idZnajomego);
        ustawZnajomych((prev) => prev.map((znajomy) =>
            znajomy.idZnajomego === idZnajomego
                ? {...znajomy, czySaNoweWiadomosci: false}
                : znajomy
        ));
    };

    // przy załadowaniu od razu ustawiamy, że nie ma nowych wiadomości. Najwyżej nagłówek potem to poprawi
    useEffect(() => {
        if(typeof ustawCzySaNoweWiadomosci === "function") ustawCzySaNoweWiadomosci(false);
    },[ustawCzySaNoweWiadomosci])

    // co minutę pobieramy nową listę znajomych, aby była aktualna
    useEffect(() => {
        const ac = new AbortController();
        let alive = true;

        const interval = setInterval(async () => {
            if (!alive || !userId) return;
            await podajZnajomych(ac.signal, alive)
        }, 60000);

        // jak wychodzimy to zatrzymujemy nasz interval
        return () => {
            alive = false;
            ac.abort();
            clearInterval(interval);
        };
    }, [userId]);

    // pobieramy listę znajomych z backendu przy załadowaniu strony
    useEffect(() => {
        if(!userId) {
            ustawZnajomych([]);
            ustawIdZnajomegoZOtwartymCzatem(null);
            ustawPseudonimZnajomegoZOtwartymCzatem("");
            ustawAwatarZnajomegoZOtwartymCzatem("");
            return;
        }

        const ac = new AbortController();
        let alive = true;

        podajZnajomych(ac.signal, alive);

        return () => {
            alive = false;
            ac.abort();
        };

    }, [userId]);

    // wydzielone, aby używać przy pobieraniu na początku i co minutę
    const podajZnajomych = async (signal, alive) => {

        // taka pomocnicza funkcja dla abort controller
        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: signal, credentials: "include" });
                if (!res.ok) return null;
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                return null;
            }
        };

        const znajomi = await fetchJsonAbort(`${API_BASE_URL}/Znajomi`, {signal});
        if(!alive || !Array.isArray(znajomi)) return;
        ustawZnajomych(znajomi);
    }

    // przy zmianie znajomego z otwartym czatem
    useEffect(() => {
        if(!znajomi.length) {
            ustawIdZnajomegoZOtwartymCzatem(null);
            ustawPseudonimZnajomegoZOtwartymCzatem("");
            ustawAwatarZnajomegoZOtwartymCzatem("");
            return;
        }

        if(!idZnajomegoZOtwartymCzatem) {
            ustawIdZnajomegoZOtwartymCzatem(znajomi[0].idZnajomego);
            return;
        }

        const znajomy = znajomi.find(z => z.idZnajomego === idZnajomegoZOtwartymCzatem);
        if(!znajomy) {
            ustawIdZnajomegoZOtwartymCzatem(znajomi[0].idZnajomego);
            // od razu ustawiamy znajomemu z otwartym czatem, że jego wiadomości są odczytane
            ustawZnajomych((prev) => prev.map((znajomy) =>
                znajomy.idZnajomego === znajomi[0].idZnajomego
                    ? {...znajomy, czySaNoweWiadomosci: false}
                    : znajomy
            ));
            return;
        }

        ustawPseudonimZnajomegoZOtwartymCzatem(znajomy.pseudonim ?? "");
        ustawAwatarZnajomegoZOtwartymCzatem(znajomy.awatar ?? "")
    }, [idZnajomegoZOtwartymCzatem, znajomi])

    // zamykamy jak klikamy poza to
    useEffect(() => {
        if (!pokazDodajZnajomego) return;

        const handleClickOutside = (e) => {
            if (frReqRef.current && !frReqRef.current.contains(e.target)) {
                ustawPokazDodajZnajomego(false);
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [pokazDodajZnajomego]);

    // render panelu dodawania znajomego, który jest na wierzchu, gdy klikniemy "dodaj znajomego"
    const PanelDodajZnajomego = () => (
        <div
            ref={frReqRef}
            className="fixed top-[230px] left-[165px] overflow-y-auto bg-white border-2 border-amber-300
            rounded-md shadow-lg p-3 justify-center items-center"
            style={{ zIndex: 2000 }}
        >
            <div className="flex flex-col">
                <div className="flex justify-end">
                    <button onClick={() => ustawPokazDodajZnajomego(false)} className="cursor-pointer text-red-600 font-bold items-end">
                        Zamknij
                    </button>
                </div>
                <div className="flex flex-col items-center">
                    <span>Podaj login zapraszanego użytkownika <br/></span>
                </div>

                <input
                    type="text"
                    className="px-2 border border-gray-800 my-2 rounded-md w-full"
                    value={loginDoZaproszenia}
                    onChange={(e)=>ustawLoginDoZaproszenia(e.target.value)}
                    autoFocus={true}
                />
                <button
                    className={czySieWysylaZaproszenie || !loginDoZaproszenia.trim() ?
                        "zablokowany-przycisk" :
                        "bg-green-900 text-white rounded-md px-3 py-1 mt-4 hover:bg-green-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"}
                    disabled={czySieWysylaZaproszenie || !loginDoZaproszenia.trim()}
                    onClick={przyWysylaniuZaproszenia}
                >
                    Wyślij zaproszenie do znajomych
                </button>
            </div>
        </div>
    );

    const przyWysylaniuZaproszenia = async () => {
        if(czySieWysylaZaproszenie) return;
        ustawCzySieWysylaZaproszenie(true);
        const toastOptions = {
            position: "top-center",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: false,
            pauseOnHover: true,
            draggable: true,
            progress: undefined,
            theme: "light",
            transition: Bounce,
            containerId: TOAST_CONTAINER_ID,
        };
        try {
            const res = await fetch(`${API_BASE_URL}/Powiadomienie/zaproszenie/znajomi`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: "include",
                body: JSON.stringify(loginDoZaproszenia.trim())
            });

            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");
            if (!res.ok) {
                switch (res.status) {
                    case 404: {
                        toast.error('Użytkownik o takim loginie nie istnieje!', toastOptions);
                        break;
                    }
                    case 409: { // konflikt, u nas to się dzieje gdy uzytkownik jest już znajomym
                        toast.error('Ten użytkownik jest już Twoim znajomym!', toastOptions);
                        break;
                    }
                    default: {
                        toast.error(`Nie można wysłać zaproszenia: ${body || 'Nieznany błąd.'}`, toastOptions);
                    }
                }
                return;
            }
            toast.success("Pomyślnie wysłano zaproszenie!", toastOptions);
        } catch (err) {
            console.error('Błąd wysyłania zaproszenia:', err);
            toast.error('Wystąpił błąd podczas wysyłania zaproszenia. Spróbuj ponownie później.', toastOptions);
        }finally {
            ustawCzySieWysylaZaproszenie(false);
        }
    }


    if(ladowanie) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna" className="!p-0 !m-0">
            <div className="grid grid-cols-3 h-f">
                {/* znajomi */}
                <div className="col-span-1 flex flex-col border-r-8 border-blue-500 py-4 h-full">
                    <h1>Twoi znajomi</h1>
                    {/* dodaj znajomego */}
                    <div className="flex flex-col items-center py-4">
                        <button
                            onClick={() => ustawPokazDodajZnajomego(v => !v)}
                            className="bg-green-900 text-white rounded-md px-3 py-1 my-4 hover:bg-green-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                            aria-expanded={pokazDodajZnajomego}
                            aria-controls="panel-dodaj-znajomego"
                        >
                            Dodaj znajomego
                        </button>
                    </div>
                    {/* lista znajomych */}
                    <div className="border-t-2 border-gray-400">
                        {znajomi.length===0
                            ? <p className="p-4 font-light text-gray-700 text-center">
                                Lista znajomych jest pusta. <br/> Użyj przycisku "dodaj znajomego", aby to zmienić!
                            </p>
                            : <ul className="overflow-y-auto">
                                {znajomi.map((znajomy) => (
                                    <ZnajomyNaLiscieKomponent key={znajomy.idZnajomego} znajomy={znajomy} idZnajomegoZOtwartymCzatem = {idZnajomegoZOtwartymCzatem} przyWyborzeZnajomego={przyWyborzeZnajomego}/>
                                ))}
                            </ul>
                        }
                    </div>
                </div>
                {/* czat */}
                <div className="flex flex-col col-span-2 w-full">
                    {userId && idZnajomegoZOtwartymCzatem && (
                        <CzatZeZnajomymKomponent
                            idZnajomegoZOtwartymCzatem={idZnajomegoZOtwartymCzatem}
                            naszeId={userId}
                            pseudonimZnajomegoZOtwartymCzatem={pseudonimZnajomegoZOtwartymCzatem}
                            awatarZnajomegoZOtwartymCzatem={awatarZnajomegoZOtwartymCzatem}
                        />
                    )}
                </div>
            </div>
        </div>
        {/* overlay dodaj znajomego, renderujemy na wierzchu tej samej strony */}
        {pokazDodajZnajomego && <PanelDodajZnajomego/>}
        <ToastContainer
            containerId={TOAST_CONTAINER_ID}
            position="top-center"
            autoClose={5000}
            hideProgressBar={false}
            newestOnTop={false}
            closeOnClick={false}
            rtl={false}
            pauseOnFocusLoss
            draggable
            pauseOnHover
            theme="light"
            transition={Bounce}
        />
    </>);
}