var jsDirectorList = [];
var jsDirectorSearchList = [];
var jsSelectedDirector = [];

$('.hidden-director p').each(function (e) {
    jsSelectedDirector.push($(this).text());
});

// push director name to director array
$('.director-name').each(function (i, cast) {
    jsDirectorList.push(cast.innerText);
});

$('#search-director').on('keyup', function () {
    let content = $(this).val();

    if (content) {
        jsDirectorList.forEach(function (d) {
            if (d.toLowerCase().includes(content.toLowerCase())) {
                jsDirectorSearchList.push(d);
            }
        });
    } else {
        jsDirectorSearchList = jsDirectorList;
    }

    let htmlContent = "";
    if (jsDirectorSearchList) {
        jsDirectorSearchList.forEach(function (e) {
            let isAdded = 'Thêm';
            let addedBtn = 'btn-info';

            if (jsSelectedDirector.includes(e)) {
                isAdded = 'Đã thêm';
                addedBtn = 'btn-success';
            }

            htmlContent = htmlContent.concat(
                `<tr>
                        <th class="director-name" scope="row">${e}</th>
                        <td>
                            <button name="${e}" class="btn ${addedBtn} add-director-btn">${isAdded}</button>
                        </td>
                    </tr>`
            );
        });
    }

    $('#modal-director tbody').html(htmlContent);
    $('.add-director-btn').click(function () {
        onClickAddDirectorBtn(this);
    });

    htmlContent = "";
    jsDirectorSearchList = [];
});

$('.add-director-btn').click(function () {
    onClickAddDirectorBtn(this);
});

function onClickAddDirectorBtn(btn) {
    let btnName = $(btn).attr('name');
    let ind = jsSelectedDirector.indexOf(btnName);

    if (ind > -1) {
        jsSelectedDirector.splice(ind, 1);
        $(btn).removeClass('btn-success').addClass('btn-info').text('Thêm');
    } else {
        jsSelectedDirector.push(btnName);
        $(btn).removeClass('btn-info').addClass('btn-success').text('Đã thêm');
    }

    $('#directors-text').text(
        jsSelectedDirector.join(',')
    );
}

function modalDirector(callback) {
    $('#confirm-modal-director-btn').click(function () {
        callback();
    });
}

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
}