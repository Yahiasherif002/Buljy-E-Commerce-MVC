using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository category { get; }
        IProductRepository product { get; }
        ICompanyRepository company { get; }
        IShoppingCartRepository shoppingCart { get; }

        IOrderHeaderRepository orderHeader { get; }
        IOrderDetailRepository orderDetails { get; }

        IApplicationUserRepository applicationUser { get; }


        void save();
    }
}
