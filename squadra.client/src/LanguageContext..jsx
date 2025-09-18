import React, { createContext, useContext, useMemo, useState } from 'react';
import jezyki from './jezyki';

const LanguageContext = createContext(null);

export function LanguageProvider({ children }) {
    const [kod, ustawKod] = useState('pl'); // domyślnie polski

    const value = useMemo(() => {
        const jezyk = jezyki[kod] ?? jezyki.pl;
        return { kod, ustawKod: ustawKod, jezyk };
    }, [kod]);

    return <LanguageContext.Provider value={value}>{children}</LanguageContext.Provider>;
}

export function useJezyk() {
    const ctx = useContext(LanguageContext);
    if (!ctx) {
        throw new Error('useJezyk musi być użyty wewnątrz <LanguageProvider>');
    }
    return ctx; // { kod, setKod, jezyk }
}