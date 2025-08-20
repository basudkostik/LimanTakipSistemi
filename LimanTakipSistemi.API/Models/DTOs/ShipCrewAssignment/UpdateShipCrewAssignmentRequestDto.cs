using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimanTakipSistemi.API.Models.DTOs.ShipCrewAssignment
{
    public class UpdateShipCrewAssignmentRequestDto
    {
        
        public int ShipId { get; set; }


       

        public int CrewId { get; set; }


        public DateTime AssignmentDate { get; set; }
    }
}
