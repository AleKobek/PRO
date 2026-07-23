import '../App.css';

import React, {useEffect, useRef, useState} from 'react';
import {useLocation, useNavigate, useParams} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {Bounce, toast, ToastContainer} from "react-toastify";
import {API_BASE_URL, CLIENT_URL} from "../config/api";
import {OkienkoTlumaczaceZintegrowanie} from "./OkienkoTlumaczaceZintegrowanie";
import AwatarComponent from "./AwatarComponent";
import CzatDruzynowyKomponent from "./CzatDruzynowyKomponent";

const TOAST_CONTAINER_ID = "szczegoly-druzyny-toast";
export default function StronaSzczegolowDruzyny({ustawCzySaNoweWiadomosciDruzynowe, powiadomienia, ustawPowiadomienia}) {

    const navigate = useNavigate();
    const location = useLocation();
    const { uzytkownik, ladowanie } = useAuth();
    const { idDruzyny } = useParams();
    const [daneDruzyny, ustawDaneDruzyny] = React.useState(null);
    const [czyZablokowanoDostep, ustawCzyZablokowanoDostep] = React.useState(false);
    const toastShownRef = useRef(false);
    const [nieZnalezionoDruzyny, ustawNieZnalezionoDruzyny] = React.useState(false);
    const [pokazOkienkoTlumaczenia, ustawPokazOkienkoTlumaczenia] = useState(false);
    const [pokazPanelZapraszania, ustawPokazPanelZapraszania] = useState(false);
    const ref = React.useRef(null);

    const [loginZapraszanego, ustawLoginZapraszanego] = useState(null);
    const [idMiejscaDoZaproszenia, ustawIdMiejscaDoZaproszenia] = useState(null);
    const [listaZnajomych, ustawListeZnajomych] = useState(null);
    const [czySieWysylaZaproszenie, ustawCzySieWysylaZaproszenie] = useState(false);

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

    useEffect(() => {
        if (location.state?.pomyslnieStworzonoDruzyne && !toastShownRef.current) {
            // Małe opóźnienie aby upewnić się że ToastContainer jest renderowany
            const timer = setTimeout(() => {
                toast.success('Pomyślnie utworzono drużynę!', toastOptions);
                toastShownRef.current = true;
            }, 100);

            return () => clearTimeout(timer);
        }
    },[location.state?.pomyslnieStworzonoDruzyne])

    /*

   daneDruzyny = {
     "nazwa": "EEEE"
     "tytulGry": "Overwatch",
     "opis": "EEEEE",
     "nastrojRozgrywki": "Zwykły",
     "idNastrojuRozgrywki": 1,
     "czlonkowie": [
       {
         "idMiejscaWDruzynie": 16,
         "czlonek": {
           "idUzytkownika": 1,
           "pseudonim": "Leczo",
           "awatar": tutaj awatar,
           "nazwaStatusu": "Dostępny"
         },
         "rola": "support",
         "wymaganie": null,
         "czyKapitan": true,
         "czyOgladajacySpelniaWymagania": null
       },
       {
         "idMiejscaWDruzynie": 17,
         "czlonek": null,
         "rola": "support",
         "wymaganie": "Ranga(support): Gold V",
         "czyKapitan": false,
         "czyOgladajacySpelniaWymagania": true
       }
     ],
     "wymaganyJezykIStopienBiegłosci": "polski - Zaawansowany",
     "wymaganiaDoWypisania": [
       {
         "idStatystyki": 1,
         "nazwaStatystyki": "Czas rozgrywki (w godzinach)",
         "wartoscStatystyki": "40"
       }
     ],
     "czyPubliczna": true,
     "nazwaPlatformy": "PC",
     "logoPlatformy": (tutaj logo),
     "statusCzlonkostwa": "Kapitan" lub "Członek" lub "Brak",
     "czyZintegrowano": true
   }

   */


    useEffect(() => {
        if(!daneDruzyny) document.title = `Szczegóły drużyny`;
        else document.title = `Szczegóły drużyny ${daneDruzyny.nazwa}`;
    }, [daneDruzyny]);

    useEffect(() => {

        const ac = new AbortController();
        let alive = true;

        const sprawdzCzySaNoweWiadomosci = async (signal) => {
            try {
                const res = await fetch(`${API_BASE_URL}/Wiadomosci/nowe/druzyny`, {credentials: "include", signal});
                if(!res.ok) return;
                const czyNowe = await res.json();
                if(alive) ustawCzySaNoweWiadomosciDruzynowe(czyNowe);
            } catch (err) {
                if (err && err.name === 'AbortError') return;
                console.error(err);
            }
        }
        if(daneDruzyny) sprawdzCzySaNoweWiadomosci(ac.signal);

        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };
    }, [daneDruzyny, ustawCzySaNoweWiadomosciDruzynowe]);

    useEffect(() => {
        if (location.state?.pomyslnieEdytowanoDruzyne && !toastShownRef.current) {
            // Małe opóźnienie aby upewnić się że ToastContainer jest renderowany
            const timer = setTimeout(() => {
                toast.success('Pomyślnie edytowano drużynę!', toastOptions);
                toastShownRef.current = true;
            }, 100);

            return () => clearTimeout(timer);
        }
    },[location.state?.pomyslnieEdytowanoDruzyne])


    useEffect(() => {

        const ac = new AbortController();
        let alive = true;

        const aktualizujDateOtwarciaCzatu = async () => {
            // przerywamy działanie funkcji
            if (!alive) return;

            const opcje = {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include",
            };

            const res = await fetch(`${API_BASE_URL}/Druzyny/czat/ostatnie-otwarcie/${idDruzyny}`, opcje);

            // Odczyt body różni się zależnie od typu odpowiedzi
            // jeżeli to 404, to zwraca tylko tekst (nie application/json), więc res.json rzuci wyjątek. musimy to uwzlgędnić
            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");

            if (!res.ok) {
                toast.error(`${body.message ?? body.errors[0].message ?? "Wystąpił błąd podczas aktualizowania daty ostatniego otwarcia czatu"}`, toastOptions);
            }
        }

        const pobierzStatystykiDruzyny = async (idDruzyny) => {
            if (!idDruzyny) return;
            if (!uzytkownik) return;

            // pobieramy szczegóły danej drużyny
            const fetchJsonAbort = async (url) => {
                try {
                    const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                    if (!res.ok) {
                        if (res.status === 403) ustawCzyZablokowanoDostep(true);
                        if (res.status === 404) ustawNieZnalezionoDruzyny(true)
                        else {
                            toast.error('Wystąpił błąd podczas pobierania danych drużyny', toastOptions);
                        }
                        return null;
                    }
                    return await res.json();
                } catch (err) {
                    if (err && err.name === 'AbortError') return null;
                    console.error('Błąd pobierania:', err);
                    toast.error('Wystąpił błąd podczas pobierania danych drużyny', toastOptions);
                    return null;
                }
            };

            const data = await fetchJsonAbort(`${API_BASE_URL}/Druzyny/szczegoly/${idDruzyny}`);

            // przerywamy działanie funkcji
            if (!alive) return;
            if (!data) return;

            ustawDaneDruzyny(data);
            if(data.statusCzlonkostwa === "Kapitan" || data.statusCzlonkostwa === "Członek") aktualizujDateOtwarciaCzatu();
        }
        
        if(!daneDruzyny) pobierzStatystykiDruzyny(idDruzyny);

        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };
    }, [daneDruzyny, idDruzyny, uzytkownik]);

    const przyKliknieciuEdycji = () => {
        if(daneDruzyny.statusCzlonkostwa !== "Kapitan") {
            toast.error('Tylko kapitan może edytować drużynę!', toastOptions);
            return;
        }
        const daneDoPrzekazania = {
            idDruzyny: idDruzyny,
            nazwa: daneDruzyny.nazwa,
            opis: daneDruzyny.opis,
            idNastrojuRozgrywki: daneDruzyny.idNastrojuRozgrywki,
            czyPubliczna: daneDruzyny.czyPubliczna
        };
        // tutaj pokazujemy panel edycji, a panel szczegółów chowamy
         navigate('/edytujDruzyne', {state: {daneDoPrzekazania}});
    }
    
    const przyKliknieciuRozwiaz = async () => {
        if(daneDruzyny.statusCzlonkostwa !== "Kapitan") {
            toast.error('Tylko kapitan może rozwiązać drużynę!', toastOptions);
            return;
        }
        // tutaj wysyłamy żądanie do backendu o rozwiązanie drużyny, a potem odświeżamy listę drużyn
        const opcje = {
            method: "DELETE",
            headers: {"Content-Type": "application/json"},
            credentials: "include"
        }

        const res = await fetch(`${API_BASE_URL}/Druzyny/` + idDruzyny, opcje);
        if(!res.ok){
            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");

            toast.error(`Wystąpił błąd podczas rozwiązywania drużyny: ${body}`, toastOptions);
            return;
        }
        // jak tu dotarliśmy, wszystko jest git
        toast.success(`Pomyślnie usunięto drużynę!`, toastOptions);

        navigate('/twojeDruzyny', {
            state: { pomyslnieUsunietoDruzyne: true }
        });
    }

    const przyKliknieciuDolaczania = (idMiejsca) => {
        if(!uzytkownik) return;
        // sprawdzamy, czy już jest członkiem i czy miejsce jest puste .
        if(daneDruzyny.statusCzlonkostwa !== "Brak"){
            toast.error(`Już jesteś członkiem tej drużyny`, toastOptions);
            return;
        }
        let miejsce = daneDruzyny.czlonkowie.find((miejsce) => miejsce.idMiejscaWDruzynie === idMiejsca);
        if(!miejsce){
            toast.error(`Nie znaleziono miejsca w drużynie`, toastOptions);
            return;
        }
        if(miejsce.czlonek){
            toast.error(`To miejsce jest już zajęte`, toastOptions);
            return;
        }
        if(miejsce.czyOgladajacySpelniaWymagania !== true){
            toast.error(`Nie spełniasz wymagań tego miejsca`, toastOptions);
            return;
        }

        // po udanym dołączeniu pobieramy nowe dane drużyny
        const pobierzStatystykiDruzyny = async (idDruzyny) => {
            if (!idDruzyny) return;
            if (!uzytkownik) return;

            // pobieramy szczegóły danej drużyny
            const fetchJsonAbort = async (url) => {
                try {
                    const res = await fetch(url, { method: 'GET', credentials: "include" });
                    if (!res.ok) {
                        if (res.status === 403) ustawCzyZablokowanoDostep(true);
                        if (res.status === 404) ustawNieZnalezionoDruzyny(true)
                        else {
                            toast.error('Wystąpił błąd podczas pobierania danych drużyny', toastOptions);
                        }
                        return null;
                    }
                    return await res.json();
                } catch (err) {
                    if (err && err.name === 'AbortError') return null;
                    console.error('Błąd pobierania:', err);
                    toast.error('Wystąpił błąd podczas pobierania danych drużyny', toastOptions);
                    return null;
                }
            };

            const data = await fetchJsonAbort(`${API_BASE_URL}/Druzyny/szczegoly/${idDruzyny}`);

            ustawDaneDruzyny(data);
        }

        const dolaczDoDruzyny = async () =>{
            // tutaj wysyłamy żądanie do backendu o dołączenie do drużyny, a potem odświeżamy dane drużyny
            const opcje = {
                method: "PUT",
                headers: {"Content-Type": "application/json"},
                credentials: "include"
            }
            const res = await fetch(`${API_BASE_URL}/Druzyny/miejsce/dolacz/` + idMiejsca, opcje);
            if(!res.ok){
                const ct = res.headers.get("content-type") || "";
                const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                    ? await res.json().catch(() => null)
                    : await res.text().catch(() => "");

                toast.error(`Wystąpił błąd podczas dołączania do drużyny: ${body}`, toastOptions);
                return;
            }
            // jak tu doszliśmy, udało się dołączyć. pobieramy nowe dane druzyny
            pobierzStatystykiDruzyny(idDruzyny);

            toast.success(`Pomyślnie dołączono do drużyny!`, toastOptions);
            let tempPowiadomienia = powiadomienia.filter(powiadomienie => powiadomienie.idPowiazanegoObiektu === idDruzyny && powiadomienie.idTypuPowiadomienia === 8)
            ustawPowiadomienia(tempPowiadomienia);
        }

        dolaczDoDruzyny();
    }

    const przyKliknieciuOpuszczaniaDruzyny = async () => {

        // tutaj wysyłamy żądanie do backendu o opuszczenie drużyny, a potem odświeżamy listę drużyn
        const opcje = {
            method: "PUT",
            headers: {"Content-Type": "application/json"},
            credentials: "include"
        }

        const res = await fetch(`${API_BASE_URL}/Druzyny/opuszczanie/` + idDruzyny, opcje);
        if(!res.ok){
            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");

            toast.error(`Wystąpił błąd podczas opuszczania drużyny: ${body}`, toastOptions);
        }
        // jak tu dotarliśmy, wszystko jest git
        toast.success(`Pomyślnie opuszczono drużynę!`, toastOptions);
        navigate('/twojeDruzyny', {
            state: { pomyslnieOpuszczonoDruzyne: true }
        });
    }

    const przyKliknieciuZaproszeniaDoDruzynyPoId = async (idMiejsca, idUzytkownika) => {
        if(!idMiejsca) return;
        if(!idUzytkownika) return;
        if(czySieWysylaZaproszenie) return;
        ustawCzySieWysylaZaproszenie(true);

        try {
            const res = await fetch(`${API_BASE_URL}/Druzyny/miejsce/zapros/${idMiejsca}/${idUzytkownika}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: "include"
            });

            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");
            if (!res.ok) {
                toast.error(body.message ?? (body.errors && body.errors[0].message) ?? body ?? `Wystąpił błąd podczas wysyłania zaproszenia`, toastOptions);
                return;
            }
            toast.success("Pomyślnie wysłano zaproszenie!", toastOptions);
        } catch (err) {
            console.error('Błąd wysyłania zaproszenia:', err);
            toast.error('Wystąpił błąd podczas wysyłania zaproszenia. Spróbuj ponownie później.', toastOptions);
        }finally {
            ustawCzySieWysylaZaproszenie(false);
            ustawIdMiejscaDoZaproszenia(null);
            ustawPokazPanelZapraszania(false);
        }

    }
    const przyKliknieciuZaproszeniaDoDruzynyPoLoginie = async (idMiejsca, login) => {
        if(!idMiejsca) return;
        if(!login || login.trim().length === 0) return;
        if(czySieWysylaZaproszenie) return;
        ustawCzySieWysylaZaproszenie(true);

        try {
            const res = await fetch(`${API_BASE_URL}/Druzyny/miejsce/zapros/${idMiejsca}/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: "include",
                body: JSON.stringify(login.trim())
            });

            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");
            if (!res.ok) {
                toast.error(body.message ?? (body.errors && body.errors[0].message) ?? body ?? `Wystąpił błąd podczas wysyłania zaproszenia`, toastOptions);
                return;
            }
            toast.success("Pomyślnie wysłano zaproszenie!", toastOptions);
        } catch (err) {
            console.error('Błąd wysyłania zaproszenia:', err);
            toast.error('Wystąpił błąd podczas wysyłania zaproszenia. Spróbuj ponownie później.', toastOptions);
        }finally {
            ustawCzySieWysylaZaproszenie(false);
            ustawIdMiejscaDoZaproszenia(null);
            ustawLoginZapraszanego(null);
            ustawPokazPanelZapraszania(false);
        }
    }

    const przyKliknieciuWyrzuceniaZDruzyny = async (idMiejsca) => {
        const opcje = {
            method: "PUT",
            headers: {"Content-Type": "application/json"},
            credentials: "include"
        }

        const res = await fetch(`${API_BASE_URL}/Druzyny/miejsce/${idMiejsca}`, opcje);
        if(!res.ok){
            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");

            toast.error(`Wystąpił błąd podczas wyrzucania z drużyny: ${body}`, toastOptions);
        }
        // jak tu dotarliśmy, wszystko jest git.
        const tempDaneDruzyny = {
            ...daneDruzyny,
            czlonkowie: daneDruzyny.czlonkowie.map(miejsce =>
                miejsce.idMiejscaWDruzynie === idMiejsca
                    ? { ...miejsce, czlonek: null }
                    : miejsce
            )
        };
        ustawDaneDruzyny(tempDaneDruzyny);
    }


    const pobierzZnajomychDoListy = async (idMiejsca) => {
        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', credentials: "include" });
                if (!res.ok) {
                    const ct = res.headers.get("content-type") || "";
                    const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                        ? await res.json().catch(() => null)
                        : await res.text().catch(() => "");
                    toast.error(`${body.message ?? body.errors[0].message ?? "Wystąpił błąd podczas pobierania listy znajomych"}`, toastOptions);
                    return null;
                }
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                toast.error('Wystąpił błąd podczas pobierania listy znajomych', toastOptions);
                return null;
            }
        };

        const data = await fetchJsonAbort(`${API_BASE_URL}/Druzyny/znajomi-spelniajacy-warunki-miejsca/${idMiejsca}`);
        ustawListeZnajomych(data);
    }

    const PanelZapraszania = () => {
        if(!pokazPanelZapraszania) return null;
        if(!idMiejscaDoZaproszenia) return null;

        return (<div
            ref={ref}
            className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 h-[800px] pt-2 p-10 overflow-y-auto
                rounded-md shadow-lg justify-center items-center bg-amber-50 border-2 border-amber-400"
            style={{zIndex: 2001}}
        >
            <div className="flex justify-end">
                <button onClick={() => {
                    ustawListeZnajomych(null)
                    ustawPokazPanelZapraszania(false)
                }}
                        className="cursor-pointer text-red-600">Zamknij
                </button>
            </div>
            <div className="flex flex-col justify-center items-center">
                <span className="text-center text-4xl font-bold mt-5 mb-1">Zaproś do drużyny</span>
                <span className="text-center">Dana osoba może mieć jedno zaproszenie na drużynę i na jedno miejsce może istnieć tylko jedno zaproszenie. Nowe zastępują stare.</span>
                <h2 className="text-2xl font-bold my-4">Dostępni znajomi do zaproszenia</h2>
                {!listaZnajomych && <div className="text-center text-2xl">
                    Ładowanie listy znajomych...
                </div>}
                {listaZnajomych != null && listaZnajomych.length === 0 && <div className="text-center">
                    Brak znajomych dostępnych do zaproszenia
                </div>}
                {listaZnajomych != null && listaZnajomych.length > 0 && <ul className="h-[520px] overflow-y-auto border-2 border-gray-300 rounded-md p-2">
                {/*
                    Dane przyjdą w formie:
                    {
                    "idUzytkownika": 1,
                    "pseudonim": "Leczo",
                    "Awatar": tutaj awatar,
                    "nazwaStatusu": "Dostepny",
                    }
                */}
                    {listaZnajomych.map((znajomy) => (
                        <li key={znajomy.idUzytkownika} className="flex flex-row items-center text-3xl gap-3 p-2">
                            <AwatarComponent
                                obraz={znajomy.awatar ?? ""}
                                wysokosc={60}
                                pseudonim={znajomy.pseudonim}
                                status={znajomy.nazwaStatusu}
                            />
                            <a href={`${CLIENT_URL}/profil/`+ znajomy.idUzytkownika}>{znajomy.pseudonim}</a>
                            <button
                                onClick={() => przyKliknieciuZaproszeniaDoDruzynyPoId(idMiejscaDoZaproszenia, znajomy.idUzytkownika)}
                                className="bg-green-700 hover:bg-green-500 text-white font-bold text-lg py-1 px-3 rounded"
                            >Zaproś</button>
                        </li>
                    ))}
                </ul>}
                <h2 className="text-center text-xl font-bold mt-5 mb-1">Zaproś użytkownika po loginie</h2>
                <input
                    type="text"
                    value={loginZapraszanego}
                    className="border-2 border-gray-300 rounded-md p-2 mb-2 w-3/4"
                    onChange={(e) => ustawLoginZapraszanego(e.target.value)}
                    autoFocus={true}
                    maxLength={64}
                />
                <button
                    onClick={() => przyKliknieciuZaproszeniaDoDruzynyPoLoginie(idMiejscaDoZaproszenia, loginZapraszanego)}
                    className={!loginZapraszanego || loginZapraszanego.trim().length ===0 ? "zablokowany-przycisk" : "bg-green-700 hover:bg-green-500 text-white font-bold py-2 px-4 rounded"}
                    disabled={!loginZapraszanego || loginZapraszanego.trim().length ===0}
                >Zaproś po loginie</button>
            </div>
        </div>)
    }

    // obok miejsca w drużynie jest przycisk zaproszenia lub usunięcia z drużyny
    const ListaCzlonkowDlaKapitana = () =>{
        return (<div>
            <table className="w-full border-collapse border-2 border-black">
                <thead>
                <tr className="bg-gray-200">
                    <th className="border border-black px-4 py-2">Numer</th>
                    <th className="border border-black px-4 py-2">Członek</th>
                    <th className="border border-black px-4 py-2">Rola</th>
                    <th className="border border-black px-4 py-2">Wymaganie</th>
                    <th className="border border-black px-4 py-2">Zarządzanie</th>
                </tr>
                </thead>
                <tbody className="divide-y divide-gray-500 text-md px-4 py-2">
                {daneDruzyny.czlonkowie.map((miejsce) => {
                    return (<tr key={miejsce.idMiejscaWDruzynie} >
                        {/* numer miejsca */}
                        <th className="px-4 py-2 border border-gray-500">{miejsce.numerMiejsca}</th>
                        {/* członek */}
                        {miejsce.czlonek ? <th className="flex items-center gap-2 px-4 py-2">
                            {miejsce.czyKapitan ? <img src="/img/crown.svg" alt="korona" className="w-50 h-50"/> : <div className="pl-9"/>}
                            <AwatarComponent
                                obraz={miejsce.czlonek.awatar}
                                status={miejsce.czlonek.nazwaStatusu}
                                wysokosc={40}
                            />
                            <a
                                className="text-sm hover:underline"
                                href={`${CLIENT_URL}/profil/` + miejsce.czlonek.idUzytkownika}>{miejsce.czlonek.pseudonim}</a>
                        </th> : <th className="px-4 py-2 border border-gray-500 text-gray-700">Puste</th>}
                        {/* rola */}
                        <th className="px-4 py-2 border border-gray-500">{miejsce.rola ?? "-"}</th>
                        <th className="px-4 py-2 border border-gray-500">{miejsce.wymaganie ?? "-"}</th>
                        <th className="px-4 py-2 border border-gray-500">
                            {miejsce.czlonek
                                ? <button
                                    onClick={() => przyKliknieciuWyrzuceniaZDruzyny(miejsce.idMiejscaWDruzynie)}
                                    className="bg-red-700 hover:bg-red-500 text-white font-bold py-2 px-4 rounded"
                                    disabled={miejsce.czyKapitan}
                                >Wyrzuć</button>
                                : <button
                                    onClick={() => {
                                        ustawIdMiejscaDoZaproszenia(miejsce.idMiejscaWDruzynie)
                                        pobierzZnajomychDoListy(miejsce.idMiejscaWDruzynie);
                                        ustawPokazPanelZapraszania(true)
                                    }}
                                    className="bg-green-700 hover:bg-green-500 text-white font-bold py-2 px-4 rounded"
                                >Zaproś</button>
                            }</th>
                    </tr>)
                })}
                </tbody>
            </table>
        </div>)
    }

    // zwykła lista, bez przycisków
    const ListaCzlonkowDlaCzlonka = () =>{
        return (<div>
            <table className="w-full border-collapse border-2 border-black">
                <thead>
                <tr className="bg-gray-200">
                    <th className="border border-black px-4 py-2">Numer</th>
                    <th className="border border-black px-4 py-2">Członek</th>
                    <th className="border border-black px-4 py-2">Rola</th>
                    <th className="border border-black px-4 py-2">Wymaganie</th>
                </tr>
                </thead>
                <tbody className="divide-y divide-gray-500 text-md px-4 py-2">
                {daneDruzyny.czlonkowie.map((miejsce) => {
                    return (<tr key={miejsce.idMiejscaWDruzynie} >
                        {/* numer miejsca */}
                        <th className="px-4 py-2 border border-gray-500">{miejsce.numerMiejsca}</th>
                        {/* członek */}
                        {miejsce.czlonek ? <th className="flex items-center gap-2 px-4 py-2">
                            {miejsce.czyKapitan ? <img src="/img/crown.svg" alt="korona" className="w-50 h-50"/> : <div className="pl-9"/>}
                            <AwatarComponent
                                obraz={miejsce.czlonek.awatar}
                                status={miejsce.czlonek.nazwaStatusu}
                                wysokosc={40}
                            />
                            <a
                                className="text-sm hover:underline"
                                href={`${CLIENT_URL}/profil/` + miejsce.czlonek.idUzytkownika}>{miejsce.czlonek.pseudonim}</a>
                        </th> : <th className="px-4 py-2 border border-gray-500 text-gray-700">Puste</th>}
                        {/* rola */}
                        <th className="px-4 py-2 border border-gray-500">{miejsce.rola ?? "-"}</th>
                        <th className="px-4 py-2 border border-gray-500">{miejsce.wymaganie ?? "-"}</th>
                    </tr>)
                })}
                </tbody>
            </table>
        </div>)
    }

    // obok pustych miejsc są przyciski z wysyłaniem prośby o dołączenie
    const ListaCzlonkowDlaObcego = () =>{
        return (<div>
            <table className="w-full border-collapse border-2 border-black">
                <thead>
                <tr className="bg-gray-200">
                    <th className="border border-black px-4 py-2">Numer</th>
                    <th className="border border-black px-4 py-2">Członek</th>
                    <th className="border border-black px-4 py-2">Rola</th>
                    <th className="border border-black px-4 py-2">Wymaganie</th>
                    <th className="border border-black px-4 py-2">Zarządzanie</th>
                </tr>
                </thead>
                <tbody className="divide-y divide-gray-500 text-md px-4 py-2">
                {daneDruzyny.czlonkowie.map((miejsce) => {
                    return (<tr key={miejsce.idMiejscaWDruzynie} >
                        {/* numer miejsca */}
                        <th className="px-4 py-2 border border-gray-500">{miejsce.numerMiejsca}</th>
                        {/* członek */}
                        {miejsce.czlonek ? <th className="flex items-center gap-2 px-4 py-2">
                            {miejsce.czyKapitan ? <img src="/img/crown.svg" alt="korona" className="w-50 h-50"/> : <div className="pl-9"/>}
                            <AwatarComponent
                                obraz={miejsce.czlonek.awatar}
                                status={miejsce.czlonek.nazwaStatusu}
                                wysokosc={40}
                            />
                            <a
                                className="text-sm hover:underline"
                                href={`${CLIENT_URL}/profil/` + miejsce.czlonek.idUzytkownika}>{miejsce.czlonek.pseudonim}</a>
                        </th> : <th className="px-4 py-2 border border-gray-500 text-gray-700">Puste</th>}
                        {/* rola */}
                        <th className="px-4 py-2 border border-gray-500">{miejsce.rola ?? "-"}</th>
                        <th className="px-4 py-2 border border-gray-500">{miejsce.wymaganie ?? "-"}</th>
                        {miejsce.czlonek
                            ? <th className="px-4 py-2 border border-gray-500 text-gray-700">Zajęte</th>
                            : <th className="px-4 py-2 border border-gray-500">
                                <button
                                    onClick={() => przyKliknieciuDolaczania(miejsce.idMiejscaWDruzynie)}
                                    className="bg-green-700 hover:bg-green-500 text-white font-bold py-2 px-4 rounded"
                                    disabled={miejsce.czyOgladajacySpelniaWymagania !== true}
                                >Dołącz</button>
                            </th>
                        }
                    </tr>)
                })}
                </tbody>
            </table>
        </div>)
    }


    if(czyZablokowanoDostep) return (<>
            <div id = "glowna">
                <h1 className="text-red-700 mt-40">Blokada dostępu.</h1>
                <div className="flex justify-center">
                    <span className="text-center items-center text-2xl">
                        Nie masz dostępu do danych tej drużyny, ponieważ jest prywatna i ani do niej nie należysz, ani nie masz zaproszenia.
                    </span>
                </div>
            </div>
        </>
    )

    if(ladowanie || !uzytkownik) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    if(nieZnalezionoDruzyny) return (<>
            <div id = "glowna">
                <h1 className="mt-40 text-red-600">Błąd 404</h1>
                <h2>Nie znaleziono w bazie drużyny o podanym id.</h2>
                <h3>Nie istnieje lub została usunięta.</h3>
            </div>
        </>
    )

    if(!daneDruzyny) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <div className="flex flex-col justify-center items-center">
                {daneDruzyny.statusCzlonkostwa !== "Brak" &&
                    <button className={"przycisk-nawigacji"} onClick={() => {navigate('/twojeDruzyny')}}>
                        Powrót do twoich drużyn
                    </button>
                }
                <h1 className="text-2xl font-bold">Strona drużyny:</h1>
                <h2 className="text-xl mb-4 text-blue-700">{daneDruzyny.nazwa}</h2>
                {/* czat drużynowy */}
                {daneDruzyny.statusCzlonkostwa !== "Brak" &&
                    <div className="w-3/4 h-[1000px] border-4 border-gray-600 mb-10 shadow-xl rounded-md"><CzatDruzynowyKomponent idDruzyny={idDruzyny}/></div>
                }
                {/*  zwykłe dane jak opis, nastrój, platforma itp.  */}
                <div className="flex flex-col gap-4 justify-center items-center text-xl border-2 border-gray-600 shadow-md rounded-md p-4 px-7 mb-6">
                    {/* tytuł gry */}
                    <label className="pole-w-szczegolach-druzyny">
                        Gra
                        <span>{daneDruzyny.tytulGry}</span>
                    </label>
                    {/* platforma */}
                    {daneDruzyny.nazwaPlatformy?.length > 0 &&
                        <label className="pole-w-szczegolach-druzyny">
                            Platforma
                            <div className="flex items-center gap-2">
                                <img
                                    src={"data:image/jpeg;base64," + daneDruzyny.logoPlatformy}
                                    alt="logo"
                                    className="h-20 w-20 my-3 rounded-full border-4 border-black"
                                />
                                <span className="font-bold">{daneDruzyny.nazwaPlatformy}</span>
                            </div>
                        </label>}
                    {/* opis */}
                    <label className="pole-w-szczegolach-druzyny">
                        Opis
                        <p className="text-gray-700 font-normal bg-blue-50 rounded-md p-2 max-w-[900px] text-wrap whitespace-normal break-words">{daneDruzyny.opis === "" ? "brak" : daneDruzyny.opis}</p>
                    </label>
                    {/* nastrój rozgrywki */}
                    <label className="pole-w-szczegolach-druzyny">
                        Nastrój rozgrywki
                        <span>{daneDruzyny.nastrojRozgrywki}</span>
                    </label>
                    {/* wymagany język */}
                    <label className="pole-w-szczegolach-druzyny">
                        Wymagany język i stopień biegłości
                        <span>{daneDruzyny.wymaganyJezykIStopienBieglosci ?? "brak"}</span>
                    </label>
                    {/* czy publiczna */}
                    <label className="pole-w-szczegolach-druzyny">
                        Publiczność
                        <span>{daneDruzyny.czyPubliczna ? "Publiczna" : "Prywatna"}</span>
                    </label>
                    {/* czy zintegrowano */}
                    <label className="pole-w-szczegolach-druzyny"> <div className="flex items-center gap-1">
                        Czy używa zintegrowanych danych:
                        <img
                            src="/img/znak-zapytania.svg"
                            alt="znak zapytania"
                            className="h-[1em] w-auto align-middle ml-2 cursor-pointer"
                            onClick={() => ustawPokazOkienkoTlumaczenia(true)}
                        /></div>
                        <span>{daneDruzyny.czyZintegrowano ? "Tak" : "Nie"}</span>
                    </label>
                </div>
                {/*  wymagania ogólne  */}
                { daneDruzyny.wymaganiaDoWypisania?.length > 0 &&
                    <div className="flex flex-col gap-4 justify-center items-center text-xl my-4">
                        <h3>Minimalne wymagane statystyki w drużynie</h3>
                        <ul className="list-disc list-inside">
                            {daneDruzyny.wymaganiaDoWypisania.map((wymaganie) => (
                                <li
                                    className="text-gray-700 font-normal bg-blue-50 rounded-md p-2 w-fit"
                                    key={wymaganie.idStatystyki}>
                                    {wymaganie.nazwaStatystyki}: {wymaganie.wartoscStatystyki}
                                </li>
                            ))}
                        </ul>
                    </div>
                }
                {/*  miejsca w drużynie  */}
                <div>
                    <h3>Miejsca w drużynie</h3>
                    {
                        daneDruzyny.statusCzlonkostwa === "Kapitan" &&
                        <ListaCzlonkowDlaKapitana/>
                    }
                    {/* jeżeli nie jest kapitanem, ale jest i tak członkiem lub ma zaproszenie do prywatnej drużyny*/}
                    {(daneDruzyny.statusCzlonkostwa === "Członek"|| (daneDruzyny.statusCzlonkostwa === "Brak" && !daneDruzyny.czyPubliczna)) &&
                        <ListaCzlonkowDlaCzlonka/>
                    }
                    {(daneDruzyny.statusCzlonkostwa === "Brak" && daneDruzyny.czyPubliczna) &&
                        <ListaCzlonkowDlaObcego/>
                    }
                </div>
                {/*  przyciski na dole  */}
                <div className="flex justify-center mt-4">
                    {
                        daneDruzyny.statusCzlonkostwa === "Kapitan" &&
                        <div className="flex justify-center gap-4 mt-4">
                            <button
                                onClick={przyKliknieciuEdycji}
                                className="mt-4 bg-amber-700 hover:bg-amber-500 text-white font-bold py-2 px-4 rounded"
                            >Edytuj</button> {/* przycisk edycji drużyny */}
                            <button
                                onClick={przyKliknieciuRozwiaz}
                                className="mt-4 bg-red-700 hover:bg-red-500 text-white font-bold py-2 px-4 rounded"
                            >Rozwiąż</button> {/* przycisk usunięcia drużyny */}
                        </div>
                    }
                    {/* jeżeli nie jest kapitanem, ale jest i tak członkiem */}
                    {daneDruzyny.statusCzlonkostwa === "Członek" &&
                        <button
                            onClick={przyKliknieciuOpuszczaniaDruzyny}
                            className="mt-4 bg-red-700 hover:bg-red-500 text-white font-bold py-2 px-4 rounded"
                        >Opuść drużynę</button>
                    }
                </div>
            </div>
        </div>
        {pokazOkienkoTlumaczenia && OkienkoTlumaczaceZintegrowanie(ref, ustawPokazOkienkoTlumaczenia)}
        {pokazPanelZapraszania && <PanelZapraszania/>}
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