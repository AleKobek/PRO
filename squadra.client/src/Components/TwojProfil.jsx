import '../App.css';

import React, {useEffect} from 'react';
import DaneProfilu from './DaneProfilu';
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
export default function TwojProfil() {
    
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
            <DaneProfilu idUzytkownika={uzytkownik.id}></DaneProfilu>
            <button className={"przycisk-nawigacji"} onClick={() => navigate('/edytujProfil')}>Edytuj profil</button>
        </div>
    </>);
}