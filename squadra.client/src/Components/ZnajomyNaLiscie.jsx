import Awatar from "./Awatar";

export default function ZnajomyNaLiscie({znajomy, przyWyborzeZnajomego, idZnajomegoZOtwartymCzatem}) {

    const className = "flex flex-row items-center text-3xl gap-3 p-2 border-b-2 border-gray-400 shadow-md "
    const openInNewTab = url => {
        window.open(url, '_blank', 'noopener,noreferrer');
    };

    if(idZnajomegoZOtwartymCzatem === znajomy.idZnajomego) return (
        <li key={znajomy.idZnajomego}
            className={className + "bg-blue-300 font-semibold"}>
            <div
                className="cursor-pointer"
                onClick={() => openInNewTab(`/profil/` + znajomy.idZnajomego)}>
                <Awatar
                    obraz={znajomy.awatar}
                    wysokosc={100}
                    pseudonim={znajomy.pseudonim}
                    status={znajomy.nazwaStatusu}
                />
            </div>
            <span>{znajomy.pseudonim}</span>
        </li>
    )

    if(znajomy.czySaNoweWiadomosci) return (
        <li key={znajomy.idZnajomego}
            className={className + "text-red-500 bg-red-100 font-bold"}
            onClick={() => przyWyborzeZnajomego(znajomy.idZnajomego)}
        >
            <div
                className="cursor-pointer"
                onClick={() => openInNewTab(`/profil/` + znajomy.idZnajomego)}>
                <Awatar
                    obraz={znajomy.awatar}
                    wysokosc={100}
                    pseudonim={znajomy.pseudonim}
                    status={znajomy.nazwaStatusu}
                />
            </div>
            <span>{znajomy.pseudonim}</span>
        </li>
    )
    // wygląda inaczej, jeśli ma otwarty czat


    return (<li key={znajomy.idZnajomego} className={className} onClick={() => przyWyborzeZnajomego(znajomy.idZnajomego)}>
        <div
        className="cursor-pointer"
            onClick={() => openInNewTab(`/profil/` + znajomy.idZnajomego)}>
            <Awatar
                obraz={znajomy.awatar}
                wysokosc={100}
                pseudonim={znajomy.pseudonim}
                status={znajomy.nazwaStatusu}
            />
        </div>
        <span>{znajomy.pseudonim}</span>
    </li>);
}
