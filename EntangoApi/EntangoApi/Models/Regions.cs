using Microsoft.EntityFrameworkCore;

namespace EntangoApi.Models
{
    public class Regions
    {
        public int Id { get; set; }
        public string? Code { get; set; } // codice regione ISTAT
        public string? Name { get; set; } // nome regione

        // completare...
        public DateTime UpdateOn { get; set; }
    }

    class RegionsDb : DbContext
    {
        public RegionsDb(DbContextOptions<RegionsDb> options)
            : base(options) { }

        public DbSet<Regions> Regions => Set<Regions>();
    }
}
