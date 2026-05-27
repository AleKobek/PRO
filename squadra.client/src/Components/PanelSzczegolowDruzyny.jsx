import React, {useState} from "react";

export default function PanelSzczegolowDruzyny({
                                                   idDruzyny,
                                                   nazwaDruzyny,
                                                   daneDruzyny,
                                                   idUzytkownika,
                                                   ref,
                                                   ustawPokazPanelSzczegolow,
                                                   ustawPokazPanelEdycji}) {

    const [idWybranegoMiejsca, ustawIdWybranegoMiejsca] = useState(null);

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
          "czyKapitan": true
        },
        {
          "idMiejscaWDruzynie": 17,
          "czlonek": null,
          "rola": "support",
          "wymaganie": "Ranga(support): Gold V",
          "czyKapitan": false
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
    const przyKliknieciuRozwiaz = () => {

    }

    const przyKliknieciuWysylaniaProsby = (idMiejsca) => {

    }

    const przyKliknieciuOpuszczaniaDruzyny = () => {

    }

    const przyKliknieciuZaproszeniaDoDruzyny = (idMiejsca) => {

    }

    // obok miejsca w drużynie jest przycisk zaproszenia lub usunięcia z drużyny
    const ListaCzlonkowDlaKapitana = () =>{
        return (<div></div>)
    }

    // zwykła lista, bez przycisków
    const ListaCzlonkowDlaCzlonka = () =>{
        return (<div></div>)
    }

    // obok pustych miejsc są przyciski z wysyłaniem prośby o dołączenie
    const ListaCzlonkowDlaObcego = () =>{
        return (<div></div>)
    }

    return(<div
        ref={ref}
        className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[1000px] h-[700px] pt-2 p-10 overflow-y-auto
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
            <div className="flex flex-col gap-4 justify-center items-center text-xl">
                {/* tytuł gry */}
                <label className="pole-w-szczegolach-druzyny">
                    Gra
                    <span>{daneDruzyny.tytulGry}</span>
                </label>
                {/* platforma */}
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
                </label>
                {/* opis */}
                <label className="pole-w-szczegolach-druzyny">
                    Opis
                    <p className="text-gray-700 font-normal bg-blue-50 rounded-md p-2 w-fit">{daneDruzyny.opis === "" ? "brak" : daneDruzyny.opis}</p>
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
