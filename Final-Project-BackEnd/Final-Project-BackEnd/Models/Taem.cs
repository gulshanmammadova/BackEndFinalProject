using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project_BackEnd.Models
{
    public class Team:BaseEntity
    {

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]

        public string? Instagram { get; set; }

        [StringLength(255)]

        public string? Facebook { get; set; }

        [StringLength(255)]

        public string? Position { get; set; }
        [StringLength(255)]

        public string? Twitter { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
