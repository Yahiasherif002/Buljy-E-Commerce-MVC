using Buljy.Models;
using Buljy.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository:IRepository<Category>
    {
        void update(Category category);
        public  Task<IEnumerable<Category>> GetAllWithP();

    }
}
