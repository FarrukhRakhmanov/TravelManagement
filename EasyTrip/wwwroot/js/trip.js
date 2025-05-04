var dataTable;

window.onload = function () {
    loadDataTable();
};

dataTable = $('#tblData').DataTable({
    "ajax": {
        url: `/admin/trip/getall`,
        dataSrc: 'data' 
    },
    "columns": [
        {
            data: 'tripId',
            "render": function (data) {
                return `
                    <div class="w-75 btn-group" role="group">
                       <a href="/admin/trip/upsert?tripId=${data}" class="btn btn-primary mx-2">
                        <i class="bi bi-pencil-square"></i>
                       </a>
                       <a onClick="Delete('/admin/trip/delete/${data}')" class="btn btn-danger mx-2">
                        <i class="bi bi-trash"></i>
                       </a>
                    </div>`;
            },
            "width": "20%"
        },
        { data: 'title', "width": "10%" },
        { data: 'destination', "width": "10%" },
        { data: 'startDate', "width": "15%" },
        { data: 'endDate', "width": "15%" },
        { data: 'discountPricePerPerson', "width": "8%" },
        { data: 'singleSupplement', "width": "8%" },
        {
            data: 'status',
            "render": function (data) {
                const statusEnum = {
                    0: { text: "Draft", color: "text-info" },
                    1: { text: "Published", color: "text-success" },
                    2: { text: "Cancelled", color: "text-dark" },
                    3: { text: "Ongoing", color: "text-primary" },
                    4: { text: "Finished", color: "text-warning" },
                    5: { text: "Sold", color: "text-danger" },
                };
                const status = statusEnum[data];
                return status
                    ? `<span class="badge ${status.color}">${status.text}</span>`
                    : `<span class="badge badge-secondary">Unknown</span>`;
            },
            "width": "8%"
        },
        { data: 'seatsAvailable', "width": "8%" },
        {
            data: 'imageUrl',
            "render": function (data) {
                const imageUrl = data ? data.replace(/\\/g, '/') : "/images/avatars/placeholder.png";
                return `<img src="${imageUrl}" class="align-items-center rounded" alt="Trip Image" style="width: 100px; height: auto;" />`;
            },
            "width": "18%"
        }
    ]
});

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,               
                type: 'DELETE',
                contentType: 'application/json', 
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                },
                error: function (xhr, status, error) {
                    console.error("Error deleting data:", error);
                    toastr.error("Failed to delete the record.");
                }
            });
        }
    });
}


