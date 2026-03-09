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

function App() {

    const [czySaNoweWiadomosci, ustawCzySaNoweWiadomosci] = useState(false);

    // jesteśmy na porcie 3000
    return (
        <AuthProvider>
          <BrowserRouter>
            <Routes>
              <Route path = "/" element = {<StronaGlowna/>}></Route>
              <Route path = "/edytujProfil" element = {<>
                  <NaglowekZalogowano czySaNoweWiadomosci={czySaNoweWiadomosci} ustawCzySaNoweWiadomosci={ustawCzySaNoweWiadomosci}/>
                  <EdytujProfil/>
              </>}></Route>
              <Route path = "/twojProfil" element = {<>
                  <NaglowekZalogowano czySaNoweWiadomosci={czySaNoweWiadomosci} ustawCzySaNoweWiadomosci={ustawCzySaNoweWiadomosci}/>
                  <TwojProfil/>
              </>}></Route>
                <Route path = "/twojeKonto" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosci={czySaNoweWiadomosci} ustawCzySaNoweWiadomosci={ustawCzySaNoweWiadomosci}/>
                    <TwojeKonto/>
                </>}></Route>
                <Route path = "/edytujKonto" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosci={czySaNoweWiadomosci} ustawCzySaNoweWiadomosci={ustawCzySaNoweWiadomosci}/>
                    <EdytujKonto/>
                </>}></Route>
                <Route path = "/twoiZnajomi" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosci={czySaNoweWiadomosci} ustawCzySaNoweWiadomosci={ustawCzySaNoweWiadomosci}/>
                    <TwoiZnajomiStrona/>
                </>}></Route>
              <Route path = "/profil/:idWlascicielaProfilu" element = {<>
                    <NaglowekZalogowano czySaNoweWiadomosci={czySaNoweWiadomosci} ustawCzySaNoweWiadomosci={ustawCzySaNoweWiadomosci}/>
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
