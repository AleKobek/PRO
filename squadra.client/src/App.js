import './App.css';
import {BrowserRouter, Routes, Route} from "react-router-dom";
import TwojProfil from "./Components/TwojProfil";
import EdytujProfil from "./Components/EdytujProfil";
import Error404 from "./Components/Error404";

function App() {

    // ######################################### tylko do prototypu! ###############################################
    localStorage.setItem("idUzytkownika", "1");
  
    return (
      <BrowserRouter>
        <Routes>
          <Route path = "/" element = {<TwojProfil/>}></Route>
          <Route path = "/edytujProfil" element = {<EdytujProfil/>}></Route>
          <Route path = "/twojProfil" element = {<TwojProfil/>}></Route>
          <Route path = "*" element = {<Error404/>}></Route>
        </Routes>
      </BrowserRouter>
    );
}

export default App;
