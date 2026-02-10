import Naglowek from "./Naglowek";
import NaglowekZalogowano from "./NaglowekZalogowano";
import {useNavigate} from "react-router-dom";

export default function Error404({czyZalogowano}) {

    const navigate = useNavigate();

    if(czyZalogowano)
        return (<>
            <NaglowekZalogowano navigate = {navigate}></NaglowekZalogowano>
            <div className="glowna" id="glowna">
                <h1>"Coś poszło nie tak, ta strona nie istnieje!"</h1>
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