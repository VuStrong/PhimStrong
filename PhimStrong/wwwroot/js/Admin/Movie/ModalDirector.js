var jsSelectedDirector = [];
var directorController;

$(function () {
    if (movieid) {
        $.get(`/api/movies/${movieid}/directors`, function (data, status) {
            if (status === 'success') {
                data.forEach(d => {
                    jsSelectedDirector.push({
                        id: d.id,
                        name: d.name
                    });
                })

                $('#directors-text').text(
                    jsSelectedDirector.map(d => d.name).join(', ')
                );
            }

            fetchDirectors();
        });
    } else {
        fetchDirectors();
    }
});

async function fetchDirectors(name, signal) {
    const url = name ? `/api/directors?q=${name}&size=100` : '/api/directors?size=100';
    const res = await fetch(url, { signal });

    if (!res.ok) return;

    const data = await res.json();
    const directors = data.results;

    let htmlContent = "";
    if (directors.length > 0) {
        directors.forEach(function (director) {
            let isAdded = 'Thêm';
            let addedBtn = 'btn-info';

            if (jsSelectedDirector.some(d => d.id == director.id)) {
                isAdded = 'Đã thêm';
                addedBtn = 'btn-success';
            }

            htmlContent = htmlContent.concat(
                `<tr>
                    <th scope="row">
                        <img src="${director.avatar || '/src/img/UserAvatars/default_avatar.png'}" class="avatar avatar-square img-fit" style="width: 50px; height:50px;">
                    </th>
                    <td class="director-name">${director.name}</td>
                    <td>
                        <button directorid="${director.id}" name="${director.name}" class="btn ${addedBtn} add-director-btn">${isAdded}</button>
                    </td>
                </tr>`
            );
        });
    }

    $('#modal-director tbody').html(htmlContent);
    $('.add-director-btn').click(function () {
        onClickAddDirectorBtn(this);
    });
}

$('#search-director').on('keyup', function () {
    let content = $(this).val();

    if (directorController) directorController.abort();

    directorController = new AbortController();
    const signal = directorController.signal;

    fetchDirectors(content, signal);
});

function onClickAddDirectorBtn(btn) {
    let btnName = $(btn).attr('name');
    let directorid = $(btn).attr('directorid');

    if (jsSelectedDirector.some(d => d.id === directorid)) {
        jsSelectedDirector = jsSelectedDirector.filter(d => d.id !== directorid);

        $(btn).removeClass('btn-success').addClass('btn-info').text('Thêm');
    } else {
        jsSelectedDirector.push({
            id: directorid,
            name: btnName
        });

        $(btn).removeClass('btn-info').addClass('btn-success').text('Đã thêm');
    }

    $('#directors-text').text(
        jsSelectedDirector.map(d => d.name).join(', ')
    );
}

$('#confirm-modal-director-btn').click(function () {
    hideModalDirector();
});

$('#modal-director').click(function () {
    hideModalDirector();
});

$('.modal-director-dialog').click(function (e) {
    e.stopPropagation()
});

// show modal :
function showModalDirector() {
    $('#modal-director').fadeIn(200).slideDown();
}

// hide modal :
function hideModalDirector() {
    $('#modal-director').hide();

    let temp = jsSelectedDirector.map(d => d.name).join(', ');
    $('#select-director').val(temp);
}