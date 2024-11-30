using Buljy.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.DataAccess.Repository.IRepository
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _db;
        public ICategoryRepository category { get; private set; }
        public IProductRepository product { get; private set; }
        public ICompanyRepository company { get; private set; }


        public UnitOfWork(AppDbContext db) 
        {
            _db = db;
            category = new CategoryRepository(_db);
            product = new ProductRepository(_db);
            company = new CompanyRepository(_db);
        }

        public void save()
        {
            _db.SaveChanges();
        }
    }
}
