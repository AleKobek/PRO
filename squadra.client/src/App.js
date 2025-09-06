import './App.css';
import {BrowserRouter, Routes, Route} from "react-router-dom";
import TwojProfil from "./Components/TwojProfil";
import EdytujProfil from "./Components/EdytujProfil";
import Error404 from "./Components/Error404";

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route path = "/" element = {<TwojProfil/>}></Route>
          <Route path = "/edytujProfil" element = {<EdytujProfil/>}></Route>
          <Route path = "*" element = {<Error404/>}></Route>
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
