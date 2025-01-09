using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Tabloid.Models;
using Microsoft.AspNetCore.Identity;

namespace Tabloid.Data;
public class TabloidDbContext : IdentityDbContext<IdentityUser>
{
    private readonly IConfiguration _configuration;

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Posts> Posts { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Tag> Tags { get; set; }


    public TabloidDbContext(DbContextOptions<TabloidDbContext> context, IConfiguration config) : base(context)
    {
        _configuration = config;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Seed Roles
        modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Id = "c3aaeb97-d2ba-4a53-a521-4eea61e59b35",
            Name = "Admin",
            NormalizedName = "admin"
        });

        //Seed Users
        modelBuilder.Entity<IdentityUser>().HasData(new IdentityUser[]
        {
            new IdentityUser
            {
                Id = "dbc40bc6-0829-4ac5-a3ed-180f5e916a5f",
                UserName = "Administrator",
                Email = "admina@strator.comx",
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, _configuration["AdminPassword"])
            },
            new IdentityUser
            {
                Id = "d8d76512-74f1-43bb-b1fd-87d3a8aa36df",
                UserName = "JohnDoe",
                Email = "john@doe.comx",
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, _configuration["AdminPassword"])
            },
            new IdentityUser
            {
                Id = "a7d21fac-3b21-454a-a747-075f072d0cf3",
                UserName = "JaneSmith",
                Email = "jane@smith.comx",
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, _configuration["AdminPassword"])
            },
            new IdentityUser
            {
                Id = "c806cfae-bda9-47c5-8473-dd52fd056a9b",
                UserName = "AliceJohnson",
                Email = "alice@johnson.comx",
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, _configuration["AdminPassword"])
            },
            new IdentityUser
            {
                Id = "9ce89d88-75da-4a80-9b0d-3fe58582b8e2",
                UserName = "BobWilliams",
                Email = "bob@williams.comx",
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, _configuration["AdminPassword"])
            },
            new IdentityUser
            {
                Id = "d224a03d-bf0c-4a05-b728-e3521e45d74d",
                UserName = "EveDavis",
                Email = "Eve@Davis.comx",
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, _configuration["AdminPassword"])
            },

        });

        //Assign Roles to Users
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>[]
        {
            new IdentityUserRole<string>
            {
                RoleId = "c3aaeb97-d2ba-4a53-a521-4eea61e59b35",
                UserId = "dbc40bc6-0829-4ac5-a3ed-180f5e916a5f"
            },
            new IdentityUserRole<string>
            {
                RoleId = "c3aaeb97-d2ba-4a53-a521-4eea61e59b35",
                UserId = "d8d76512-74f1-43bb-b1fd-87d3a8aa36df"
            },

        });

        //Seed UserProfiles
        modelBuilder.Entity<UserProfile>().HasData(new UserProfile[]
        {
            new UserProfile
            {
                Id = 1,
                IdentityUserId = "dbc40bc6-0829-4ac5-a3ed-180f5e916a5f",
                FirstName = "Admina",
                LastName = "Strator",
                IsActive = true,
                ImageLocation = "https://robohash.org/numquamutut.png?size=150x150&set=set1",
                CreateDateTime = new DateTime(2022, 1, 25)
            },
             new UserProfile
            {
                Id = 2,
                FirstName = "John",
                LastName = "Doe",
                IsActive = true,
                CreateDateTime = new DateTime(2023, 2, 2),
                ImageLocation = "https://robohash.org/nisiautemet.png?size=150x150&set=set1",
                IdentityUserId = "d8d76512-74f1-43bb-b1fd-87d3a8aa36df",
            },
            new UserProfile
            {
                Id = 3,
                FirstName = "Jane",
                LastName = "Smith",
                IsActive = true,
                CreateDateTime = new DateTime(2022, 3, 15),
                ImageLocation = "https://robohash.org/molestiaemagnamet.png?size=150x150&set=set1",
                IdentityUserId = "a7d21fac-3b21-454a-a747-075f072d0cf3",
            },
            new UserProfile
            {
                Id = 4,
                FirstName = "Alice",
                LastName = "Johnson",
                IsActive = true,
                CreateDateTime = new DateTime(2023, 6, 10),
                ImageLocation = "https://robohash.org/deseruntutipsum.png?size=150x150&set=set1",
                IdentityUserId = "c806cfae-bda9-47c5-8473-dd52fd056a9b",
            },
            new UserProfile
            {
                Id = 5,
                FirstName = "Bob",
                LastName = "Williams",
                IsActive = true,
                CreateDateTime = new DateTime(2023, 5, 15),
                ImageLocation = "https://robohash.org/quiundedignissimos.png?size=150x150&set=set1",
                IdentityUserId = "9ce89d88-75da-4a80-9b0d-3fe58582b8e2",
            },
            new UserProfile
            {
                Id = 6,
                FirstName = "Eve",
                LastName = "Davis",
                IsActive = true,
                CreateDateTime = new DateTime(2022, 10, 18),
                ImageLocation = "https://robohash.org/hicnihilipsa.png?size=150x150&set=set1",
                IdentityUserId = "d224a03d-bf0c-4a05-b728-e3521e45d74d",
            }
        });

        //Seed Posts
        modelBuilder.Entity<Posts>().HasData(new Posts[]
            {
                new Posts
                {
                    Id = 1,
                    Title = "Introduction to Tabloid",
                    Author = "Admina Strator",
                    CategoryId = 1,
                    PublicationDate = new DateTime(2022, 12, 25),
                    Content = "Welcome to Tabloid, your one-stop platform for insightful articles across various categories. Stay tuned for exciting content!",
                    IsApproved = true,
                    HeaderImage = "https://example.com/images/tabloid_intro.jpg"
                },
                new Posts
                {
                    Id = 2,
                    Title = "The Future of AI",
                    Author = "John Doe",
                    CategoryId = 2,
                    PublicationDate = new DateTime(2023, 1, 15),
                    IsApproved = true,
                    Content = "Artificial intelligence is transforming industries worldwide. Explore its potential and challenges in this article.",
                    HeaderImage = "https://example.com/images/future_of_ai.jpg"
                },
                new Posts
                {
                    Id = 3,
                    Title = "Gardening Tips for Spring",
                    Author = "Alice Johnson",
                    CategoryId = 3,
                    PublicationDate = new DateTime(2023, 2, 20),
                    IsApproved = false,
                    Content = "Spring is the perfect time to start your gardening journey. Learn essential tips to make your garden thrive.",
                    HeaderImage = "https://example.com/images/gardening_tips.jpg"
                },
                new Posts
                {
                    Id = 4,
                    Title = "10 Best Travel Destinations",
                    Author = "Bob Williams",
                    CategoryId = 4,
                    PublicationDate = new DateTime(2023, 3, 5),
                    IsApproved = true,
                    Content = "Discover the top 10 travel destinations for your next adventure. These spots offer unique experiences for everyone.",
                    HeaderImage = "https://example.com/images/travel_destinations.jpg"
                },
                new Posts
                {
                    Id = 5,
                    Title = "Understanding Quantum Physics",
                    Author = "Eve Davis",
                    CategoryId = 5,
                    PublicationDate = new DateTime(2023, 4, 1),
                    IsApproved = true,
                    Content = "Quantum physics might seem complex, but this article breaks it down into simple concepts for easy understanding.",
                    HeaderImage = "https://example.com/images/quantum_physics.jpg"
                }
            });

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(new Category[]
        {
            new Category { Id = 1, Name = "Tech" },
            new Category { Id = 2, Name = "Science" },
            new Category { Id = 3, Name = "Lifestyle" },
            new Category { Id = 4, Name = "Travel" },
            new Category { Id = 5, Name = "Education" }
        });

        modelBuilder.Entity<Tag>().HasData(new Tag[]
        {
            new Tag { Id = 1, Name = "Technology" },
            new Tag { Id = 2, Name = "Science" },
            new Tag { Id = 3, Name = "Health" },
            new Tag { Id = 4, Name = "Education" },
            new Tag { Id = 5, Name = "Travel" }
        });
    }
}