import AwatarComponent from "./AwatarComponent";
import {CLIENT_URL} from "../config/api";

export default function ZnajomyNaLiscieKomponent({znajomy}) {


    const className = "flex flex-row items-center text-3xl gap-3 p-2 border-b-2 border-gray-400 shadow-md "

    if(znajomy.czySaNoweWiadomosci) return (
        <li key={znajomy.idZnajomego}
            className={className + "text-red-500 bg-red-100 font-bold"}>
            <AwatarComponent
                obraz={znajomy.awatar}
                wysokosc={100}
                pseudonim={znajomy.pseudonim}
                status={znajomy.nazwaStatusu}
            />
            <a href={`${CLIENT_URL}/profil/`+ znajomy.idZnajomego}>{znajomy.pseudonim}</a>
        </li>
    )


    return (<li key={znajomy.idZnajomego} className={className}>
        <AwatarComponent
            obraz={znajomy.awatar}
            wysokosc={100}
            pseudonim={znajomy.pseudonim}
            status={znajomy.nazwaStatusu}
        />
        <a href={`${CLIENT_URL}/profil/`+ znajomy.idZnajomego}>{znajomy.pseudonim}</a>
    </li>);
}
