import React, {useState} from "react";
import AwatarComponent from "./AwatarComponent";
import {API_BASE_URL, CLIENT_URL} from "../config/api";
import MiniAwatarKomponent from "./MiniAwatarKomponent";
import {Bounce, toast} from "react-toastify";

export default function PanelSzczegolowDruzyny({
                                                   idDruzyny,
                                                   nazwaDruzyny,
                                                   daneDruzyny,
                                                   idUzytkownika,
                                                   ref,
                                                   ustawPokazPanelSzczegolow,
                                                   ustawPokazPanelEdycji,
                                                   usunDruzyne}) {


    /*

    daneDruzyny = {
      "tytulGry": "Overwatch",
      "opis": "EEEEE",
      "nastrojRozgrywki": "Zwykły",
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
      "czy18Plus": false,
      "nazwaPlatformy": "PC",
      "logoPlatformy": (tutaj logo),
      "statusCzlonkostwa": "Kapitan" lub "Członek" lub "Brak"
    }

    */

    // tutaj będzie  .
    // przycisk edycji będzie chował panel szczegółów i pokazywał panel edycji .

    const przyKliknieciuEdycji = () => {
        ustawPokazPanelEdycji(true);
        ustawPokazPanelSzczegolow(false);
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
        usunDruzyne(idDruzyny);
        ustawPokazPanelSzczegolow(false);
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
        usunDruzyne(idDruzyny);
        ustawPokazPanelSzczegolow(false);
    }

    const przyKliknieciuZaproszeniaDoDruzyny = (idMiejsca) => {

    }

    const przyKliknieciuWyrzuceniaZDruzyny = (idMiejsca) => {

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
                                onClick={przyKliknieciuWyrzuceniaZDruzyny(miejsce.idMiejscaWDruzynie)}
                                className="bg-red-700 hover:bg-red-500 text-white font-bold py-2 px-4 rounded"
                                disabled={miejsce.czyKapitan}
                            >Wyrzuć</button>
                            : <button
                                onClick={przyKliknieciuZaproszeniaDoDruzyny(miejsce.idMiejscaWDruzynie)}
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

    if(daneDruzyny === null) {return <div></div>}

    return(<div
        ref={ref}
        className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-3/4 h-3/4 pt-2 p-10 overflow-y-auto
                rounded-md shadow-lg justify-center items-center bg-white border-2 border-amber-400"
        style={{zIndex: 2000}}
    >
        <div className="flex justify-end">
            <button onClick={() => ustawPokazPanelSzczegolow(false)} className="cursor-pointer">Zamknij</button>
        </div>
        <div className="flex flex-col justify-center items-center">
            <h2 className="text-2xl font-bold">Szczegóły drużyny:</h2>
            <h3 className="text-xl mb-4 text-blue-700">{nazwaDruzyny}</h3>
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
                {/* czy 18+ */}
                <label className="pole-w-szczegolach-druzyny">
                    Czy 18+?
                    <span>{daneDruzyny.czy18Plus ? "tak" : "nie"}</span>
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
    </div>);
}
