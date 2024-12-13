using Buljy.DataAccess.Data;
using Buljy.Models;
using Buljy.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;

        public DbInitializer(AppDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task initializeAsync()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }
            //create roles if they don't exist
            if (!_roleManager.RoleExistsAsync(SD.CustomerRole).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.AdminRole));
                await _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole));
                await _roleManager.CreateAsync(new IdentityRole(SD.CompanyRole));
                await _roleManager.CreateAsync(new IdentityRole(SD.EmployeeRole));

                //create the admin user
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@ynet.com",
                    Email = "admin@ynet.com",
                    FirstName = "Yahia",
                    LastName = "Sherif",
                    PhoneNumber = 01060082542.ToString(),
                    StreetAddress="Mansoura-Tksim Khattab",
                    City = "Mansoura",
                    State = "Dakahlia",
                    PostalCode = "35511",
                    EmailConfirmed = true
                }, "Yahia123@admin").GetAwaiter().GetResult();

                ApplicationUser user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "admin@ynet.com");
                await _userManager.AddToRoleAsync(user, SD.AdminRole);
            }
            return;
        }

       
    }
}
