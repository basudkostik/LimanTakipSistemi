using System.ComponentModel.DataAnnotations;

namespace LimanTakipSistemi.API.Models.DTOs.CrewMember
{
    public class CrewMemberDto
    {
        public int CrewId { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }  
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}
