using H4WebApi.Models.BaseInfo;
using H4WebApi.Models.Work;
using Microsoft.EntityFrameworkCore;
using NFCWebApi.Models;

namespace H4WebApi.Models
{
    public class H4WebContext : DbContext
    {
        public H4WebContext(DbContextOptions<H4WebContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Driver> Driver { get; set; }
        public DbSet<OilStation> OilStation { get; set; }
        public DbSet<Truck> Truck { get; set; }

        public DbSet<WorkTicket> WorkTicket { get; set; }

        public DbSet<Device> Device { get; set; }
        public DbSet<Inspect> Inspect { get; set; }
        public DbSet<InspectLine> InspectLine { get; set; }
        public DbSet<InspectCycles> InspectCycles { get; set; }
        public DbSet<InspectData> InspectData { get; set; }
        public DbSet<InspectItems> InspectItems { get; set; }
        public DbSet<InspectTask> InspectTask { get; set; }
        public DbSet<NFCCard> NFCCard { get; set; }
        public DbSet<DeviceInspectItem> DeviceInspectItem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

