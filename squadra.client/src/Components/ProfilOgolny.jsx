import '../App.css';

import React, {useEffect} from 'react';
import DaneProfilu from './DaneProfilu';
import {useNavigate, useParams} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {API_BASE_URL} from "../config/api";
import {Bounce, toast, ToastContainer} from "react-toastify";
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
    }, [idUzytkownika, navigate, uzytkownik?.id]); // z jakiegoś powodu dodanie dep użytkownik wywala stronę


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
            toast.error('Nie udało się wysłać zaproszenia do znajomych.', {
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