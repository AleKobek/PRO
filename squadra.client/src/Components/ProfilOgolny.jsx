import '../App.css';

import React, {useEffect} from 'react';
import DaneProfilu from './DaneProfilu';
import {useNavigate, useParams} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {API_BASE_URL} from "../config/api";
export default function ProfilOgolny() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const { idUzytkownika} = useParams();


    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate("/login"); // jeśli jest niezalogowany
        }
    }, [ladowanie, uzytkownik, navigate, idUzytkownika]);

    useEffect(() => {
        if(uzytkownik === null) return;
        if(idUzytkownika === uzytkownik.id.toString()) navigate("/twojProfil");
    }, [idUzytkownika, navigate, uzytkownik?.id]);


    const przyWysylaniuZaproszenia = async () => {
        const opcje = {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            credentials: "include",
        }

        const res = await fetch(`${API_BASE_URL}/Powiadomienie/zaproszenie/znajomi/id?idZapraszanegoUzytkownika=${idUzytkownika}`, opcje);

        if(!res.ok) {
            const body = await res.json().catch(() => ({}));
            console.error(body);
        }
    }


    if(ladowanie) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Profil użytkownika</h1>
            <DaneProfilu idUzytkownika={parseInt(idUzytkownika)}></DaneProfilu>
            <button className="block !mx-auto bg-green-900 !text-[25px] text-white rounded-md !px-3 !py-1 !my-4 hover:bg-green-600 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105" onClick={() =>przyWysylaniuZaproszenia()}>Wyślij zaproszenie do znajomych</button>
        </div>
    </>);
}