using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Final_Project_BackEnd.Models
{
    public class Product : BaseEntity
    {

        [StringLength(255)]
        public string Title { get; set; }
        [Column(TypeName = "money")]
        public double Price { get; set; }
        [Column(TypeName = "money")]
        public double DiscountedPrice { get; set; }
        [Column(TypeName = "money")]
        public int Count { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        [StringLength(4, MinimumLength = 4)]
        public string? Seria { get; set; }
        public int? Code { get; set; }
        public bool IsNewArrival { get; set; }
        public bool IsSpecialSale { get; set; }
        public bool IsFeatured { get; set; }
        [StringLength(255)]
        public string? MainImage { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        [NotMapped]
        public IEnumerable<IFormFile>? Files { get; set; }
        [NotMapped]
        public IFormFile? MainFile { get; set; }

        public IEnumerable<Review>? Reviews { get; set; }

        public IEnumerable<Basket>? Baskets { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

    }
}
