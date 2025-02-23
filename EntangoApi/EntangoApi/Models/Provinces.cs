using Microsoft.EntityFrameworkCore;

namespace EntangoApi.Models
{
    public class Provinces
    {
        public int Id { get; set; }
        public string? Code { get; set; } // codice regione ISTAT
        public string? Name { get; set; } // nome regione

        // completare...
        public DateTime UpdateOn { get; set; }
    }
    class ProvincesDb : DbContext
    {
        public ProvincesDb(DbContextOptions<ProvincesDb> options)
            : base(options) { }

        public DbSet<Provinces> Regions => Set<Provinces>();
    }
}
