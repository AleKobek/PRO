import KropkaStatusuKomponent from "./KropkaStatusuKomponent";

export default function AwatarComponent({obraz, wysokosc, status}) {

    const kontenerClass = "relative inline-block";

    const imgSrc = obraz === "" ? "/img/domyslny_awatar.png" : "data:image/jpeg;base64," + obraz;

    return (
        <span className={kontenerClass} style={{ width: wysokosc, height: wysokosc }}>
            <img
                src={imgSrc}
                alt="awatar"
                className="awatar block h-full w-full object-cover rounded-full border-4 border-black"
            />
            <KropkaStatusuKomponent status={status}/>
        </span>
    );
}