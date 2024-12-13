function validateShipOrder(orderId) {
    const carrier = document.querySelector('[name="OrderHeader.Carrier"]').value;
    const trackingNumber = document.querySelector('[name="OrderHeader.TrackingNumber"]').value;

    if (!carrier || !trackingNumber) {
        Swal.fire({
            title: 'Missing Information',
            text: 'Please provide both carrier and tracking number before shipping the order.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Update Details',
            cancelButtonText: 'Cancel',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                // Open the update modal
                const updateModal = new bootstrap.Modal(document.getElementById('updateOrderModal'));
                updateModal.show();
            }
        });
        return false;
    }

    // If validation passes, show confirmation
    return Swal.fire({
        title: 'Ship Order',
        text: 'Are you sure you want to mark this order as shipped?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes, ship it!',
        cancelButtonText: 'Cancel',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            // Submit the form
            document.getElementById('shipOrderForm').submit();
            return true;
        }
        return false;
    });
}

function confirmCancelOrder(orderId) {
    return Swal.fire({
        title: 'Cancel Order',
        text: 'Are you sure you want to cancel this order? This action cannot be undone.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, cancel it!',
        cancelButtonText: 'No, keep it',
        confirmButtonColor: '#dc3545',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById('cancelOrderForm').submit();
            return true;
        }
        return false;
    });
}