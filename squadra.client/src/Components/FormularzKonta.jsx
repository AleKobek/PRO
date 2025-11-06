import React, {useMemo} from "react";
export default function FormularzKonta({ // pamiętać, aby dać nawias klamrowy!
        // trochę dużo tego >_<
        pseudonim,
        ustawPseudonim,
        login,
        ustawLogin,
        email,
        ustawEmail,
        dataUrodzenia,
        ustawDateUrodzenia,
        numerTelefonu,
        ustawNumerTelefonu,
        haslo1,
        ustawHaslo1,
        haslo2,
        ustawHaslo2,
        czySieWysyla,
        bladPseudonimu,
        bladLoginu,
        bladEmaila,
        bladHasla,
        bladNumeruTelefonu,
        bladDatyUrodzenia,
        bladOgolny,
        przyWysylaniu
    }) {
    
    const czyZablokowaneWyslij = useMemo(() =>{
        return(
            !pseudonim ||
            !login ||
            !email ||
            !haslo1||
            !haslo2 ||
            !dataUrodzenia ||
            czySieWysyla    
        )
    },[pseudonim, email, dataUrodzenia, haslo1, haslo2, czySieWysyla]);


    return(<>
        <form id = "form" name= "formularz-rejestracji">

            <label>
                Login <br/>
                <input
                    type="text"
                    value={login}
                    onChange={(e) => ustawLogin(e.target.value)}
                    placeholder="np. gamer123"
                    autoComplete="username"
                />
            </label><br/>
            <span id = "error-login" className="error-wiadomosc">{bladLoginu}</span><br/>
            
            <label>
                Pseudonim (maks. 20 znaków)<br/>
                <input
                    type="text"
                    value={pseudonim}
                    onChange={(e) => ustawPseudonim(e.target.value)}
                    placeholder="np. ProGamer"
                    maxLength={20}
                />
            </label><br/>
            <span id = "error-pseudonim" className="error-wiadomosc">{bladPseudonimu}</span><br/>

            
            <label>
                E-mail <br/>
                <input
                    type="email"
                    value={email}
                    onChange={(e) => ustawEmail(e.target.value)}
                    placeholder="pro.gamer@squadra.com"
                    autoComplete="email"
                />
            </label><br/>
            <span id = "error-email" className="error-wiadomosc">{bladEmaila}</span><br/>
            

            <label>
                Data urodzenia (min. wiek to 18 lat) <br/>
                <input
                    type="date"
                    name="data-urodzenia"
                    value={dataUrodzenia}
                    onChange={(e) => ustawDateUrodzenia(e.target.value)}
                    id="data-urodzenia" />
            </label><br/>
            <span id = "error-data-urodzenia" className="error-wiadomosc">{bladDatyUrodzenia}</span><br/>

            <label>
                Numer telefonu <br/>
                <input
                    type="text"
                    value={numerTelefonu}
                    onChange={(e) => ustawNumerTelefonu(e.target.value)}
                    placeholder="000-000-000"
                />
            </label><br/>
            <span id = "error-numer-telefonu" className="error-wiadomosc">{bladNumeruTelefonu}</span><br/>


            <br/><label>
                Hasło (Musi mieć dużą literę, małą literę, cyfrę, znak specjalny i minimum 8 znaków) <br/>
                <input
                    type="password"
                    value={haslo1}
                    onChange={(e) => ustawHaslo1(e.target.value)}
                    placeholder="••••••••"
                    autoComplete="new-password"
                />
            </label><br/>

            
            <label>
                Powtórz hasło <br/>
                <input
                    type="password"
                    value={haslo2}
                    onChange={(e) => ustawHaslo2(e.target.value)}
                    placeholder="••••••••"
                    autoComplete="new-password"
                    />
            </label><br/>
            <span id = "error-haslo" className="error-wiadomosc">{bladHasla}</span><br/>

            
            <button
                type="button"
                disabled={czyZablokowaneWyslij}
                onClick={przyWysylaniu}
            >Zarejestruj się</button>
            <span id = "error-ogolny" className="error-wiadomosc">{bladOgolny}</span><br/>
        </form>
    </>)
}