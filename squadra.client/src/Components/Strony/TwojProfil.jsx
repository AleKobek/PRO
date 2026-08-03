import '../../App.css';

import React, {useEffect} from 'react';
import DaneProfilu from '../DaneProfilu';
import {useLocation, useNavigate} from "react-router-dom";
import {useAuth} from "../../Context/AuthContext";
import {Bounce, toast, ToastContainer} from "react-toastify";
import TabelkaBibliotekiGier from "../TabelkaBibliotekiGier";
import {useWspoldzieloneFunkcje} from "../../Context/WspoldzieloneFunkcjeContext";
export default function TwojProfil() {
    
    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const location = useLocation();
    const {toastOptions} = useWspoldzieloneFunkcje();

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    useEffect(() => {
        if (location.state?.pomyslnieEdytowanoProfil) {
            toast.success('Pomyślnie edytowano profil!', toastOptions);
        }
    },[location.state?.pomyslnieEdytowanoProfil, toastOptions])

    if(ladowanie || !uzytkownik) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Twój profil</h1>
            <DaneProfilu idUzytkownika={uzytkownik.id}></DaneProfilu>
            <button className={"przycisk-nawigacji"} onClick={() => navigate('/edytujProfil')}>Edytuj profil</button>
            <h2 className="mt-10">Biblioteka gier</h2>
            <TabelkaBibliotekiGier idUzytkownika={uzytkownik.id}/>
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