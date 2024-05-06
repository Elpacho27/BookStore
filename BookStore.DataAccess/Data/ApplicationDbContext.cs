using BookStore.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace BookStore.DataAccess.Data;

public class ApplicationDbContext :IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Company> Companies { get; set; }

    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
   

    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<OrderHeader> OrderHeaders { get; set; }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new 
            {
                Id = 1,
                Name = "SciFi",
                DisplayOrder = 1
            },
            new 
            {
                Id = 2,
                Name = "Novel",
                DisplayOrder = 2
            },
            new 
            {
                Id = 3,
                Name = "Thriller",
                DisplayOrder = 3
            }

            );

        modelBuilder.Entity<Product>().HasData(

            new
            {
                Id=4,
                Title="first product",
                Description="tttt",
                ISBN="ISBN",
                Author="author",
                ListPrice=0.3,
                Price=0.5,
                Price50=0.4,
                Price100=0.9,
                CategoryID=1,
                ImageURL=""

            }

            );

        modelBuilder.Entity<Company>().HasData(

            new
            {
                Id = 1,
                Name = "FootLocker",
                StreetAddress = "Lincoln Street 44",
                City = "Florida",
                State = "Florida",
                PostalCode = 31511,
                PhoneNumber = "099222",
                

            }

            );

    }
}
