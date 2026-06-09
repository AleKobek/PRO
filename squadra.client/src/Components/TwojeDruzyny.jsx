import {Bounce, toast, ToastContainer} from "react-toastify";
import React, {useEffect, useState} from "react";
import {useLocation, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import TabelkaDruzynKomponent from "./TabelkaDruzynKomponent";
import {API_BASE_URL} from "../config/api";

export default function TwojeDruzyny() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const location = useLocation();

    const [idDruzyn, ustawIdDruzyn] = useState([]);
    const [pierwszaStronaDruzyn, ustawDruzynyNaStronie] = useState([])

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    useEffect(() => {
        if (location.state?.pomyslnieStworzonoDruzyne) {
            toast.success('Pomyślnie utworzono drużynę!', {
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
            
        if (location.state?.pomyslnieOpuszczonoDruzyne) {
            toast.success('Pomyślnie opuszczono drużynę!', {
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

    },[location.state?.pomyslnieOpuszczonoDruzyne, location.state?.pomyslnieStworzonoDruzyne, location.state?.pomyslnieUsunietoDruzyne])


    // pobieramy tabelkę drużyn
    useEffect(() => {

        if(!uzytkownik) return;

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
            const dane = await fetchJsonAbort(`${API_BASE_URL}/Druzyna/twoje`);
            if (!alive) return;

            ustawIdDruzyn(dane.idDruzyn);
            ustawDruzynyNaStronie(dane.pierwszaStronaDruzyn);
        };

        podajTwojeDruzyny();

        return () => {
            alive = false;
            ac.abort();
        };
    }, [uzytkownik]);

    if(ladowanie || !uzytkownik) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Twoje drużyny</h1>
            <div className="flex justify-center gap-6">
                <button
                    className="bg-blue-600 text-white text-2xl p-2 hover:bg-blue-500 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                    onClick={() => navigate('/wyszukajDruzyne')}
                >Wyszukaj nową</button>
                <button
                    className="bg-green-600 text-white text-2xl p-2 hover:bg-green-500 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                    onClick={() => navigate('/stworzDruzyne')}
                >Stwórz drużynę
                </button>
            </div>
            <div className="mt-10 text-2xl">
                <TabelkaDruzynKomponent
                    idDruzyn={idDruzyn}
                    brakDruzynWiadomosc={"Nie należysz do żadnej drużyny. Czas to zmienić! Razem raźniej!"}
                    czySzczegolyWNowejKarcie = {false}
                    pierwszaStronaDruzyn={pierwszaStronaDruzyn}
                />
            </div>
        </div>
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