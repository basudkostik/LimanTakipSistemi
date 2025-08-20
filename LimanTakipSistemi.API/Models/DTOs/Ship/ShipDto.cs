using LimanTakipSistemi.API.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace LimanTakipSistemi.API.Models.DTOs.Ship
{
    public class ShipDto
    {
        public int ShipId { get; set; }
        public string Name { get; set; }
        public string IMO { get; set; }
        public string Type { get; set; }
        public string Flag { get; set; }
        public int YearBuilt { get; set; }
    }
}
