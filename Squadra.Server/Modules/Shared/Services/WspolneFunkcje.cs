using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Squadra.Server.Modules.Shared.Services;

public static class WspolneFunkcje
{
    // to będzie zawsze JPEG o rozmiarze 256 x 256
    public static byte[]? NormalizujObraz(byte[]? awatar)
    {
        if(awatar == null || awatar.Length == 0) return null;
        
        using var image = Image.Load(awatar); // wykrywa PNG/JPG/WEBP automatycznie, jak to nie jest obraz rzuci wyjątkiem

        // Skalowanie do kwadratu np. 256x256 (zachowa proporcje)
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(256, 256),
            Mode = ResizeMode.Crop // wypełnia cały kwadrat
        }));

        // Konwersja do JPEG
        using var ms = new MemoryStream();
        image.Save(ms, new JpegEncoder
        {
            Quality = 85 // 70–90 to najlepszy kompromis
        });

        return ms.ToArray();
    }
    
    public static async Task<byte[]> NormalizujObraz(IFormFile obraz)
    {
        using (var memoryStream = new MemoryStream())
        {
            // przenosimy to do memory stream
            await obraz.CopyToAsync(memoryStream);
            // przenosimy to do tablicy bajtów, tak jak chcemy to mieć
            var obrazWBajtach = memoryStream.ToArray();
            return NormalizujObraz(obrazWBajtach) ?? [];
        }
    }
}