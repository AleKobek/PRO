import {useMemo} from "react";

export default function AwatarComponent({obraz, wysokosc, status}) {


    const classNameStatusu = useMemo(() => {
        switch(status){
            case "Dostępny": return "bg-green-500";
            case "Zaraz wracam": return "bg-yellow-400";
            case "Nie przeszkadzać": return "bg-red-500";
            case "Offline": return "bg-gray-400";
            default : return "bg-gray-400";
        }
    },[status]);

    const kontenerClass = "relative inline-block";
    const kropkaClass = `absolute bottom-0 right-0 translate-x-1/4 translate-y-1/4 h-6 w-6 rounded-full border border-black ${classNameStatusu}`;

    const imgSrc = obraz === "" ? "/img/domyslny_awatar.png" : "data:image/jpeg;base64," + obraz;

    return (
        <span className={kontenerClass} style={{ width: wysokosc, height: wysokosc }}>
            <img
                src={imgSrc}
                alt="awatar"
                className="awatar block h-full w-full object-cover rounded-full border-4 border-black"
            />
            <span className={kropkaClass} />
        </span>
    );
}