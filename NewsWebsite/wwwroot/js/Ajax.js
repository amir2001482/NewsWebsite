$(function () {
    var placeholder = $("#modal-placeholder");
    $(document).on('click','button[data-toggle="ajax-modal"]',function () {
        var url = $(this).data('url');
        $.ajax({
            url: url,
            beforeSend: function () { ShowLoading(); },
            complete: function () { $("body").preloader("remove"); },
            error: function () {
                ShowSweetErrorAlert();
            }
        }).done(function (result) {
            placeholder.html(result);
            placeholder.find('.modal').modal('show');
        });
    });

    placeholder.on('click', 'button[data-save="modal"]', function () {
        ShowLoading();
        var form = $(this).parents(".modal").find('form');
        var actionUrl = form.attr('action');
        if (form.length == 0)
        {
            form = $(".card-body").find("form");
            actionUrl = form.attr('action') + "/" + $(".modal").attr("id");
        }
        var dataToSend = new FormData(form.get(0));

        $.ajax({
            url: actionUrl, type: "post", data: dataToSend, processData: false, contentType: false, error: function () {
                ShowSweetErrorAlert();
            }}).done(function (data) {
                var newBody = $(".modal-body", data);
                var newFooter = $(".modal-footer", data);
                placeholder.find(".modal-body").replaceWith(newBody);
                placeholder.find(".modal-footer").replaceWith(newFooter);

            var IsValid = newBody.find("input[name='IsValid']").val() === "True";
            if (IsValid) {
                $.ajax({ url: '/Admin/Base/Notification', error: function () { ShowSweetErrorAlert(); } }).done(function (notification) {
                    ShowSweetSuccessAlert(notification)
                });

                $table.bootstrapTable('refresh')
                placeholder.find(".modal").modal('hide');
            }
        });

        $("body").preloader("remove");
    });
});

function ShowSweetErrorAlert() {
    Swal.fire({
        type: 'error',
        title: 'خطایی رخ داده است !!!',
        text: 'لطفا تا برطرف شدن خطا شکیبا باشید.',
        confirmButtonText: 'بستن'
    });
}

function ShowLoading() {
    $("body").preloader({ text: "لطفا صبر کنید..." });
}

function ShowSweetSuccessAlert(message) {
    Swal.fire({
        position: 'top-middle',
        type: 'success',
        title: message,
        confirmButtonText: 'بستن',
    })
}

