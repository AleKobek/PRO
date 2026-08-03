import Awatar from "./Awatar";

export default function WiadomoscNaLiscie({wiadomosc, awatarNadawcy, pseudonimNadawcy, czyNieznany = false}){

    /*
    przykładowa wiadomość:
    {
        "idNadawcy": 1,
        "idOdbiorcy": 8,
        "dataWyslania": "31.12.2025 16:57",
        "tresc": "Ejo",
        "idTypuWiadomosci": 1
    },
    */

    const classNamePseudonimu= czyNieznany ? "font-semibold text-2xl text-gray-500" : "font-semibold text-2xl"

    // zżynamy z discorda, bo musi być awatar i pseudonim, bo potem na czacie gildii będzie potrzebne
    return(<li className= {"flex justify-start w-full gap-2"} key = {wiadomosc.id}>
        <Awatar obraz={awatarNadawcy} wysokosc={70} status={""}/>
        <div className="flex-1 min-w-0 ml-2">
            <div className="flex flex-row gap-2 items-center">
                <span className={classNamePseudonimu}>{pseudonimNadawcy}</span>
                <span className="text-sm text-gray-500">{wiadomosc.dataWyslania}</span>
            </div>
            <div className={"my-2 break-words whitespace-pre-wrap pr-10"}>
                {wiadomosc.tresc}
            </div>
        </div>
    </li>)
}