import '../App.css';

import React, {useEffect, useRef} from 'react';
import {useLocation, useNavigate, useParams} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {Bounce, toast, ToastContainer} from "react-toastify";
import {API_BASE_URL, CLIENT_URL} from "../config/api";
import MiniAwatarKomponent from "./MiniAwatarKomponent";
export default function StronaSzczegolowDruzyny() {

    const navigate = useNavigate();
    const location = useLocation();
    const { uzytkownik, ladowanie } = useAuth();
    const { idDruzyny } = useParams();
    const [daneDruzyny, ustawDaneDruzyny] = React.useState(null);
    const [czyZablokowanoDostep, ustawCzyZablokowanoDostep] = React.useState(false);
    const toastShownRef = useRef(false);
    const [czyUsunietoDruzyne, ustawCzyUsunietoDruzyne] = React.useState(false);
    const [nieZnalezionoDruzyny, ustawNieZnalezionoDruzyny] = React.useState(false);


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
     "statusCzlonkostwa": "Kapitan" lub "Członek" lub "Brak"
   }

   */

    // tutaj będzie  .
    // przycisk edycji będzie chował panel szczegółów i pokazywał panel edycji .

    useEffect(() => {
        if(!daneDruzyny) document.title = `Szczegóły drużyny`;
        else document.title = `Szczegóły drużyny ${daneDruzyny.nazwa}`;
    }, [daneDruzyny]);

    useEffect(() => {
        if (location.state?.pomyslnieEdytowanoDruzyne && !toastShownRef.current) {
            // Małe opóźnienie aby upewnić się że ToastContainer jest renderowany
            const timer = setTimeout(() => {
                toast.success('Pomyślnie edytowano drużynę!', {
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
                toastShownRef.current = true;
            }, 100);

            return () => clearTimeout(timer);
        }
    },[location.state?.pomyslnieEdytowanoDruzyne])
    

    useEffect(() => {

        const ac = new AbortController();
        let alive = true;

        const pobierzStatystykiDruzyny = async (idDruzyny) => {
            if (!idDruzyny) return;
            if (!uzytkownik) return;

            // pobieramy szczegóły danej drużyny
            const fetchJsonAbort = async (url) => {
                try {
                    const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                    if (!res.ok) {
                        console.log(res)
                        if (res.status === 403) ustawCzyZablokowanoDostep(true);
                        if (res.status === 404) ustawNieZnalezionoDruzyny(true)
                        else {
                            toast.error('Wystąpił błąd podczas pobierania danych drużyny', {
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
                        return null;
                    }
                    return await res.json();
                } catch (err) {
                    if (err && err.name === 'AbortError') return null;
                    console.error('Błąd pobierania:', err);
                    toast.error('Wystąpił błąd podczas pobierania danych drużyny', {
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

            const data = await fetchJsonAbort(`${API_BASE_URL}/Druzyna/szczegoly/${idDruzyny}`);

            // przerywamy działanie funkcji
            if (!alive) return;
            if (!data) return;

            ustawDaneDruzyny(data);
        }
        
        if(!daneDruzyny) pobierzStatystykiDruzyny(idDruzyny);

        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };
    }, [daneDruzyny, idDruzyny, uzytkownik]);

    const przyKliknieciuEdycji = () => {
        if(daneDruzyny.statusCzlonkostwa !== "Kapitan") {
            toast.error('Tylko kapitan może edytować drużynę!', {
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
            toast.error('Tylko kapitan może rozwiązać drużynę!', {
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
        // tutaj wysyłamy żądanie do backendu o rozwiązanie drużyny, a potem odświeżamy listę drużyn
        const opcje = {
            method: "DELETE",
            headers: {"Content-Type": "application/json"},
            credentials: "include"
        }

        const res = await fetch(`${API_BASE_URL}/Druzyna/` + idDruzyny, opcje);
        if(!res.ok){
            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");

            toast.error(`Wystąpił błąd podczas rozwiązywania drużyny: ${body}`, {
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
        // jak tu dotarliśmy, wszystko jest git
        toast.success(`Pomyślnie usunięto drużynę!`, {
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

        ustawCzyUsunietoDruzyne(true);
    }

    const przyKliknieciuWysylaniaProsby = (idMiejsca) => {

    }

    const przyKliknieciuOpuszczaniaDruzyny = async () => {

        // tutaj wysyłamy żądanie do backendu o opuszczenie drużyny, a potem odświeżamy listę drużyn
        const opcje = {
            method: "PUT",
            headers: {"Content-Type": "application/json"},
            credentials: "include"
        }

        const res = await fetch(`${API_BASE_URL}/Druzyna/opuszczanie/` + idDruzyny, opcje);
        if(!res.ok){
            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");

            toast.error(`Wystąpił błąd podczas opuszczania drużyny: ${body}`, {
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
        // jak tu dotarliśmy, wszystko jest git
        toast.success(`Pomyślnie opuszczono drużynę!`, {
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
        navigate('/twojeDruzyny', {
            state: { pomyslnieOpuszczonoDruzyne: true }
        });
    }

    const przyKliknieciuZaproszeniaDoDruzyny = (idMiejsca) => {

    }

    const przyKliknieciuWyrzuceniaZDruzyny = async (idMiejsca) => {
        const opcje = {
            method: "PUT",
            headers: {"Content-Type": "application/json"},
            credentials: "include"
        }

        const res = await fetch(`${API_BASE_URL}/Druzyna/miejsce/${idMiejsca}`, opcje);
        if(!res.ok){
            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");

            toast.error(`Wystąpił błąd podczas wyrzucania z drużyny: ${body}`, {
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

    // obok miejsca w drużynie jest przycisk zaproszenia lub usunięcia z drużyny
    const ListaCzlonkowDlaKapitana = () =>{
        return (<div>
            <table className="w-full border-collapse border-2 border-black">
                <thead>
                <tr className="bg-gray-200">
                    <th className="border border-black px-4 py-2">Członek</th>
                    <th className="border border-black px-4 py-2">Rola</th>
                    <th className="border border-black px-4 py-2">Wymaganie</th>
                    <th className="border border-black px-4 py-2">Zarządzanie</th>
                </tr>
                </thead>
                <tbody className="divide-y divide-gray-500 text-md px-4 py-2">
                {daneDruzyny.czlonkowie.map((miejsce) => {
                    return (<tr key={miejsce.idMiejscaWDruzynie} >
                        {/* członek */}
                        {miejsce.czlonek ? <th className="flex items-center gap-2 px-4 py-2">
                            {miejsce.czyKapitan ? <img src="/img/crown.svg" alt="korona" className="w-50 h-50"/> : <div className="pl-9"/>}
                            <MiniAwatarKomponent
                                obraz={miejsce.czlonek.awatar}
                                status={miejsce.czlonek.nazwaStatusu}/>
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
                                    onClick={() => przyKliknieciuZaproszeniaDoDruzyny(miejsce.idMiejscaWDruzynie)}
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
                    <th className="border border-black px-4 py-2">Członek</th>
                    <th className="border border-black px-4 py-2">Rola</th>
                    <th className="border border-black px-4 py-2">Wymaganie</th>
                </tr>
                </thead>
                <tbody className="divide-y divide-gray-500 text-md px-4 py-2">
                {daneDruzyny.czlonkowie.map((miejsce) => {
                    return (<tr key={miejsce.idMiejscaWDruzynie} >
                        {/* członek */}
                        {miejsce.czlonek ? <th className="flex items-center gap-2 px-4 py-2">
                            {miejsce.czyKapitan ? <img src="/img/crown.svg" alt="korona" className="w-50 h-50"/> : <div className="pl-9"/>}
                            <MiniAwatarKomponent
                                obraz={miejsce.czlonek.awatar}
                                status={miejsce.czlonek.nazwaStatusu}/>
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
                    <th className="border border-black px-4 py-2">Członek</th>
                    <th className="border border-black px-4 py-2">Rola</th>
                    <th className="border border-black px-4 py-2">Wymaganie</th>
                    <th className="border border-black px-4 py-2">Zarządzanie</th>
                </tr>
                </thead>
                <tbody className="divide-y divide-gray-500 text-md px-4 py-2">
                {daneDruzyny.czlonkowie.map((miejsce) => {
                    return (<tr key={miejsce.idMiejscaWDruzynie} >
                        {/* członek */}
                        {miejsce.czlonek ? <th className="flex items-center gap-2 px-4 py-2">
                            {miejsce.czyKapitan ? <img src="/img/crown.svg" alt="korona" className="w-50 h-50"/> : <div className="pl-9"/>}
                            <MiniAwatarKomponent
                                obraz={miejsce.czlonek.awatar}
                                status={miejsce.czlonek.nazwaStatusu}/>
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
                                    onClick={przyKliknieciuWysylaniaProsby(miejsce.idMiejscaWDruzynie)}
                                    className="bg-green-700 hover:bg-green-500 text-white font-bold py-2 px-4 rounded"
                                    disabled={miejsce.czyOgladajacySpelniaWymagania !== true}
                                >Wyślij prośbę o dołączenie</button>
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
                        Nie masz dostępu do danych tej drużyny, ponieważ jest prywatna i do niej nie należysz.
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

    if(czyUsunietoDruzyne) return (<>
            <div id = "glowna">
                <h1 className="mt-40">Pomyślnie usunięto drużynę:</h1>
                <h2 className="text-xl mb-4 text-blue-700">{daneDruzyny.nazwa}</h2>
                <h3 className="flex justify-center">Pozostanie w tabelce do momentu jej ponownego załadowania</h3>
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
                <h2 className="text-2xl font-bold">Szczegóły drużyny:</h2>
                <h3 className="text-xl mb-4 text-blue-700">{daneDruzyny.nazwa}</h3>
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
                    {/* jeżeli nie jest kapitanem, ale jest i tak członkiem */}
                    {daneDruzyny.statusCzlonkostwa === "Członek" &&
                        <ListaCzlonkowDlaCzlonka/>
                    }
                    {daneDruzyny.statusCzlonkostwa === "Brak" &&
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