import {useLocation, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import React, {useEffect, useState} from "react";
import {Bounce, toast, ToastContainer} from "react-toastify";
import TabelkaDruzynKomponent from "./TabelkaDruzynKomponent";
import {API_BASE_URL} from "../config/api";

export default function WynikiWyszukiwaniaDruzyn() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const location = useLocation();

    const [idDruzyn, ustawIdDruzyn] = useState([]);
    const [druzynyNaStronie, ustawDruzynyNaStronie] = useState([])

    const [liczbaDruzynNaStronie, ustawLiczbeDruzynNaStronie] = useState(20); // domyślne ustawienie liczby drużyn na stronie to 20
    const [aktualnaStrona, ustawAktualnaStrone] = useState(0)
    const liczbaStron = Math.max(1, Math.ceil((idDruzyn?.length ?? 0) / liczbaDruzynNaStronie));

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    useEffect(() => {
        ustawIdDruzyn(location.state.dane.idDruzyn);
        ustawDruzynyNaStronie(location.state.dane.pierwszaStronaDruzyn);
    },[location.state.dane, location.state.dane.idDruzyn, location.state.dane.pierwszaStronaDruzyn])


    useEffect(() => {
        if(!idDruzyn) return;
        if(idDruzyn.length === 0) return;
        if(aktualnaStrona < 0) return;
        if(aktualnaStrona >= liczbaStron) return;
        if(liczbaDruzynNaStronie === 0) return;
        if(!uzytkownik) return;

        const ac = new AbortController();
        let alive = true;

        const pobierzNoweDruzyny = async () => {
            const idDruzynDoPobrania = idDruzyn.slice(aktualnaStrona * liczbaDruzynNaStronie, (aktualnaStrona + 1) * liczbaDruzynNaStronie);
            const opcje = {
                method: 'POST', // nie GET, bo to nie jest klasyczne wyszukiwanie zasobu po url
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(idDruzynDoPobrania),
                credentials: "include",
                abortSignal: ac.signal,
            }

            const res = await fetch(`${API_BASE_URL}/Druzyna/tabelka`, opcje);
            const body = await res.json().catch(() => null);

            if (!res.ok) {
                toast.error(`Wystąpił błąd podczas pobierania drużyn: ${body?.message ?? res.statusText ?? "nieznany błąd"}`, {
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
            if(!alive || !body) return;
            ustawDruzynyNaStronie(body);
        }

        pobierzNoweDruzyny();

        return () => {
            alive = false;
            ac.abort();
        };
    },[aktualnaStrona, idDruzyn, liczbaDruzynNaStronie, liczbaStron, uzytkownik])

    if(ladowanie || !uzytkownik) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <button className={"przycisk-nawigacji"} onClick={() => navigate('/wyszukajDruzyne')}>Wyszukaj inne drużyny</button>
            <h1>Wyszukane drużyny</h1>
            <span className="mr-2">Liczba drużyn na stronie:</span>
            <select
                className="border border-gray-300 rounded-md p-1 mb-2"
                value={liczbaDruzynNaStronie}
                onChange={ (e) => {
                    ustawLiczbeDruzynNaStronie(parseInt(e.target.value));
                    ustawAktualnaStrone(0);
                }}>
                <option value="10">10</option>
                <option value="20">20</option>
                <option value="30">30</option>
                <option value="50">50</option>
            </select>
            <TabelkaDruzynKomponent druzyny={druzynyNaStronie} brakDruzynWiadomosc="Brak dostępnch drużyn spełniających warunki wyszukiwania" czySzczegolyWNowejKarcie={true}/>
            {/* paginacja */}
            <div className="flex justify-center mt-3"><span>Strona {aktualnaStrona + 1} z {liczbaStron}</span></div>
            <div className="flex justify-center gap-6">
                <button className={aktualnaStrona === 0 ? "zablokowany-przycisk" : "bg-blue-900 hover:bg-blue-600 text-white"} disabled={aktualnaStrona === 0}
                        onClick={ () => { ustawAktualnaStrone(aktualnaStrona - 1) } }
                >Poprzednia strona</button>

                {/* przycisk następnej strony*/}
                <button className={liczbaStron === 0 || aktualnaStrona === liczbaStron - 1 ? "zablokowany-przycisk" : "bg-blue-900 hover:bg-blue-600 text-white"} disabled= {liczbaStron === 0 || aktualnaStrona === liczbaStron - 1}
                        onClick={ () => { ustawAktualnaStrone(aktualnaStrona + 1) } }
                >Następna strona</button>
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