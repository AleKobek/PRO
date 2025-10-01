import {useEffect} from "react";

export default function JezykNaLiscieKomponent({
                                                   jezykDoKomponentu,
                                                   coPrzyKlikaniu,
                                                   idZListy,
                                                   czyEdytuj,
                                               }) {

    
    useEffect(() => {
    }, [jezykDoKomponentu]);
    
    if(!jezykDoKomponentu){
        return (<></>);
    }
    
    else if(jezykDoKomponentu.jezyk === undefined){
        return (
            <li key={idZListy - jezykDoKomponentu?.idJezyka}>
      <span className="jezyk-na-liscie">
          {jezykDoKomponentu?.nazwaJezyka} : {jezykDoKomponentu?.nazwaStopnia}
      </span>
                {czyEdytuj && (
                    <button type="button" id={idZListy} onClick={coPrzyKlikaniu}>
                        Usuń
                    </button>
                )}
            </li>
        );
    }
    
    return (
        <li key={idZListy - jezykDoKomponentu?.idJezyka}>
      <span className="jezyk-na-liscie">
        <div className="nazwa-jezyka">{jezykDoKomponentu?.jezyk.nazwa}</div> : {jezykDoKomponentu?.stopien.nazwa}
      </span>
            {czyEdytuj && (
                <button type="button" id={idZListy} onClick={coPrzyKlikaniu}>
                    Usuń
                </button>
            )}
        </li>
    );
}