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

// scroll to top :
$('#scroll-top-btn').click(function () {
    $("html, body").animate({ scrollTop: 0 });
});

// Event search movie
$('#search-form').submit(function (e) {
    e.preventDefault();

    var value = $(this).children('input').val().trim();
    if (!value) return;

    $(this).attr('action', "/movie/" + value);
    this.submit();
});