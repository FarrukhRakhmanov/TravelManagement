using Domain.Enums;
using Domain.Models;
using Domain.StaticDetails;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Agent.Controllers
{
    [Area(UserRole.Agent)]
    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Agent}")]
    public class ItineraryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ItineraryController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(int TripId)
        {
            ViewBag.TripId = TripId;
            var itineraries = _unitOfWork.Itinerary.GetAll(u => u.TripId == TripId).OrderBy(i => i.Order).ToList();

            if (itineraries.Any())
            {
                ViewBag.Itineraries = itineraries;
            }

            return View();
        }

        [HttpGet]
        public IActionResult Upsert(int ItineraryId, int TripId)
        {
            var itinerary = new Itinerary();
            itinerary.TripId = TripId;

            if (ItineraryId == 0 || ItineraryId == null)
            {
                return View(itinerary);
            }

            itinerary = _unitOfWork.Itinerary.Get(i => i.Id == ItineraryId);

            return View(itinerary);
        }

        [HttpPost]
        public IActionResult Upsert(Itinerary itinerary, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (itinerary.TripId == 0)
                {
                    return BadRequest("TripId is required.");
                }

                var trip = _unitOfWork.Trip.Get(i => i.Id == itinerary.TripId);


                if (itinerary.Date < trip.StartDate)
                {
                    ModelState.AddModelError("Date", " Date should more or equal to  trip start date");

                    ViewBag.TripId = itinerary.TripId;
                    return View(itinerary);
                }

                if (itinerary.Date > trip.EndDate)
                {
                    ModelState.AddModelError("Date", " Date should be less or equal to trip end date");

                    ViewBag.TripId = itinerary.TripId;
                    return View(itinerary);
                }



                trip.Status = TripStatus.Draft;
                _unitOfWork.Trip.Update(trip);

                string wwwRootPath = _hostingEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string itineraryPath = Path.Combine(wwwRootPath, @"images\itineraries");

                    if (!string.IsNullOrEmpty(itinerary.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath =
                            Path.Combine(wwwRootPath, itinerary.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(itineraryPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    itinerary.ImageUrl = @"\images\itineraries\" + fileName;
                }


                if (itinerary.Id == 0)
                {
                    // Create new itinerary
                    _unitOfWork.Itinerary.Add(itinerary);
                    TempData["success"] = "Itinerary created successfully";
                }
                else
                {
                    _unitOfWork.Itinerary.Update(itinerary);
                    TempData["success"] = "Itinerary updated successfully";
                }

                _unitOfWork.Save();

                // Redirect to the list of itineraries for the trip
                return RedirectToAction("Index", new { tripId = itinerary.TripId });
            }

            ViewBag.TripId = itinerary.TripId;
            return View(itinerary);
        }

        [HttpPost]
        public IActionResult Delete(int ItineraryId, int? tripId = null)
        {
            var itineraryToBeDeleted = _unitOfWork.Itinerary.Get(c => c.Id == ItineraryId);
            if (itineraryToBeDeleted == null)
            {
                return NotFound();
            }

            if (itineraryToBeDeleted.ImageUrl != null)
            {
                string oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, itineraryToBeDeleted.ImageUrl.TrimStart('\\'));

                if (oldImagePath != null)
                {
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
            }

            _unitOfWork.Itinerary.Remove(itineraryToBeDeleted);
            _unitOfWork.Save();

            return RedirectToAction("Index", new { TripId = tripId });
        }
    }
}
