import {Bounce, ToastContainer} from "react-toastify";
import React from "react";
import {useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";

export default function TwojeDruzyny() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();




    if(ladowanie) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Twoje drużyny</h1>
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