import '../App.css';

import React, {useEffect, useState} from 'react';
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {Bounce, toast, ToastContainer} from "react-toastify";
import {API_BASE_URL} from "../config/api";
import ZmienHaslo from "./ZmienHaslo";
export default function PanelAdmina() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const [idUzytkownikaDoUsuniecia, ustawIdUzytkownikaDoUsuniecia] = useState(null);
    const [idDruzynyDoUsuniecia, ustawIdDruzynyDoUsuniecia] = useState(null);


    const [pokazUsunUzytkownika, ustawPokazUsunUzytkownika] = useState(false);
    const [pokazUsunDruzyne, ustawPokazUsunDruzyne] = useState(false);
    const [czyZablokowaneUsunUzytkownika, ustawCzyZablokowaneUsunUzytkownika] = useState(true);
    const [czyZablokowaneUsunDruzyne, ustawCzyZablokowaneUsunDruzyne] = useState(true);
    const ref = React.useRef(null);


    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate('/login');
        }
    }, [uzytkownik, navigate, ladowanie]);

    // timer odliczający 5 sekund po otworzeniu panelu usunięcia użytkownika
    useEffect(() => {
        if(!pokazUsunUzytkownika) return;
        if(!czyZablokowaneUsunUzytkownika) return;

        // jeżeli panel jest pokazany, po pięciu sekundach odblokowujemy przycisk usuwania
        const timer = setTimeout(() => {
            ustawCzyZablokowaneUsunUzytkownika(false);
        }, 5000);

        return () => clearTimeout(timer);

    },[czyZablokowaneUsunUzytkownika, pokazUsunUzytkownika])

    // timer odliczający 5 sekund po otworzeniu panelu usunięcia drużyny
    useEffect(() => {
        if(!pokazUsunDruzyne) return;
        if(!czyZablokowaneUsunDruzyne) return;

        // jeżeli panel jest pokazany, po pięciu sekundach odblokowujemy przycisk usuwania
        const timer = setTimeout(() => {
            ustawCzyZablokowaneUsunDruzyne(false);
        }, 5000);

        return () => clearTimeout(timer);

    },[czyZablokowaneUsunDruzyne, pokazUsunDruzyne])

    const przyUsuwaniuUzytkownika = async () => {
        if(czyZablokowaneUsunUzytkownika) return;
        if(!uzytkownik) return;

        const opcje = {
            method: "DELETE",
            headers: {"Content-Type": "application/json"},
            credentials: "include"
        }

        const res = await fetch(`${API_BASE_URL}/Uzytkownik/${idUzytkownikaDoUsuniecia}`, opcje);
        if(!res.ok){
            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");

            toast.error(`Wystąpił błąd podczas usuwania użytkownika: ${body}`, {
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
        toast.success(`Użytkownik został pomyślnie usunięty!`, {
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
        ustawPokazUsunUzytkownika(false);
        ustawIdUzytkownikaDoUsuniecia(null);
    }

    const przyUsuwaniuDruzyny = async () => {
        if(czyZablokowaneUsunDruzyne) return;
        if(!uzytkownik) return;

        const opcje = {
            method: "DELETE",
            headers: {"Content-Type": "application/json"},
            credentials: "include"
        }

        const res = await fetch(`${API_BASE_URL}/Druzyna/admin/${idDruzynyDoUsuniecia}`, opcje);
        if(!res.ok){
            const ct = res.headers.get("content-type") || "";
            const body = ct.includes("application/json") || ct.includes("application/problem+json") // to jest jak są błędy
                ? await res.json().catch(() => null)
                : await res.text().catch(() => "");

            toast.error(`Wystąpił błąd podczas usuwania drużyny: ${body}`, {
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

        toast.success(`Drużyna została pomyślnie usunięta!`, {
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
        ustawPokazUsunDruzyne(false);
        ustawIdDruzynyDoUsuniecia(null);
    }

    const PanelUsunUzytkownika = () => (
        <div
            ref={ref}
            className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] p-10 overflow-y-auto bg-red-200 border-2 border-red-900
            rounded-md shadow-lg justify-center items-center"
            style={{ zIndex: 5000 }}
        >
            <div className="flex flex-col">

                <div className="flex flex-col items-center gap-2">
                    <span className="text-4xl text-center font-bold"> Czy na pewno chcesz usunąć użytkownika? Tej operacji nie da się odwrócić!<br/></span>
                    <span>Za 5 sekund przycisk się odblokuje</span>
                </div>
                <div className="flex justify-center items-center gap-8 mt-7 text-xl font-semibold">
                    {/* przycisk anulowania */}
                    <button
                        onClick={() => {
                            ustawCzyZablokowaneUsunUzytkownika(true);
                            ustawPokazUsunUzytkownika(false)
                        }}
                        className="bg-red-900 text-white rounded-md px-6 py-3.5 hover:bg-red-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105">
                        Anuluj
                    </button>
                    {/* przycisk potwierdzenia */}
                    <button
                        className={czyZablokowaneUsunUzytkownika ?
                            "text-black bg-gray-300 rounded-md px-4 py-3 border border-black shadow-md cursor-not-allowed" :
                            "bg-green-900 text-white rounded-md px-5 py-3.5 hover:bg-green-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"}
                        disabled={czyZablokowaneUsunUzytkownika}
                        onClick={przyUsuwaniuUzytkownika}
                    >
                        Potwierdź
                    </button>
                </div>
            </div>
        </div>
    );

    const PanelUsunDruzyne = () => (
        <div
            ref={ref}
            className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] p-10 overflow-y-auto bg-red-200 border-2 border-red-900
            rounded-md shadow-lg justify-center items-center"
            style={{ zIndex: 5000 }}
        >
            <div className="flex flex-col">

                <div className="flex flex-col items-center gap-2">
                    <span className="text-4xl text-center font-bold"> Czy na pewno chcesz usunąć drużynę? Tej operacji nie da się odwrócić!<br/></span>
                    <span>Za 5 sekund przycisk się odblokuje</span>
                </div>
                <div className="flex justify-center items-center gap-8 mt-7 text-xl font-semibold">
                    {/* przycisk anulowania */}
                    <button
                        onClick={() => {
                            ustawCzyZablokowaneUsunDruzyne(true);
                            ustawPokazUsunDruzyne(false)
                        }}
                        className="bg-red-900 text-white rounded-md px-6 py-3.5 hover:bg-red-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105">
                        Anuluj
                    </button>
                    {/* przycisk potwierdzenia */}
                    <button
                        className={czyZablokowaneUsunDruzyne ?
                            "text-black bg-gray-300 rounded-md px-4 py-3 border border-black shadow-md cursor-not-allowed" :
                            "bg-green-900 text-white rounded-md px-5 py-3.5 hover:bg-green-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"}
                        disabled={czyZablokowaneUsunDruzyne}
                        onClick={przyUsuwaniuDruzyny}
                    >
                        Potwierdź
                    </button>
                </div>
            </div>
        </div>
    );

    if(ladowanie || !uzytkownik) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1 className="text-red-600">Panel admina</h1>
            <div className="flex flex-col gap-20 justify-center items-center mt-20">
                <div className="border-gray-600 border-2 rounded-md p-10 shadow-md bg-white">
                    <h2>Usuń użytkownika o podanym id</h2>
                    <div className="flex justify-center gap-5">
                        <input
                            type="number"
                            className="border-2 border-gray-500 rounded-md p-2"
                            placeholder="Podaj id użytkownika"
                            value={idUzytkownikaDoUsuniecia}
                            onChange={(e) => ustawIdUzytkownikaDoUsuniecia(e.target.value)}
                        />
                        <button
                            className="bg-red-900 text-white rounded-md px-5 py-3.5 hover:bg-red-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                            onClick={() => {
                                ustawCzyZablokowaneUsunUzytkownika(true);
                                ustawPokazUsunUzytkownika(true);
                            }}
                            disabled={!idUzytkownikaDoUsuniecia}
                        >
                            Usuń
                        </button>
                    </div>
                </div>
                <div className="border-gray-600 border-2 rounded-md p-10 shadow-md bg-white">
                    <h2>Usuń drużynę o podanym id</h2>
                    <div className="flex justify-center gap-5">
                        <input
                            type="number"
                            className="border-2 border-gray-500 rounded-md p-2"
                            placeholder="Podaj id drużyny"
                            value={idDruzynyDoUsuniecia}
                            onChange={(e) => ustawIdDruzynyDoUsuniecia(e.target.value)}
                        />
                        <button
                            className="bg-red-900 text-white rounded-md px-5 py-3.5 hover:bg-red-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                            onClick={() => {
                                ustawCzyZablokowaneUsunDruzyne(true);
                                ustawPokazUsunDruzyne(true);
                            }}
                            disabled = {!idDruzynyDoUsuniecia}
                        >
                            Usuń
                        </button>
                    </div>
                </div>
            </div>
            <ZmienHaslo/>
        </div>
        {pokazUsunUzytkownika && <PanelUsunUzytkownika />}
        {pokazUsunDruzyne && <PanelUsunDruzyne />}
        <ToastContainer
            position="top-center"
            autoClose={5000}
            hideProgressBar={false}
            newestOnTop={false}
            closeOnClick={false}
            rtl={false}
            pauseOnFocusLoss
            draggable
            pauseOnHover
            theme="light"
            transition={Bounce}
        />
    </>);
}