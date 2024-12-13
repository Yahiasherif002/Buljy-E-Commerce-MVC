using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.Models.ViewModels
{
    public class RoleManagementVM
    {
        public ApplicationUser applicationUser  { get; set; } = new ApplicationUser();

        public IEnumerable<SelectListItem> roleList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> companyList { get; set; } = new List<SelectListItem>();
    }
}
