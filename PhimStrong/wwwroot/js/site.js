// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// hover dropdown :
$(document).ready(function () {
    $('ul.navbar-nav li.dropdown:not(.ignore-hover)').hover(function () {
        $(this).find('.dropdown-menu').attr('data-bs-popper', 'none').stop(true, true).delay(100).fadeIn(200);
    }, function () {
        $(this).find('.dropdown-menu').stop(true, true).delay(100).fadeOut(200);
    });
});

// show modal :
function showModal(title, body) {
    $('#modal').fadeIn(200).slideDown();
    $('#modal .modal-title').text(title);
    $('#modal .modal-body p').text(body);
}

// hide modal :
function hideModal() {
    $('#modal').hide();
}