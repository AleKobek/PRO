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

    return(
    <div className="flex flex-col overflow-y-auto w-full max-h-[700px] border-5">
        <h2 className="border-b-2 p-2 bg-gray-100 item">
            Czat
        </h2>
        {/* lista wiadomości */}
        <ul ref={listaWiadomosciRef} className="overflow-y-auto w-full flex flex-col gap-4 p-2">
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
    </div>
)
}