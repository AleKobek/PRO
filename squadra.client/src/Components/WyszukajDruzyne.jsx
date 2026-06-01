import React, {useEffect, useState} from "react";
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import {OkienkoTlumaczaceZintegrowanie} from "./OkienkoTlumaczaceZintegrowanie";

export default function WyszukajDruzyne() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();

    const [pokazOkienkoTlumaczenia, ustawPokazOkienkoTlumaczenia] = useState(false);
    const ref = React.useRef(null);

    useEffect(() => {
        document.title = `Squadra`;
    }, []);

    if(ladowanie || !uzytkownik) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Wyszukaj drużynę</h1>
            <button className={"przycisk-nawigacji"} onClick={() => {navigate('/twojeDruzyny')}}>Powrót do twoich drużyn</button>
            <br/><br/>
            {pokazOkienkoTlumaczenia && OkienkoTlumaczaceZintegrowanie(ref, ustawPokazOkienkoTlumaczenia)}
        </div>
    </>);
}