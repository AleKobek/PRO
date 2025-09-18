import { useJezyk } from "../LanguageContext.";

export default function JezykNaLiscieKomponent({
                                                   jezykDoKomponentu,
                                                   coPrzyKlikaniu,
                                                   idZListy,
                                                   czyEdytuj,
                                               }) {
    const { jezyk } = useJezyk();

    // Zwracamy jeden, spójny element (li), a przycisk umieszczamy w środku.
    // Dzięki temu rodzic może bez problemu nadać key na komponencie.
    return (
        <li key={idZListy - jezykDoKomponentu?.idJezyka}>
      <span>
        {jezykDoKomponentu?.nazwaJezyka} : {jezykDoKomponentu?.nazwaStopnia}
      </span>
            {czyEdytuj && (
                <button type="button" id={idZListy} onClick={coPrzyKlikaniu}>
                    {jezyk.usun}
                </button>
            )}
        </li>
    );
}