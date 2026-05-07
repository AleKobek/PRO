import React from "react";
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";

export default function StworzDruzyne() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();

    if(ladowanie || !uzytkownik) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Stwórz drużynę</h1>
            <button className={"przycisk-nawigacji"} onClick={() => {navigate('/twojeDruzyny')}}>Powrót do twoich drużyn</button>
            <br/><br/>

        </div>
    </>);
}