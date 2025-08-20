using System.ComponentModel.DataAnnotations;

namespace LimanTakipSistemi.API.Models.DTOs.ShipVisit
{
    public class UpdateShipVisitRequestDto
    {

        public int ShipId { get; set; }


        public int PortId { get; set; }

        public DateTime ArrivalDate { get; set; }

        public DateTime DepartureDate { get; set; }


        [StringLength(100)]
        public string Purpose { get; set; }
    }
}
