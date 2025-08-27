using LimanTakipSistemi.API.Data;
using LimanTakipSistemi.API.Mapping;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using LimanTakipSistemi.API.Swagger;
using LimanTakipSistemi.API.Repositories.CargoRepository;
using LimanTakipSistemi.API.Repositories.CrewMemberRepository;
using LimanTakipSistemi.API.Repositories.PortRepository;
using LimanTakipSistemi.API.Repositories.ShipRepository.cs;
using LimanTakipSistemi.API.Repositories.ShipCrewAssignmentRepository;
using LimanTakipSistemi.API.Repositories.ShipVisitRepository;
using LimanTakipSistemi.API.Services.CargoService;
using LimanTakipSistemi.API.Services.CrewMemberService;
using LimanTakipSistemi.API.Services.PortService;
using LimanTakipSistemi.API.Services.ShipService;
using LimanTakipSistemi.API.Services.ShipCrewAssignmentService;
using LimanTakipSistemi.API.Services.ShipVisitService;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

Env.Load();

builder.Services.AddDbContext<LimanTakipDbContext>(options =>
options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STR")));


builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));


builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); 
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader());
}).AddApiExplorer(option =>
{
    option.GroupNameFormat = "'v'VVV";
    option.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// Register Repositories
builder.Services.AddScoped<ICargoRepository, SQLCargoRepository>();
builder.Services.AddScoped<ICrewMemberRepository, SQLCrewMemberRepository>();
builder.Services.AddScoped<IPortRepository, SQLPortRepository>();
builder.Services.AddScoped<IShipRepository, SQLShipRepository>();
builder.Services.AddScoped<IShipCrewAssignmentRepository, SQLShipCrewAssignmentRepository>();
builder.Services.AddScoped<IShipVisitRepository, SQLShipVisitRepository>();

// Register Services
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<ICrewMemberService, CrewMemberService>();
builder.Services.AddScoped<IPortService, PortService>();
builder.Services.AddScoped<IShipService, ShipService>();
builder.Services.AddScoped<IShipCrewAssignmentService, ShipCrewAssignmentService>();
builder.Services.AddScoped<IShipVisitService, ShipVisitService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
    $"Liman Takip Sistemi API {description.ApiVersion}");

        }
    });
}



app.UseHttpsRedirection();

app.UseRouting();
// Use CORS
app.UseCors("AllowLocalhost5173");

app.UseAuthorization();

app.MapControllers();

app.Run();