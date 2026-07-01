import WiadomoscNaLiscieKomponent from "./WiadomoscNaLiscieKomponent";
import React, {useEffect, useState, useRef} from "react";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast, ToastContainer} from "react-toastify";

export default function CzatDruzynowyKomponent({
                                                    idDruzyny
                                                }){

    const [czat, ustawCzat] = useState([]);
    const [czyTrwaLadowanieCzatu, ustawCzyTrwaLadowanieCzatu] = useState(true);
    const [uczestnicy, ustawUczestnikow] = useState([]);

    const [wiadomoscDoWyslania, ustawWiadomoscDoWyslania] = useState("");
    const [czySieWysylaWiadomosc, ustawCzySieWysylaWiadomosc] = useState(false);

    // referencja do listy wiadomości
    const listaWiadomosciRef = useRef(null);
    // śledzenie poprzedniej liczby wiadomości
    const poprzedniaCzatLengthRef = useRef(0);
    

    // co 5 sekund aktualizujemy czat
    useEffect(() => {
        const ac = new AbortController();

        const interval = setInterval(async () => {
            if(!idDruzyny) return;
            if(czyTrwaLadowanieCzatu) return; // nie chcemy się wcinać gdy już się główne pobiera

            podajCzat(ac);


            await aktualizujDateOtwarciaCzatu();

        }, 5000);

        return () => {
            clearInterval(interval);
            ac.abort();
        };
    },[czyTrwaLadowanieCzatu, idDruzyny])


    // pobieramy czat
    useEffect(() => {
        const ac = new AbortController();
        if(!idDruzyny) return;
        ustawCzyTrwaLadowanieCzatu(true);

        podajCzat(ac);

        ustawCzyTrwaLadowanieCzatu(false);
        return () => {
            ac.abort(); // przerywamy fetch
        };
    },[idDruzyny]);


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
        if (!idDruzyny) return null;

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

            const res = await fetch(`${API_BASE_URL}/Druzyna/czat/ostatnie-otwarcie/${idDruzyny}`, opcje);

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
        if(!idDruzyny) return;
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
                body: JSON.stringify(wiadomoscDoWyslania)
            }
            const res = await fetch(`${API_BASE_URL}/Wiadomosc/druzynowa/${idDruzyny}`, opcje);
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
            await podajCzat(ac);
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

    const podajCzat = async (ac) => {
        const data = await fetchJsonAbort(`${API_BASE_URL}/Wiadomosc/czat-druzynowy/${idDruzyny}`, ac, " czatu");
        if(data && data.wiadomosci && data.wiadomosci.length > 0) {
            ustawCzat(data.wiadomosci);
            ustawUczestnikow(data.uczestnicy);
        }
        await aktualizujDateOtwarciaCzatu()
    }

    if(czyTrwaLadowanieCzatu) return (
        <div className="col-span-2 flex flex-col w-full h-3/4 min-h-0 overflow-hidden ">
            <h2 className="border-b-2 p-2 m-4">
                Czat
            </h2>
            <div className="flex flex-col items-center justify-center flex-1 min-h-0 overflow-hidden">
                <p>Ładowanie...</p>
            </div>
        </div>
    )

    return(<>
        <div className="grid grid-rows-[minmax(0,1fr)_auto] w-full h-full min-h-0 overflow-hidden border-5">
            <div className="min-h-0 flex flex-col overflow-hidden">

                <div className="flex flex-col justify-center border-b-2 p-2 bg-gray-100 item shrink-0">
                    <h2>Czat</h2>
                    <span className="text-center text-sm">Limit wiadomości to 250. Po tym najstarsze będą usuwane.</span>
                </div>
                {/* lista wiadomości */}
                <ul ref={listaWiadomosciRef} className="min-h-0 overflow-y-auto flex flex-col gap-4 p-2">
                    {
                        // jeśli czat jest pusty
                        czat.length === 0 ? (
                                <div className="flex flex-col items-center justify-center h-full mt-10 text-gray-700 text-xl">
                                    <p>Brak wiadomości. Zacznij konwersację!</p>
                                </div>
                        ) :

                        czat.map((wiadomosc, index)=> {

                            const nadawca = uczestnicy.find(x => x.idUzytkownika === wiadomosc.idNadawcy);
                            const awatar = nadawca ? nadawca.awatar : "/img/default-avatar.png";
                            const pseudonim = nadawca ? nadawca.pseudonim : "Nieznany";
                            return (<WiadomoscNaLiscieKomponent
                                wiadomosc={wiadomosc}
                                awatarNadawcy={awatar}
                                pseudonimNadawcy={pseudonim}
                                key={wiadomosc.idNadawcy + index}
                            />);
                        })
                    }
                </ul>
            </div>
            {/* pole do wysyłania */}
            <div className="border-t-4 border-gray-800 p-4 bg-gray-300 flex flex-col items-center justify-center shrink-0">
                <div className="flex flex-row items-center justify-center gap-2 w-full">
                    <textarea className="flex-1 min-w-0 w-full h-20 overflow-y-auto rounded-lg p-1 px-2 resize-none" maxLength={1000} value={wiadomoscDoWyslania} onChange={(e)=>ustawWiadomoscDoWyslania(e.target.value)}/>
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