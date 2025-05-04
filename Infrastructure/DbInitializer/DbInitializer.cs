using Domain.Enums;
using Domain.Models;
using Domain.StaticDetails;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            //migrations if they are not applied
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(UserRole.Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(UserRole.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(UserRole.Agent)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(UserRole.User)).GetAwaiter().GetResult();

                //create admin user if it is not created
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "easytrip.conestoga@gmail.com",
                    Email = "easytrip.conestoga@gmail.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                }, "Admin@123").GetAwaiter().GetResult();

                ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
                _userManager.AddToRoleAsync(user, UserRole.Admin).GetAwaiter().GetResult();

                var notificationPreferences = Enum.GetValues(typeof(NotificationType))
                    .Cast<NotificationType>()
                    .ToList();

                foreach (NotificationType notificationType in notificationPreferences)
                {
                    var notificationPreference = new NotificationPreference()
                    {
                        Type = notificationType,
                        InApp = true,
                        ByEmail = true,
                        ApplicationUserId = user.Id
                    };
                    _context.NotificationPreferences.Add(notificationPreference);
                }
                _context.SaveChanges();
            }
        }
    }
}
