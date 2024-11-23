using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buljy.Models;


namespace Buljy.DataAccess.Data.Configuration
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Title).IsRequired();
            builder.Property(p => p.Description).IsRequired();
            builder.Property(p => p.Author).IsRequired();


           

            builder.HasData(LoadData());
        }
        private static List<Product> LoadData()
        {
            List<Product> products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Title = "The Alchemist",
                    Description = "A book about following your dreams",
                    ISBN = "978-0062315007",
                    Author = "Paulo Coelho",
                    ListPrice = 9.99,
                    Price = 8.99,
                    Price50 = 7.99,
                    Price100 = 6.99,
                    CategoryId=1,
                    ImageUrl=""
                },
                new Product
                {
                    Id = 2,
                    Title = "The Power of Now",
                    Description = "A book about spiritual enlightenment",
                    ISBN = "978-1577314806",
                    Author = "Eckhart Tolle",
                    ListPrice = 12.99,
                    Price = 11.99,
                    Price50 = 10.99,
                    Price100 = 9.99,
                    CategoryId=2,
                    ImageUrl=""

                },
                new Product
                {
                    Id = 3,
                    Title = "The Lean Startup",
                    Description = "A book about entrepreneurship",
                    ISBN = "978-0307887894",
                    Author = "Eric Ries",
                    ListPrice = 14.99,
                    Price = 13.99,
                    Price50 = 12.99,
                    Price100 = 11.99,
                    CategoryId=3,
                    ImageUrl=""

                },
                new Product
                {
                    Id = 4,
                    Title = "The 4-Hour Workweek",
                    Description = "A book about time management",
                    ISBN = "978-0307465351",
                    Author = "Tim Ferriss",
                    ListPrice = 11.99,
                    Price = 10.99,
                    Price50 = 9.99,
                    Price100 = 8.99,
                    CategoryId=4,
                                        ImageUrl=""

                },
                new Product {
                    Id = 5,
                    Title = "Fortune of Time",
                    Author="Billy Spark",
                    Description= "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN="SWD9999001",
                    ListPrice=99,
                    Price=90,
                    Price50=85,
                    Price100=80,
                    CategoryId=8,
                                        ImageUrl=""

                },
                new Product
                {
                    Id = 6,
                    Title = "Dark Skies",
                    Author = "Nancy Hoover",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "CAW777777701",
                    ListPrice = 40,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId=10,
                                        ImageUrl=""

                },
                new Product
                {
                    Id = 7,
                    Title = "Vanish in the Sunset",
                    Author = "Julian Button",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "RITO5555501",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
                    CategoryId=4,
                                        ImageUrl=""

                },
                new Product
                {
                    Id = 8,
                    Title = "Cotton Candy",
                    Author = "Abby Muscles",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "WS3333333301",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                                        ImageUrl=""

                },
                new Product
                {
                    Id = 9,
                    Title = "Rock in the Ocean",
                    Author = "Ron Parker",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SOTJ1111111101",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId=4,
                                        ImageUrl=""

                },
                new Product
                {
                    Id = 10,
                    Title = "Leaves and Wonders",
                    Author = "Laura Phantom",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CategoryId=4,
                                        ImageUrl=""

                }
            };
            return products;
        }
    }
}
