import '../App.css';

import React, {useEffect} from 'react';
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
export default function TwoiZnajomiStrona() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();


    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate("/login"); // jeśli jest niezalogowany
        }
    }, [ladowanie, uzytkownik, navigate]);

    if(ladowanie) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Twoi znajomi</h1>
        </div>
    </>);
}