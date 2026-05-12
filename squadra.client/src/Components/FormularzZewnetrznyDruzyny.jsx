import React, {useState} from "react";
import FormularzWyboruGryDruzyny from "./FormularzWyboruGryDruzyny";
import FormularzDruzynyZintegrowano from "./FormularzDruzynyZintegrowano";
import FormularzDruzynyNieZintegrowano from "./FormularzDruzynyNieZintegrowano";


export default function FormularzZewnetrznyDruzyny({uzytkownik, ladowanie}) {



    const [czyZintegrowano, ustawCzyZintegrowano] = useState(false);

    const [idGryDruzyny, ustawIdGryDruzyny] = useState(0);

    if(ladowanie || !uzytkownik) return (<>
            <h1>Ładowanie...</h1>
        </>
    )

    return (<>
        {idGryDruzyny === 0
            ? <FormularzWyboruGryDruzyny
                uzytkownik={uzytkownik}
                ustawIdGryDruzyny={ustawIdGryDruzyny}
                czyZintegrowano={czyZintegrowano}
                ustawCzyZintegrowano={ustawCzyZintegrowano}
            />
            : czyZintegrowano
                ? <FormularzDruzynyZintegrowano
                    uzytkownik={uzytkownik}
                    idGryDruzyny={idGryDruzyny}
                    ustawIdGryDruzyny={ustawIdGryDruzyny}
                    czyZintegrowano={czyZintegrowano}
                />
                : <FormularzDruzynyNieZintegrowano uzytkownik={uzytkownik} ustawIdGryDruzyny={ustawIdGryDruzyny} idGryDruzyny={idGryDruzyny}/>
        }
    </>);
}