import Naglowek from "./Naglowek";
import NaglowekZalogowano from "./NaglowekZalogowano";
import {useNavigate} from "react-router-dom";
import {useEffect} from "react";
import {useAuth} from "../Context/AuthContext";

export default function Error404() {

    const navigate = useNavigate();
    const { uzytkownik } = useAuth();


    useEffect(() => {
        document.title = `Squadra: Błąd 404`;
    }, []);

    if(uzytkownik)
        return (<>
            <NaglowekZalogowano navigate = {navigate}></NaglowekZalogowano>
            <div className="glowna" id="glowna">
                <h1>Coś poszło nie tak, ta strona nie istnieje!</h1>
                <h2>404</h2>
            </div>
        </>);
    
    
  return (<>
      <Naglowek></Naglowek>
      <div className="glowna" id="glowna">
          <h1>"Coś poszło nie tak, ta strona nie istnieje!"</h1>
          <h2>404</h2>
      </div>
  </>);
}