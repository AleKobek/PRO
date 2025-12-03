namespace Squadra.Server.Services;

// Reprezentuje pojedynczy błąd:
//      Message (treść),
//      Field (opcjonalna nazwa pola, np. do ModelState),
//      Code (opcjonalny kod błędu biznesowego).
public sealed record ErrorItem(string Message, string? Field = null, string? Code = null);

// Reprezentuje wynik operacji w service
public sealed class ServiceResult<T>
{
    public bool Succeeded { get; init; } // informuje, czy operacja zakończyła się sukcesem.

    public T? Value { get; init; } // to co byśmy normalnie zwracali (np. DTO utworzonego użytkownika). Ustawiany tylko przy sukcesie

    public int StatusCode { get; init; } // sugerowany kod HTTP (np. 200, 201, 400, 404, 409)

    
    // lista błędów. Domyślnie pusta (nigdy null), dzięki czemu nie trzeba sprawdzać nulli
    public IReadOnlyList<ErrorItem> Errors { get; init; } = [];

    
    // Metody fabrykujące
    
    // tworzy wynik sukcesu z podanym Value i kodem (domyślnie 200; można podać 201, gdy coś utworzono, 204 no content gdy coś np usunięto)
    public static ServiceResult<T> Ok(T value, int statusCode = 200) =>
        new() { Succeeded = true, Value = value, StatusCode = statusCode };

    // tworzy wynik porażki z kodem i listą błędów (materializowaną ToArray, by zapewnić niezmienność)
    public static ServiceResult<T> Fail(int statusCode, IEnumerable<ErrorItem> errors) =>
        new() { Succeeded = false, StatusCode = statusCode, Errors = errors.ToArray() };

    
    // skróty do Fail z odpowiednimi kodami 400, 401, 403, 404, 409
    public static ServiceResult<T> BadRequest(params ErrorItem[] errors) => Fail(400, errors);
    public static ServiceResult<T> Unauthorized(params ErrorItem[] errors) => Fail(401, errors);
    public static ServiceResult<T> Forbidden(params ErrorItem[] errors) => Fail(403, errors);
    public static ServiceResult<T> Conflict(params ErrorItem[] errors) => Fail(409, errors);
    public static ServiceResult<T> NotFound(params ErrorItem[] errors) => Fail(404, errors);
}
