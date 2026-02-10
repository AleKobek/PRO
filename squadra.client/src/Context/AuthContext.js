import { createContext, useContext, useState, useEffect } from "react";
import {API_BASE_URL} from "../config/api";

// tworzy obiekt kontekstu (AuthContext), który React używa do przekazywania danych w dół drzewa komponentów, 
// bez konieczności przekazywania ich przez props
const AuthContext = createContext(null);

export function AuthProvider({ children }) {
    const [uzytkownik, ustawUzytkownika] = useState(null); // dane zalogowanego użytkownika
    const [ladowanie, ustawLadowanie] = useState(true); // czy sprawdzamy /me
    
    // przy załadowniu strony, gdzie używamy useAuth(), pobieramy zalogowanego użytkownika
    useEffect(() => {
        // to niżej daje fetch throttling - ograniczamy, jak często wykonywany jest ten request, aby nie zapchało bazy
        let czyTrwaFetch = false;
        async function fetchMe() {
            if (czyTrwaFetch) return;
            czyTrwaFetch = true;
            try {
                const res = await fetch(`${API_BASE_URL}/Auth/me`, {
                    credentials: "include"
                });
                if (res.ok) {
                    const data = await res.json();
                    ustawUzytkownika(data);
                    // to normalne jeżeli jest niezalogowany, trochę zaspami konsolę w przeglądarce ale trudno
                }else if (res.status === 401) {
                    ustawUzytkownika(null);
                }else {
                    console.warn("Nieoczekiwany status z /me:", res.status);
                    ustawUzytkownika(null);
                }
            } catch (err) {
                console.error("Błąd podczas sprawdzania sesji:", err);
                ustawUzytkownika(null);
            } finally {
                ustawLadowanie(false);
                czyTrwaFetch = false;
            }
        }

        fetchMe();
    }, []);

    return (
        // dzięki ".Provider" wszyscy potomkowie będą mieli dostęp do tych trzech rzeczy
        <AuthContext.Provider value={{ uzytkownik, ustawUzytkownika, ladowanie }}>
            {children}
        </AuthContext.Provider>
    );
}

// mała pomocnicza funkcja, by łatwo używać kontekstu
export function useAuth() {
    return useContext(AuthContext);
}