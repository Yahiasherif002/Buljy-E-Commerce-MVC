using Buljy.DataAccess.Data;
using Buljy.DataAccess.Reposoitory.IRepository;
using Buljy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.DataAccess.Repository.IRepository
{
    public class ProductRepository: Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(Product product)
        {
            var local = _db.products.Local.FirstOrDefault(entry => entry.Id == product.Id);

            if (local != null)
            {
                // Detach the existing tracked entity
                _db.Entry(local).State = EntityState.Detached;
            }

            var objFromDb = _db.products.FirstOrDefault(u => u.Id == product.Id);

            if (objFromDb != null)
            {
                objFromDb.Title = product.Title;
                objFromDb.Description = product.Description;
                objFromDb.CategoryId = product.CategoryId;
                objFromDb.Price = product.Price;
                objFromDb.ImageUrl = product.ImageUrl;
            }
            if(product.ImageUrl!= null)
            {
                objFromDb.ImageUrl = product.ImageUrl;
            }

            
        }
    }
    
}
