var dataTable;

$(document).ready(function () {
    loadDataTable()
});

function loadDataTable() {
    dataTable = $("#tblData").dataTable({
        "ajax": {
            "url": "/inquiry/GetInquiryList"
        },
        "language": {
            "sEmptyTable": "Нет данных",
            "sInfo": "Показано с _START_ по _END_ из _TOTAL_ записей",
            "sInfoEmpty": "Показано 0 из 0 записей",
            "sInfoFiltered": "(фильтровано из _MAX_ записей)",
            "sInfoPostFix": "",
            "sInfoThousands": ",",
            "sLengthMenu": "Показать _MENU_ записей",
            "sLoadingRecords": "Загрузка...",
            "sProcessing": "Обработка...",
            "sSearch": "Поиск:",
            "sZeroRecords": "Записей не найдено",
            "oPaginate": {
                "sFirst": "Первая",
                "sLast": "Последняя",
                "sNext": "Следующая",
                "sPrevious": "Предыдущая"
            },
            "oAria": {
                "sSortAscending": ": активировать для сортировки столбца по возрастанию",
                "sSortDescending": ": активировать для сортировки столбца по убыванию"
            }
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "fullName", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "email", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                        <a href="/Inquiry/Details/${data}" class="text-white" style="cursor:pointer">
                            <i class="fas fa-edit btn btn-success"></i>
                        </a>
                    </div>
                    `;
                },
                "width": "5%"
            }
        ]
    })
}
