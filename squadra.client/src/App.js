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

function App() {

    // jesteśmy na porcie 3000
    return (
        <AuthProvider>
          <BrowserRouter>
            <Routes>
              <Route path = "/" element = {<StronaGlowna/>}></Route>
              <Route path = "/edytujProfil" element = {<>
                  <NaglowekZalogowano/>
                  <EdytujProfil/>
              </>}></Route>
              <Route path = "/twojProfil" element = {<>
                  <NaglowekZalogowano/>
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
              <Route path = "/profil/:idUzytkownika" element = {<>
                    <NaglowekZalogowano/>
                    <ProfilOgolny/>
                </>}></Route>
              <Route path = "/login" element = {<Logowanie/>}></Route>
              <Route path = "/rejestracja" element = {<Rejestracja/>}></Route>
              <Route path = "*" element = {<Error404/>}></Route>
            </Routes>
          </BrowserRouter>
        </AuthProvider>
    );
}

export default App;
