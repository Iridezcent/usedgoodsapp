using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace UsedGoodApp.Models
{
    public class ProductsDb : DbContext
    {
        public ProductsDb():
            base("ProductsContext")
        {

        }

        static ProductsDb()
        {
            Database.SetInitializer(new ProductContextInitializer());
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Price> Prices { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Repair> Repairs { get; set; }

        public DbSet<ProductDescription> ProductDescriptions { get; set; }

        public DbSet<CategoryDescription> CategoryDescriptions { get; set; }
    }

    public class ProductContextInitializer : CreateDatabaseIfNotExists<ProductsDb>
    {
        protected override void Seed(ProductsDb context)
        {
            for (int i = 0; i < 500; i++)
            {
                context.Products.Add(new Product
                {
                    IsOutOfUse = false,
                    ArrivalDate = DateTime.Today.Date,
                    Price = new Price
                    {
                        Base = 400 + i
                    },
                    Warehouse = new Warehouse
                    {
                        Name = "Киев"
                    },
                    Category = new Category
                    {
                        Description = new CategoryDescription
                        {
                            Name = "ROBOT BLYAT'"
                        }
                    },
                    Description = new ProductDescription
                    {
                        Name = "Альтрон"
                    },
                    Repair = new Repair
                    {

                    }
                });
            }
            //context.Products.Add(new Product
            //{
            //    IsOutOfUse = false,
            //    ArrivalDate = DateTime.Today.Date,
            //    Price = new Price
            //    {
            //        Base = 400
            //    },
            //    Warehouse = new Warehouse
            //    {
            //        Name = "Киев"
            //    },
            //    Category = new Category
            //    {
            //        Description = new CategoryDescription
            //        {
            //            Name = "ROBOT BLYAT'"
            //        }
            //    },
            //    Description = new ProductDescription
            //    {
            //        Name = "Альтрон"
            //    },
            //    Repair = new Repair
            //    {

            //    }
            //});
            base.Seed(context);
        }
    }

    public class Product
    {
        public int Id { get; set; }

        public bool IsOutOfUse { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ArrivalDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime SaleDate { get; set; } 

        public Price Price { get; set; }

        public Warehouse Warehouse { get; set; }

        public Category Category { get; set; }

        public ProductDescription Description { get; set; }

        public Repair Repair { get; set; }
    }    

    public class Price
    {
        public int Id { get; set; } 

        public double Base { get; set; }

        public double Sale { get; set; }

        public double Purchase { get; set; }
    }

    public class Warehouse
    {
        public int Id { get; set; }

        public IEnumerable<Product> Products { get; set; }

        public string Name { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }

        public Category Parent { get; set; }

        public IEnumerable<Product> Products { get; set; }

        public CategoryDescription Description { get; set; }
    }

    public class CategoryDescription
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }

    public class Repair
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public string PersonName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime FinishDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime StartDate { get; set; }
    }

    public class ProductDescription
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int StatusId { get; set; }

        public string Issue { get; set; }

        public string Reserved { get; set; }
    }
}