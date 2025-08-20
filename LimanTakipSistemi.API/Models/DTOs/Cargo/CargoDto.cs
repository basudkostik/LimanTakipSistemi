using LimanTakipSistemi.API.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimanTakipSistemi.API.Models.DTOs.Cargo
{
    public class CargoDto
    {
        public int CargoId { get; set; }

        public int ShipId { get; set; }

        public string Description { get; set; }
        
        public decimal Weight { get; set; }

        public string CargoType { get; set; }
    }
}
