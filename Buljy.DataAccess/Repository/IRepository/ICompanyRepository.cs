using Buljy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository:IRepository<Company>
    {
        void update(Company company);
    }
}
