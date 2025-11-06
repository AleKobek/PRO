import React from 'react';
import { NavLink } from "react-router-dom";

export default function Naglowek() {
    
    return(<div className = "menu" id = "menu">
        <span className="logo" id ="logo">Squadra</span>
        <ul id = "menu-na-pasku">
            <NavLink to = '/' className = "nawigacja">Strona główna</NavLink>
            <NavLink to = '/login' className = "nawigacja">Zaloguj się</NavLink>
            <NavLink to = '/rejestracja' className = "nawigacja">Zarejestruj się</NavLink>
        </ul>
    </div>)
    
}