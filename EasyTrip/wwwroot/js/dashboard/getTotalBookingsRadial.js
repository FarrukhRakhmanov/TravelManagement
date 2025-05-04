$(document).ready(function () {
    loadTotalBookingsRadial();
});

function loadTotalBookingsRadial() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/admin/dashboard/GetBookingRadialBarChartData",
        type: "GET",
        dataType: "json",
        success: function (data) {
            document.querySelector("#spanTotalBookingCount").innerHTML = data.totalCount;

            var sectionCurrentCount = document.createElement("span");
            if (data.hasRatioIncreased) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i><span>' + data.currentMonthCount + '</span>';
            }
            else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i><span>' + data.currentMonthCount + '</span>';
            }

            document.querySelector("#sectionBookingCount").append(sectionCurrentCount);
            document.querySelector("#sectionBookingCount").append("since last month");

            $(".chart-spinner").hide();

            loadRadialBarChart("totalBookingsRadialChart", data);
        }
    });
}