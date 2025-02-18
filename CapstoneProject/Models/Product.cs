using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        public string ProductDescription { get; set; } = string.Empty;
        [Required]
        public decimal ProductPrice { get; set; }
        [Required]
        public string ProductImage { get; set; } = string.Empty;
    }
}
