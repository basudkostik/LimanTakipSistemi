using LimanTakipSistemi.API.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace LimanTakipSistemi.API.Models.DTOs.Port
{
    public class PortDto
    {
        public int PortId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}
