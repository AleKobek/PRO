import React from "react";

export const OkienkoTlumaczaceZintegrowanie = (ref, ustawPokazOkienkoTlumaczenia) => <div
    ref={ref}
    className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] pt-2 p-10 overflow-y-auto
                rounded-md shadow-lg justify-center items-center bg-amber-50 border-2 border-amber-400"
    style={{zIndex: 2005}}
>
    <div className="flex justify-end">
        <button onClick={() => ustawPokazOkienkoTlumaczenia(false)} className="cursor-pointer text-red-600">Zamknij</button>
    </div>
    <div className="flex flex-col">
        <h2 className="text-2xl font-bold mb-4">Łączenie konta i zintegrowane dane</h2>
        <p className="mb-4">
            Użytkownik połączyć swoje konto z kontem na zewnętrznym serwisie, pobierającym statystyki dotyczące gier użytkownika, wybierając tę opcję na stronie "edytuj konto".
            Pojęcie "zintegrowane dane" oznacza dane pobrane z zewnętrznego serwisu.
            Bez danych z zewnętrznego konta nie da się ustawiać wymagań w drużynie ani ich spełniać, ale
            wciąż dostępne jest tworzenie i dołączanie do drużyn niewymagających zintegrowanych danych.
            Zintegrowane dane są też potrzebne do wyświetlenia biblioteki gier użytkownika na stronie jego profilu.
        </p>
    </div>
</div>