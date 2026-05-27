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

    const przyKliknieciuWysylaniaProsby = () => {

    }

    const przyKliknieciuOpuszczaniaDruzyny = () => {

    }

    const przyKliknieciuZaproszeniaDoDruzyny = () => {

    }

    return(<div
        ref={ref}
        className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[1000px] h-[700px] pt-2 p-10 overflow-y-auto
                rounded-md shadow-lg justify-center items-center bg-amber-50 border-2 border-amber-400"
        style={{zIndex: 2000}}
    >
        <div className="flex justify-end">
            <button onClick={() => ustawPokazPanelSzczegolow(false)} className="cursor-pointer">Zamknij</button>
        </div>
        <div className="flex flex-col">
            <h2 className="text-2xl font-bold">Szczegóły drużyny:</h2>
            <h3 className="mb-4 text-gray-700">{nazwaDruzyny}</h3>
        {/*  zwykłe dane jak opis, nastrój, platforma itp.  */}
            <div></div>
        {/*  wymagania ogólne  */}
            <div></div>
        {/*  miejsca w drużynie  */}
            <div></div>
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
                            disabled={!idWybranegoMiejsca}
                            onClick={przyKliknieciuZaproszeniaDoDruzyny}
                            className="mt-4 bg-blue-700 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded"
                        >Wyślij zaproszenie na wybrane miejsce</button> {/* przycisk zaproszenia do drużyny na zaznaczone miejsce */}
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
                {daneDruzyny.statusCzlonkostwa === "Brak" &&
                    <button
                    disabled={!idWybranegoMiejsca}
                    onClick={przyKliknieciuWysylaniaProsby}
                    className="mt-4 bg-blue-700 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded"
                    >Wyślij prośbę o dołączenie na wybrane miejsce</button>
                }
                </div>
        </div>
    </div>);
}
