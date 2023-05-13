$(function () {
	// render like movie button after page loaded
	$('.like-movie').load("/Movie/GetLikeButton?movieid=" + movieid, "", function () {
		$('#like-movie-btn').click(function (e) {
			e.preventDefault();

			if ($(this).is('[notsignin]')) {
				window.location.href = "/login?returnUrl=/Movie/Detail/" + movieid;
				return;
			}

			$.post(
				"/Movie/LikeMovie?movieid=" + movieid,
				{},
				function (data) {
					if (data.success) {
						if (data.like) {
							$('#like-movie-btn').find('strong').text('Đã thích');
							let likeCount = parseInt($('#like-movie-btn').find('span').text());
							$('#like-movie-btn').find('span').text(likeCount + 1);

							toastr.success('Like thành công !');
						} else {
							$('#like-movie-btn').find('strong').text('Thích');
							let likeCount = parseInt($('#like-movie-btn').find('span').text());
							$('#like-movie-btn').find('span').text(likeCount - 1);

							toastr.success('Đã bỏ like phim này !');
						}
					} else {
						toastr.error('Like thất bại, hãy thử lại :((');
					}
				}
			);
		});
	});
});