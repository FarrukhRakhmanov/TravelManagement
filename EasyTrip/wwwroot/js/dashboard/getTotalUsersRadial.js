$(document).ready(function () {
    loadTotalUsersRadial();
});

function loadTotalUsersRadial() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/admin/dashboard/GetRegisteredUserChartData",
        type: "GET",
        dataType: "json",
        success: function (data) {
            document.querySelector("#spanTotalUsersCount").innerHTML = data.totalCount;

            var sectionCurrentCount = document.createElement("span");
            if (data.hasRatioIncreased) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i><span>' + data.currentMonthCount + '</span>';
            }
            else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i><span>' + data.currentMonthCount + '</span>';
            }

            document.querySelector("#sectionUsersCount").append(sectionCurrentCount);
            document.querySelector("#sectionUsersCount").append("since last month");

            $(".chart-spinner").hide();

            loadRadialBarChart("totalUsersRadialChart", data);
        }
    });
}
