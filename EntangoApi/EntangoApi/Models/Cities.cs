﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EntangoApi.Models
{
    public class Cities
    {
        public int Id { get; set; }
        public string? Code { get; set; } // codice comune progressivo
        public string? Name { get; set; } // nome comune
        public string? CountryCode { get; set; } // unità terrirotiale
        public string? IstatCode { get; set; } // ProvinceCode + Code
        public string? RegionCode { get; set; }
        public string? RegionDescription { get; set; }
        public string? ProvinceCode { get; set; }
        public string? ProvinceDescription { get; set; } // Non presente nel tracciato ISTAT
        public string? ProvinceAbbreviation { get; set; }
        public string? GeographicCode { get; set; }
        public string? GeographicDescription { get; set; }
        public string? LandRegistryCode { get; set; } // codic catastale
        public DateTime UpdateOn { get; set; }
    }

    class CitiesDb : DbContext
    {
        public CitiesDb(DbContextOptions<CitiesDb> options)
            : base(options) { }

        public DbSet<Cities> Cities => Set<Cities>();
    }
}
