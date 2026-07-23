import '../App.css';

import React, {useEffect, useMemo, useState} from 'react';
import NaglowekZalogowano from './NaglowekZalogowano';
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import FormularzKonta from "./FormularzKonta";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast} from "react-toastify";
import EdytujIntegracjeWKoncieKomponent from "./EdytujIntegracjeWKoncieKomponent";
import {OkienkoTlumaczaceZintegrowanie} from "./OkienkoTlumaczaceZintegrowanie";
import ZmienHaslo from "./ZmienHaslo";
export default function EdytujKonto() {

    const navigate = useNavigate();
    const { uzytkownik, ustawUzytkownika, ladowanie } = useAuth();

    const [login, ustawLogin] = useState("");
    const [staryLogin, ustawStaryLogin] = useState("");
    
    const [email, ustawEmail] = useState("");
    const [staryEmail, ustawStaryEmail] = useState("");
    
    const [numerTelefonu, ustawNumerTelefonu] = useState("");
    const [staryNumerTelefonu, ustawStaryNumerTelefonu] = useState("");
    
    const [dataUrodzenia, ustawDateUrodzenia] = useState("");
    const [staraDataUrodzenia, ustawStaraDateUrodzenia] = useState("");

    const [zewnetrzneId, ustawZewnetrzneId] = useState(null);
    const [zewnetrznyLogin, ustawZewnetrznyLogin] = useState("");

    const[bladLoginu, ustawBladLoginu] = useState("");
    const[bladEmaila, ustawBladEmaila] = useState("");
    const[bladNumeruTelefonu, ustawBladNumeruTelefonu] = useState("");
    const[bladDatyUrodzenia, ustawBladDatyUrodzenia] = useState("");
    const[bladOgolnyKonta, ustawBladOgolnyKonta] = useState("");

    const [pokazOkienkoTlumaczenia, ustawPokazOkienkoTlumaczenia] = useState(false);
    const ref = React.useRef(null);

    const [pokazUsunKonto, ustawPokazUsunKonto] = useState(false);
    const [czyZablokowaneUsun, ustawCzyZablokowaneUsun] = useState(true);

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

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
                if (!res.ok) return null;
                return await res.json();
            } catch (err) {
                if (err && err.name === 'AbortError') return null;
                console.error('Błąd pobierania:', err);
                return null;
            }
        };

        const podajDaneKonta = async () => {
            const data = await fetchJsonAbort(`${API_BASE_URL}/Uzytkownicy/`);

            // przerywamy działanie funkcji
            if (!alive) return;

            ustawStaryLogin(data.login ?? "");
            ustawLogin(data.login ?? "");
            
            ustawStaryEmail(data.email ?? "");
            ustawEmail(data.email ?? "");
            
            ustawStaryNumerTelefonu(data.numerTelefonu ?? "");
            ustawNumerTelefonu(data.numerTelefonu ?? "");
            
            ustawStaraDateUrodzenia(data.dataUrodzenia ?? "");
            ustawDateUrodzenia(data.dataUrodzenia ?? "");

            ustawZewnetrzneId(data.idNaZewnetrznymSerwisie);
            ustawZewnetrznyLogin(data.loginNaZewnetrznymSerwisie);
        };

        if(!login && !staryEmail) {
            podajDaneKonta();
        }

        // to funkcja sprzątająca. Odpali się od razu, gdy ten element zniknie, np. użytkownik zmieni stronę
        // albo pod koniec całej funkcji
        return () => {
            alive = false;
            ac.abort(); // przerywamy fetch
        };
    }, [login, staryEmail, uzytkownik]);

    // timer odliczający 5 sekund po otworzeniu panelu usunięcia konta
    useEffect(() => {
        if(!pokazUsunKonto) return;
        if(!czyZablokowaneUsun) return;

        // jeżeli panel jest pokazany, po pięciu sekundach odblokowujemy przycisk usuwania
        const timer = setTimeout(() => {
            ustawCzyZablokowaneUsun(false);
        }, 5000);

        return () => clearTimeout(timer);

    },[czyZablokowaneUsun, pokazUsunKonto])

    const czyZablokowaneWyslijKonta = useMemo(() =>{
        return(
            (login === staryLogin && 
            email === staryEmail && 
            numerTelefonu === staryNumerTelefonu && 
            dataUrodzenia === staraDataUrodzenia) 
            ||
            login.trim().length === 0 || 
            email.trim().length === 0
        )
    },[login, staryLogin, email, staryEmail, numerTelefonu, staryNumerTelefonu, dataUrodzenia, staraDataUrodzenia]);

    const przyWysylaniuZmianyKonta = async() => {
        ustawBladOgolnyKonta("");
        ustawBladLoginu("");
        ustawBladEmaila("");
        ustawBladNumeruTelefonu("");
        ustawBladDatyUrodzenia("");

        const kontoDoWyslania = {
            login: login.trim(),
            email: email.trim(),
            numerTelefonu: numerTelefonu.trim(),
            dataUrodzenia: dataUrodzenia,
        };

        const opcje = {
            method: "PUT",
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: "include",
            body: JSON.stringify(kontoDoWyslania)
        }
        
        const res = await fetch(`${API_BASE_URL}/Uzytkownicy/`, opcje);
        
        // Odczyt body różni się zależnie od typu odpowiedzi
        // jeżeli to 404, to zwraca tylko tekst (nie application/json), więc res.json rzuci wyjątek. musimy to uwzlgędnić
        const ct = res.headers.get("content-type") || "";
        const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
            ? await res.json().catch(() => null)
            : await res.text().catch(() => "");

        if (!res.ok) {
            if(res.status === 400){
                let bledy = body.errors;
                ustawBladLoginu(bledy.Login ? bledy.Login[0] : "");
                ustawBladEmaila(bledy.Email ? bledy.Email[0] : "");
                ustawBladNumeruTelefonu(bledy.NumerTelefonu ? bledy.NumerTelefonu[0] : "");
                ustawBladDatyUrodzenia(bledy.DataUrodzenia ? bledy.DataUrodzenia[0] : "");
                ustawBladOgolnyKonta(body.message);
            }
            toast.error('Wystąpił błąd podczas edycji danych konta', {
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
        // jak tutaj dojdziemy, wszystko jest git
        uzytkownik.role.includes("Admin")
            ? navigate("/twojeKontoAdmin", {
                state: { pomyslnieEdytowanoKonto: true }
            })
            : navigate("/twojeKonto", {
            state: { pomyslnieEdytowanoKonto: true }
            });

    }


    const przyUsuwaniuKonta = async () => {
        if(czyZablokowaneUsun) return;
        if(!uzytkownik) return;

        const opcje = {
            method: "DELETE",
            headers: {"Content-Type": "application/json"},
            credentials: "include"
        }

        const res = await fetch(`${API_BASE_URL}/Uzytkownicy/`, opcje);
        if(!res.ok){
            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");

            toast.error(`Wystąpił błąd podczas usuwania konta: ${body}`, {
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
        ustawUzytkownika(null);
        navigate('/login');
    }

    const PanelUsunKonto = () => (
        <div
            ref={ref}
            className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] p-10 overflow-y-auto bg-red-200 border-2 border-red-900
            rounded-md shadow-lg justify-center items-center"
            style={{ zIndex: 5000 }}
        >
            <div className="flex flex-col">

                <div className="flex flex-col items-center gap-2">
                    <span className="text-4xl text-center font-bold"> Czy na pewno chcesz usunąć konto? Tej operacji nie da się odwrócić!<br/></span>
                    <span>Za 5 sekund przycisk się odblokuje</span>
                </div>
                <div className="flex justify-center items-center gap-8 mt-7 text-xl font-semibold">
                    {/* przycisk anulowania */}
                    <button
                        onClick={() => {
                            ustawCzyZablokowaneUsun(true);
                            ustawPokazUsunKonto(false)
                        }}
                        className="bg-red-900 text-white rounded-md px-6 py-3.5 hover:bg-red-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105">
                        Anuluj
                    </button>
                    {/* przycisk potwierdzenia */}
                    <button
                        className={czyZablokowaneUsun ?
                            "text-black bg-gray-300 rounded-md px-4 py-3 border border-black shadow-md cursor-not-allowed" :
                            "bg-green-900 text-white rounded-md px-5 py-3.5 hover:bg-green-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"}
                        disabled={czyZablokowaneUsun}
                        onClick={przyUsuwaniuKonta}
                    >
                        Potwierdź
                    </button>
                </div>
            </div>
        </div>
    );
    
    if(ladowanie) return (<>
            <NaglowekZalogowano/>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna" className="flex flex-col items-center justify-center">
            <h1>Edytuj konto</h1>
            <button className={"przycisk-nawigacji"} onClick={() => {navigate('/twojeKonto')}}>Powrót do konta</button>
            <br/><br/>
            <form id = "formularz-konta" name = "formularz-konta">
                <FormularzKonta
                    login={login}
                    ustawLogin={ustawLogin}
                    email={email}
                    ustawEmail={ustawEmail}
                    dataUrodzenia={dataUrodzenia}
                    ustawDateUrodzenia={ustawDateUrodzenia}
                    numerTelefonu={numerTelefonu}
                    ustawNumerTelefonu={ustawNumerTelefonu}
                    bladLoginu={bladLoginu}
                    bladEmaila={bladEmaila}
                    bladNumeruTelefonu={bladNumeruTelefonu}
                    bladDatyUrodzenia={bladDatyUrodzenia}
                />
                <input className={czyZablokowaneWyslijKonta ? "zablokowany-przycisk" :"wyslij-formularz-przycisk"} type = "button" value = "Zapisz" onClick={przyWysylaniuZmianyKonta} disabled={czyZablokowaneWyslijKonta}/>
                <span id = "error-zapisz" className="error-wiadomosc">{bladOgolnyKonta}</span><br/>
            </form>
            <br></br>
            <ZmienHaslo/>
            {!uzytkownik.role.includes("Admin") && <div className="flex flex-col items-center justify-center">
                <h3 className="flex items-center mb-4">
                    Zewnętrzny serwis
                    <img
                        src="/img/znak-zapytania.svg"
                        alt="znak zapytania"
                        className="h-[1em] w-auto align-middle ml-2 cursor-pointer"
                        onClick={() => ustawPokazOkienkoTlumaczenia(true)}
                    />
                </h3>
                <EdytujIntegracjeWKoncieKomponent zewnetrzneId={zewnetrzneId} ustawZewnetrzneId={ustawZewnetrzneId}
                                                  zewnetrznyLogin={zewnetrznyLogin}
                                                  ustawZewnetrznyLogin={ustawZewnetrznyLogin}
                                                  ustawPokazOkienkoTlumaczenia={ustawPokazOkienkoTlumaczenia}/>
                <br/>
                <button
                    className="block !mx-auto bg-red-900 !text-[25px] text-white rounded-md !px-3 !py-1 !my-4 hover:bg-red-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                    onClick={() => {
                        ustawCzyZablokowaneUsun(true);
                        ustawPokazUsunKonto(v => !v)
                    }}
                >
                    Usuń konto
                </button>
            </div>}
        </div>
        {pokazOkienkoTlumaczenia && OkienkoTlumaczaceZintegrowanie(ref, ustawPokazOkienkoTlumaczenia)}
        {pokazUsunKonto && <PanelUsunKonto/>}
    </>);
}