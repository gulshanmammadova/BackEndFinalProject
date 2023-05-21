using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Final_Project_BackEnd.Models
{
    public class Blog:BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; }
        [StringLength(255)]

        public string? LittleDesc { get; set; }
        public string? Description { get; set; }

        public IEnumerable<Comment>? Comments { get; set; }

        public string? MainImage { get; set; }
        public string? CreatorImage { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
