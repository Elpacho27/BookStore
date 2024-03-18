using BookStore.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace BookStore.DataAccess.Data;

public class ApplicationDbContext : DbContext
{



    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
                Price100=0.9

            }

            );

    
        
    }



}
