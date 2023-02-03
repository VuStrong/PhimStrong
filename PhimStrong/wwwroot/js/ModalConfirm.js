function modalConfirm(callback) {
    $('#confirm-modal-btn').click(function () {
        callback();
    });
}

$('#modal').click(function () {
    hideModal();
});

$('.modal-dialog').click(function (e) {
    e.stopPropagation()
});

// show modal :
function showModal(title, body) {
    $('#modal').fadeIn(200).show();
    $('#modal .modal-title').text(title);
    $('#modal .modal-body p').text(body);
}

// hide modal :
function hideModal() {
    $('#modal').hide();
}