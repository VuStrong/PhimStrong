var jsSelectedCate = [];

$(function () {
    if (movieid) {
        $.get(`/api/movies/${movieid}/categories`, function (data, status) {
            if (status === 'success') {
                data.forEach(d => {
                    jsSelectedCate.push({
                        id: d.id,
                        name: d.name
                    });
                })

                $('#categories-text').text(
                    jsSelectedCate.map(c => c.name).join(', ')
                );
            }

            fetchCategories();
        });
    } else {
        fetchCategories();
    }
});

async function fetchCategories() {
    const res = await fetch('/api/categories');

    if (!res.ok) return;

    const categories = await res.json();

    let htmlContent = "";
    if (categories.length > 0) {
        categories.forEach(function (category) {
            let isAdded = 'Thêm';
            let addedBtn = 'btn-info';

            if (jsSelectedCate.some(c => c.id == category.id)) {
                isAdded = 'Đã thêm';
                addedBtn = 'btn-success';
            }

            htmlContent = htmlContent.concat(
                `<tr>
                    <th class="cate-name" scope="row">${category.name}</th>
                    <td>
                        <button cateid="${category.id}" name="${category.name}" class="btn ${addedBtn} add-cate-btn">${isAdded}</button>
                    </td>
                </tr>`
            );
        });
    }

    $('#modal-category tbody').html(htmlContent);
    $('.add-cate-btn').click(function () {
        onClickAddCateBtn(this);
    });
}

// hàm push selected category vào array
function onClickAddCateBtn(btn) {
    let btnName = $(btn).attr('name');
    let cateid = $(btn).attr('cateid');

    if (jsSelectedCate.some(c => c.id === cateid)) {
        jsSelectedCate = jsSelectedCate.filter(c => c.id !== cateid);

        $(btn).removeClass('btn-success').addClass('btn-info').text('Thêm');
    } else {
        jsSelectedCate.push({
            id: cateid,
            name: btnName
        });

        $(btn).removeClass('btn-info').addClass('btn-success').text('Đã thêm');
    }

    $('#categories-text').text(
        jsSelectedCate.map(c => c.name).join(', ')
    );
}

$('#confirm-modal-category-btn').click(function () {
    hideModalCategory();
});

$('#modal-category').click(function () {
    hideModalCategory();
});

$('.modal-category-dialog').click(function (e) {
    e.stopPropagation()
});

// show modal :
function showModalCategory() {
    $('#modal-category').fadeIn(200).slideDown();
}

// hide modal :
function hideModalCategory() {
    $('#modal-category').hide();

    let temp = jsSelectedCate.map(c => c.name).join(', ');
    $('#select-category').val(temp);
}