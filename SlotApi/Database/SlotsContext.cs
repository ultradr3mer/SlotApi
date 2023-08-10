using System.Collections.Generic;
using System.Data.Common;
using System;
using Microsoft.EntityFrameworkCore;

namespace SlotApi.Database
{
  public class SlotsContext : DbContext
  {
    public SlotsContext()
    { }

    //public ZiviContext(string? connectionString) : base(connectionString)
    //{
    //}

    //public ZiviContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
    //{
    //}

    //public DbSet<DiscordUser> User { get; set; }
    //public DbSet<SavedSeeds> SavedSeeds { get; set; }
    //public DbSet<Offer> Offers { get; set; }
    //public DbSet<GrowTent> GrowTents { get; set; }
    //public DbSet<GrowingPlant> GrowingPlants { get; set; }
  }
}
