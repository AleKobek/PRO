import React, {Fragment, useEffect, useState} from "react";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast} from "react-toastify";

// pamiętać o tym, aby to było w nawiasach klamrowych!
export default function DaneKonta({uzytkownik}) {

    const [login, ustawLogin] = useState("");
    const [email, ustawEmail] = useState("");
    // {id, nazwa}
    const [numerTelefonu, ustawNumerTelefonu] = useState("");
    // {id, nazwa}
    const [dataUrodzenia, ustawDateUrodzenia] = useState(null);
    const [platformy, ustawPlatformy] = useState([]);

    useEffect(() => {

        // czekamy aż się załaduje id użytkownika
        if(!uzytkownik) return;
        if(!uzytkownik.id) return;

        const ac = new AbortController();
        let alive = true;

        // taka pomocnicza funkcja dla abort controller
        const fetchJsonAbort = async (url) => {
            try {
                const res = await fetch(url, { method: 'GET', signal: ac.signal, credentials: "include" });
                if (!res.ok) {
                    toast.error('Wystąpił błąd podczas pobierania danych konta', {
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
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                toast.error('Wystąpił błąd podczas pobierania danych konta', {
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

        const podajDaneKonta = async () => {
            const data = await fetchJsonAbort(`${API_BASE_URL}/Uzytkownicy/`);
            const platformy =  !uzytkownik.role.includes("Admin") ? await fetchJsonAbort(`${API_BASE_URL}/Platformy/uzytkownik/${uzytkownik.id}`) : [];

            // przerywamy działanie funkcji
            if (!alive) return;

            ustawLogin(data.login ?? "");
            ustawEmail(data.email ?? "");
            ustawNumerTelefonu(data.numerTelefonu ?? "");
            ustawDateUrodzenia(data.dataUrodzenia ?? null);
            ustawPlatformy(platformy ?? []);
        };

        if(!login && !email) {
            podajDaneKonta();
        }
        
        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };
    }, [email, login, uzytkownik]);


    if(!uzytkownik || !uzytkownik.id) return(<><p>Ładowanie...</p></>);

    return(<div className="w-full flex flex-col items-center justify-center">
        <div className = "dane-konta">
            <label>
                Login:
                <p id = "login" className= "pole-w-danych-konta">{login}</p>
            </label>
            <label>
                Email:
                <p id= "email" className= "pole-w-danych-konta">{email}</p>
            </label>
            <label>
                Data urodzenia:
                <p id = "data-urodzenia" className= "pole-w-danych-konta">{dataUrodzenia ? dataUrodzenia : "Nie określono"}</p>
            </label>
            <label>
                Numer telefonu:
                <p id = "numer-telefonu" className= "pole-w-danych-konta">{numerTelefonu ? numerTelefonu : "Nie podano"}</p>
            </label>
        </div>
        {/*  tutaj będzie lista platform, mapujemy jak w innych listach */}
        {platformy.length > 0 && <div>
            <h3>Platformy</h3>
            <ul className="items-center justify-center flex-col gap-9 mb-6 border-2 border-black rounded-lg">
            {platformy.map((platforma) => (
                <li key = {platforma.idPlatformy} className="flex items-center gap-5 mx-4">
                    <img
                        src={"data:image/jpeg;base64," + platforma.logo}
                        alt="logo"
                        className="h-20 w-20 my-3 rounded-full border-4 border-black"
                    />
                    <span className="font-bold">{platforma.nazwa}</span>
                </li>
            ))}
            </ul>
        </div>}
    </div>)
}