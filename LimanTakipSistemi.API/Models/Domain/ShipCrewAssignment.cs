using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimanTakipSistemi.API.Models.Domain
{
    public class ShipCrewAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [ForeignKey("ShipId")]
        public Ship Ship { get; set; }
        public int ShipId { get; set; }


        [ForeignKey("CrewId")]
        public CrewMember CrewMember { get; set; }

        public int CrewId { get; set; }


        public DateTime AssignmentDate { get; set; }


    }
}
