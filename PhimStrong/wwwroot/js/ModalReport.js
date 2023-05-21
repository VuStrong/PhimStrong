$('#report-modal-btn').click(function () {
    let email = $('#report-email').val().trim();
    let content = $('#report-content').val().trim();

    if (!email) {
        $('#report-email-error').text('Chưa nhập Email !');
    }
    if (!content) {
        $('#report-content-error').text('Chưa nhập nội dung !');
    }

    if (email && content) {
        $('#report-modal-btn').addClass('disabled');
        $.post(
            `/user/report`,
            {
                Email: email,
                Content: content
            },
            function (data) {
                if (data.success) {
                    toastr.success('Báo lỗi thành công, hãy chờ web phản hồi nhé.');
                } else {
                    toastr.error('Báo lỗi thất bại, hãy thử lại.');
                }

                $('#report-modal-btn').removeClass('disabled');
                hideModalReport();
            }
        );
    }
});

$('#report-email').on('change', function () {
    $('#report-email-error').text('');
})

$('#report-content').on('change', function () {
    $('#report-content-error').text('');
})

$('#report-modal').click(function () {
    hideModalReport();
});

$('.report-modal-dialog').click(function (e) {
    e.stopPropagation()
});

// show modal :
function showModalReport(title) {
    $('#report-modal').fadeIn(200).show();
    $('#report-modal .modal-title').text(title);
}

// hide modal :
function hideModalReport() {
    $('#report-modal').hide();
}