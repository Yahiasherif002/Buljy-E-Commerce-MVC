let dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes('approved')) {
        loadDataTable('approved');
    }
    else if (url.includes('pending')) {
        loadDataTable('pending');
    }
    else if (url.includes('inprocess')) {
        loadDataTable('inprocess');
    }
    else if (url.includes('completed')) {
        loadDataTable('completed');
    }
    else if (url.includes('cancelled')) {
        loadDataTable('cancelled');
    } else { 
        loadDataTable('all');
    }

});

function loadDataTable(status) {
    dataTable = $('#ordersTable').DataTable({
        responsive: true,
        scrollX: true,
        ajax: {
            url: '/Admin/Order/GetAllOrders?status='+status,
            type: 'GET',
            dataType: 'json',
            error: function (xhr, error, thrown) {
                toastr.error('Error loading orders. Please try again.');
                console.error('DataTables error:', error);
                console.error('Server response:', xhr.responseText);
            }
        },
        processing: true,
        language: {
            processing: '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>',
            emptyTable: 'No orders found',
            zeroRecords: 'No matching orders found',
            info: "Showing _START_ to _END_ of _TOTAL_ orders",
            infoEmpty: "Showing 0 to 0 of 0 orders",
            infoFiltered: "(filtered from _MAX_ total orders)",
            search: "Search orders:",
            lengthMenu: "Show _MENU_ orders per page"
        },
        columns: [
            {
                data: 'id',
                className: 'text-center',
                width: '10%'
            },
            {
                data: 'applicationUser',
                className: 'align-middle',
                render: function (data) {
                    return data ? `<div class="user-info">
                                    <div class="user-name">${data.firstName} ${data.lastName}</div>
                                    <div class="user-email">${data.email}</div>
                                 </div>` : 'N/A';
                },
                width: '20%'
            },
            {
                data: 'orderDate',
                className: 'align-middle',
                render: function (data) {
                    return data ? new Date(data).toLocaleDateString('en-US', {
                        year: 'numeric',
                        month: 'short',
                        day: 'numeric'
                    }) : 'N/A';
                },
                width: '15%'
            },
            {
                data: 'orderTotal',
                className: 'text-end align-middle',
                render: function (data) {
                    return data ? new Intl.NumberFormat('en-US', {
                        style: 'currency',
                        currency: 'USD'
                    }).format(data) : '$0.00';
                },
                width: '15%'
            },
            {
                data: 'orderStatus',
                className: 'text-center align-middle',
                render: function (data) {
                    if (!data) return '<span class="badge bg-secondary">Unknown</span>';

                    let badgeClass = '';
                    switch (data.toLowerCase()) {
                        case 'pending':
                            badgeClass = 'bg-warning text-dark';
                            break;
                        case 'approved':
                            badgeClass = 'bg-success';
                            break;
                        case 'processing':
                            badgeClass = 'bg-info text-dark';
                            break;
                        case 'shipped':
                            badgeClass = 'bg-primary';
                            break;
                        case 'cancelled':
                            badgeClass = 'bg-danger';
                            break;
                        default:
                            badgeClass = 'bg-secondary';
                    }
                    return `<span class="badge ${badgeClass}">${data}</span>`;
                },
                width: '15%'
            },
            {
                data: 'paymentStatus',
                className: 'text-center align-middle',
                render: function (data) {
                    if (!data) return '<span class="badge bg-secondary">Unknown</span>';

                    let badgeClass = '';
                    let icon = '';

                    switch (data.toLowerCase()) {
                        case 'approved':
                            badgeClass = 'bg-success';
                            icon = 'bi-check-circle-fill';
                            break;
                        case 'ApprovedForDelayedPayment':
                            badgeClass = 'bg-warning text-dark';
                            icon = 'bi-clock-fill';
                            break;
                        case 'Pending':
                            badgeClass = 'bg-warning text-dark';
                            icon = 'bi-clock-fill';
                            break;
                        case 'cancelled':
                            badgeClass = 'bg-danger';
                            icon = 'bi-x-circle-fill';
                            break;
                        default:
                            badgeClass = 'bg-secondary';
                            icon = 'bi-question-circle-fill';
                    }

                    return `<span class="badge ${badgeClass}">
                                <i class="bi ${icon} me-1"></i>${data}
                            </span>`;
                },
                width: '15%'
            },
            {
                data: 'id',
                className: 'text-center align-middle',
                render: function (data) {
                    return `
                        <div class="btn-group" role="group">
                            <a href="/Admin/Order/Details/?orderId=${data}" 
                               class="btn btn-sm btn-outline-primary"
                               title="View Details">
                                <i class="bi bi-eye"></i>
                            </a>
                            <button onclick="printOrder(${data})" 
                                    class="btn btn-sm btn-outline-secondary"
                                    title="Print Order">
                                <i class="bi bi-printer"></i>
                            </button>
                        </div>`;
                },
                width: '10%',
                orderable: false
            }
        ],
        order: [[2, 'desc']],
        drawCallback: function () {
            $(window).trigger('resize');
        },
        createdRow: function (row, data, dataIndex) {
            $(row).addClass('align-middle');
        }
    });
}

function printOrder(orderId) {
    if (!orderId) {
        toastr.error('Invalid order ID');
        return;
    }
    window.open(`/Admin/Order/Print/${orderId}`, '_blank');
}

function refreshTable() {
    if (dataTable) {
        dataTable.ajax.reload();
    }
}