using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimanTakipSistemi.API.Models.Domain
{
    public class ShipVisit
    {

        [Key]
        public int VisitId { get; set; }

        [ForeignKey("ShipId")]
        public Ship Ship { get; set; }
        public int ShipId { get; set; }

        [ForeignKey("PortId")]
        public Port Port { get; set; }
        public int PortId { get; set; }

        public DateTime ArrivalDate {  get ; set; }

        public DateTime DepartureDate { get; set; }


        [StringLength(100)]
        public string Purpose { get; set; }


       
      
    }
}
