import './App.css';
import {BrowserRouter, Routes, Route} from "react-router-dom";
import TwojProfil from "./Components/TwojProfil";
import EdytujProfil from "./Components/EdytujProfil";
import Error404 from "./Components/Error404";
import Logowanie from "./Components/Logowanie";
import Rejestracja from "./Components/Rejestracja";
import {AuthProvider} from "./Context/AuthContext";
import StronaGlowna from "./Components/StronaGlowna";
import NaglowekZalogowano from "./Components/NaglowekZalogowano";
import TwojeKonto from "./Components/TwojeKonto";
import EdytujKonto from "./Components/EdytujKonto";
import TwoiZnajomiStrona from "./Components/TwoiZnajomiStrona";
import ProfilOgolny from "./Components/ProfilOgolny";
import {useState} from "react";
import {Bounce, ToastContainer} from "react-toastify";
import TwojeDruzyny from "./Components/TwojeDruzyny";
import StworzDruzyne from "./Components/StworzDruzyne";
import StronaSzczegolowDruzyny from "./Components/StronaSzczegolowDruzyny";
import EdytujDruzyne from "./Components/EdytujDruzyne";
import WyszukajDruzyne from "./Components/WyszukajDruzyne";
import WynikiWyszukiwaniaDruzyn from "./Components/WynikiWyszukiwaniaDruzyn";
import PanelAdmina from "./Components/PanelAdmina";
import NaglowekAdmina from "./Components/NaglowekAdmina";

function App() {

    const [czySaNoweWiadomosciPrywatne, ustawCzySaNoweWiadomosciPrywatne] = useState(false);
    const [czySaNoweWiadomosciDruzynowe, ustawCzySaNoweWiadomosciDruzynowe] = useState(false);
    const [powiadomienia, ustawPowiadomienia] = useState([])
    const [awatarUrl, ustawAwatarUrl] = useState("");

    // jesteśmy na porcie 3000
    return (
        <AuthProvider>
          <BrowserRouter>
            <Routes>
              <Route path = "/" element = {<StronaGlowna/>}></Route>
              <Route path = "/edytujProfil" element = {<>
                  <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                      czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                      awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                  <EdytujProfil ustawAwatarUrl={ustawAwatarUrl}/>
              </>}></Route>
              <Route path = "/twojProfil" element = {<>
                  <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                      czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                      awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                  <TwojProfil/>
              </>}></Route>
                <Route path = "/twojeKonto" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                        czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                        awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                    <TwojeKonto/>
                </>}></Route>
                <Route path = "/edytujKonto" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                        czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                        awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                    <EdytujKonto/>
                </>}></Route>
                <Route path = "/twoiZnajomi" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                        czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                        awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                    <TwoiZnajomiStrona ustawCzySaNoweWiadomosci={ustawCzySaNoweWiadomosciPrywatne}/>
                </>}></Route>
              <Route path = "/profil/:idWlascicielaProfilu" element = {<>
                  <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                      czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                      awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                  <ProfilOgolny/>
              </>}></Route>
                <Route path = "/twojeDruzyny" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                        czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                        awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                    <TwojeDruzyny/>
                </>}></Route>
                <Route path = "/stworzDruzyne" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                        czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                        awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                    <StworzDruzyne/>
                </>}></Route>
                <Route path = "/druzyna/:idDruzyny" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                        czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                        awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                    <StronaSzczegolowDruzyny ustawCzySaNoweWiadomosciDruzynowe = {ustawCzySaNoweWiadomosciDruzynowe} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                </>}></Route>
                <Route path = "/edytujDruzyne" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                        czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                        awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                    <EdytujDruzyne/>
                </>}></Route>
                <Route path = "/wyszukajDruzyne" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                        czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                        awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
                    <WyszukajDruzyne/>
                </>}></Route>
                <Route path = "/wyszukaneDruzyny" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosciPrywatne={czySaNoweWiadomosciPrywatne} ustawCzySaNoweWiadomosciPrywatne={ustawCzySaNoweWiadomosciPrywatne}
                                        czySaNoweWiadomosciDruzynowe={czySaNoweWiadomosciDruzynowe} ustawCzySaNoweWiadomosciDruzynowe={ustawCzySaNoweWiadomosciDruzynowe}
                                        awatarUrl={awatarUrl} ustawAwatarUrl={ustawAwatarUrl} powiadomienia={powiadomienia} ustawPowiadomienia={ustawPowiadomienia}/>
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
        </AuthProvider>
    );
}

export default App;
