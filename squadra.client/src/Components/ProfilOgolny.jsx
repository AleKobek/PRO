import '../App.css';

import React, {useEffect, useState} from 'react';
import DaneProfilu from './DaneProfilu';
import {useNavigate, useParams} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast, ToastContainer} from "react-toastify";
export default function ProfilOgolny() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const { idWlascicielaProfilu} = useParams();

    const [czyZnajomy, ustawCzyZnajomy] = useState(false);
    const [ladowanieCzyZnajomy, ustawLadowanieCzyZnajomy] = useState(true);

    const frDelRef = React.useRef(null);
    const [pokazUsunZnajomego, ustawPokazUsunZnajomego] = useState(false);
    const [czyZablokowaneUsun, ustawCzyZablokowaneUsun] = useState(true);



    useEffect(() => {
        if(uzytkownik === null) return;
        if(idWlascicielaProfilu === uzytkownik.id.toString()) navigate("/twojProfil");
    }, [idWlascicielaProfilu, navigate, uzytkownik?.id]); // z jakiegoś powodu dodanie dep użytkownik wywala stronę

    // przy załadowaniu strony pobieramy, czy dana osoba jest naszym znajomym, aby wyświetlić odpowiedni przycisk
    useEffect(() => {
        if (uzytkownik === null) return;
        if (idWlascicielaProfilu === uzytkownik.id.toString()) return;

        const ac = new AbortController();

        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: "GET", signal: ac.signal, credentials: "include" });
                if (!res.ok) {
                    toast.error("Wystąpił błąd podczas pobierania danych profilu", {
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
                if (err?.name === "AbortError") return null;
                console.error("Błąd pobierania:", err);
                toast.error("Wystąpił błąd podczas pobierania danych profilu", {
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

        const czyJestesmyZnajomymi = async () => {
            ustawLadowanieCzyZnajomy(true);
            try {
                const res = await fetchJsonAbort(`${API_BASE_URL}/Znajomi/czyZnajomosc/` + idWlascicielaProfilu);
                if (res === null) return;
                ustawCzyZnajomy(Boolean(res));
            } finally {
                if (!ac.signal.aborted) {
                    ustawLadowanieCzyZnajomy(false);
                }
            }
        };

        czyJestesmyZnajomymi();

        return () => {
            ac.abort();
        };
    }, [idWlascicielaProfilu, uzytkownik?.id]); // wyrzuca błąd, gdy dodaje się użytkownika

    // timer odliczający 5 sekund po otworzeniu panelu usunięcia znajomego
    useEffect(() => {
        if(!pokazUsunZnajomego) return;
        if(!czyZablokowaneUsun) return;
        
        // jeżeli panel jest pokazany, po pięciu sekundach odblokowujemy przycisk usuwania
        const timer = setTimeout(() => {
            ustawCzyZablokowaneUsun(false);
        }, 5000);

        return () => clearTimeout(timer);

    },[czyZablokowaneUsun, pokazUsunZnajomego])

    const przyWysylaniuZaproszenia = async () => {
        const opcje = {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            credentials: "include",
        }

        const res = await fetch(`${API_BASE_URL}/Powiadomienie/zaproszenie/znajomi/`+idWlascicielaProfilu, opcje);
        if(!res.ok) {
            const contentType = res.headers.get("content-type");
            let wiadomosc = "Nie udało się wysłać zaproszenia do znajomych.";

            if (contentType && contentType.includes("application/json")) {
                const body = await res.json().catch(() => ({}));
                wiadomosc = body.message || body.error || wiadomosc;
            } else {
                wiadomosc = await res.text();
            }

            console.error(wiadomosc);
            toast.error(wiadomosc, {
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

        toast.success('Zaproszenie zostało wysłane!', {
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
    }

    // na razie puste
    const przyUsuwaniu = async () => {

    }

    const PrzyciskPodProfilem = () => {
        if(uzytkownik === null) return;

        if(ladowanieCzyZnajomy) return null;
        if(!czyZnajomy) return (<button
            className="block !mx-auto bg-green-900 !text-[25px] text-white rounded-md !px-3 !py-1 !my-4 hover:bg-green-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
            onClick={() =>przyWysylaniuZaproszenia()}>
            Wyślij zaproszenie do znajomych
        </button>)
        // tu będzie przycisk pokazujący nakładkę "usuń znajomego"
        return (<button
            className="block !mx-auto bg-red-900 !text-[25px] text-white rounded-md !px-3 !py-1 !my-4 hover:bg-red-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
            onClick={() => {
                ustawCzyZablokowaneUsun(true);
                ustawPokazUsunZnajomego(v => !v)
            }}
        >
            Usuń znajomego
        </button>)
    }

    const PanelUsunZnajomego = () => (
        <div
            ref={frDelRef}
            className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] p-10 overflow-y-auto bg-red-200 border-2 border-red-900
            rounded-md shadow-lg justify-center items-center"
            style={{ zIndex: 2000 }}
        >
            <div className="flex flex-col">

                <div className="flex flex-col items-center gap-2">
                    <span className="text-4xl text-center font-bold"> Czy na pewno chcesz usunąć tę osobę z listy znajomych? Wasza historia czatu zostanie utracona na zawsze!<br/></span>
                    <span>Za 5 sekund przycisk się odblokuje</span>
                </div>
                <div className="flex justify-center items-center gap-8 mt-7 text-xl font-semibold">
                    {/* przycisk anulowania */}
                    <button
                        onClick={() => {
                            ustawCzyZablokowaneUsun(true);
                            ustawPokazUsunZnajomego(false)
                        }}
                        className="bg-green-900 text-white rounded-md px-6 py-3.5 hover:bg-green-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105">
                        Anuluj
                    </button>
                    {/* przycisk potwierdzenia */}
                    <button
                        className={czyZablokowaneUsun ?
                            "text-black bg-gray-300 rounded-md px-4 py-3 border border-black shadow-md cursor-not-allowed" :
                            "bg-red-900 text-white rounded-md px-5 py-3.5 hover:bg-red-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"}
                        disabled={czyZablokowaneUsun}
                        onClick={przyUsuwaniu}
                    >
                        Potwierdź
                    </button>
                </div>
            </div>
        </div>
    );


    if(ladowanie) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Profil użytkownika</h1>
            <DaneProfilu idUzytkownika={parseInt(idWlascicielaProfilu)}></DaneProfilu>
            <PrzyciskPodProfilem/>
        </div>
        {pokazUsunZnajomego && <PanelUsunZnajomego/>}
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
    </>);
}