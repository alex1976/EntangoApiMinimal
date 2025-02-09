using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EntangoApi.Models
{
    public class ConstructionSites
    {
        public int Id { get; set; }
        public string? Code { get; set; } // codice cantiere
        public string? Description { get; set; } // descrizione cantiere
        public string? Client { get; set; } // cliente
        public string? Contractor { get; set; } // appaltatore
        public decimal Latitde { get; set; }
        public decimal Longitde { get; set; }
        public string? Address { get; set; }
        public string? Location { get; set; }
        public decimal WorksAmount { get; set; }
        public DateTime WorksStartDate { get; set; }
        public DateTime WorksEndDate { get; set; }
        public decimal Progress { get; set; }
        public Cities Town { get; set; } // comune del cantiere
        public DateTime UpdateOn { get; set; }
    }

    class ConstructionSiteDb : DbContext
    {
        public ConstructionSiteDb(DbContextOptions<ConstructionSiteDb> options)
            : base(options) { }

        public DbSet<ConstructionSites> ConstructionSites => Set<ConstructionSites>();
    }
}
