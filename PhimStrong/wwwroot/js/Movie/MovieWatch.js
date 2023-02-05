$(function () {
    setTimeout(function () {
        $.post(
            '/Movie/IncreaseView?id=' + movieid,
        );
    }, 5000);

    var v = document.querySelector('#movie-video source');
    v.addEventListener("error", function (e) {
        $('#movie-player').append('<p class="text-danger position-absolute">Video không thể tải được <i class="bi bi-emoji-frown-fill"></i></p>');
    });
});