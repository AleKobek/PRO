import '../App.css';

import React, {useEffect} from 'react';
import {useLocation, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import DaneKonta from "./DaneKonta";
import {Bounce, toast, ToastContainer} from "react-toastify";
export default function TwojeKonto() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const location = useLocation();


    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate("/login"); // jeśli jest niezalogowany
        }
    }, [ladowanie, uzytkownik, navigate]);

    useEffect(() => {
        if (location.state?.pomyslnieEdytowanoKonto) {
            toast.success('Pomyślnie edytowano konto!', {
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
    },[location.state?.pomyslnieEdytowanoKonto])


    if(ladowanie) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Twoje konto</h1>
            <DaneKonta uzytkownik = {uzytkownik}></DaneKonta>
            <button className={"przycisk-nawigacji"} onClick={() => navigate('/edytujKonto')} style={{textAlign: "center", alignSelf: "center"}}>Edytuj konto</button>
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