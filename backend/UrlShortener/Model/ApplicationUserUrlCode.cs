using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Model
{
    public class ApplicationUserUrlCode
    {
        [Key]
        public int Id { get; set; }
        public string UrlCodeCode { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("UrlCodeCode")]
        public UrlCode UrlCode { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
