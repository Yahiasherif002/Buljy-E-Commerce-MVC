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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly AppDbContext _db;

        public CompanyRepository(AppDbContext db) : base(db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
        public void update(Company company)
        {
            var local = _db.companies.Local.FirstOrDefault(entry => entry.Id == company.Id);
            if (local != null)
            {
                _db.Entry(local).State = EntityState.Detached;
            }
            _db.companies.Update(company);
        }
    }
}

        
    

