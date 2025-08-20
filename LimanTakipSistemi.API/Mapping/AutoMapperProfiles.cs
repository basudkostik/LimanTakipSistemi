using AutoMapper;
using LimanTakipSistemi.API.Models.Domain;
using LimanTakipSistemi.API.Models.DTOs.Cargo;
using LimanTakipSistemi.API.Models.DTOs.CrewMember;
using LimanTakipSistemi.API.Models.DTOs.Port;
using LimanTakipSistemi.API.Models.DTOs.Ship;
using LimanTakipSistemi.API.Models.DTOs.ShipCrewAssignment;
using LimanTakipSistemi.API.Models.DTOs.ShipVisit;


namespace LimanTakipSistemi.API.Mapping
{
    
        public class AutoMapperProfiles : Profile
        {
            public AutoMapperProfiles()
            {
                CreateMap<Cargo, CargoDto>().ReverseMap();
                CreateMap<Cargo, UpdateCargoRequestDto>().ReverseMap();
                CreateMap<Cargo, AddCargoRequestDto>().ReverseMap();

                CreateMap<CrewMember , CrewMemberDto>().ReverseMap();
                CreateMap<CrewMember, UpdateCrewMemberRequestDto>().ReverseMap();
                CreateMap<CrewMember, AddCrewMemberRequestDto>().ReverseMap();

                CreateMap<Port , PortDto>().ReverseMap();
                CreateMap<Port, UpdatePortRequestDto>().ReverseMap();
                CreateMap<Port, AddPortRequestDto>().ReverseMap();

                CreateMap<Ship, ShipDto>().ReverseMap();
                CreateMap<Ship, UpdateShipRequestDto>().ReverseMap();
                CreateMap<Ship, AddShipRequestDto>().ReverseMap();


                CreateMap<ShipCrewAssignment , ShipCrewAssignmentDto>().ReverseMap();
                CreateMap<ShipCrewAssignment, AddShipCrewAssignmentRequestDto>().ReverseMap();
                CreateMap<ShipCrewAssignment, UpdateShipCrewAssignmentRequestDto>().ReverseMap();

                CreateMap<ShipVisit , ShipVisitDto>().ReverseMap();
                CreateMap<ShipVisit, AddShipVisitRequestDto>().ReverseMap();
                CreateMap<ShipVisit, UpdateShipVisitRequestDto>().ReverseMap();


        }
        }
     
}
