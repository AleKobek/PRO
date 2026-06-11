import {useEffect, useMemo, useState} from "react";
import {CLIENT_URL} from "../config/api";

/*
        powiadomienia przyjdą w formacie:
        [
          {
            "id": 0,
            "idTypuPowiadomienia": 0,
            "uzytkownikId": 0,
            "idPowiazanegoObiektu": 0,
            "nazwaPowiazanegoObiektu": "string",
            "idDrugiegoPowiazanegoObiektu": 0,
            "nazwaDrugiegoPowiazanegoObiektu": "string",
            "tresc": "string",
            "dataWyslania": "2026-02-07T17:12:02.518Z"
          }
        ]

*/

export default function Powiadomienie({powiadomienie, przyRozpatrzaniuPowiadomienia}) {
    
    const [trescPowiadomieniaCz1, ustawTrescPowiadomieniaCz1] = useState("");
    const [trescPowiadomieniaCz2, ustawTrescPowiadomieniaCz2] = useState("");
    const [trescPowiadomieniaCz3, ustawTrescPowiadomieniaCz3] = useState("");
    const [typPowiadomienia, ustawTypPowiadomienia] = useState("");


    const TypyPowiadomien = useMemo(()=>Object.freeze({
        SYSTEMOWE: 1,
        ZAPROSZENIE_DO_ZNAJOMYCH: 2,
        ZAAKCEPTOWANIE_ZAPROSZENIA_DO_ZNAJOMYCH: 3,
        ODRZUCENIE_ZAPROSZENIA_DO_ZNAJOMYCH: 4,
        USUNIETO_CIE_ZE_ZNAJOMYCH: 5,
        UZYTKOWNIK_DOLACZYL_DO_DRUZYNY: 6
    }),[]);

    // ustawiamy treść powiadomienia na podstawie typu powiadomienia
    useEffect(() => {

        if (!powiadomienie) return;

        switch (powiadomienie.idTypuPowiadomienia){
            case TypyPowiadomien.SYSTEMOWE:{
                ustawTypPowiadomienia("Powiadomienie systemowe")
                ustawTrescPowiadomieniaCz1(powiadomienie.tresc);
                break;
            }
            case TypyPowiadomien.ZAPROSZENIE_DO_ZNAJOMYCH:{
                ustawTypPowiadomienia("Zaproszenie do znajomych")
                ustawTrescPowiadomieniaCz1("Uzytkownik ");
                ustawTrescPowiadomieniaCz2(" wysłał Ci zaproszenie do znajomych.");
                break;
            }
            case TypyPowiadomien.ZAAKCEPTOWANIE_ZAPROSZENIA_DO_ZNAJOMYCH:{
                ustawTypPowiadomienia("Zaakceptowanie zaproszenia do znajomych")
                ustawTrescPowiadomieniaCz1("Uzytkownik ");
                ustawTrescPowiadomieniaCz2(" zaakceptował Twoje zaproszenie do znajomych.");
                break;
            }
            case TypyPowiadomien.ODRZUCENIE_ZAPROSZENIA_DO_ZNAJOMYCH:{
                ustawTypPowiadomienia("Odrzucenie zaproszenia do znajomych")
                ustawTrescPowiadomieniaCz1("Uzytkownik ");
                ustawTrescPowiadomieniaCz2(" odrzucił Twoje zaproszenie do znajomych.");
                break;
            }
            case TypyPowiadomien.USUNIETO_CIE_ZE_ZNAJOMYCH:{
                ustawTypPowiadomienia("Usunięto cię ze znajomych")
                ustawTrescPowiadomieniaCz1("Uzytkownik ");
                ustawTrescPowiadomieniaCz2(" usunął cię ze znajomych.");
                break;
            }
            case TypyPowiadomien.UZYTKOWNIK_DOLACZYL_DO_DRUZYNY:{
                ustawTypPowiadomienia("Użytkownik dołączył do drużyny")
                ustawTrescPowiadomieniaCz1("Uzytkownik ");
                ustawTrescPowiadomieniaCz2(" dołączył do Twojej drużyny: ");
                if(powiadomienie.tresc?.length > 0){
                    ustawTrescPowiadomieniaCz3(" jako: " + powiadomienie.tresc + "."); // tu będzie rola
                } else {
                    ustawTrescPowiadomieniaCz3(".");
                }
                break;
            }
            default:{
                ustawTypPowiadomienia("Nieznany typ powiadomienia");
            }
        }

    }, [TypyPowiadomien.SYSTEMOWE, TypyPowiadomien.ZAPROSZENIE_DO_ZNAJOMYCH, TypyPowiadomien.ZAAKCEPTOWANIE_ZAPROSZENIA_DO_ZNAJOMYCH, TypyPowiadomien.ODRZUCENIE_ZAPROSZENIA_DO_ZNAJOMYCH, TypyPowiadomien.USUNIETO_CIE_ZE_ZNAJOMYCH, powiadomienie.idTypuPowiadomienia, powiadomienie.nazwaPowiazanegoObiektu, powiadomienie.tresc, powiadomienie, TypyPowiadomien.UZYTKOWNIK_DOLACZYL_DO_DRUZYNY]);


    if(!powiadomienie) return (<></>);

    if(
        powiadomienie.idTypuPowiadomienia === TypyPowiadomien.SYSTEMOWE
    ) return (
        <li key={powiadomienie.id} className="p-2 border-b border-gray-200">
        <div className="flex flex-row justify-between items-center w-full">
            <div className="font-semibold">{typPowiadomienia}</div>
            <button onClick={() => przyRozpatrzaniuPowiadomienia(powiadomienie.id, null)}>
                <img
                src = "/img/x.svg"
                alt = "x"
                className="w-4 h-4 cursor-pointer"
            />
            </button>
        </div>
        <div className="text-sm text-gray-600">{trescPowiadomieniaCz1}</div>
        <div className="text-xs text-gray-400 mt-1.5">{powiadomienie.dataWyslania}</div>
    </li>);

    if(powiadomienie.idTypuPowiadomienia === TypyPowiadomien.ZAPROSZENIE_DO_ZNAJOMYCH)
        return (<li key={powiadomienie.id} className="p-2 border-b border-gray-200">
            <div className="font-semibold mb-1">{typPowiadomienia}</div>
            <div className="text-sm text-gray-600">
                {trescPowiadomieniaCz1}
                <a href={`${CLIENT_URL}/profil/`+powiadomienie.idPowiazanegoObiektu} className="text-black font-semibold">{powiadomienie.nazwaPowiazanegoObiektu}</a>
                {trescPowiadomieniaCz2}
                {powiadomienie.idDrugiegoPowiazanegoObiektu &&
                    <a href={`${CLIENT_URL}/profil/`+powiadomienie.idDrugiegoPowiazanegoObiektu} className="text-black font-semibold">{powiadomienie.nazwaDrugiegoPowiazanegoObiektu}</a>
                }
                {trescPowiadomieniaCz3}
            </div>
            <div className="text-xs text-gray-400 mt-1">{powiadomienie.dataWyslania}</div>
            {/* przycisk zaakceptuj i odrzuć */}
            <div className="flex justify-center items-center mt-2">
                <button
                    className="text-white bg-green-900 rounded-lg px-3 py-2 mx-2 hover:bg-green-600"
                    onClick={() => przyRozpatrzaniuPowiadomienia(powiadomienie.id, true)}>Zaakceptuj</button>
                <button
                    className="text-white bg-red-900 rounded-lg px-3 py-2 mx-2 hover:bg-red-600"
                    onClick={() => przyRozpatrzaniuPowiadomienia(powiadomienie.id, false)}>Odrzuć</button>
            </div>
        </li>);


    return (<li key={powiadomienie.id} className="p-2 border-b border-gray-200">
        <div className="flex flex-row justify-between items-center w-full">
            <div className="font-semibold">{typPowiadomienia}</div>
            <button onClick={() => przyRozpatrzaniuPowiadomienia(powiadomienie.id, null)}>
                <img
                    src = "/img/x.svg"
                    alt = "x"
                    className="w-4 h-4 cursor-pointer"
                />
            </button>
        </div>
        <div className="text-sm text-gray-600">
            {trescPowiadomieniaCz1}
            <a href={`${CLIENT_URL}/profil/`+powiadomienie.idPowiazanegoObiektu} className="text-black font-semibold">{powiadomienie.nazwaPowiazanegoObiektu}</a>
            {trescPowiadomieniaCz2}
            {powiadomienie.idDrugiegoPowiazanegoObiektu &&
                <a href={`${CLIENT_URL}/profil/`+powiadomienie.idDrugiegoPowiazanegoObiektu} className="text-black font-semibold">{powiadomienie.nazwaDrugiegoPowiazanegoObiektu}</a>
            }
            {trescPowiadomieniaCz3}
        </div>
        <div className="text-xs text-gray-400 mt-1.5">{powiadomienie.dataWyslania}</div>
    </li>)
}


