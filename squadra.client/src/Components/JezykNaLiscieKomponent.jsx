export default function JezykNaLiscieKomponent({jezyk, jezykDoKomponentu, coPrzyKlikaniu, idZListy, czyEdytuj}){
    if(czyEdytuj) return(
        <>
            <li>
                {jezykDoKomponentu.jezyk} : {jezykDoKomponentu.stopien}
            </li>
            <button id = {idZListy} onClick={coPrzyKlikaniu}>{jezyk.usun}</button>
        </>
    )
    
    return(
        <>
            <li>
                {jezykDoKomponentu.jezyk} : {jezykDoKomponentu.stopien}
            </li>
        </>
    )
}