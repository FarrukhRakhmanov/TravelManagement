using Application;
using Domain.Enums;
using Domain.Models;
using Domain.StaticDetails;
using Domain.ViewModels;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Web.Areas.Agent.Controllers
{
    [Area(UserRole.Agent)]
    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Agent}")]
    public class TripController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly NotificationService _notificationService;
        private readonly UserManager<IdentityUser> _userManager;
        public TripController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment, NotificationService notificationService, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
            _notificationService = notificationService;
            _userManager = userManager;
        }
        
        public ActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var trips = _unitOfWork.Trip.GetAll(includeProperties: "ApplicationUser").ToList();

            if (User.IsInRole(UserRole.Agent))
            {
                trips = trips.Where(t => t.ApplicationUserId == userId).ToList();
            }

            return View(trips);
        }

        public IActionResult Upsert(int? tripId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Get the list of enum values
            var tripStatus = Enum.GetValues(typeof(TripStatus))
                .Cast<TripStatus>()
                .Select(t => new SelectListItem
                {
                    Text = t.ToString(),
                    Value = ((int)t).ToString() // Store enum value as string
                }).ToList();

            // Get the list of enum values
            var tripStyle = Enum.GetValues(typeof(TripStyle))
                .Cast<TripStyle>()
                .Select(t => new SelectListItem
                {
                    Text = t.ToString(),
                    Value = ((int)t).ToString() // Store enum value as string
                }).ToList();

            // Pass the list to the view
            ViewBag.TripStatus = tripStatus;
            ViewBag.TripStyle = tripStyle;

            var tripVM = new TripVM()
            {
                Trip = new Trip(),
            };
            tripVM.Trip.ApplicationUserId = userId;

            if (tripId == null || tripId == 0)
            {
                //create
                return View(tripVM);
            }

            //update
            tripVM.Trip = _unitOfWork.Trip.Get(u => u.Id == tripId);
            var itineraries = _unitOfWork.Itinerary
                .GetAll(u => u.TripId == tripId).ToArray();

            tripVM.Trip.Itineraries = itineraries;

            return View(tripVM);
        }

        [HttpPost]
        public IActionResult Upsert(TripVM tripVM, IFormFile? file)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostingEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string tripPath = Path.Combine(wwwRootPath, @"images\trips");

                    if (!string.IsNullOrEmpty(tripVM.Trip.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath =
                            Path.Combine(wwwRootPath, tripVM.Trip.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(tripPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    tripVM.Trip.ImageUrl = @"\images\trips\" + fileName;
                }

                if (tripVM.Trip.Id == 0)
                {
                    tripVM.Trip.ApplicationUserId = userId;
                    _unitOfWork.Trip.Add(tripVM.Trip);
                    TempData["success"] = "Trip created successfully";
                }
                else
                {
                    _unitOfWork.Trip.Update(tripVM.Trip);
                    TempData["success"] = "Trip updated successfully";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");

            }

            // Get the list of enum values
            var tripStatus = Enum.GetValues(typeof(TripStatus))
                .Cast<TripStatus>()
                .Select(t => new SelectListItem
                {
                    Text = t.ToString(),
                    Value = ((int)t).ToString() // Store enum value as string
                }).ToList();


            // Get the list of enum values
            var tripStyle = Enum.GetValues(typeof(TripStyle))
                .Cast<TripStyle>()
                .Select(t => new SelectListItem
                {
                    Text = t.ToString(),
                    Value = ((int)t).ToString() // Store enum value as string
                }).ToList();

            // Pass the list to the view
            ViewBag.TripStatus = tripStatus;
            ViewBag.TripStyle = tripStyle;
            return View(tripVM);
        }

        [HttpPost]
        public IActionResult Delete(int TripId)
        {
            var tripToBeDeleted = _unitOfWork.Trip.Get(c => c.Id == TripId);
            if (tripToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            if (tripToBeDeleted.Status == TripStatus.Published || tripToBeDeleted.Status == TripStatus.Ongoing || tripToBeDeleted.Status == TripStatus.Sold || tripToBeDeleted.Status == TripStatus.Sold)
            {
                TempData["error"] = "Trip cannot be deleted! Only draft Trips can be deleted";
                return RedirectToAction(nameof(Index));
            }

            var bookings = _unitOfWork.Booking.GetAll(b =>
                b.TripId == tripToBeDeleted.Id && b.Status == BookingStatus.Confirmed);

            if (bookings.Count() > 1)
            {
                TempData["error"] = "Trip cannot be deleted! Trip has confirmed bookings";
                return RedirectToAction(nameof(Index));
            }

            if (tripToBeDeleted.ImageUrl != null)
            {
                string stringtripOldImagePath = Path.Combine(_hostingEnvironment.WebRootPath,
                    tripToBeDeleted.ImageUrl.TrimStart('\\'));

                if (stringtripOldImagePath != null)
                {
                    if (System.IO.File.Exists(stringtripOldImagePath))
                    {
                        System.IO.File.Delete(stringtripOldImagePath);
                    }
                }
            }

            var itineraryList = _unitOfWork.Itinerary.GetAll(c => c.TripId == TripId);

            if (itineraryList != null && itineraryList.Any())
            {
                foreach (var itinerary in itineraryList)
                {
                    if (itinerary.ImageUrl != null)
                    {
                        string itineraryOldImagePath = Path.Combine(_hostingEnvironment.WebRootPath,
                            itinerary.ImageUrl.TrimStart('\\'));

                        if (itineraryOldImagePath != null)
                        {
                            if (System.IO.File.Exists(itineraryOldImagePath))
                            {
                                System.IO.File.Delete(itineraryOldImagePath);
                            }
                        }
                    }
                }

                _unitOfWork.Itinerary.RemoveRange(itineraryList);
            }

            TempData["success"] = "Trip deleted successfully";

            _unitOfWork.Trip.Remove(tripToBeDeleted);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Publish(int tripId)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            var trip = _unitOfWork.Trip.Get(u => u.Id == tripId);

            if (trip != null)
            {
                var itineraries = _unitOfWork.Itinerary.GetAll(i => i.TripId == trip.Id);

                if (!itineraries.Any())
                {
                    TempData["error"] = "Trip cannot be published! Trip itinerary is required";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var itinerary in itineraries)
                {
                    if (itinerary.Date < today)
                    {
                        TempData["error"] = "Trip cannot be published! Trip has itinerary with date in the past";
                        return RedirectToAction(nameof(Index));
                    }
                    if (itinerary.ImageUrl == null)
                    {
                        TempData["error"] = "Trip cannot be published! Trip has itinerary with missing image";
                        return RedirectToAction(nameof(Index));
                    }
                }

                if (trip.ImageUrl == null)
                {
                    TempData["error"] = "Trip cannot be published! Trip image is required";
                    return RedirectToAction(nameof(Index));
                }

                if (trip.StartDate <= today)
                {
                    TempData["error"] = "Trip cannot be published! Trip start date cannot be in the past";
                    return RedirectToAction(nameof(Index));
                }

                if (trip.EndDate < today)
                {
                    TempData["error"] = "Trip cannot be published! Trip end date cannot be in the past";
                    return RedirectToAction(nameof(Index));
                }

                if (trip.SeatsAvailable < 0)
                {
                    TempData["error"] = "Trip cannot be published! Seats available should be greater than 0";
                    return RedirectToAction(nameof(Index));
                }

                if (trip.DiscountPricePerPerson <= 0)
                {
                    TempData["error"] = "Trip cannot be published! Seats available should be greater than 0";
                    return RedirectToAction(nameof(Index));
                }

                trip.Status = TripStatus.Published;
                _unitOfWork.Trip.Update(trip);
                _unitOfWork.Save();

                TempData["success"] = "Trip published successfully";

                await _notificationService.CreateAndSendNotificationAsync(trip.ApplicationUserId, $" Trip: {trip.Title} - published",
                    "Trip has been published", NotificationType.Trip);
            }
            else
            {
                TempData["success"] = "Trip not found";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Cancel(int tripId)
        {
            var trip = _unitOfWork.Trip.Get(u => u.Id == tripId);

            var bookings = _unitOfWork.Booking.GetAll(b =>
                b.TripId == trip.Id && b.Status == BookingStatus.Confirmed);

            if (bookings.Count() > 0)
            {
                TempData["error"] = "Trip cannot be cancelled! Trip has confirmed bookings";
                return RedirectToAction(nameof(Index));
            }

            if (trip != null)
            {
                trip.Status = TripStatus.Cancelled;
                TempData["success"] = "Trip cancelled successfully";
                _unitOfWork.Trip.Update(trip);
                _unitOfWork.Save();

                await _notificationService.CreateAndSendNotificationAsync(trip.ApplicationUserId, $" Trip: {trip.Title} - published",
                    "Trip has been cancelled", NotificationType.Trip);
            }
            else
            {
                TempData["success"] = "Trip not found";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
