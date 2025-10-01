import Naglowek from "./Naglowek";
import NaglowekZalogowano from "./NaglowekZalogowano";

export default function Error404({czyZalogowano}) {


    if(czyZalogowano)
        return (<>
            <NaglowekZalogowano></NaglowekZalogowano>
            <h1>"Coś poszło nie tak, ta strona nie istnieje!",</h1>
            <h2>404</h2>
        </>);
    
    
  return (<>
      <Naglowek></Naglowek>
      <h1>"Coś poszło nie tak, ta strona nie istnieje!",</h1>
      <h2>404</h2>
  </>);
}