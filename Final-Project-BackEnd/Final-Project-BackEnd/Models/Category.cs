using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Final_Project_BackEnd.Models
{
    public class Category : BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        public IEnumerable<Product>? Products { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
