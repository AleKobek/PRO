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
    const [druzynyNaStronie, ustawDruzynyNaStronie] = useState([])

    const [liczbaDruzynNaStronie, ustawLiczbeDruzynNaStronie] = useState(20); // domyślne ustawienie liczby drużyn na stronie to 20
    const [aktualnaStrona, ustawAktualnaStrone] = useState(0)
    const liczbaStron = Math.max(1, Math.ceil((idDruzyn?.length ?? 0) / liczbaDruzynNaStronie));

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    useEffect(() => {
        ustawIdDruzyn(location.state.dane.idDruzyn);
        ustawDruzynyNaStronie(location.state.dane.pierwszaStronaDruzyn);
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
            <select
                onChange={ (e) => {
                    ustawLiczbeDruzynNaStronie(parseInt(e.target.value));
                    ustawAktualnaStrone(0);
                    // pobierzNoweDruzyny();
                }}>
                <option value="10">10</option>
                <option value="20">20</option>
                <option value="30">30</option>
                <option value="50">50</option>
            </select>
            <TabelkaDruzynKomponent druzyny={druzynyNaStronie} brakDruzynWiadomosc="Brak dostępnch drużyn spełniających warunki wyszukiwania" czySzczegolyWNowejKarcie={true}/>
            {/* paginacja */}
            <div className="flex justify-center mt-3"><span>Strona {aktualnaStrona + 1} z {liczbaStron}</span></div>
            <div className="flex justify-center gap-6">
                <button className={aktualnaStrona === 0 ? "zablokowany-przycisk" : "bg-blue-900 hover:bg-blue-600 text-white"} disabled={aktualnaStrona === 0}
                        onClick={ () => { ustawAktualnaStrone(aktualnaStrona - 1) } }
                >Poprzednia strona</button>

                {/* przycisk następnej strony*/}
                <button className={liczbaStron === 0 || aktualnaStrona === liczbaStron - 1 ? "zablokowany-przycisk" : "bg-blue-900 hover:bg-blue-600 text-white"} disabled= {liczbaStron === 0 || aktualnaStrona === liczbaStron - 1}
                        onClick={ () => { ustawAktualnaStrone(aktualnaStrona + 1) } }
                >Następna strona</button>
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