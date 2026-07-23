import React, {useMemo, useState} from "react";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast} from "react-toastify";

export default function ZmienHaslo() {

    const [stareHaslo, ustawStareHaslo] = useState("");
    const [noweHaslo, ustawNoweHaslo] = useState("");
    const [powtorzHaslo, ustawPowtorzHaslo] = useState("");

    const przyWysylaniuZmianyHasla = async() => {

        const hasloDoWyslania = {
            stareHaslo: stareHaslo.trim(),
            noweHaslo: noweHaslo.trim(),
        };

        if(hasloDoWyslania.stareHaslo.length === 0 || hasloDoWyslania.noweHaslo.length === 0){
            toast.error('Wszystkie pola muszą być wypełnione!', {
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

        const opcje = {
            method: "PUT",
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: "include",
            body: JSON.stringify(hasloDoWyslania)
        }

        const res = await fetch(`${API_BASE_URL}/Uzytkownicy/haslo`, opcje);


        // Odczyt body różni się zależnie od typu odpowiedzi
        // jeżeli to 404, to zwraca tylko tekst (nie application/json), więc res.json rzuci wyjątek. musimy to uwzlgędnić
        const ct = res.headers.get("content-type") || "";
        const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
            ? await res.json().catch(() => null)
            : await res.text().catch(() => "");

        if (!res.ok) {
            if(res.status === 400){
                toast.error(body[0].message, {
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
            return;
        }

        toast.success('Pomyślnie edytowano hasło!', {
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
        ustawStareHaslo("");
        ustawNoweHaslo("");
        ustawPowtorzHaslo("");
    }


    const czyZablokowaneWyslijHasla = useMemo(() =>{
        let tempNoweHaslo = noweHaslo.trim();
        let tempPowtorzHaslo = powtorzHaslo.trim();
        return(
            stareHaslo.trim().length === 0 ||
            tempNoweHaslo.length === 0 ||
            tempPowtorzHaslo.length === 0 ||
            tempNoweHaslo !== tempPowtorzHaslo
        )
    },[stareHaslo, noweHaslo, powtorzHaslo]);


    return (
        <div className="box-zmiany-hasla text-center my-10 border-2 border-red-900 rounded-md p-2.5 bg-[#fcb7ab]">
            <h2>Zmień hasło</h2>
            <form id= "formularz-hasła" name = "formularz-hasła" className="border-0">
                <label>
                    Stare hasło<br/>
                    <input
                        type="password" value={stareHaslo} onChange={(e) => ustawStareHaslo(e.target.value)}/><br/>
                </label>
                <label>
                    Nowe hasło<br/>
                    <input
                        type="password" value={noweHaslo} onChange={(e) => ustawNoweHaslo(e.target.value)}/><br/>
                </label>
                <label>
                    Powtórz nowe hasło<br/>
                    <input
                        type="password" value={powtorzHaslo} onChange={(e) => ustawPowtorzHaslo(e.target.value)}/><br/>
                </label>
                <input
                    className={czyZablokowaneWyslijHasla ? "zablokowany-przycisk mt-2" : "wyslij-formularz-przycisk"}
                    type="button"
                    value="Zapisz" onClick={przyWysylaniuZmianyHasla} disabled={czyZablokowaneWyslijHasla}/><br/>
            </form>
        </div>
    );
}