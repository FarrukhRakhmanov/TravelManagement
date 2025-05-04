$(document).ready(function () {
    loadCustomerBookingPieChart();
});

function loadCustomerBookingPieChart() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/admin/dashboard/GetBookingPieChartData",
        type: "GET",
        dataType: "json",
        success: function (data) {
            console.log("Raw Response:", data);
            try {
                if (typeof data === "string") {
                    data = JSON.parse(data); // Force parse if string
                }
                console.log("Parsed JSON:", data);
                loadPieChart("customerBookingsPieChart", data);
            } catch (e) {
                console.error("Invalid JSON:", e);
            }

            $(".chart-spinner").hide();
        }

    });
}

function loadPieChart(id, data) {
    var chartColors = getChartColorsArray(id);
    options = {
        series: data.series,
        labels: data.labels,
        colors: chartColors,
        chart: {
            type: 'pie',
            width: 240
        },
        stroke: {
            show: false
        },
        legend: {
            position: 'bottom',
            horizontalAlign: 'center',
            labels: {
                colors: "#fff",
                useSeriesColors: true
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}