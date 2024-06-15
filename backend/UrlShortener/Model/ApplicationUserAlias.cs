using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Model
{
    public class ApplicationUserAlias
    {
        [Key]
        public int Id { get; set; }
        public string AliasCode { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("AliasCode")]
        public Alias Alias { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
