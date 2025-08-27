using AutoMapper;
using LimanTakipSistemi.API.Mapping;

namespace LimanTakipSistemi.Test
{
    public class TestBase
    {
        protected readonly IMapper Mapper;

        public TestBase()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfiles>();
            });
            Mapper = config.CreateMapper();
        }
    }
}
