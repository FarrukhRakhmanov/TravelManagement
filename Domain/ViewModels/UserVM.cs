using Domain.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Domain.ViewModels
{
    /// <summary>
    /// View Model for ApplicationUser
    /// </summary>
    public class ApplicationUserVM
    {
        [ValidateNever]
        public ApplicationUser? User { get; set; }

        [ValidateNever]
        public List<string>? Roles { get; set; }
    }
}
