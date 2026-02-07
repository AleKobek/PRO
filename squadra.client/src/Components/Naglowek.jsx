import React from 'react';
import { NavLink } from "react-router-dom";

export default function Naglowek() {
    
    return(<>
        <header>
            <title>Squadra</title>
        </header>
        <div className = "menu" id="menu">
        <span className="logo">Squadra</span>
        <ul id = "menu-na-pasku">
            <NavLink to = '/' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Strona główna</NavLink>
            <NavLink to = '/login' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Zaloguj się</NavLink>
            <NavLink to = '/rejestracja' className={({isActive}) => isActive ? 'nawigacja active' : 'nawigacja'}>Zarejestruj się</NavLink>
        </ul>
    </div></>)
    
}