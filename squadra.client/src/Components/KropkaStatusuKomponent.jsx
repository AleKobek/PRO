import {useMemo} from "react";

export default function KropkaStatusuKomponent({status}){

    const classNameStatusu = useMemo(() => {
        const Status = {
            1: "DOSTĘPNY",
            2: "ZARAZ WRACAM",
            3: "NIE PRZESZKADZAĆ",
            5: "OFFLINE"
        }
        const tempStatus = Status[status]
        // dostajemy nazwę (w awatar komponent)
        if(!tempStatus)
            switch(status){
                case "Dostępny": return "bg-green-500";
                case "Zaraz wracam": return "bg-yellow-400";
                case "Nie przeszkadzać": return "bg-red-500";
                case "Offline": return "bg-gray-400";
                default : return null; // jak nie ma statusu, nic nie rysujemy
            }
        // dostajemy id
        switch(tempStatus){
            case "DOSTĘPNY": return "bg-green-500";
            case "ZARAZ WRACAM": return "bg-yellow-400";
            case "NIE PRZESZKADZAĆ": return "bg-red-500";
            case "OFFLINE": return "bg-gray-400";
            default : return null; // jak coś się popsuje
        }
    },[status]);

    const kropkaClass = `absolute bottom-0 right-0 translate-x-1/4 translate-y-1/4 h-6 w-6 rounded-full border border-black ${classNameStatusu}`;

    return(<span className={status && kropkaClass}/>)
}
