var jsSelectedCast = [];
var castController;

$(function () {
    if (movieid) {
        $.get(`/api/movies/${movieid}/casts`, function (data, status) {
            if (status === 'success') {
                data.forEach(d => {
                    jsSelectedCast.push({
                        id: d.id,
                        name: d.name
                    });
                })

                $('#casts-text').text(
                    jsSelectedCast.map(c => c.name).join(', ')
                );
            }

            fetchCasts();
        });
    } else {
        fetchCasts();
    }
});

async function fetchCasts(name, signal) {
    const url = name ? `/api/casts?q=${name}&size=100` : '/api/casts?size=100';
    const res = await fetch(url, { signal });

    if (!res.ok) return;

    const data = await res.json();
    const casts = data.results;

    let htmlContent = "";
    if (casts.length > 0) {
        casts.forEach(function (cast) {
            let isAdded = 'Thêm';
            let addedBtn = 'btn-info';

            if (jsSelectedCast.some(c => c.id == cast.id)) {
                isAdded = 'Đã thêm';
                addedBtn = 'btn-success';
            }

            htmlContent = htmlContent.concat(
                `<tr>
                    <th scope="row">
                        <img src="${cast.avatar || '/src/img/UserAvatars/default_avatar.png'}" class="avatar avatar-square img-fit" style="width: 50px; height:50px;">
                    </th>
                    <td class="cast-name overflow-hidden">${cast.name}</td>
                    <td>
                        <button castid="${cast.id}" name="${cast.name}" class="btn ${addedBtn} add-cast-btn">${isAdded}</button>
                    </td>
                </tr>`
            );
        });
    }

    $('#modal-cast tbody').html(htmlContent);
    $('.add-cast-btn').click(function () {
        onClickAddCastBtn(this);
    });
}

$('#search-cast').on('keyup', function () {
    let content = $(this).val();

    if (castController) castController.abort();

    castController = new AbortController();
    const signal = castController.signal;

    fetchCasts(content, signal);
});

function onClickAddCastBtn(btn) {
    let btnName = $(btn).attr('name');
    let castid = $(btn).attr('castid');

    if (jsSelectedCast.some(c => c.id === castid)) {
        jsSelectedCast = jsSelectedCast.filter(c => c.id !== castid);

        $(btn).removeClass('btn-success').addClass('btn-info').text('Thêm');
    } else {
        jsSelectedCast.push({
            id: castid,
            name: btnName
        });

        $(btn).removeClass('btn-info').addClass('btn-success').text('Đã thêm');
    }

    $('#casts-text').text(
        jsSelectedCast.map(c => c.name).join(', ')
    );
}

$('#confirm-modal-cast-btn').click(function () {
    hideModalCast();
});

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

    let temp = jsSelectedCast.map(c => c.name).join(', ');
    $('#select-cast').val(temp);
}