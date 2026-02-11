import '../App.css';

import React, {useEffect} from 'react';
import DaneProfilu from './DaneProfilu';
import {useLocation, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {Bounce, toast, ToastContainer} from "react-toastify";
export default function TwojProfil() {
    
    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const location = useLocation();

    useEffect(() => {
        if (!ladowanie && !uzytkownik) {
            navigate("/login"); // jeśli jest niezalogowany
        }
    }, [ladowanie, uzytkownik, navigate]);

    useEffect(() => {
        if (location.state?.pomyslnieEdytowanoProfil) {
            toast.success('Pomyślnie edytowano profil!', {
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
    },[location.state?.pomyslnieEdytowanoProfil])

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