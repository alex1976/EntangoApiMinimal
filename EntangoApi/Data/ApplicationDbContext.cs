using Microsoft.EntityFrameworkCore;
using EntangoApi.Models;

namespace EntangoApi;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<PriceList> PriceLists { get; set; } = null!;
    public DbSet<DocumentStore> DocumentStores { get; set; } = null!;
}