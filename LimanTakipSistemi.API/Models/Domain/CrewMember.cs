using System.ComponentModel.DataAnnotations;

namespace LimanTakipSistemi.API.Models.Domain
{
    public class CrewMember
    {
        [Key]
        public int CrewId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required,StringLength(50)]
        public string LastName { get; set; }

        [Required , StringLength(100) , EmailAddress] 
        public string Email { get; set; }

        [Required , StringLength (20) ,  Phone]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        [Required]
        public string Role { get; set; }
    }
}
