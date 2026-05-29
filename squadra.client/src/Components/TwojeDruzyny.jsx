import {Bounce, toast, ToastContainer} from "react-toastify";
import React, {useEffect} from "react";
import {useLocation, useNavigate} from "react-router-dom";
import {useAuth} from "../Context/AuthContext";
import TabelkaTwoichDruzynKomponent from "./TabelkaTwoichDruzynKomponent";

export default function TwojeDruzyny() {

    const navigate = useNavigate();
    const { uzytkownik, ladowanie } = useAuth();
    const location = useLocation();

    useEffect(() => {
        if (location.state?.pomyslnieStworzonoDruzyne) {
            toast.success('Pomyślnie utworzono drużynę!', {
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
        if (location.state?.pomyslnieUsunietoDruzyne) {
            toast.success('Pomyślnie usunięto drużynę!', {
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
            
            if (location.state?.pomyslnieOpuszczonoDruzyne) {
                toast.success('Pomyślnie opuszczono drużynę!', {
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
        }
    },[location.state?.pomyslnieOpuszczonoDruzyne, location.state?.pomyslnieStworzonoDruzyne, location.state?.pomyslnieUsunietoDruzyne])


    if(ladowanie || !uzytkownik) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
        </>
    )

    return (<>
        <div id = "glowna">
            <h1>Twoje drużyny</h1>
            <div className="flex justify-center gap-6">
                <button className="bg-blue-600 text-white text-2xl p-2 hover:bg-blue-500 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105">Wyszukaj nową</button>
                <button
                    className="bg-green-600 text-white text-2xl p-2 hover:bg-green-500 transition-transform duration-100 ease-out hover:-translate-y-0.5 hover:scale-105"
                    onClick={() => navigate('/stworzDruzyne')}
                >Stwórz drużynę
                </button>
            </div>
            <div className="mt-10 text-2xl">
                <TabelkaTwoichDruzynKomponent idUzytkownika={uzytkownik.id}/>
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