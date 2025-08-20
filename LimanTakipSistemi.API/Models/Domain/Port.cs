using System.ComponentModel.DataAnnotations;

namespace LimanTakipSistemi.API.Models.Domain
{
    public class Port
    {
        [Key]
        public int PortId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        [Required]
        [StringLength(50)]
        public string City  { get; set; }

        public ICollection<ShipVisit> ShipVisits { get; set; } = new List<ShipVisit>(); 
    }
}
