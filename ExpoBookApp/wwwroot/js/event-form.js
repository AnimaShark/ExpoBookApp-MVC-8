/*await Html.RenderPartialAsync("_ValidationScriptsPartial");*/

// Toggle visibility of price and quota fields based on IsPublicToggle checkbox

$(document).ready(function () {
    console.log("DOM ready");

    const toggle = $('#IsPublicToggle');
    const priceInput = $('#TicketPrice');
    const quotaInput = $('#TicketQuota');

    function togglePriceQuota() {
        if (toggle.is(':checked')) {
            $('#priceQuotaSection').hide();
            priceInput.val(0).prop('readonly', true);
            quotaInput.val(0).prop('readonly', true);
        } else {
            $('#priceQuotaSection').show();
            priceInput.prop('readonly', false);
            quotaInput.prop('readonly', false);
        }
    }

    toggle.on('change', togglePriceQuota);

    // Trigger it once on page load
    togglePriceQuota();
});

//function togglePriceQuota() {
//    const toggle = document.getElementById("IsPublicToggle");
//    //const section = document.getElementById("priceQuotaSection");
//    const ticketPrice = document.getElementById("TicketPrice");
//    const ticketQuota = document.getElementById("TicketQuota");

//    console.log("Toggle is:", toggle?.checked);
//    if (!toggle || !section) {
//        console.error("Toggle or section not found!");
//        return;
//    }

//    if (toggle.checked) {
//        //section.classList.add("d-none");
//        if (ticketPrice) ticketPrice.value = 0;
//        if (ticketQuota) ticketQuota.value = 0;
//        ticketPrice.readOnly = true;
//        ticketQuota.readOnly = true;
//        ticketPrice.classList.add("bg-light");
//        ticketQuota.classList.add("bg-light");
//    } else {
//        //section.classList.remove("d-none");
//        ticketPrice.readOnly = false;
//        ticketQuota.readOnly = false;
//        ticketPrice.classList.remove("bg-light");
//        ticketQuota.classList.remove("bg-light");
//    }
//}

//$(document).ready(function () {
//    togglePriceQuota(); /// Run on initial load
//    const toggle = document.getElementById("IsPublicToggle");
//    if (toggle) {
//        toggle.addEventListener("change", togglePriceQuota); // On change
//    }
//});

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