namespace Squadra.Server.Modules.Powiadomienia.Enums;

public enum TypPowiadomieniaEnum
{
    Systemowe = 1, // brak powiązanych obiektów, tylko treść powiadomienia
    ZaproszenieDoZnajomych = 2, // jedynym powiązanym obiektem jest nadawca.
    PrzyjecieZaproszeniaDoZnajomych = 3, // jedynym powiązanym obiektem jest drugi użytkownik.
    OdrzucenieZaproszeniaDoZnajomych = 4, // jedynym powiązanym obiektem jest drugi użytkownik.
    UsuniecieZnajomosci = 5, // jedynym powiązanym obiektem jest drugi użytkownik.
    UzytkownikDolaczylDoDruzyny = 6, // pierwszym powiązanym obiektem jest użytkownik, drugim powiązanym obiektem jest drużyna. W treści jest rola miejsca
    UsuniecieZDruzyny = 7,
    ZaproszenieDoDruzyny = 8, // pierwszym powiązanym obiektem jest drużyna, drugim jest miejsce (bez nazwy). W treści jest rola miejsca
    PrzyjecieZaproszeniaDoDruzyny = 9,
    OdrzucenieZaproszeniaDoDruzyny = 10
}