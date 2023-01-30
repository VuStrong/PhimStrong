var jsCastList = [];
var jsCastSearchList = [];
var jsSelectedCast = [];

$('.hidden-cast p').each(function (e) {
    jsSelectedCast.push($(this).text());
});

// push cast name to cast array
$('.cast-name').each(function (i, cast) {
    jsCastList.push(cast.innerText);
});

$('#search-cast').on('keyup', function () {
    let content = $(this).val();

    if (content) {
        jsCastList.forEach(function (c) {
            if (c.toLowerCase().includes(content.toLowerCase())) {
                jsCastSearchList.push(c);
            }
        });
    } else {
        jsCastSearchList = jsCastList;
    }

    let htmlContent = "";
    if (jsCastSearchList) {
        jsCastSearchList.forEach(function (e) {
            let isAdded = 'Thêm';
            let addedBtn = 'btn-info';

            if (jsSelectedCast.includes(e)) {
                isAdded = 'Đã thêm';
                addedBtn = 'btn-success';
            }

            htmlContent = htmlContent.concat(
                `<tr>
                    <th class="cast-name" scope="row">${e}</th>
                    <td>
                        <button name="${e}" class="btn ${addedBtn} add-cast-btn">${isAdded}</button>
                    </td>
                </tr>`
            );
        });
    }

    $('#modal-cast tbody').html(htmlContent);
    $('.add-cast-btn').click(function () {
        onClickAddCastBtn(this);
    });

    htmlContent = "";
    jsCastSearchList = [];
});

$('.add-cast-btn').click(function () {
    onClickAddCastBtn(this);
});

function onClickAddCastBtn(btn) {
    let btnName = $(btn).attr('name');
    let ind = jsSelectedCast.indexOf(btnName);

    if (ind > -1) {
        jsSelectedCast.splice(ind, 1);
        $(btn).removeClass('btn-success').addClass('btn-info').text('Thêm');
    } else {
        jsSelectedCast.push(btnName);
        $(btn).removeClass('btn-info').addClass('btn-success').text('Đã thêm');
    }

    $('#casts-text').text(
        jsSelectedCast.join(',')
    );
}

function modalCast(callback) {
    $('#confirm-modal-cast-btn').click(function () {
        callback();
    });
}

$('#modal-cast').click(function () {
    hideModalCast();
});

$('.modal-cast-dialog').click(function (e) {
    e.stopPropagation()
});

// show modal :
function showModalCast() {
    $('#modal-cast').fadeIn(200).slideDown();
}

// hide modal :
function hideModalCast() {
    $('#modal-cast').hide();
}