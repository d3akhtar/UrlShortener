using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Model
{
    public class UrlCodeKey
    {
        [Key]
        public string Key { get; set; }
        public string LastUsedValue { get; set; }
    }
}
