var jsCateList = [];
var jsCateSearchList = [];
var jsSelectedCate = [];

// push selected category from hidden to array
$('.hidden-category p').each(function (e) {
    jsSelectedCate.push($(this).text());
});

// push category name to cate array
$('.category-name').each(function (i, cate) {
    jsCateList.push(cate.innerText);
});

// event khi search
$('#search-category').on('keyup', function () {
    let content = $(this).val();

    if (content) {
        // push ptử khớp kết quả vào array
        jsCateList.forEach(function (c) {
            if (c.toLowerCase().includes(content.toLowerCase())) {
                jsCateSearchList.push(c);
            }
        });
    } else {
        jsCateSearchList = jsCateList;
    }

    // tạo html chứa các ptử khớp vs kết quả search
    let htmlContent = "";
    if (jsCateSearchList) {
        jsCateSearchList.forEach(function (e) {
            let isAdded = 'Thêm';
            let addedBtn = 'btn-info';

            if (jsSelectedCate.includes(e)) {
                isAdded = 'Đã thêm';
                addedBtn = 'btn-success';
            }

            htmlContent = htmlContent.concat(
                `<tr>
                        <th class="category-name" scope="row">${e}</th>
                        <td>
                            <button name="${e}" class="btn ${addedBtn} add-cate-btn">${isAdded}</button>
                        </td>
                    </tr>`
            );
        });
    }

    $('#modal-category tbody').html(htmlContent);
    $('.add-cate-btn').click(function () {
        onClickAddCateBtn(this);
    });

    // reset
    htmlContent = "";
    jsCateSearchList = [];
});

$('.add-cate-btn').click(function () {
    onClickAddCateBtn(this);
});

// hàm push selected category vào array
function onClickAddCateBtn(btn) {
    let btnName = $(btn).attr('name');
    let ind = jsSelectedCate.indexOf(btnName);

    if (ind > -1) {
        jsSelectedCate.splice(ind, 1);
        $(btn).removeClass('btn-success').addClass('btn-info').text('Thêm');
    } else {
        jsSelectedCate.push(btnName);
        $(btn).removeClass('btn-info').addClass('btn-success').text('Đã thêm');
    }

    $('#categories-text').text(
        jsSelectedCate.join(',')
    );
}

// tạo event khi chọn confirm
function modalCategory(callback) {
    $('#confirm-modal-category-btn').click(function () {
        callback();
    });
}

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
}