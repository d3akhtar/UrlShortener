using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Model
{
    public class UrlCode
    {
        [Key]
        public string Code { get; set; }
        public string Url { get; set; }
        public string PngQrCodeImage { get; set; }
        public string SvgQrCodeImage { get; set; }
        public string AsciiQrCodeRepresentation { get; set; }
    }
}
