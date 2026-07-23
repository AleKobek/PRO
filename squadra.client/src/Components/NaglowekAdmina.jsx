import React, {useEffect} from "react";
import {NavLink, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {API_BASE_URL} from "../config/api";
import {Bounce, ToastContainer} from "react-toastify";

export default function NaglowekAdmina(){

    const navigate = useNavigate();
    const {uzytkownik, ustawUzytkownika, ladowanie} = useAuth();

    // Jeśli nie ma użytkownika, nawiguj do logowania
    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate('/login');
        }
        if (!ladowanie && uzytkownik && !uzytkownik.role.includes("Admin")) {
            navigate('/twojeDruzyny');
        }
    }, [uzytkownik, navigate, ladowanie]);
    
    // ping
    useEffect(() => {
        const interval = setInterval(async () => {
            await fetch(`${API_BASE_URL}/Uzytkownicy/ping`, {credentials: "include"});
        }, 60000); // co minutę

        // jak wychodzimy to usuwamy nasz interval
        return () => clearInterval(interval);
    }, []);




    // przy załadowaniu strony także jednorazowo sprawdzamy czy są nowe wiadomości i pingamy
    useEffect(() => {
        async function fetchNow() {
            await fetch(`${API_BASE_URL}/Uzytkownicy/ping`, {credentials: "include"});
        }
        fetchNow();

    },[])


    const przyWylogowywaniu = async () =>{
        
        const res = await fetch(`${API_BASE_URL}/Auth/logout`, {method: "POST", credentials: "include"});
        if(!res.ok) {
            console.error(res);
            console.error("Nie udało się wylogować")
            return;
        }
        ustawUzytkownika(null);
        navigate('/login')
    }

    return(<>
        <header>
            <title>Squadra</title>
        </header>
        <div className ="menu relative z-[2000]" id = "menu">
            <div className="flex items-center flex-row  gap-3 h-full">
                <span className="logo">Squadra</span>
                <img src="/img/gamepad-2.svg" alt="gamepad" className="h-[clamp(28px,3vw,52px)] w-auto" />
            </div>

            <div id = "menu-na-pasku">
                <div className="nawigacja-na-pasku">
                    {/* na razie wszystkie prowadzą do profilu, bo nie ma reszty */}
                    <NavLink to = '/panelAdmina' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Panel admina</NavLink>
                    <NavLink to = '/twojeKontoAdmin' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Twoje konto</NavLink>
                </div>
                <button
                    className="text-white bg-red-900 hover:bg-red-600 mx-2 rounded-[clamp(6px,0.55vw,12px)] px-[clamp(8px,0.9vw,14px)] py-[clamp(6px,0.6vw,10px)] text-[clamp(14px,1.05vw,20px)] leading-none"
                    onClick={przyWylogowywaniu}>
                    Wyloguj
                </button>
            </div>
        </div>
        {<ToastContainer
            containerId="naglowek-toast-container"
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
        />}
    </>)


}
