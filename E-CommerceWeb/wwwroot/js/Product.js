//$(document).ready(function () {
//    $('#tblData').DataTable({
//        "processing": true,
//        "serverSide": true,
//        "ajax": {
//            "url": "/admin/product/GetAll",
//            "type": "GET",
//            "dataSrc": "data" // Ensures it points to the 'data' property in JSON
//        },
//        "columns": [
//            { "data": "title" },
//            { "data": "author" },
//            { "data": "isbn" },
//            { "data": "price" }, // Ensure price is formatted correctly
//            { "data": "category.name" } // Nested property
//        ],
//        "language": {
//            "emptyTable": "No data available",
//            "info": "Showing _START_ to _END_ of _TOTAL_ entries",
//            "infoEmpty": "Showing 0 to 0 of 0 entries",
//            "infoFiltered": "(filtered from _MAX_ total entries)",
//            "lengthMenu": "Show _MENU_ entries",
//            "loadingRecords": "Loading...",
//            "processing": "Processing...",
//            "search": "Search:",
//            "zeroRecords": "No matching records found"
//        }
//    });
//});
