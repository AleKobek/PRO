import '../App.css';

import React, {useEffect} from 'react';
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {Bounce, ToastContainer} from "react-toastify";
export default function PanelAdmina() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate('/login');
        }
        if (!ladowanie && uzytkownik && uzytkownik.role.includes("Admin")) {
            navigate('/panelAdmina');
        }
    }, [uzytkownik, navigate, ladowanie]);

    if(ladowanie || !uzytkownik) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Panel admina</h1>
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