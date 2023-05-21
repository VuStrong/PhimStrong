$(function () {

    $("#edit-button").click(function () {
        $(this).hide();
        $("#save-button").show();
        $("#cancel-button").show();
        $("#avatar-text").show();

        $(".edit-elements").each(function () {
            let item = $(this);
            item.show().val(item.prev().text());
            item.prev().hide();
        });
    });

    $("#cancel-button").click(function () {
        cancel();
    });

    $("#save-button").click(function () {
        $(this).addClass('disabled');

        let myFiles = $("#edit-avatar").prop('files');
        let formData = new FormData();

        if (myFiles.length > 0) {
            for (let i = 0; i < myFiles.length; i++) {
                formData.append('AvatarFile', myFiles[i]);
            }
        }

        formData.append('DisplayName', $("#edit-name").val());
        formData.append('PhoneNumber', $("#edit-phone").val());
        formData.append('FavoriteMovie', $("#edit-fav-movie").val());
        formData.append('Hobby', $("#edit-hobby").val());

        $.ajax({
            url: '/identity/account/edit-information',
            dataType: "json",
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                $('#save-button').removeClass('disabled');
                cancel();

                if (data.success) {
                    $("#display-name").text(data.displayname);
                    $("#phone").text(data.phone);
                    $("#fav-movie").text(data.favoritemovie);
                    $("#hobby").text(data.hobby);
                    if (data.avatar) $("#avatar").attr("src", data.avatar);

                    toastr.success("Cập nhập thông tin thành công !");
                } else {
                    if (data.error)
                        toastr.error(data.error);
                    else
                        toastr.error("Cập nhập thông tin thất bại !");
                }

            }
        });
    });

    function cancel() {
        $("#cancel-button").hide();
        $("#save-button").hide();
        $("#edit-button").show();
        $("#avatar-text").hide();

        $(".edit-elements").each(function () {
            let item = $(this);
            item.hide();
            item.prev().show();
        });
    }
});