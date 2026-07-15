import React, {useEffect} from 'react';
import {NavLink, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";

export default function Naglowek() {

    const { uzytkownik, ladowanie } = useAuth();
    const navigate = useNavigate();


    useEffect(() => {
        if (!ladowanie && uzytkownik) {
            navigate("/twojeDruzyny"); // jeśli jest zalogowany
        }
    }, [ladowanie, uzytkownik, navigate])

    return(<>
        <header>
            <title>Squadra</title>
        </header>
        <div className = "menu" id="menu">
            <div className="flex items-center flex-row  gap-3 h-full">
                <span className="logo">Squadra</span>
                <img src="/img/gamepad-2.svg" alt="gamepad" className="h-[clamp(28px,3vw,52px)] w-auto" />
            </div>
        <ul id = "menu-na-pasku">
            <NavLink to = '/' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Strona główna</NavLink>
            <NavLink to = '/login' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Zaloguj się</NavLink>
            <NavLink to = '/rejestracja' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Zarejestruj się</NavLink>
        </ul>
    </div></>)
    
}