import AwatarComponent from "./AwatarComponent";
import {CLIENT_URL} from "../config/api";

export default function ZnajomyNaLiscieKomponent({znajomy, ustawIdZnajomegoZOtwartymCzatem, idZnajomegoZOtwartymCzatem}) {


    const className = "flex flex-row items-center text-3xl gap-3 p-2 border-b-2 border-gray-400 shadow-md "

    if(idZnajomegoZOtwartymCzatem === znajomy.idZnajomego) return (
        <li key={znajomy.idZnajomego}
            className={className + "bg-blue-300 font-semibold"}>
            <AwatarComponent
                obraz={znajomy.awatar}
                wysokosc={100}
                pseudonim={znajomy.pseudonim}
                status={znajomy.nazwaStatusu}
            />
            <a href={`${CLIENT_URL}/profil/`+ znajomy.idZnajomego}>{znajomy.pseudonim}</a>
        </li>
    )

    if(znajomy.czySaNoweWiadomosci) return (
        <li key={znajomy.idZnajomego}
            className={className + "text-red-500 bg-red-100 font-bold"}
            onClick={() => ustawIdZnajomegoZOtwartymCzatem(znajomy.idZnajomego)}
        >
            <AwatarComponent
                obraz={znajomy.awatar}
                wysokosc={100}
                pseudonim={znajomy.pseudonim}
                status={znajomy.nazwaStatusu}
            />
            <a href={`${CLIENT_URL}/profil/`+ znajomy.idZnajomego}>{znajomy.pseudonim}</a>
        </li>
    )
    // wygląda inaczej, jeśli ma otwarty czat


    return (<li key={znajomy.idZnajomego} className={className} onClick={() => ustawIdZnajomegoZOtwartymCzatem(znajomy.idZnajomego)}>
        <AwatarComponent
            obraz={znajomy.awatar}
            wysokosc={100}
            pseudonim={znajomy.pseudonim}
            status={znajomy.nazwaStatusu}
        />
        <a href={`${CLIENT_URL}/profil/`+ znajomy.idZnajomego}>{znajomy.pseudonim}</a>
    </li>);
}
