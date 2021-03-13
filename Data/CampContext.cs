using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoreCodeCamp.Data
{
    public class CampContext : DbContext
  {
    private readonly IConfiguration _config;

    public CampContext(DbContextOptions options, IConfiguration config) : base(options)
    {
      _config = config;
    }

    public DbSet<Camp> Camps { get; set; }
    public DbSet<Speaker> Speakers { get; set; }
    public DbSet<Talk> Talks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(_config.GetConnectionString("CodeCamp"));
    }

    protected override void OnModelCreating(ModelBuilder bldr)
    {
      bldr.Entity<Camp>()
        .HasData(new 
        {
            CampId = 1,
            Moniker = "ATL2018",
            Name = "Atlanta Code Camp",
            EventDate = new DateTime(2018, 10, 18),
            LocationId = 1,
            Length = 1
        });

      bldr.Entity<Location>()
        .HasData(new 
        {
          LocationId = 1,
          VenueName = "Atlanta Convention Center",
          Address1 = "123 Main Street",
          CityTown = "Georgia",
          StateProvince = "GA",
          PostalCode = "12345",
          Country = "USA"
        });

      bldr.Entity<Talk>()
        .HasData(new 
        {
          TalkId = 1,
          CampId = 1,
          SpeakerId = 1,
          Title = "Entity Framework From Scratch",
          Abstract = "Entity Framework from scratch in an hour. Probably cover it all",
          Level = 100
        },
        new
        {
          TalkId = 2,
          CampId = 1,
          SpeakerId = 2,
          Title = "Writing Sample Data Made Easy",
          Abstract = "Thinking of good sample data examples is tiring.",
          Level = 200
        });

      bldr.Entity<Speaker>()
        .HasData(new
        {
          SpeakerId = 1,
          FirstName = "Tom",
          LastName = "Hunk",
          BlogUrl = "http://hunk.com",
          Company = "Tom Hunk Gmbh.",
          CompanyUrl = "http://hunk&co.com",
          GitHub = "tommyHub",
          Twitter = "tommyTweet"
        }, new
        {
          SpeakerId = 2,
          FirstName = "Matt",
          LastName = "Blanc",
          BlogUrl = "http://blancM.com",
          Company = "Matt Blanc LTD.",
          CompanyUrl = "http://blancM.com",
          GitHub = "mattBlancHub",
          Twitter = "mattBlancHub"
        });

    }

  }
}
