//used to hide the result notification after some seconds on page load
$(document).ready(function () {
    $('.alert-position').delay(2000).slideUp('slow');
});

//ajax call to mark the selected image as profile image
function selectImageAsProfile(id) {
    $.post("/PersonalDetails/SetPhotoAsProfile", { id: id },
        function (data) {
            window.location.reload();
        });
}

//makes the modal for image upload visible
function showUploadModal() {
    $('#exampleModalCenter').modal('show')
}

//ajax call the delete the selected image
function deleteImage(id) {
    $.post("/PersonalDetails/DeleteImage", { id: id },
        function (data) {
            window.location.reload();
        });
}


//used to show - hide the corresponding input fields on payment method change 
function onPaymentMethodChanged() {
    var value = $('#payment-method').val();
    if (value == "1") { //card
        $('#payment-details-card').show();
        $('#card-number').attr("required", "required");
        $('#card-expiry-month').attr("required", "required");
        $('#card-expiry-year').attr("required", "required");
        $('#card-cvc').attr("required", "required");
        $('#payment-details-paypal').hide();
        $('#paypal-email').removeAttr("required");
    }
    else { //paypal
        $('#card-number').removeAttr("required");
        $('#card-expiry-month').removeAttr("required");
        $('#card-expiry-year').removeAttr("required");
        $('#card-cvc').removeAttr("required");
        $('#payment-details-card').hide();
        $('#payment-details-paypal').show();
        $('#paypal-email').attr("required", "required");
    }
}


//used to toggle the selected list
function toggleHobbiesList(listContainer, toggleButton, container) {
    var showHeight;
    var hideHeight;

    switch (container) { //based on the list, different heights are needed for visible - hidden
        case "preferences-personal-info":
            showHeight = 250;
            hideHeight = 130;
            break;
        case "preferences-appearance-info":
            showHeight = 380;
            hideHeight = 220;
            break;
        default:
            break;
    }

    if ($('#'+listContainer).hasClass('hobbies-hidden')) { //if hidden, show
        $('#' + listContainer).slideDown('slow');
        $('#' + toggleButton).removeClass('fa-angle-down');
        $('#'+toggleButton).addClass('fa-angle-right');
        $('#' + listContainer).removeClass('hobbies-hidden');
        if(container != null && container != undefined)
            $('#' + container).animate({ height: showHeight }, { queue: false, duration: 500 });
    }
    else { //if visible, hide
        $('#' + listContainer).slideUp('slow');
        $('#' + toggleButton).removeClass('fa-angle-right');
        $('#'+toggleButton).addClass('fa-angle-down');
        $('#' + listContainer).addClass('hobbies-hidden');
        if (container != null && container != undefined) {
            if (container == 'preferences-appearance-info') { //2 lists side by side, we need to toggle the height of the container only when both are hidden
                if ($('#eye-colors').hasClass('hobbies-hidden') && $('#hair-colors').hasClass('hobbies-hidden')) {
                    $('#' + container).animate({ height: hideHeight }, { queue: false, duration: 500 });
                }
            }
            else {
                $('#' + container).animate({ height: hideHeight }, { queue: false, duration: 500 });
            }
        }   
    }
}

//ajax call to insert the rating of the selected user
function rateUser(id, rating) {
    $.post("/Rating/RateUser", { id: id, rating: rating },
        function (response) {
            window.location.reload();
    });
}

//used to refresh the download button since it was stuck for some reason after clicking
function refreshBtn() {
    $('#downloadBtn').blur();
}