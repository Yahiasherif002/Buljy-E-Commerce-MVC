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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Buljy.DataAccess.Repository.IRepository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly AppDbContext _db;
        public ShoppingCartRepository(AppDbContext db ):base(db)
        {
            _db = db;
        }

        public Task<IEnumerable<ShoppingCart>> GetAllWithP()
        {
            throw new NotImplementedException();
        }

        public void  update (ShoppingCart shoppingCart)
        {
            _db.Update(shoppingCart);
        }

        

        
    }
}
