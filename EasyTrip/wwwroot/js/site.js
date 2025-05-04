function confirmDelete(event) {
    event.preventDefault(); // Prevent the form from submitting immediately

    Swal.fire({
        title: 'Are you sure?',
        text: "This action cannot be undone!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel',
    }).then((result) => {
        if (result.isConfirmed) {
            // Get the closest form element relative to the button clicked
            var form = event.target.closest('form');

            // Debugging: Check if form is found
            if (form) {
                console.log("Form found, submitting...");
                form.submit(); // Submit the form
            } else {
                console.log("Form not found!");
            }
        }
    });
}
