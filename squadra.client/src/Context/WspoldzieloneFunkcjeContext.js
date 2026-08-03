import { createContext, useContext, useState } from "react";
import {Bounce} from "react-toastify";

// tworzy obiekt kontekstu (WspoldzieloneFunkcjeContext), który React używa do przekazywania danych w dół drzewa komponentów,
// bez konieczności przekazywania ich przez props
const WspoldzieloneFunkcjeContext = createContext(null);

export function WspoldzieloneFunkcjeProvider({ children }) {

    const [czySaNoweWiadomosciPrywatne, ustawCzySaNoweWiadomosciPrywatne] = useState(false);
    const [czySaNoweWiadomosciDruzynowe, ustawCzySaNoweWiadomosciDruzynowe] = useState(false);
    const [powiadomienia, ustawPowiadomienia] = useState([])
    const [awatarUrl, ustawAwatarUrl] = useState("");
    const [znajomi, ustawZnajomych] = useState([]);

    const toastOptions = {
        position: "top-center",
        autoClose: 5000,
        hideProgressBar: false,
        closeOnClick: false,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
        theme: "light",
        transition: Bounce
    };


    return (
        // dzięki ".Provider" wszyscy potomkowie będą mieli dostęp do tych trzech rzeczy
        <WspoldzieloneFunkcjeContext.Provider value={{
            czySaNoweWiadomosciPrywatne, ustawCzySaNoweWiadomosciPrywatne,
            czySaNoweWiadomosciDruzynowe, ustawCzySaNoweWiadomosciDruzynowe,
            powiadomienia, ustawPowiadomienia,
            awatarUrl, ustawAwatarUrl,
            znajomi, ustawZnajomych,
            toastOptions,
        }}>
            {children}
        </WspoldzieloneFunkcjeContext.Provider>
    );
}

// mała pomocnicza funkcja, by łatwo używać kontekstu
export function useWspoldzieloneFunkcje() {
    return useContext(WspoldzieloneFunkcjeContext);
}