/*await Html.RenderPartialAsync("_ValidationScriptsPartial");*/

// Toggle visibility of price and quota fields based on IsPublicToggle checkbox
function togglePriceQuota() {
    const toggle = document.getElementById("IsPublicToggle");
    const section = document.getElementById("priceQuotaSection");
    const ticketPrice = document.getElementById("TicketPrice");
    const ticketQuota = document.getElementById("TicketQuota");

    if (toggle.checked) {
        section.style.display = "none";
        if (ticketPrice) ticketPrice.value = 0;
        if (ticketQuota) ticketQuota.value = 0;
    } else {
        section.style.display = "block";
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const toggle = document.getElementById("IsPublicToggle");
    if (toggle) {
        togglePriceQuota(); // Initial load
        toggle.addEventListener("change", togglePriceQuota); // On change
    }
});

////Image upload and preview functionality
//$(function () {
//    var $dropArea = $('#drop-area');
//    var $fileInput = $('#ImageFile');
//    var $preview = $('#preview');

//    // Click to open file dialog
//    $dropArea.on('click', function () {
//        $fileInput.click();
//    });

//    // Highlight on drag over
//    $dropArea.on('dragover', function (e) {
//        e.preventDefault();
//    e.stopPropagation();
//    $(this).addClass('border-primary');
//    });

//    // Remove highlight on drag leave
//    $dropArea.on('dragleave', function (e) {
//        e.preventDefault();
//    e.stopPropagation();
//    $(this).removeClass('border-primary');
//    });

//    // Handle file drop
//    $dropArea.on('drop', function (e) {
//        e.preventDefault();
//    e.stopPropagation();
//    $(this).removeClass('border-primary');

//    var files = e.originalEvent.dataTransfer.files;
//                if (files.length > 0) {
//        $fileInput[0].files = files; // Set file input value
//    showPreview(files[0]);
//                }
//    });

//    // Handle file selected from dialog
//    $fileInput.on('change', function () {
//                if (this.files && this.files[0]) {
//        showPreview(this.files[0]);
//                }
//    });

//    function showPreview(file) {
//                var reader = new FileReader();
//    reader.onload = function (e) {
//        $preview.attr('src', e.target.result).show();
//                };
//    reader.readAsDataURL(file);
//            }
//});