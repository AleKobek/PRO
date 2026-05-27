import {useMemo} from "react";

export default function MiniAwatarKomponent({obraz, status = null}) {

    const imgSrc = obraz === ""
        ? "/img/domyslny_awatar.png"
        : obraz === "puste"
            ? "/img/puste_miejsce.png"
            : "data:image/jpeg;base64," + obraz;

    if(status === null)
        return <img
            src={imgSrc}
            alt="awatar"
            className="awatar block h-10 w-10 object-cover rounded-full border border-3 border-blue-900"
        />;

    return (
        <span className="relative inline-block h-10 w-10 mb-2">
            <img
                src={imgSrc}
                alt="awatar"
                className="awatar block h-full w-full object-cover rounded-full border border-3 border-black"
            />
            <MiniKropkaStatusuKomponent status={status}/>
        </span>
    );
}

function MiniKropkaStatusuKomponent({status}){

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

    const kropkaClass = `absolute bottom-0 right-0 translate-x-1/4 translate-y-1/4 h-3 w-3 rounded-full border border-black ${classNameStatusu}`;

    return(<span className={status && kropkaClass}/>)
}