import {useLocation, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import React, {useEffect, useState} from "react";
import {Bounce, ToastContainer} from "react-toastify";
import TabelkaDruzynKomponent from "./TabelkaDruzynKomponent";

export default function WynikiWyszukiwaniaDruzyn() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const location = useLocation();

    const [idDruzyn, ustawIdDruzyn] = useState([]);
    const [pierwszaStronaDruzyn, ustawPierwszaStroneDruzyn] = useState([])

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    useEffect(() => {
        ustawIdDruzyn(location.state.dane.idDruzyn);
        ustawPierwszaStroneDruzyn(location.state.dane.pierwszaStronaDruzyn);
    },[location.state.dane, location.state.dane.idDruzyn, location.state.dane.pierwszaStronaDruzyn])

    if(ladowanie || !uzytkownik) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <button className={"przycisk-nawigacji"} onClick={() => navigate('/wyszukajDruzyne')}>Wyszukaj inne drużyny</button>
            <h1>Wyszukane drużyny</h1>
            <h3>Szczegóły wyświetlą się w nowych kartach</h3>
            <TabelkaDruzynKomponent
                idDruzyn={idDruzyn}
                brakDruzynWiadomosc="Brak dostępnch drużyn spełniających warunki wyszukiwania"
                pierwszaStronaDruzyn={pierwszaStronaDruzyn}
            />
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