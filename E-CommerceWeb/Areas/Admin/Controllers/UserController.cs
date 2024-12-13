using Buljy.DataAccess.Data;
using Buljy.Utility;
using Buljy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Buljy.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_CommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class UserController : Controller
    {
        private readonly AppDbContext appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(AppDbContext appDbContext, UserManager<ApplicationUser> userManager)
        {
            this.appDbContext = appDbContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult roleManagement(string id)
        {
            var user = appDbContext.applicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == id);
            var userRole = appDbContext.UserRoles.FirstOrDefault(u => u.UserId == id);
            if (userRole == null)
            {
                return NotFound();
            }
            string roleId = userRole.RoleId;
            var role = appDbContext.Roles.FirstOrDefault(r => r.Id == roleId);
            var roleManagementVM = new RoleManagementVM
            {
                applicationUser = user,
                roleList = appDbContext.Roles.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Name
                }),
                companyList = appDbContext.companies.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            if (role != null)
            {
                roleManagementVM.applicationUser.role = role.Name;
            }
            return View(roleManagementVM);
        }

        [HttpPost]
        public IActionResult roleManagement(RoleManagementVM roleManagementVM)
        {
            var user = appDbContext.applicationUsers.FirstOrDefault(u => u.Id == roleManagementVM.applicationUser.Id);
            if (user == null)
            {
                return NotFound();
            }
            var roleId = appDbContext.UserRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
            var OldRole = appDbContext.Roles.FirstOrDefault(u => u.Id == roleId).Name;

            if (!(roleManagementVM.applicationUser.role == OldRole))
            {
                if (roleManagementVM.applicationUser.role == SD.CompanyRole)
                {
                    user.CompanyId = roleManagementVM.applicationUser.CompanyId;
                }
                if (OldRole == SD.CompanyRole)
                {
                    user.CompanyId = null;
                }
                appDbContext.SaveChanges();

                _userManager.RemoveFromRoleAsync(user, OldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, roleManagementVM.applicationUser.role).GetAwaiter().GetResult();


            }
            return RedirectToAction(nameof(Index));
        }









        [HttpGet]
        public IActionResult GetAll()
        {
            var users = appDbContext.applicationUsers.Include(u => u.Company).ToList();
            var userRoles = appDbContext.UserRoles.ToList();
            var roles = appDbContext.Roles.ToList();

            foreach (var user in users)
            {
                var userRole = userRoles.FirstOrDefault(u => u.UserId == user.Id);
                if (userRole != null)
                {
                    var roleId = userRole.RoleId;
                    var role = roles.FirstOrDefault(u => u.Id == roleId);
                    if (role != null)
                    {
                        user.role = role.Name;
                    }
                }
            }

            return Json(new { data = users });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var user = appDbContext.applicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(1);
            }
            appDbContext.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            var user = appDbContext.applicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            appDbContext.applicationUsers.Remove(user);
            appDbContext.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }


    }
}

