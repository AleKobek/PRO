import { createContext, useContext, useState } from "react";

// tworzy obiekt kontekstu (WspoldzieloneFunkcjeContext), który React używa do przekazywania danych w dół drzewa komponentów,
// bez konieczności przekazywania ich przez props
const WspoldzieloneFunkcjeContext = createContext(null);

export function WspoldzieloneFunkcjeProvider({ children }) {

    const [czySaNoweWiadomosciPrywatne, ustawCzySaNoweWiadomosciPrywatne] = useState(false);
    const [czySaNoweWiadomosciDruzynowe, ustawCzySaNoweWiadomosciDruzynowe] = useState(false);
    const [powiadomienia, ustawPowiadomienia] = useState([])
    const [awatarUrl, ustawAwatarUrl] = useState("");
    const [znajomi, ustawZnajomych] = useState([]);


    return (
        // dzięki ".Provider" wszyscy potomkowie będą mieli dostęp do tych trzech rzeczy
        <WspoldzieloneFunkcjeContext.Provider value={{
            czySaNoweWiadomosciPrywatne, ustawCzySaNoweWiadomosciPrywatne,
            czySaNoweWiadomosciDruzynowe, ustawCzySaNoweWiadomosciDruzynowe,
            powiadomienia, ustawPowiadomienia,
            awatarUrl, ustawAwatarUrl,
            znajomi, ustawZnajomych
        }}>
            {children}
        </WspoldzieloneFunkcjeContext.Provider>
    );
}

// mała pomocnicza funkcja, by łatwo używać kontekstu
export function useWspoldzieloneFunkcje() {
    return useContext(WspoldzieloneFunkcjeContext);
}