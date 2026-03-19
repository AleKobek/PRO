import {useMemo} from "react";

export default function SelectStatusowNaNaglowkuKomponent({idAktualnegoStatusu, przyZmianieStatusu, listaStatusow}){

    const classNameSelect = useMemo(() => {
        const Status = {
            1: "DOSTĘPNY",
            2: "ZARAZ WRACAM",
            3: "NIE PRZESZKADZAĆ",
            4: "NIEWIDOCZNY"
        }
        const status = Status[idAktualnegoStatusu]
        switch(status){
            case "DOSTĘPNY": return "bg-green-300";
            case "ZARAZ WRACAM": return "bg-yellow-300";
            case "NIE PRZESZKADZAĆ": return "bg-red-300";
            case "NIEWIDOCZNY": return "bg-gray-300";
            default : return null; // jak coś się popsuje
        }
    },[idAktualnegoStatusu]);

    return(<select
        value={idAktualnegoStatusu || ''}
        onChange={(e) => {
            const id = Number(e.target.value);
            przyZmianieStatusu(id);
        }}
        className={classNameSelect}
    >
        {listaStatusow.map(item =>
            <StatusNaSelectKomponent idStatusu={item.id} nazwaStatusu={item.nazwa} key={item.id} />
        )}
    </select>)
}

function StatusNaSelectKomponent({idStatusu, nazwaStatusu}){

    const classNameStatusu = useMemo(() => {
        const Status = {
            1: "DOSTĘPNY",
            2: "ZARAZ WRACAM",
            3: "NIE PRZESZKADZAĆ",
            4: "NIEWIDOCZNY"
        }
        const status = Status[idStatusu]
        switch(status){
            case "DOSTĘPNY": return "bg-green-300";
            case "ZARAZ WRACAM": return "bg-yellow-300";
            case "NIE PRZESZKADZAĆ": return "bg-red-300";
            case "NIEWIDOCZNY": return "bg-gray-300";
            default : return null; // jak coś się popsuje
        }
    },[idStatusu]);
    return(<option value={idStatusu} className={classNameStatusu}>{nazwaStatusu}</option>)

}


