namespace Squadra.Server.Modules.Powiadomienia.Enums;

public enum TypPowiadomieniaEnum
{
    Systemowe = 1, // brak powiązanych obiektów, tylko treść powiadomienia
    ZaproszenieDoZnajomych = 2, // jedynym powiązanym obiektem jest nadawca.
    PrzyjecieZaproszeniaDoZnajomych = 3, // jedynym powiązanym obiektem jest drugi użytkownik.
    OdrzucenieZaproszeniaDoZnajomych = 4, // jedynym powiązanym obiektem jest drugi użytkownik.
    UsuniecieZnajomosci = 5, // jedynym powiązanym obiektem jest drugi użytkownik.
    UzytkownikDolaczylDoDruzyny = 6, // pierwszym powiązanym obiektem jest użytkownik, drugim powiązanym obiektem jest drużyna. W treści jest rola miejsca
    UsuniecieZDruzyny = 7, // jedynym powiazanym obiektem jest drużyna
    ZaproszenieDoDruzyny = 8, // pierwszym powiązanym obiektem jest drużyna, drugim jest miejsce (bez nazwy). W treści jest rola miejsca
    PrzyjecieZaproszeniaDoDruzyny = 9, // pierwszym obiektem jest użytkownik, drugim drużyna
    OdrzucenieZaproszeniaDoDruzyny = 10, // pierwszym obiektem jest użytkownik, drugim drużyna
    UzytkownikOpuscilDruzyne = 13, // pierwszym obiektem jest użytkownik, drugim drużyna
    DruzynaZostalaRozwiazana = 14 // jedynym powiązanym obiektem jest drużyna, dokładnie to jej nazwa, bo sama drużyna już nie istnieje
}