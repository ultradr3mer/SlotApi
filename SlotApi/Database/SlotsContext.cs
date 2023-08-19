using Microsoft.EntityFrameworkCore;

namespace SlotApi.Database
{
  public class SlotsContext : DbContext
  {
    public SlotsContext(string? connectionString) : base(new DbContextOptions<SlotsContext>())
    {
    }

    public SlotsContext(DbContextOptions<SlotsContext> options) : base(options)
    { }

    public DbSet<DiscordUser> User { get; set; }
    public DbSet<SlotSpin> SlotSpins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      var decimalProps = modelBuilder.Model
       .GetEntityTypes()
       .SelectMany(t => t.GetProperties())
       .Where(p => (System.Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType) == typeof(decimal));

      foreach (var property in decimalProps)
      {
        property.SetPrecision(18);
        property.SetScale(2);
      }
    }
  }
}