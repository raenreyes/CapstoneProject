using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models.VM
{
    public class TicketVM
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
