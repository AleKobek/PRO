import {Bounce, toast, ToastContainer} from "react-toastify";
import React, {useEffect, useState} from "react";
import {useLocation, useNavigate} from "react-router-dom";
import {useAuth} from "../../Context/AuthContext";
import TabelkaDruzyn from "../TabelkaDruzyn";
import {API_BASE_URL} from "../../config/api";
import {useWspoldzieloneFunkcje} from "../../Context/WspoldzieloneFunkcjeContext";

export default function TwojeDruzyny() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const location = useLocation();
    const {toastOptions} = useWspoldzieloneFunkcje();

    const [idDruzyn, ustawIdDruzyn] = useState([]);
    const [pierwszaStronaDruzyn, ustawDruzynyNaStronie] = useState([])
    const [ladowanieDruzyn, ustawLadowanieDruzyn] = useState(true);

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    useEffect(() => {
        if (location.state?.pomyslnieUsunietoDruzyne) {
            toast.success('Pomyślnie usunięto drużynę!', toastOptions);
        }
            
        if (location.state?.pomyslnieOpuszczonoDruzyne) {
            toast.success('Pomyślnie opuszczono drużynę!', toastOptions);
        }

    },[location.state?.pomyslnieOpuszczonoDruzyne, location.state?.pomyslnieUsunietoDruzyne, toastOptions])


    // pobieramy tabelkę drużyn
    useEffect(() => {

        if(!uzytkownik) return;

        const ac = new AbortController();
        let alive = true;

        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                if (!res.ok) {
                    if (alive) toast.error('Wystąpił błąd podczas pobierania twoich drużyn', toastOptions);
                    return null;
                }
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                if (alive) {
                    console.error('Błąd pobierania:', err);
                    toast.error('Wystąpił błąd podczas pobierania twoich drużyn', toastOptions);
                }
                return null;
            }
        };

        const podajTwojeDruzyny = async () => {
            const dane = await fetchJsonAbort(`${API_BASE_URL}/Druzyny/twoje`);
            if (!alive) return;

            ustawIdDruzyn(dane.idDruzyn);
            ustawDruzynyNaStronie(dane.pierwszaStronaDruzyn);
            if (alive) ustawLadowanieDruzyn(false);
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
                >Wyszukaj drużynę</button>
                <button
                    className="bg-green-600 text-white text-2xl p-2 hover:bg-green-500 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                    onClick={() => navigate('/stworzDruzyne')}
                >Stwórz drużynę
                </button>
            </div>
            <div className="mt-10 text-2xl">
                {ladowanieDruzyn
                    ? <h1>Ładowanie...</h1>
                    : <TabelkaDruzyn
                        idDruzyn={idDruzyn}
                        brakDruzynWiadomosc={"Nie należysz do żadnej drużyny. Czas to zmienić! Razem raźniej!"}
                        czySzczegolyWNowejKarcie={false}
                        pierwszaStronaDruzyn={pierwszaStronaDruzyn}
                        idUzytkownika={uzytkownik.id}
                    />
                }
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