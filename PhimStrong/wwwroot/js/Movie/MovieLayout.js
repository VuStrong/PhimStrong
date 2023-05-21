$(function () {
	function handleLikeAndResponse() {
		// add event to like comment button
		$('.like-btn').click(function (e) {
			e.preventDefault();

			var likeBtn = $(this);
			let cmtid = likeBtn.closest('.cmt-item').attr('cmt-id');
			$.post(
				`/comment/like-comment?commentid=${cmtid}`,
				{},
				function (data) {
					if (data.success) {
						let likeCount = parseInt(likeBtn.siblings('.cmt-like-count').text());

						// update html
						likeBtn.siblings('.cmt-like-count')
							.html(`<i class="bi bi-hand-thumbs-up-fill text-info"></i> ${likeCount + 1}`);

						toastr.success('Đã like comment !');
					} else {
						toastr.error('Có lỗi khi like comment :((');
					}
				}
			);
		});

		$('.delete-cmt-btn').click(function (e) {
			e.preventDefault();

			var deleteCmt = $(this);
			var deleteUrl = deleteCmt.attr('href');
			if (deleteUrl) {
				$.post(
					deleteUrl,
					{},
					function (data) {
						if (data.success) {
							toastr.success('Đã xóa comment này !');
							deleteCmt.closest(".cmt-item").fadeOut().remove();
						} else {
							toastr.error('Có lỗi khi xóa comment này :((');
						}
					}
				);
			}
		});

		// add event to response comment button
		$('.response-btn').click(function (e) {
			e.preventDefault();

			$('.response-area button').parent().html('');

			$(this).closest('.cmt-item').find('.response-area').html(
				`<textarea type="text" placeholder="Phản hồi"></textarea>
						<button class="btn btn-success">Gửi</button>`
			);

			$('.response-area button').click(function () {
				let content = $('.response-area textarea').val().trim();
				let responseCmt = $(this).closest('.cmt-item');

				if (content) {
					$(this).addClass('disabled');

					$.post(
						'/comment/create',
						{
							Content: content,
							MovieId: movieid,
							ResponseToId: responseCmt.attr('cmt-id')
						},
						function(data) {
							if (data.success) {
								let rowColor = "gray";
								if (data.userrole == "Thủy Tổ") rowColor = "yellowgreen";
								else if (data.userrole == "Admin") rowColor = "red";

								$(responseCmt).find('.response-container').append(
									`<div class="row flex-nowrap mb-2">
										<div style="width: 80px; height: 80px;">
										<img src="${data.useravatar}" class= "w-100" />
										</div>
										<div class="flex-fill">
										<strong class="text-success">
											${data.username}
											<span class="ms-2" style="color: ${rowColor}; font-size: 13px;">(${data.userrole})</span>
										</strong>
										<p class="text-dark">${data.cmtcontent}</p>
										<div>
										<a href="#" class="me-2">Thích</a>
										<p class="text-dark d-inline mx-2">
											<i class="bi bi-hand-thumbs-up-fill text-info"></i> 0
										</p>
										<p class="d-inline" style="color: gray;">0 ngày</p>
										</div>
										</div>
									</div>`
								);
							} else {
								toastr.error('Có lỗi xảy ra khi comment :((');
							}

							$('.response-area button').removeClass('disabled').parent().html('');
						}
					);
				}
			});
		});
	}

	var commentPage = 1;
	var firstUrl = `/comment/get-comments-partial?movieid=${movieid}&page=${commentPage}`;
	// render comment after page finish loaded
	$('#cmt-container').load(firstUrl, "", function () {
		handleLikeAndResponse();

		// event click expand btn to load more comment
		$('#expand-cmt-btn').click(function () {
			commentPage++;
			let url = `/comment/load-more-comments?movieid=${movieid}&page=${commentPage}`;

			$('#expand-cmt-btn').addClass('disabled');

			$.get(url, function (data) {
				$('#expand-cmt-btn').removeClass('disabled');
				$('#cmt-body').append(data);
				handleLikeAndResponse();
			});
		});

		// event click comment btn to create comment
		$('#cmt-btn').click(function () {
			let content = $('#user-cmt-input').val().trim();

			if (content) {
				$(this).addClass('disabled');

				$.post(
					'/comment/create',
					{
						Content: content,
						MovieId: movieid
					},
					function(data) {
						$('#user-cmt-input').val('');

						if (data.success) {
							let rowColor = "gray";
							if (data.userrole == "Thủy Tổ") rowColor = "yellowgreen";
							else if (data.userrole == "Admin") rowColor = "red";

							$('#cmt-body').prepend(
								`<div class="row flex-nowrap mb-2">
									<div style="width: 80px; height: 80px;">
									<img src="${data.useravatar}" class= "w-100" />
									</div>
									<div class="flex-fill">
									<strong class="text-success">
										${data.username}
										<span class="ms-2" style="color: ${rowColor}; font-size: 13px;">(${data.userrole})</span>
									</strong>
									<p class="text-dark">${data.cmtcontent}</p>
									<div>
									<a href="#" class="me-2">Thích</a>
									<a href="#">Phản hồi</a>
									<p class="text-dark d-inline mx-2">
										<i class="bi bi-hand-thumbs-up-fill text-info"></i> 0
									</p>
									<p class="d-inline" style="color: gray;">0 ngày</p>
									</div>
									</div>
								</div>`
							);

							$('#cmt-btn').removeClass('disabled');
						} else {
							toastr.error('Có lỗi xảy ra khi comment :((');
							$('#cmt-btn').removeClass('disabled');
						}
					}
				);
			}
		});
	});

	// Render related movie after page loaded
	$('#relate-movies').load("/movie/get-related-movies?movieid=" + movieid, "", function () {
		// scroll
		var itemIndex = 1;
		var distance = $('.owl-slide .movie-item:nth-child(2)').offset().left - $('.owl-slide').offset().left
		var justClick = false;

		setInterval(function () {
			if (!isOnScreen(document.querySelector('#relate-movies'))) return;

			if (justClick)
				justClick = false;
			else {
				itemIndex++;
				if (itemIndex > 6) itemIndex = 1;

				scroll(itemIndex);
			}
		}, 3000);

		$('.prev-btn').click(function () {
			justClick = true;

			itemIndex--;
			if (itemIndex <= 0) itemIndex = 6;

			scroll(itemIndex);
		});

		$('.next-btn').click(function () {
			justClick = true;

			itemIndex++;
			if (itemIndex > 6) itemIndex = 1;

			scroll(itemIndex);
		});

		function scroll(el) {
			$('.owl-slide').stop().animate(
				{
					scrollLeft: (el - 1) * distance
				},
				1000 //speed
			);
		}

		function isOnScreen(element) {
			const position = element.getBoundingClientRect();

			return position.top >= 0 && position.bottom <= window.innerHeight;
		}
	});
});