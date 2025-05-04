using Domain.StaticDetails;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Area(UserRole.Admin)]
    [Authorize(Roles = UserRole.Admin)]
    public class ContactMessageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactMessageController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var messages = _unitOfWork.ContactMessage.GetAll().ToList();

            return View(messages);
        }

        public IActionResult Details(int messageId)
        {
            var message = _unitOfWork.ContactMessage.Get(u => u.Id == messageId);

            if (message == null)
            {
                TempData["error"] = "Contact Message not found";
                return RedirectToAction(nameof(Index));
            }

            message.IsRead = true;

            _unitOfWork.ContactMessage.Update(message);
            _unitOfWork.Save();

            return View(message);
        }


        [HttpPost]
        public IActionResult Delete(int messageId)
        {
            var message = _unitOfWork.ContactMessage.Get(u => u.Id == messageId);

            if (message == null)
            {
                TempData["error"] = "Contact Message not found";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.ContactMessage.Remove(message);
            _unitOfWork.Save();

            TempData["success"] = "Contact Message deleted successfully";

            return RedirectToAction(nameof(Index));


        }

    }
}
