using LimanTakipSistemi.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LimanTakipSistemi.API.Data
{
    public class LimanTakipDbContext : DbContext
    {
        public LimanTakipDbContext(DbContextOptions dbContextOptions):base(dbContextOptions)
        {
        }
        public DbSet<Cargo> Cargoes { get; set; }
        public DbSet<CrewMember> CrewMembers { get; set; }
        public DbSet<Port> Ports { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<ShipCrewAssignment> ShipCrewAssignments { get; set; }
        public DbSet<ShipVisit> ShipVisits { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ship>().HasIndex(s => s.IMO).IsUnique();
            modelBuilder.Entity<ShipCrewAssignment>().HasIndex(a => new { a.ShipId, a.CrewId, a.AssignmentDate }).IsUnique();

        }
    }
}
