$(function () {
    setTimeout(function () {
        $.post(
            '/Movie/IncreaseView?id=' + movieid,
        );
    }, 5000);
});