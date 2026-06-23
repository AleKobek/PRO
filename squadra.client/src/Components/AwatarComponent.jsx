import KropkaStatusuKomponent from "./KropkaStatusuKomponent";

export default function AwatarComponent({obraz, wysokosc, status}) {

    const kontenerClass = "relative inline-block";

    const imgSrc = obraz === "" || obraz === null
        ? "/img/domyslny_awatar.png"
        : obraz === "puste"
            ? "/img/puste_miejsce.png"
            : "data:image/jpeg;base64," + obraz;

    const gruboscObramowania =
        wysokosc > 40
        ? wysokosc > 70
            ? "border-4"
            : "border-2"
        : "border";
    const kolorObramowania = obraz === "puste" ? " border-blue-900" : " border-black";

    return (
        <span className={kontenerClass} style={{ width: wysokosc, height: wysokosc }}>
            <img
                src={imgSrc}
                alt="awatar"
                className={"awatar block h-full w-full object-cover rounded-full " + gruboscObramowania + kolorObramowania}
            />
            <KropkaStatusuKomponent status={status} wysokoscAwatara={wysokosc}/>
        </span>
    );
}