// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Application;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Web.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly NotificationService _notificationService;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager, NotificationService notificationService)
        {
            _userManager = userManager;
            _notificationService = notificationService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = (ApplicationUser)await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            // In ConfirmEmail action (in Identity/AccountController)
            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            //Send notification to admins
            if (admins.Any())
            {
                foreach (var admin in admins)
                {
                    await _notificationService.CreateAndSendNotificationAsync(userId, $"New user registration: {user.FirstName} {user.LastName}",
                        "New user registration", NotificationType.User);
                }
            }

            //code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);
            StatusMessage = result.Succeeded ? "Thank you for confirming your email." +
                                               "\n<br><a class='btn btn-primary mt-3' style='text-decoration:none;' href=\"/Identity/Account/Login?returnUrl=%2F\">Login</a>" : "Error confirming your email.";
            return Page();
        }
    }
}
