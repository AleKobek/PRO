export default function JezykNaLiscieKomponent({jezyk, jezykDoKomponentu, coPrzyKlikaniu, idZListy, czyEdytuj}){
    
    // język to język wyświetlania, a język do komponentu to język na liście
    
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