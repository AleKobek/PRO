import Naglowek from "./Naglowek";
import NaglowekZalogowano from "./NaglowekZalogowano";

export default function Error404({jezyk, czyZalogowano}) {
 
    if(czyZalogowano)
        return (<>
            <NaglowekZalogowano jezyk ={jezyk}></NaglowekZalogowano>
            <h1>{jezyk.stronaNieIstnieje}</h1>
            <h2>404</h2>
        </>);
    
    
  return (<>
      <Naglowek jezyk ={jezyk}></Naglowek>
      <h1>{jezyk.stronaNieIstnieje}</h1>
      <h2>404</h2>
  </>);
}