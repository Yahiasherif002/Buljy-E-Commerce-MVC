using Buljy.DataAccess.Data;
using Buljy.DataAccess.Reposoitory.IRepository;
using Buljy.Models;
using Buljy.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.DataAccess.Repository.IRepository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _db;
        public CategoryRepository(AppDbContext db ):base(db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Category>> GetAllWithP()
        {
            return await _db.categories.Include(c => c.Products).ToListAsync();
        }
  public void update(Category category)
        {
            var local = _db.products.Local.FirstOrDefault(entry => entry.Id == category.Id);

            if (local != null)
            {
                // Detach the existing tracked entity
                _db.Entry(local).State = EntityState.Detached;
            }

            // Now proceed to update
            _db.categories.Update(category);
        }

        

        
    }
}
