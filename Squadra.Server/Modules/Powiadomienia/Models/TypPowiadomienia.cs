namespace Squadra.Server.Modules.Powiadomienia.Models;

public class TypPowiadomienia
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = null!;
    
    public virtual ICollection<Powiadomienie> PowiadomienieCollection { get; set; } = null!;

}

/*
 
 Typy jak na razie:
    -> systemowe - nie mają id powiązanego obiektu, bo są od systemu do użytkownika. po prostu przekazywana jest treść
        -> np powiadomienia odnośnie odrzucenia zaproszenia do znajomych, też chyba nie trzeba robić powiązania
        -> interakcja to usuń
    -> zaproszenia do znajomych - powiązane z obiektem innego użytkownika, 
        -> interakcja to odpowiedź tak lub nie
 */