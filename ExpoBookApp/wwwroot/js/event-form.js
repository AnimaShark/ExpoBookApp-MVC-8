/*await Html.RenderPartialAsync("_ValidationScriptsPartial");*/

// Toggle visibility of price and quota fields based on IsPublicToggle checkbox
$(document).ready(function () {
    console.log("DOM ready");

    const toggle = $('#IsPublicToggle');
    const priceInput = $('#TicketPrice');
    const quotaInput = $('#TicketQuota');

    // Disable submit button initially
    $('#submitButton').prop('disabled', true);

    // Enable/disable based on form validity
    $('#eventForm').on('input change', function () {
        if (this.checkValidity()) {
            $('#submitButton').prop('disabled', false);
        } else {
            $('#submitButton').prop('disabled', true);
        }
    });

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

    const venueSelect = $('#VenueSelect');

    //Handle auto capacity estimate for Venue selection
    venueSelect.on('change', function () {
        const selectedOption = $(this).find('option:selected');
        const size = parseInt(selectedOption.data('size'));

        if (!isNaN(size)) {
            const estimatedCapacity = Math.floor((size * 0.8) / 5);
            quotaInput.val(estimatedCapacity);
        }
    });
});

//Start date and end date validation
$(document).ready(function () {
    const $form = $('#eventForm');
    const $start = $('#StartDate');
    const $end = $('#EndDate');
    const $submit = $('#submitButton');

    if ($form.length === 0 || $start.length === 0 || $end.length === 0) {
        console.warn("Form or date fields not found.");
        return;
    }

    function validateDates() {
        const start = new Date($start.val());
        const end = new Date($end.val());
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        let valid = true;
        $start[0].setCustomValidity('');
        $end[0].setCustomValidity('');

        if ($end.val() && end < start) {
            $end[0].setCustomValidity("End date must be after start date.");
            valid = false;
        }

        if ($start.val() && start < today) {
            $start[0].setCustomValidity("Start date must be after today.");
            valid = false;
        }

        return valid;
    }

    function toggleSubmit() {
        const isValid = validateDates();
        $submit.prop('disabled', !($form[0].checkValidity() && isValid));
    }

    $form.on('input change', toggleSubmit);
        toggleSubmit(); // initial check
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