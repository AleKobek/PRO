import './App.css';
import {BrowserRouter, Routes, Route} from "react-router-dom";
import TwojProfil from "./Components/Strony/TwojProfil";
import EdytujProfil from "./Components/Strony/EdytujProfil";
import Error404 from "./Components/Strony/Error404";
import Logowanie from "./Components/Strony/Logowanie";
import Rejestracja from "./Components/Strony/Rejestracja";
import {AuthProvider} from "./Context/AuthContext";
import StronaGlowna from "./Components/Strony/StronaGlowna";
import NaglowekZalogowano from "./Components/NaglowekZalogowano";
import TwojeKonto from "./Components/Strony/TwojeKonto";
import EdytujKonto from "./Components/Strony/EdytujKonto";
import TwoiZnajomiStrona from "./Components/Strony/TwoiZnajomiStrona";
import ProfilOgolny from "./Components/Strony/ProfilOgolny";
import {Bounce, ToastContainer} from "react-toastify";
import TwojeDruzyny from "./Components/Strony/TwojeDruzyny";
import StworzDruzyne from "./Components/Strony/StworzDruzyne";
import StronaSzczegolowDruzyny from "./Components/Strony/StronaSzczegolowDruzyny";
import EdytujDruzyne from "./Components/Strony/EdytujDruzyne";
import WyszukajDruzyne from "./Components/Strony/WyszukajDruzyne";
import WynikiWyszukiwaniaDruzyn from "./Components/Strony/WynikiWyszukiwaniaDruzyn";
import PanelAdmina from "./Components/Strony/PanelAdmina";
import NaglowekAdmina from "./Components/NaglowekAdmina";
import {WspoldzieloneFunkcjeProvider} from "./Context/WspoldzieloneFunkcjeContext";

function App() {

    // jesteśmy na porcie 3000
    return (
        <AuthProvider>
            <WspoldzieloneFunkcjeProvider>
              <BrowserRouter>
                <Routes>
                  <Route path = "/" element = {<StronaGlowna/>}></Route>
                  <Route path = "/edytujProfil" element = {<>
                      <NaglowekZalogowano />
                      <EdytujProfil/>
                  </>}></Route>
                  <Route path = "/twojProfil" element = {<>
                      <NaglowekZalogowano />
                      <TwojProfil/>
                  </>}></Route>
                    <Route path = "/twojeKonto" element = {<>
                        <NaglowekZalogowano/>
                        <TwojeKonto/>
                    </>}></Route>
                    <Route path = "/edytujKonto" element = {<>
                        <NaglowekZalogowano/>
                        <EdytujKonto/>
                    </>}></Route>
                    <Route path = "/twoiZnajomi" element = {<>
                        <NaglowekZalogowano/>
                        <TwoiZnajomiStrona/>
                    </>}></Route>
                  <Route path = "/profil/:idWlascicielaProfilu" element = {<>
                      <NaglowekZalogowano/>
                      <ProfilOgolny/>
                  </>}></Route>
                    <Route path = "/twojeDruzyny" element = {<>
                        <NaglowekZalogowano/>
                        <TwojeDruzyny/>
                    </>}></Route>
                    <Route path = "/stworzDruzyne" element = {<>
                        <NaglowekZalogowano/>
                        <StworzDruzyne/>
                    </>}></Route>
                    <Route path = "/druzyna/:idDruzyny" element = {<>
                        <NaglowekZalogowano/>
                        <StronaSzczegolowDruzyny/>
                    </>}></Route>
                    <Route path = "/edytujDruzyne" element = {<>
                        <NaglowekZalogowano/>
                        <EdytujDruzyne/>
                    </>}></Route>
                    <Route path = "/wyszukajDruzyne" element = {<>
                        <NaglowekZalogowano/>
                        <WyszukajDruzyne/>
                    </>}></Route>
                    <Route path = "/wyszukaneDruzyny" element = {<>
                        <NaglowekZalogowano/>
                        <WynikiWyszukiwaniaDruzyn/>
                    </>}></Route>
                    <Route path = "/panelAdmina" element = {<>
                        <NaglowekAdmina/>
                        <PanelAdmina/>
                    </>}></Route>
                    <Route path = "/twojeKontoAdmin" element = {<>
                        <NaglowekAdmina/>
                        <TwojeKonto/>
                    </>}></Route>
                    <Route path = "/edytujKontoAdmin" element = {<>
                        <NaglowekAdmina/>
                        <EdytujKonto/>
                    </>}></Route>
                  <Route path = "/login" element = {<Logowanie/>}></Route>
                  <Route path = "/rejestracja" element = {<Rejestracja/>}></Route>
                  <Route path = "*" element = {<Error404/>}></Route>
                </Routes>
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
              </BrowserRouter>
            </WspoldzieloneFunkcjeProvider>
        </AuthProvider>
    );
}

export default App;
