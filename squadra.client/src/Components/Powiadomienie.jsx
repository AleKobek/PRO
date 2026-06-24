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
        UZYTKOWNIK_DOLACZYL_DO_DRUZYNY: 6,
        USUNIETO_CIE_Z_DRUZYNY: 7,
        ZAPROSZENIE_DO_DRUZYNY: 8,
        UZYTKOWNIK_PRZYJAL_ZAPROSZENIE_DO_DRUZYNY: 9,
        UZYTKOWNIK_ODRZUCIL_ZAPROSZENIE_DO_DRUZYNY: 10,
        UZYTKOWNIK_OPUSCIL_DRUZYNE: 13,
        DRUZYNA_ZOSTALA_USUNIETA: 14,
        UZYTKOWNIK_OPUSCIL_DRUZYNE_BO_USUNAL_KONTO: 15,
        DRUZYNA_ZOSTALA_USUNIETA_AUTOAMTYCZNIE: 16
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
                    ustawTrescPowiadomieniaCz3(" jako " + powiadomienie.tresc + "."); // tu będzie rola
                } else {
                    ustawTrescPowiadomieniaCz3(".");
                }
                break;
            }
            case TypyPowiadomien.USUNIETO_CIE_Z_DRUZYNY:{
                ustawTypPowiadomienia("Usunięto cię z drużyny");
                ustawTrescPowiadomieniaCz1("Usunięto cię z drużyny: ");
                break;
            }
            case TypyPowiadomien.ZAPROSZENIE_DO_DRUZYNY:{
                ustawTypPowiadomienia("Zaproszenie do drużyny");
                ustawTrescPowiadomieniaCz1("Masz zaproszenie do drużyny: ");
                break;
            }
            case TypyPowiadomien.UZYTKOWNIK_PRZYJAL_ZAPROSZENIE_DO_DRUZYNY:{
                ustawTypPowiadomienia("Użytkownik przyjął zaproszenie do drużyny")
                ustawTrescPowiadomieniaCz1("Uzytkownik ");
                ustawTrescPowiadomieniaCz2(" przyjął zaproszenie do Twojej drużyny: ");
                if(powiadomienie.tresc?.length > 0){
                    ustawTrescPowiadomieniaCz3(" na rolę " + powiadomienie.tresc + "."); // tu będzie rola
                } else {
                    ustawTrescPowiadomieniaCz3(".");
                }
                break;
            }
            case TypyPowiadomien.UZYTKOWNIK_ODRZUCIL_ZAPROSZENIE_DO_DRUZYNY:{
                ustawTypPowiadomienia("Użytkownik odrzucił zaproszenie do drużyny")
                ustawTrescPowiadomieniaCz1("Uzytkownik ");
                ustawTrescPowiadomieniaCz2(" odrzucił zaproszenie do Twojej drużyny: ");
                if(powiadomienie.tresc?.length > 0){
                    ustawTrescPowiadomieniaCz3(" na rolę " + powiadomienie.tresc + "."); // tu będzie rola
                } else {
                    ustawTrescPowiadomieniaCz3(".");
                }
                break;
            }
            case TypyPowiadomien.UZYTKOWNIK_OPUSCIL_DRUZYNE:{
                ustawTypPowiadomienia("Użytkownik opuścił drużynę")
                ustawTrescPowiadomieniaCz1("Uzytkownik ");
                if(powiadomienie.tresc?.length > 0){
                    ustawTrescPowiadomieniaCz2("("+ powiadomienie.tresc +") opuścił Twoją drużynę: ");
                }else {
                    ustawTrescPowiadomieniaCz2(" opuścił Twoją drużynę: ");
                }
                break;
            }
            case TypyPowiadomien.DRUZYNA_ZOSTALA_USUNIETA:{
                ustawTypPowiadomienia("Drużyna została usunięta")
                ustawTrescPowiadomieniaCz1("Drużyna ");
                ustawTrescPowiadomieniaCz2(", do której należałeś, została usunięta.");
                break;
            }
            case TypyPowiadomien.UZYTKOWNIK_OPUSCIL_DRUZYNE_BO_USUNAL_KONTO:{
                ustawTypPowiadomienia("Użytkownik opuścił drużynę")
                ustawTrescPowiadomieniaCz1("Członek Twojej drużyny, która nosi nazwę ");
                if(powiadomienie.tresc?.length > 0){
                    ustawTrescPowiadomieniaCz2(", usunął konto i ją opuścił. Miał rolę " + powiadomienie.tresc + "."); // tu będzie rola
                } else {
                    ustawTrescPowiadomieniaCz2("usunął konto i ją opuścił.");
                }
                break;
            }
            case TypyPowiadomien.DRUZYNA_ZOSTALA_USUNIETA_AUTOAMTYCZNIE:{
                ustawTypPowiadomienia("Twoja drużyna została usunięta")
                ustawTrescPowiadomieniaCz1("Twoja drużyna ");
                ustawTrescPowiadomieniaCz2(" została usunięta przez Twój brak aktywności.");
                break;
            }
            default:{
                ustawTypPowiadomienia("Nieznany typ powiadomienia");
            }
        }

    }, [powiadomienie.idTypuPowiadomienia, powiadomienie.nazwaPowiazanegoObiektu, powiadomienie.tresc, powiadomienie,
        TypyPowiadomien.SYSTEMOWE, TypyPowiadomien.ZAPROSZENIE_DO_ZNAJOMYCH, TypyPowiadomien.ZAAKCEPTOWANIE_ZAPROSZENIA_DO_ZNAJOMYCH,
        TypyPowiadomien.ODRZUCENIE_ZAPROSZENIA_DO_ZNAJOMYCH, TypyPowiadomien.USUNIETO_CIE_ZE_ZNAJOMYCH,
        TypyPowiadomien.UZYTKOWNIK_DOLACZYL_DO_DRUZYNY, TypyPowiadomien.USUNIETO_CIE_Z_DRUZYNY, TypyPowiadomien.ZAPROSZENIE_DO_DRUZYNY,
        TypyPowiadomien.UZYTKOWNIK_OPUSCIL_DRUZYNE, TypyPowiadomien.UZYTKOWNIK_PRZYJAL_ZAPROSZENIE_DO_DRUZYNY,
        TypyPowiadomien.UZYTKOWNIK_ODRZUCIL_ZAPROSZENIE_DO_DRUZYNY, TypyPowiadomien.DRUZYNA_ZOSTALA_USUNIETA,
        TypyPowiadomien.UZYTKOWNIK_OPUSCIL_DRUZYNE_BO_USUNAL_KONTO]);


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

    if(powiadomienie.idTypuPowiadomienia === TypyPowiadomien.ZAPROSZENIE_DO_ZNAJOMYCH
        || powiadomienie.idTypuPowiadomienia === TypyPowiadomien.ZAPROSZENIE_DO_DRUZYNY
    ) {
        let href = powiadomienie.idTypuPowiadomienia === TypyPowiadomien.ZAPROSZENIE_DO_ZNAJOMYCH
            ? `${CLIENT_URL}/profil/` + powiadomienie.idPowiazanegoObiektu
            : `${CLIENT_URL}/druzyna/` + powiadomienie.idPowiazanegoObiektu

        return (<li key={powiadomienie.id} className="p-2 border-b border-gray-200">
            <div className="font-semibold mb-1">{typPowiadomienia}</div>
            <div className="text-sm text-gray-600">
                {trescPowiadomieniaCz1}
                <a href={href} className="text-black font-semibold">{powiadomienie.nazwaPowiazanegoObiektu}</a>
                {trescPowiadomieniaCz2}
            </div>
            <div className="text-xs text-gray-400 mt-1">{powiadomienie.dataWyslania}</div>
            {/* przycisk zaakceptuj i odrzuć */}
            <div className="flex justify-center items-center mt-2">
                <button
                    className="text-white bg-green-900 rounded-lg px-3 py-2 mx-2 hover:bg-green-600"
                    onClick={() => przyRozpatrzaniuPowiadomienia(powiadomienie.id, true)}>Zaakceptuj
                </button>
                <button
                    className="text-white bg-red-900 rounded-lg px-3 py-2 mx-2 hover:bg-red-600"
                    onClick={() => przyRozpatrzaniuPowiadomienia(powiadomienie.id, false)}>Odrzuć
                </button>
            </div>
        </li>);
    }

    // typy powiadomień: użytkownik dołączył do drużyny / opuścił drużynę, przyjął / odrzucił zaproszenie do drużyny

    if(powiadomienie.idTypuPowiadomienia === TypyPowiadomien.UZYTKOWNIK_DOLACZYL_DO_DRUZYNY
        || powiadomienie.idTypuPowiadomienia === TypyPowiadomien.UZYTKOWNIK_OPUSCIL_DRUZYNE
        || powiadomienie.idTypuPowiadomienia === TypyPowiadomien.UZYTKOWNIK_PRZYJAL_ZAPROSZENIE_DO_DRUZYNY
        || powiadomienie.idTypuPowiadomienia === TypyPowiadomien.UZYTKOWNIK_ODRZUCIL_ZAPROSZENIE_DO_DRUZYNY
    )
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
                <a href={`${CLIENT_URL}/druzyna/`+powiadomienie.idDrugiegoPowiazanegoObiektu} className="text-black font-semibold">{powiadomienie.nazwaDrugiegoPowiazanegoObiektu}</a>
            }
            {trescPowiadomieniaCz3}
        </div>
        <div className="text-xs text-gray-400 mt-1.5">{powiadomienie.dataWyslania}</div>
    </li>)

    if(
        powiadomienie.idTypuPowiadomienia === TypyPowiadomien.DRUZYNA_ZOSTALA_USUNIETA
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
            <span className="text-sm text-gray-600">{trescPowiadomieniaCz1}</span>
            <span className="text-sm text-gray-800 font-bold">{powiadomienie.nazwaPowiazanegoObiektu}</span>
            <span className="text-sm text-gray-600">{trescPowiadomieniaCz2}</span>
            <div className="text-xs text-gray-400 mt-1.5">{powiadomienie.dataWyslania}</div>
        </li>);

    if(powiadomienie.idTypuPowiadomienia === TypyPowiadomien.UZYTKOWNIK_OPUSCIL_DRUZYNE_BO_USUNAL_KONTO)
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
                <a href={`${CLIENT_URL}/druzyna/`+powiadomienie.idPowiazanegoObiektu} className="text-black font-semibold">{powiadomienie.nazwaPowiazanegoObiektu}</a>
                {trescPowiadomieniaCz2}
            </div>
            <div className="text-xs text-gray-400 mt-1.5">{powiadomienie.dataWyslania}</div>
        </li>)

    // typy powiadomień: zaakceptowano / odrzucono zaproszenie do znajomych, usunięto cię ze znajomych
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
        </div>
        <div className="text-xs text-gray-400 mt-1.5">{powiadomienie.dataWyslania}</div>
    </li>)
}


