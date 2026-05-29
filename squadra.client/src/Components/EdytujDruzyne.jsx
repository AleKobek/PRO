import '../App.css';

import React, {useEffect, useState} from 'react';
import {useLocation, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {Bounce, ToastContainer} from "react-toastify";
export default function EdytujDruzyne() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const location = useLocation();
    const [stareDane, ustawStareDane] = useState({})

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    useEffect(() => {
        if (location.state?.daneDoPrzekazania) {
            ustawStareDane(location.state.daneDoPrzekazania)
            console.log(location.state.daneDoPrzekazania)
        }
    },[location.state.daneDoPrzekazania])

    if(ladowanie || !uzytkownik || !stareDane) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <div className="flex justify-start">
                <button className="przycisk-nawigacji">Powrót do szczegółów drużyny</button>
            </div>
            <div>

            </div>
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