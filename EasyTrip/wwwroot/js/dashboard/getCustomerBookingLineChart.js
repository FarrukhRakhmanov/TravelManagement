$(document).ready(function () {
    loadCustomerAndBookingLineChart();
});

function loadCustomerAndBookingLineChart() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/admin/dashboard/GetUserAndBookingLineChartData",
        type: "GET",
        dataType: "json",
        success: function (data) {
            console.log("Raw Response:", data);
            try {
                if (typeof data === "string") {
                    data = JSON.parse(data); // Force parse if string
                }
                console.log("Parsed JSON:", data);
                loadLineChart("newCustomersAndBookingsChart", data);
            } catch (e) {
                console.error("Invalid JSON:", e);
            }

            $(".chart-spinner").hide();
        }

    });
}

function loadLineChart(id, data) {
    var chartColors = getChartColorsArray(id);
    options = {
        series: data.series,
        colors: chartColors,
        chart: {
            type: 'line',
            width: 500,
            height: 200
        },
        stroke: {
            curve: 'smooth',
            width: 2
        },
        markers: {
            size: 6,
            strokeWidth: 0,
            hover: {
                size: 9
            }
        },
        xaxis: {
            categories: data.categories,
        },
        
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}