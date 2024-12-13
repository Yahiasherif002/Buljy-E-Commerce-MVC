using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.Models.ViewModels
{
    public class ShoppingCartVM
    {
       
        public IEnumerable<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
        public OrderHeader OrderHeader { get; set; }


    }
}
