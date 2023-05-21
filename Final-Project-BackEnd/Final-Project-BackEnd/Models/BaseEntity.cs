using System.ComponentModel.DataAnnotations;

namespace Final_Project_BackEnd.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [StringLength(255)]
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        [StringLength(255)]
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [StringLength(255)]
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
