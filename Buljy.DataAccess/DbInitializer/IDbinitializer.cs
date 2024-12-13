using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {
         Task initializeAsync();
    }
}
