using Domain.Models;
using Domain.StaticDetails;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRole.Admin)]
    public class StripeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StripeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult StripeSettings()
        {
            var stripeSettings = _unitOfWork.StripeSettings.GetAll().FirstOrDefault();

            return View(stripeSettings);
        }

        [HttpPost]
        public IActionResult SaveStripeSettings(StripeSettings model)
        {
            var existingSettings = _unitOfWork.StripeSettings.GetAll().FirstOrDefault();

            if (existingSettings == null)
            {
                _unitOfWork.StripeSettings.Add(model);
                TempData["success"] = "Stripe keys added successfully";
            }
            else
            {
                existingSettings.PublishableKey = model.PublishableKey;
                existingSettings.SecretKey = model.SecretKey;
                _unitOfWork.StripeSettings.Update(existingSettings);
                TempData["success"] = "Stripe keys updated successfully";
            }

            _unitOfWork.Save();
            return RedirectToAction("StripeSettings");
        }
    }
}
