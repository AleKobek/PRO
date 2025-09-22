import { useJezyk } from "../LanguageContext.";
import {useEffect} from "react";

export default function JezykNaLiscieKomponent({
                                                   jezykDoKomponentu,
                                                   coPrzyKlikaniu,
                                                   idZListy,
                                                   czyEdytuj,
                                               }) {
    const { jezyk } = useJezyk();

    // Zwracamy jeden, spójny element (li), a przycisk umieszczamy w środku.
    // Dzięki temu rodzic może bez problemu nadać key na komponencie.
    
    useEffect(() => {
    }, [jezykDoKomponentu]);
    
    if(!jezykDoKomponentu){
        return (<></>);
    }
    
    else if(jezykDoKomponentu.jezyk === undefined){
        return (
            <li key={idZListy - jezykDoKomponentu?.idJezyka}>
      <span>
        {jezykDoKomponentu?.nazwaJezyka} : {jezykDoKomponentu?.nazwaStopnia}
      </span>
                {czyEdytuj && (
                    <button type="button" id={idZListy} onClick={coPrzyKlikaniu}>
                        {jezyk.usun}
                    </button>
                )}
            </li>
        );
    }
    
    return (
        <li key={idZListy - jezykDoKomponentu?.idJezyka}>
      <span>
        {jezykDoKomponentu?.jezyk.nazwa} : {jezykDoKomponentu?.stopien.nazwa}
      </span>
            {czyEdytuj && (
                <button type="button" id={idZListy} onClick={coPrzyKlikaniu}>
                    {jezyk.usun}
                </button>
            )}
        </li>
    );
}