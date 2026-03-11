import WiadomoscNaLiscieKomponent from "./WiadomoscNaLiscieKomponent";
import React, {useEffect, useState, useRef} from "react";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast, ToastContainer} from "react-toastify";

export default function CzatZeZnajomymKomponent({
                                                    idZnajomegoZOtwartymCzatem,
                                                    naszeId, // do awatara i pseudonimu
                                                    awatarZnajomegoZOtwartymCzatem,
                                                    pseudonimZnajomegoZOtwartymCzatem,
                                                }){

    const [czat, ustawCzat] = useState([]);
    const [czyTrwaLadowanieCzatu, ustawCzyTrwaLadowanieCzatu] = useState(true);
    const [naszAwatar, ustawNaszAwatar] = useState("");
    const [naszPseudonim, ustawNaszPseudonim] = useState(null);

    const [wiadomoscDoWyslania, ustawWiadomoscDoWyslania] = useState("");
    const [czySieWysylaWiadomosc, ustawCzySieWysylaWiadomosc] = useState(false);

    // referencja do listy wiadomości
    const listaWiadomosciRef = useRef(null);
    // śledzenie poprzedniej liczby wiadomości
    const poprzedniaCzatLengthRef = useRef(0);

    // podajemy dane profilu
    useEffect(() => {

        if(!naszeId) return;

        const ac = new AbortController();
        let alive = true;

        // jeżeli nie ma danych naszego profilu, pobieramy je
        const podajDaneProfilu = async () => {

            const data = await fetchJsonAbort(`${API_BASE_URL}/Profil/${naszeId}`, ac, " profilu");

            // przerywamy działanie funkcji
            if (!alive) return;

            ustawNaszPseudonim(data.pseudonim ?? "");
            ustawNaszAwatar(data.awatar ?? "");
        };
        
        if(!naszAwatar && !naszPseudonim) podajDaneProfilu();
        
        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };
    },[naszAwatar, naszPseudonim, naszeId]);

    // co 5 sekund aktualizujemy czat
    useEffect(() => {
        const interval = setInterval(async () => {
            if(!idZnajomegoZOtwartymCzatem) return;
            if(czyTrwaLadowanieCzatu) return; // nie chcemy się wcinać gdy już się główne pobiera
            
            const data = await fetchJsonAbort(`${API_BASE_URL}/Wiadomosc/konwersacja/${idZnajomegoZOtwartymCzatem}`, null, " czatu");

            if(!data) ustawCzat([]);
            else ustawCzat(data);

            await aktualizujDateOtwarciaCzatu();

        }, 5000);

        return () => {
            clearInterval(interval)
        };
    },[czyTrwaLadowanieCzatu, idZnajomegoZOtwartymCzatem])

    // pobieramy czat
    useEffect(() => {
        const ac = new AbortController();
        if(!idZnajomegoZOtwartymCzatem) return;
        ustawCzyTrwaLadowanieCzatu(true);

        const podajCzat = async () => {
            const data = await fetchJsonAbort(`${API_BASE_URL}/Wiadomosc/konwersacja/${idZnajomegoZOtwartymCzatem}`, ac, " czatu");
            if(!data) ustawCzat([]);
            else ustawCzat(data);
            await aktualizujDateOtwarciaCzatu()
        }

        podajCzat();
        
        ustawCzyTrwaLadowanieCzatu(false);
        return () => {
            ac.abort(); // przerywamy fetch
        };
    },[idZnajomegoZOtwartymCzatem]);

    // automatyczne przewijanie do dołu tylko przy dodaniu nowej wiadomości
    useEffect(() => {
        // przewijamy tylko jeśli przybyła nowa wiadomość (liczba się zwiększyła)
        if (listaWiadomosciRef.current && czat.length > poprzedniaCzatLengthRef.current) {
            // przewijamy do dołu. timeout gwarantuje, że przewijanie odbywa się po renderowaniu DOM
            setTimeout(() => {
                listaWiadomosciRef.current.scrollTop = listaWiadomosciRef.current.scrollHeight;
            }, 0);
        }
        // aktualizujemy referencję poprzedniej liczby wiadomości
        poprzedniaCzatLengthRef.current = czat.length;
    }, [czat]);

    const aktualizujDateOtwarciaCzatu = async () => {
        if (!idZnajomegoZOtwartymCzatem) return null;

        const ac = new AbortController();

        try {
            const opcje = {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include",
                signal: ac.signal,
            };

            const res = await fetch(`${API_BASE_URL}/Znajomi/${idZnajomegoZOtwartymCzatem}`, opcje);

            // Bezpieczne czytanie body: backend może zwrócić pustą odpowiedź (np. 204)
            const raw = await res.text();
            let body = null;
            if (raw) {
                try {
                    body = JSON.parse(raw);
                } catch {
                    body = raw; // fallback, gdy to nie jest JSON
                }
            }

            if (!res.ok) {
                const message =
                    (body && typeof body === "object" && body.message) ||
                    (typeof body === "string" && body) ||
                    "Wystąpił błąd podczas aktualizacji daty otwarcia czatu";

                toast.error(message, {
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

            return body; // może być null przy pustym body i to jest OK
        } catch (err) {
            if (err && err.name === "AbortError") return null;
            console.error("Błąd aktualizacji:", err);
            toast.error("Wystąpił błąd podczas aktualizacji daty otwarcia czatu", {
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

    const przyWysylaniuWiadomosci = async () => {
        if(!idZnajomegoZOtwartymCzatem) return;
        if(!wiadomoscDoWyslania) return;
        if(wiadomoscDoWyslania.trim() === "") return;
        if(wiadomoscDoWyslania.length > 1000) return;
        if(czySieWysylaWiadomosc) return;

        const ac = new AbortController();
        ustawCzySieWysylaWiadomosc(true);
        try{
            const opcje = {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include",
                signal: ac.signal,
                body: JSON.stringify({
                    tresc: wiadomoscDoWyslania,
                    idTypuWiadomosci: 1,
                })
            }
            const res = await fetch(`${API_BASE_URL}/Wiadomosc/${idZnajomegoZOtwartymCzatem}`, opcje);
            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");
            if (!res.ok) {
                toast.error(body || 'Wystąpił błąd podczas wysyłania wiadomości', {
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
            ustawWiadomoscDoWyslania("");
            // odświeżamy czat po wysłaniu wiadomości
            const podajCzat = async () => {
                const data = await fetchJsonAbort(`${API_BASE_URL}/Wiadomosc/konwersacja/${idZnajomegoZOtwartymCzatem}`, ac, " czatu");
                if(!data) ustawCzat([]);
                else ustawCzat(data);
                await aktualizujDateOtwarciaCzatu()
            }
            podajCzat(ac);
        }catch (err) {
            console.error('Błąd wysyłania wiadomości:', err);
            toast.error('Wystąpił błąd podczas wysyłania wiadomości. Spróbuj ponownie później.', {
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
        }finally {
            ustawCzySieWysylaWiadomosc(false);
        }
    }


    // pomocnicza funkcja do pobierania danych z API
    const fetchJsonAbort = async (url, ac, coPobieramy) => {

        // nie chcę się męczyć z abortcontroller w interwale
        let init = {method: 'GET', credentials: "include"};
        
        if(ac) init = {method: 'GET', signal: ac.signal, credentials: "include"};
        
        try {
            const res = await fetch(url, init);
            if (!res.ok) {
                const body = await res.json();
                toast.error(body.message || 'Wystąpił błąd podczas pobierania '+coPobieramy, {
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
            toast.error('Wystąpił błąd podczas pobierania '+coPobieramy, {
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

    if(czyTrwaLadowanieCzatu) return (
        <div className="col-span-2 flex flex-col w-full">
            <h1 className="border-b-2 p-2 m-4">
                Czat
            </h1>
            <div className="flex flex-col items-center justify-center h-full">
                <p>Ładowanie...</p>
            </div>
        </div>
    )

    return(<>
    <div className="grid grid-rows-6 flex-col overflow-y-auto w-full h-[859px] border-5">
        <div className="row-span-5 min-h-0 flex flex-col">
            <h2 className="border-b-2 p-2 bg-gray-100 item">
                Czat
            </h2>
            {/* lista wiadomości */}
            <ul ref={listaWiadomosciRef} className="overflow-y-auto flex flex-col gap-4 p-2">
                {
                    // jeśli czat jest pusty
                    czat.length === 0 ? (
                            <div className="flex flex-col items-center justify-center h-full mt-10 text-gray-700 text-4xl">
                                <p>Brak wiadomości. Zacznij konwersację!</p>
                            </div>
                        ) :

                        czat.map((wiadomosc)=>(
                            <WiadomoscNaLiscieKomponent
                                wiadomosc={wiadomosc}
                                awatarNadawcy={wiadomosc.idNadawcy === idZnajomegoZOtwartymCzatem
                                    ? awatarZnajomegoZOtwartymCzatem
                                    : naszAwatar
                                }
                                pseudonimNadawcy={wiadomosc.idNadawcy === idZnajomegoZOtwartymCzatem
                                    ? pseudonimZnajomegoZOtwartymCzatem
                                    : naszPseudonim
                                }
                            />
                        ))
                }
            </ul>
        </div>
        {/* pole do wysyłania */}
        <div className="row-span-1 border-t-4 border-gray-800 p-4 bg-gray-300 flex flex-col items-center justify-center">
            <div className="flex flex-row items-center justify-center gap-2">
                <textarea className="w-[1100px] h-20 overflow-y-auto rounded-lg p-1 px-2" maxLength={1000} value={wiadomoscDoWyslania} onChange={(e)=>ustawWiadomoscDoWyslania(e.target.value)}/>
                <button
                    style={{ width: '64px', height: '64px', minWidth: '64px', minHeight: '64px', borderRadius: '50%' }}
                    className={czySieWysylaWiadomosc || wiadomoscDoWyslania.trim() === ""
                    ? "bg-gray-200 flex items-center justify-center border-2 border-gray-800 shrink-0 cursor-not-allowed"
                    :"bg-blue-200 flex items-center justify-center border-2 border-gray-800 hover:bg-blue-300 transition-colors shrink-0"}
                    onClick={przyWysylaniuWiadomosci}
                    disabled={czySieWysylaWiadomosc}
                >
                    <img
                        src="/img/send.svg"
                        alt="Wyślij"
                        className="w-8 h-8"
                    />
                </button>
            </div>
        </div>
    </div>
    {/* ma własny kontener, bo nie chce mi się łączyć z tamtym */}
    <ToastContainer
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
</>)
}