using Domain.Enums;
using Domain.StaticDetails;
using Domain.ViewModels;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = UserRole.Admin)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetBookingRadialBarChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u =>
                u.Status == BookingStatus.Confirmed || u.Status == BookingStatus.Completed);

            var countByCurrentMonth = totalBookings.Count(u => u.BookingDate >= currentMonthStartDate
            );

            var countByPreviousMonth = totalBookings.Count(u => u.BookingDate >= previousMonthStartDate
                                                                && u.BookingDate <= currentMonthStartDate);
            return Json(GetRadialBarChartVM(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth));
        }

        public IActionResult GetRegisteredUserChartData()
        {
            var totalUsers = _unitOfWork.ApplicationUser.GetAll();

            var countByCurrentMonth = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate);

            var countByPreviousMonth = totalUsers.Count(u => u.CreatedAt >= previousMonthStartDate
                                                                && u.CreatedAt <= currentMonthStartDate);

            return Json(GetRadialBarChartVM(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth));
        }

        public IActionResult GetRevenueChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u =>
               u.Status == BookingStatus.Completed);

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(u => u.TotalAmount));

            var countByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate
            ).Sum(u => u.TotalAmount);

            var countByPreviousMonth = totalBookings.Where(u => u.BookingDate >= previousMonthStartDate
                                                                && u.BookingDate <= currentMonthStartDate).Sum(u => u.TotalAmount);

            return Json(GetRadialBarChartVM(totalRevenue, countByCurrentMonth, countByPreviousMonth));
        }

        public IActionResult GetBookingPieChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.UtcNow.AddDays(-30)
                                                                && (u.Status == BookingStatus.Confirmed || u.Status == BookingStatus.Completed));

            var customerWithOneBooking = totalBookings.GroupBy(b => b.ApplicationUserId)
                .Where(x => x.Count() == 1).Select(x => x.Key).ToList();

            int bookingsByNewCustomer = customerWithOneBooking.Count();
            int bookingsByReturningCustomer = totalBookings.Count() - bookingsByNewCustomer;

            PieChartVM pieChartVm = new()
            {
                Labels = ["New Users Bookings", "Returning Users Bookings"],
                Series = [bookingsByNewCustomer, bookingsByReturningCustomer]
            };

            return Json(pieChartVm);
        }

        public IActionResult GetUserAndBookingLineChartData()
        {
            var bookingData = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.UtcNow.AddDays(-30)
                                                              && u.BookingDate.Date <= DateTime.UtcNow)
                .GroupBy(b => b.BookingDate.Date)
                .Select(u => new
                {
                    DateTime = u.Key,
                    NewBookingCount = u.Count()
                });

            var customerData = _unitOfWork.ApplicationUser.GetAll(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30)
                                                              && u.CreatedAt.Date <= DateTime.UtcNow)
                .GroupBy(b => b.CreatedAt.Date)
                .Select(u => new
                {
                    DateTime = u.Key,
                    NewCustomerCount = u.Count()
                });

            var leftJoin = bookingData.GroupJoin(customerData, booking => booking.DateTime,
                customer => customer.DateTime,
                (booking, customer) => new
                {
                    booking.DateTime,
                    booking.NewBookingCount,
                    NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()

                });

            var rightJoin = customerData.GroupJoin(bookingData, customer => customer.DateTime,
                booking => booking.DateTime,
                (customer, booking) => new
                {
                    customer.DateTime,
                    NewBookingCount = booking.Select(x => x.NewBookingCount).FirstOrDefault(),
                    customer.NewCustomerCount

                });

            var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();

            var newBookingData = mergedData.Select(x => x.NewBookingCount).ToArray();
            var newCustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();
            var categories = mergedData.Select(x => x.DateTime.ToString("MM/dd/yyyy")).ToArray();

            LineChartVM lineChartVm = new()
            {
                Categories = categories,
                Series = new List<ChartData>
                {
                    new ChartData
                    {
                        Name = "New Bookings",
                        Data = newBookingData
                    },
                    new ChartData
                    {
                        Name = "New Users",
                        Data = newCustomerData
                    }
                }
            };
            return Json(lineChartVm);
        }

        private static RadialBarChartVM GetRadialBarChartVM(int total, double? currentMonth, double? previousMonth)
        {
            RadialBarChartVM radialBarChartVm = new RadialBarChartVM();
            int increaseDecreaseRatio = 100;
            if (previousMonth != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((currentMonth - previousMonth) / previousMonth * 100);
            }
            radialBarChartVm.TotalCount = total;
            radialBarChartVm.CurrentMonthCount = Convert.ToInt32(currentMonth);
            radialBarChartVm.HasRatioIncreased = currentMonth > previousMonth;
            radialBarChartVm.Series = new int[] { increaseDecreaseRatio };
            return radialBarChartVm;
        }
    }
}
