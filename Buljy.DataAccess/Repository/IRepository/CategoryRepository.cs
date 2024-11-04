using Buljy.DataAccess.Data;
using Buljy.DataAccess.Reposoitory.IRepository;
using Buljy.Models;
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

        
         
        public void update(Category category)
        {
            _db.categories.Update(category);
        }
    }
}
