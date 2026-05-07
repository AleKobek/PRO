import React, {useState} from "react";
import FormularzWyboruGryDruzyny from "./FormularzWyboruGryDruzyny";


export default function FormularzZewnetrznyDruzyny({uzytkownik, ladowanie}) {



    const [czyZintegrowano, ustawCzyZintegrowano] = useState(false);

    const [idGryDruzyny, ustawIdGryDruzyny] = useState(0);

    if(ladowanie || !uzytkownik) return (<>
            <div id = "glowna">
                <h1>Ładowanie...</h1>
            </div>
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
            : <>Tu będzie drugi formularz</>
        }
    </>);
}