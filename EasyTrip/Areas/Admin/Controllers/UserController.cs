using Domain.Enums;
using Domain.Models;
using Domain.StaticDetails;
using Domain.ViewModels;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Areas.Admin.Controllers
{
    [Area(UserRole.Admin)]
    [Authorize(Roles = UserRole.Admin)]
    public class UserController : Controller
    {
        private bool _test = false;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser>? _userManager;
        private readonly RoleManager<IdentityRole>? _roleManager;

        public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        private UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _test = true;

        }

        public static UserController GetTestController(IUnitOfWork unitOfWork)
        {
            return new UserController(unitOfWork);

        }


        public async Task<IActionResult> Index()
        {
            var users = _unitOfWork.ApplicationUser.GetAll();


            var usersVM = new List<ApplicationUserVM>();

            foreach (var user in users)
            {

                var roles = await (_userManager?.GetRolesAsync(user) ?? Task.FromResult<IList<string>>(new List<string>([UserRole.User])));

                if (!roles.Contains(UserRole.Admin) && !roles.Contains(UserRole.Agent))
                {
                    var applicationUserVM = new ApplicationUserVM
                    {
                        User = user,
                        Roles = roles.ToList()
                    };
                    usersVM.Add(applicationUserVM);
                }

            }

            return View(usersVM);
        }

        public async Task<IActionResult> GetTeam()
        {
            var users = _unitOfWork.ApplicationUser.GetAll();


            var usersVM = new List<ApplicationUserVM>();

            foreach (var user in users)
            {

                var roles = await _userManager?.GetRolesAsync(user) ?? new List<string>([UserRole.Admin]);

                if (roles.Contains(UserRole.Agent) || roles.Contains(UserRole.Admin))
                {
                    var applicationUserVM = new ApplicationUserVM
                    {
                        User = user,
                        Roles = roles.ToList()
                    };
                    usersVM.Add(applicationUserVM);
                }

            }

            return View(usersVM);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await (_userManager?.GetRolesAsync(user) ?? Task.FromResult<IList<string>>(new List<string>([UserRole.Admin])));
            var userVM = new ApplicationUserVM
            {
                User = user,
                Roles = roles.ToList()
            };

            if (!_test)
                ViewBag.RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                });

            return View(userVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationUserVM userVM)
        {
            if (ModelState.IsValid)
            {
                // Fetch the existing user instance from the database
                var user = _unitOfWork.ApplicationUser.Get(u => u.Id == userVM.User.Id);

                if (user == null)
                {
                    return NotFound();
                }

                // Update user details
                user.FirstName = userVM.User.FirstName;
                user.LastName = userVM.User.LastName;
                user.Email = userVM.User.Email;
                user.PhoneNumber = userVM.User.PhoneNumber;

                // Update user in database
                _unitOfWork.ApplicationUser.Update(user);
                _unitOfWork.Save();

                TempData["success"] = "User updated successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            });

            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string userId, string role)
        {
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    if (role != null)
                    {
                        var roleExists = await _userManager.IsInRoleAsync(user, role);
                        if (!roleExists)
                        {
                            await _userManager.AddToRoleAsync(user, role);

                            SetNotificationPreferences(user, role);

                            TempData["success"] = "Role added successfully";
                        }
                        else
                        {
                            TempData["error"] = "Role is already in user's role list";
                        }
                    }
                }
            }

            return RedirectToAction("Edit", "User", new { id = userId });
        }

        private bool SetNotificationPreferences(IdentityUser user, string role)
        {
            var notifPref = _unitOfWork.NotificationPreference.GetAll(n => n.ApplicationUserId == user.Id);

            if (notifPref.Count() > 0 || role == UserRole.User)
            {
                return false;
            }
            if (role == UserRole.Admin || role == UserRole.Agent)
            {
                var notificationPreferences = Enum.GetValues(typeof(NotificationType))
                    .Cast<NotificationType>()
                    .ToList();

                foreach (NotificationType notificationType in notificationPreferences)
                {
                    var userNotificationPreference =
                        _unitOfWork.NotificationPreference.GetAll(n => n.ApplicationUserId == user.Id
                                                                       && n.Type == notificationType);

                    if (userNotificationPreference.Count() > 0)
                    {
                        return false;
                    }

                    var notificationPreference = new NotificationPreference()
                    {
                        Type = notificationType,
                        InApp = true,
                        ByEmail = true,
                        ApplicationUserId = user.Id
                    };
                    _unitOfWork.NotificationPreference.Add(notificationPreference);
                    _unitOfWork.Save();

                    return true;

                }

            }
            return false;
        }


        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    if (role != null)
                    {
                        var roleExists = await _userManager.IsInRoleAsync(user, role);
                        if (roleExists)
                        {
                            await _userManager.RemoveFromRoleAsync(user, role);
                            TempData["success"] = "Role removed successfully";
                        }
                    }
                }
            }

            return RedirectToAction("Edit", "User", new { id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUser(string userId)
        {
            if (userId != null)
            {
                var userToBeDeleted = await _userManager.FindByIdAsync(userId);
                var trips = _unitOfWork.Trip.GetAll(t => t.ApplicationUserId == userId);
                var adminUsers = _userManager.GetUsersInRoleAsync(UserRole.Admin);
                var adminUser = adminUsers.Result.FirstOrDefault();
                var participants = _unitOfWork.Participant.GetAll(p => p.ApplicationUserId == userId);

                if (trips != null)
                {
                    foreach (var trip in trips)
                    {
                        trip.ApplicationUserId = adminUser.Id;
                        _unitOfWork.Trip.Update(trip);
                    }
                }

                if (participants != null)
                {
                    foreach (var participant in participants)
                    {
                        participant.ApplicationUserId = null;
                        _unitOfWork.Participant.Update(participant);
                    }
                }

                if (userToBeDeleted != null)
                {
                    await _userManager.DeleteAsync(userToBeDeleted);
                    TempData["success"] = "User deleted successfully";
                }
            }
            else
            {
                TempData["success"] = "User not found";
                return NotFound();

            }
            return RedirectToAction("Index");
        }
    }
}
