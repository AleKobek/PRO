import Naglowek from "./Naglowek";
import NaglowekZalogowano from "./NaglowekZalogowano";
import {useJezyk} from "../LanguageContext.";

export default function Error404({czyZalogowano}) {

    const { jezyk } = useJezyk();

    if(czyZalogowano)
        return (<>
            <NaglowekZalogowano></NaglowekZalogowano>
            <h1>{jezyk.stronaNieIstnieje}</h1>
            <h2>404</h2>
        </>);
    
    
  return (<>
      <Naglowek></Naglowek>
      <h1>{jezyk.stronaNieIstnieje}</h1>
      <h2>404</h2>
  </>);
}