$(document).ready(function () {
    loadTotalRevenueRadial();
});

function loadTotalRevenueRadial() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/admin/dashboard/GetRevenueChartData",
        type: "GET",
        dataType: "json",
        success: function (data) {
            document.querySelector("#spanTotalRevenueCount").innerHTML = data.totalCount;

            var sectionCurrentCount = document.createElement("span");
            if (data.hasRatioIncreased) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i><span>' + data.currentMonthCount + '</span>';
            }
            else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i><span>' + data.currentMonthCount + '</span>';
            }

            document.querySelector("#sectionRevenueCount").append(sectionCurrentCount);
            document.querySelector("#sectionRevenueCount").append("since last month");

            $(".chart-spinner").hide();

            loadRadialBarChart("totalRevenueRadialChart", data);
        }
    });
}
