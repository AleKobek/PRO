import React from "react";
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
        bladPseudonimu,
        bladLoginu,
        bladEmaila,
        bladNumeruTelefonu,
        bladDatyUrodzenia,
    }) {
    return(<>
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
    </>)
}