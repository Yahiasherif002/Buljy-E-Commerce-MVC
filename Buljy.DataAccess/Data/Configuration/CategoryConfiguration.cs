using Buljy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buljy.DataAccess.Data.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.DisplayOrder).IsRequired();

            builder.HasData(LoadData());
        }

        private static List<Category> LoadData()
        {
            List<Category> categories = new List<Category>
            {
                new Category
                {
                    Id = 1,
                    Name = "Action",
                    DisplayOrder = 1
                },
                new Category
                {
                    Id = 2,
                    Name = "Comedy",
                    DisplayOrder = 2
                },
                new Category
                {
                    Id = 3,
                    Name = "Drama",
                    DisplayOrder = 3
                }
            };
            return categories;

        }
    }
}
