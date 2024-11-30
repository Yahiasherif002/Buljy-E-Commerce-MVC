using Buljy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.DataAccess.Data.Configuration
{
    internal class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired();

            builder.HasData(LoadData());

        }
        private static List<Company> LoadData()
        {
            List<Company> companies = new List<Company>
            {
                new Company
                {
                    Id = 1,
                    Name = "Company 1",
                    Address = "123 Main St",
                    City = "New York",
                    Country = "USA",
                    Phone = "123-456-7890",
                    Email = ""
                },
                new Company
                {
                    Id = 2,
                    Name = "Company 2",
                    Address = "456 Elm St",
                    City = "Los Angeles",
                    Country = "USA",
                    Phone = "123-456-7890",
                    Email = ""
                },
                new Company
                {
                    Id = 3,
                    Name = "Company 3",
                    Address = "789 Oak St",
                    City = "Chicago",
                    Country = "USA",
                    Phone = "123-456-7890",
                    Email = ""
                }
               
            };
            return companies;
        }
    }
}
