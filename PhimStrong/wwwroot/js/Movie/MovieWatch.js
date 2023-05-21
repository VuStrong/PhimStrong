$(function () {
    // increase view after 5s
    setTimeout(function () {
        $.post(
            '/movie/increase-view?id=' + movieid,
        );
    }, 5000);

    // if load movie fail, show message
    var v = document.querySelector('#movie-video source');
    v.addEventListener("error", function (e) {
        $('#movie-player').append('<p class="text-danger position-absolute p-3">Video không thể tải được <i class="bi bi-emoji-frown-fill"></i></p>');
    });

    $('#report-btn').click(function () {
        showModalReport('Báo lỗi phim');
    });
});