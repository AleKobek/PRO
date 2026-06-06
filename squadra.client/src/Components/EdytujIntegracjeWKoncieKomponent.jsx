import {API_BASE_URL} from "../config/api";
import {Bounce, toast} from "react-toastify";
import React, {useState} from "react";

export default function EdytujIntegracjeWKoncieKomponent({zewnetrzneId, ustawZewnetrzneId, zewnetrznyLogin, ustawZewnetrznyLogin}){

    const [login, ustawLogin] = useState("");
    const [haslo, ustawHaslo] = useState("");

    const przyWysylaniuLaczenia = async() => {

        const dane = {
            login: login,
            haslo: haslo
        }

        const opcje = {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: "include",
            body: JSON.stringify(dane)
        }

        const res = await fetch(`${API_BASE_URL}/IntegracjeZewnetrzne`, opcje);


        // Odczyt body różni się zależnie od typu odpowiedzi
        // jeżeli to 404, to zwraca tylko tekst (nie application/json), więc res.json rzuci wyjątek. musimy to uwzlgędnić
        const ct = res.headers.get("content-type") || "";
        const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
            ? await res.json().catch(() => null)
            : await res.text().catch(() => "");

        if (!res.ok) {
            toast.error(`Wystąpił błąd: ${body}`, {
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

        ustawZewnetrzneId(body.id);
        ustawZewnetrznyLogin(body.login);

        toast.success('Pomyślnie zintegrowano konto!', {
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

    const przyWysylaniuOdlaczenia = async() => {

        const opcje = {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: "include",
        }

        const res = await fetch(`${API_BASE_URL}/IntegracjeZewnetrzne/przerwij`, opcje);


        // Odczyt body różni się zależnie od typu odpowiedzi
        // jeżeli to 404, to zwraca tylko tekst (nie application/json), więc res.json rzuci wyjątek. musimy to uwzlgędnić
        const ct = res.headers.get("content-type") || "";
        const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
            ? await res.json().catch(() => null)
            : await res.text().catch(() => "");


        if (!res.ok) {
            toast.error(`Wystąpił błąd: ${body.message}`, {
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

        ustawZewnetrzneId(null);
        ustawZewnetrznyLogin("");
        ustawLogin("");
        ustawHaslo("");

        toast.success('Pomyślnie przerwano integrację!', {
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

    if(zewnetrzneId === null){
        return (<div>
            <h3>Połącz konto z zewnętrznym serwisiem</h3>
            <form className="flex flex-col items-center justify-center border-4 border-gray-700 rounded-lg p-8 w-fit gap-4 text-xl">
                <label>Login<br/>
                    <input type="text" id = "login" name ="login" value={login} onChange={(e)=>ustawLogin(e.target.value)}/>
                </label>
                <br/>
                <label>Haslo<br/>
                    <input type="password" id = "haslo" name ="haslo" value={haslo} onChange={(e)=>ustawHaslo(e.target.value)}/>
                </label>
                <br/>
                <input className={login.trim().length === 0 || haslo.trim().length === 0 ? "zablokowany-przycisk" :"wyslij-formularz-przycisk"} type = "button" value = "Wyślij" onClick={przyWysylaniuLaczenia} disabled={login.trim().length === 0 || haslo.trim().length === 0}/>
            </form>
        </div>);
    }

    return(<div className="flex flex-col items-center justify-center border-4 border-gray-700 rounded-lg p-8 space-y-6 w-fit gap-4 text-xl">
        <div><span>Połączono jako użytkownik:</span> <span className="font-bold">{zewnetrznyLogin}</span></div>
        <button className="bg-red-900 text-white rounded-md px-5 py-3.5 text-4xl hover:bg-red-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105" onClick={przyWysylaniuOdlaczenia}>Przerwij integrację</button>
        <span>Przerwanie integracji wyrzuci Cię z drużyn, które uzywają zintegrowanych danych, i usunie Twoje drużyny używające zintegrowanych danych.</span>
    </div>);
}