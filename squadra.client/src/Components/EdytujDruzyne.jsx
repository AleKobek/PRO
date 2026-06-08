import '../App.css';

import React, {useEffect, useMemo, useState} from 'react';
import {useLocation, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {Bounce, toast, ToastContainer} from "react-toastify";
import {API_BASE_URL} from "../config/api";
export default function EdytujDruzyne() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const location = useLocation();
    const [stareDane, ustawStareDane] = useState({})
    const [nowaNazwa, ustawNazwe] = useState("")
    const [bladNazwy, ustawBladNazwy] = useState("")
    const [nowyOpis, ustawOpis] = useState("")
    const [bladOpisu, ustawBladOpisu] = useState("")
    const [idNowegoNastrojuRozgrywki, ustawIdNastrojuRozgrywki] = useState("")
    const [noweCzyPubliczna, ustawCzyPubliczna] = useState(true)

    const [nastrojeRozgrywki, ustawNastrojeRozgrywki] = useState(null)

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    useEffect(() => {
        if (location.state?.daneDoPrzekazania) {
            ustawStareDane(location.state.daneDoPrzekazania)
            ustawNazwe(location.state.daneDoPrzekazania.nazwa)
            ustawOpis(location.state.daneDoPrzekazania.opis)
            ustawIdNastrojuRozgrywki(location.state.daneDoPrzekazania.idNastrojuRozgrywki)
            ustawCzyPubliczna(location.state.daneDoPrzekazania.czyPubliczna)
        }
    },[location.state.daneDoPrzekazania])

    /*
    Przekazane dane:
    {
        idDruzyny: idDruzyny,
        nazwa: daneDruzyny.nazwa,
        opis: daneDruzyny.opis,
        idNastrojuRozgrywki: daneDruzyny.idNastrojuRozgrywki,
        czyPubliczna: daneDruzyny.czyPubliczna
    }
    */

    useEffect(() =>{
        const ac = new AbortController();
        let alive = true;
        if(!uzytkownik) return;

        // taka pomocnicza funkcja dla abort controller
        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                if (!res.ok) return null;
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                toast.error('Wystąpił błąd podczas pobierania danych do formularza', {
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

        const podajNastroje = async () => {

            const dane = await fetchJsonAbort(`${API_BASE_URL}/Druzyna/nastroje-rozgrywki`);
            if (!alive || !dane) return;

            ustawNastrojeRozgrywki(dane);
        };

        if(!nastrojeRozgrywki) podajNastroje();

        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            ac.abort();
            alive = false; // przerywamy fetch
        }
    },[nastrojeRozgrywki, uzytkownik])

    /*
    Dane muszą mieć format:
    {
        string Nazwa,
        bool CzyPubliczna,
        string? Opis,
        int IdNastrojuRozgrywki
    }
    */
    const przyWysylaniu = async () => {
        if(!uzytkownik) return;
        ustawBladNazwy("")
        ustawBladOpisu("")

        const druzynaDoWyslania = {
            nazwa: nowaNazwa,
            opis: nowyOpis,
            idNastrojuRozgrywki: idNowegoNastrojuRozgrywki,
            czyPubliczna: noweCzyPubliczna
        }

        const opcje = {
            method: "PUT",
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: "include",
            body: JSON.stringify(druzynaDoWyslania)
        }

        const res = await fetch(`${API_BASE_URL}/Druzyna/${stareDane.idDruzyny}`, opcje);

        const ct = res.headers.get("content-type") || "";
        const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
            ? await res.json().catch(() => null)
            : await res.text().catch(() => "");

        if (!res.ok) {
            if(res.status === 400){
                let bledy = body.errors;
                ustawBladNazwy(bledy.Nazwa ? bledy.Nazwa[0] : "");
                ustawBladOpisu(bledy.Opis ? bledy.Opis[0] : "");
            }
            toast.error('Wystąpił błąd podczas edycji drużyny', {
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

        // jak tutaj dojdziemy, wszystko jest git
        navigate(`/druzyna/${stareDane.idDruzyny}`, {
            state: { pomyslnieEdytowanoDruzyne: true }
        });
    }
    
    const czyZablokowane = useMemo(() => {
        return (
            nowaNazwa.trim().length === 0 || nowaNazwa.trim().length > 40 || nowyOpis?.trim().length > 300 ||
            (
                stareDane.nazwa === nowaNazwa.trim()
                && stareDane.opis === nowyOpis?.trim()
                && stareDane.idNastrojuRozgrywki === idNowegoNastrojuRozgrywki
                && stareDane.czyPubliczna === noweCzyPubliczna
            )
        )
    },[idNowegoNastrojuRozgrywki, nowaNazwa, noweCzyPubliczna, nowyOpis, stareDane.czyPubliczna, stareDane.idNastrojuRozgrywki, stareDane.nazwa, stareDane.opis])

    if(ladowanie || !uzytkownik || !stareDane || !nastrojeRozgrywki) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <div className="flex justify-start">
                <button
                    className="przycisk-nawigacji"
                    onClick={() => navigate(`/druzyna/${stareDane.idDruzyny}`)}
                >Powrót do szczegółów drużyny</button>
            </div>
            <h1>Edytuj drużynę:</h1>
            <h2 className="text-amber-700">{stareDane.nazwa}</h2>
            <div className="flex flex-col items-center justify-center gap-5 mt-5">
                <div className="flex flex-col items-center justify-center">
                    <span>
                        Nazwa</span><br/>
                    <input
                        type="text"
                        value={nowaNazwa}
                        className="border-2 border-gray-300 rounded-md p-2 w-[600px]"
                        onChange={(e) => ustawNazwe(e.target.value)}
                    />
                    <span className="error-wiadomosc">{bladNazwy}</span>
                </div>
                <div className="flex flex-col items-center justify-center">
                    <span>Opis:</span><br/>
                    <textarea
                        value={nowyOpis}
                        onChange={(e) => ustawOpis(e.target.value)}
                        className="border-2 border-gray-300 rounded-md p-2 w-[600px]"
                        maxLength={300}
                    />
                <span className="error-wiadomosc">{bladOpisu}</span>
                </div>
                <label>
                    Nastrój rozgrywki: <br/>
                    <select
                        className="border-2 border-gray-300 rounded-md p-2"
                        onChange={(e) => ustawIdNastrojuRozgrywki(parseInt(e.target.value))}
                        value={idNowegoNastrojuRozgrywki}
                    >
                        {
                            nastrojeRozgrywki.map((nastroj) =>
                                <option value={nastroj.id} key={nastroj.id}>{nastroj.nazwa}</option>
                            )
                        }
                    </select>
                </label>
                <label>
                    Czy publiczna:
                    <input
                        type="checkbox"
                        className="mx-2"
                        checked={noweCzyPubliczna}
                        onChange={() => ustawCzyPubliczna(!noweCzyPubliczna)}
                    />
                </label>
                <button
                    className={ czyZablokowane
                        ? "zablokowany-przycisk"
                        : "p-2 bg-green-900 text-white rounded-md px-3 py-1 mt-10 hover:bg-green-600"}
                    disabled={czyZablokowane}
                    onClick={przyWysylaniu}>
                    Zapisz
                </button>
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