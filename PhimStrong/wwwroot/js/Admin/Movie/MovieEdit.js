// modal confirm
$('#delete-movie-btn').click(function (e) {
    e.preventDefault();
    showModal("Xóa ?", "Xác nhận xóa phim này ?");
});

modalConfirm(function () {
    $('#delete-movie').submit();
});

// Tạo event khi ấn nút confirm modal của category modal
modalCategory(function () {
    hideModalCategory();

    let temp = jsSelectedCate.join(',');
    $('#select-category').val(temp);
});

// Tạo event khi ấn nút confirm modal của cast modal
modalCast(function () {
    hideModalCast();

    let temp = jsSelectedCast.join(',');
    $('#select-cast').val(temp);
});

// Tạo event khi ấn nút confirm modal của director modal
modalDirector(function () {
    hideModalDirector();

    let temp = jsSelectedDirector.join(',');
    $('#select-director').val(temp);
});

$('#movie-name').on('change', function () {
    $('#movie-name-valid').text('');
});

$('#movie-translate-name').on('change', function () {
    $('#movie-translate-name-valid').text('');
});

$('#movie-img').on('change', function () {
    $('#movie-image-valid').text('');
});

$('#movie-rating').on('change', function () {
    $('#movie-rating-valid').text('');
});

$('#status-list').on('change', function () {
    $('#status-list-valid').text('');
});

$('#select-episode').on('change', handlePhimBo);

var selectedVideos = [];
// event khi change thẻ select loại phim
$('#type-list').on('change', function (e) {
    $('#type-list-valid').text('');

    if ($(this).val() === 'Phim lẻ') {

        $('#select-video').html(
            `<p class="text-success"> ** Video</p>
                    <label>Dán Url Video vào đây : </label>
                    <input class="url-video" placeholder="Url"/>`
        );
        $('#select-url').html('');

    } else if ($(this).val() === 'Phim bộ') {

        $('#select-video').html(
            `<p class="text-success">Nhập số tập : </p>
                    <label>Số tập : </label>
                    <input id="select-episode" type="number" placeholder="0" />`
        );

        // blur khỏi thẻ input số tập thì sẽ sinh ra html tương ứng số tập
        $('#select-episode').on('change', handlePhimBo);
    } else {
        $('#select-video').html('');
        $('#select-url').html('');
    }

});

function handlePhimBo() {
    selectedVideos = [];
    $('.url-video').each(function (i, e) {
        selectedVideos.push({
            'Episode': i + 1,
            'VideoUrl': $(e).val()
        });
    });

    let episodeCount = $(this).val();
    let episodeHtml = '';
    for (let i = 1; i <= episodeCount; i++) {
        let urlVid = "";
        if (selectedVideos[i - 1]) urlVid = selectedVideos[i - 1]['VideoUrl'];

        episodeHtml = episodeHtml.concat(
            `<div id="episodes" class="mx-5">
                                        <label>Tập ${i} : </label>
                                        <input class="url-video" value="${urlVid}" placeholder="Dán Url vào đây"/>
                                    </div>`
        );
    }

    $('#select-url').html(
        `${episodeHtml}`
    );
}

// click to send data to server
$('#edit-movie-btn').click(function () {
    var error = undefined;

    if ($('#type-list').val() === 'none') {
        $('#type-list-valid').text('Chưa chọn loại phim !');
        error = "error";
    }

    if ($('#status-list').val() === 'none') {
        $('#status-list-valid').text('Chưa chọn trạng thái !');
        error = "error";
    }

    if (!$('#movie-name').val()) {
        $('#movie-name-valid').text('Chưa nhập tên phim !');
        error = "error";
    }

    if (!$('#movie-translate-name').val()) {
        $('#movie-translate-name-valid').text('Chưa nhập tên phim !');
        error = "error";
    }

    // Check rating valid
    var ratingPattern = /^\d+,?\d*$/;
    if (!ratingPattern.test($('#movie-rating').val())) {
        $('#movie-rating-valid').text('Điểm rating không phù hợp !');
        error = "error";
    }

    // check .png, .jpg image
    var files = $('#movie-img').prop("files");
    if (files.length > 0) {

        if (!files[0].name.endsWith('.png') && !files[0].name.endsWith('.jpg')) {
            $('#movie-image-valid').text('File ảnh phải có định dạng .png, .jpg');

            error = "error";
        }
    }

    // nếu có lỗi thì return ko gửi lên server
    if (error) {
        toastr.error("Chưa nhập hết các trường !");
        return;
    }

    // tạo mảng chứa các url video
    var postVidUrl = [];
    $('.url-video').each(function (i, e) {
        postVidUrl.push($(e).val());
    });

    var formData = new FormData();
    formData.append("Name", $('#movie-name').val());
    formData.append("TranslateName", $('#movie-translate-name').val());
    formData.append("Description", $('#movie-desc').val());
    formData.append("ReleaseDate", $('#movie-date').val());
    formData.append("Type", $('#type-list').val());
    formData.append("Status", $('#status-list').val());
    formData.append("Length", $('#movie-length').val());
    formData.append("Rating", $('#movie-rating').val());
    formData.append("Tags", $('#select-tag').val());

    if ($('#movie-image-text').val())
        formData.append("Image", $('#movie-image-text').val());

    let count = $('#type-list').val() === "Phim lẻ" ? 1 : postVidUrl.length;
    formData.append("EpisodeCount", count);

    if (files.length > 0) {
        formData.append("ImageFile", files[0]);
    }

    if (jsSelectedCate.length > 0) {
        jsSelectedCate.forEach(c => {
            formData.append("Categories[]", c);
        })
    }

    if (jsSelectedCast.length > 0) {
        jsSelectedCast.forEach(c => {
            formData.append("Casts[]", c);
        })
    }

    if (jsSelectedDirector.length > 0) {
        jsSelectedDirector.forEach(d => {
            formData.append("Directors[]", d);
        })
    }

    if ($('#select-country').val())
        formData.append("Country", $('#select-country').val());

    if (postVidUrl.length > 0) {
        postVidUrl.forEach(v => {
            formData.append("Videos[]", v);
        })
    }

    if ($('#movie-trailer').val().trim())
        formData.append("Trailer", $('#movie-trailer').val().trim());

    $('#edit-movie-btn').addClass('disabled');

    $.ajax({
        url: '/admin/movie/edit?movieid=' + movieid,
        dataType: "json",
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        traditional: true,
        success: function (data) {
            if (data.success) {
                window.location.href = "/admin/movie";
            } else {
                toastr.error(data.error);
                $('#edit-movie-btn').removeClass('disabled');
            }
        },
        error: function (xhr, err) {
            toastr.error(err);
            $('#edit-movie-btn').removeClass('disabled');
        }
    });
});