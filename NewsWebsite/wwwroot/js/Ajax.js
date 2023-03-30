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
    $(document).on('click', 'a[data-toggle="ajax-modal"]', function () {
        console.log("111111111111111");
        var url = $(this).data('url');
        $.ajax({
            url: url,
            beforeSend: function () { $('#modal-placeholder').after('<div class="preloader d-flex align-items-center justify-content-center"><div class="lds-ellipsis"><div></div><div></div><div></div><div></div></div></div>'); },
            complete: function () { $('.preloader').remove(); },
            error: function () {
                ShowSweetErrorAlert();
            }
        }).done(function (result) {
            placeholder.html(result);
            placeholder.find('.modal').modal('show');
        });
    });

    $(document).on('click', 'a[data-toggle="ajax-load-register"]', function () {
        var url = $(this).data('url');
        $.ajax({
            url: url,
            beforeSend: function () { $('#modal-placeholder').after('<div class="preloader d-flex align-items-center justify-content-center"><div class="lds-ellipsis"><div></div><div></div><div></div><div></div></div></div>'); },
            complete: function () { $('.preloader').remove(); },
            error: function () {
                ShowSweetErrorAlert();
            }
        }).done(function (result) {
            $("#pills-signout").html(result);
            $("#pills-signout").addClass("active show");
            $("#pills-signin").removeClass("active show");
            $("#pills-signin-tab").removeClass("active  bg-danger text-white").addClass("bg-gray");
            $("#pills-signup-tab").addClass("active  bg-danger text-white");
            $("#modal-title").html("عضویت در سایت")
            $(".modal-footer").html(' <button id="btn-register" type="button" class="btn btn-block btn-danger" data-save="modal-ajax"><i class="fa fa-sign-out"></i> عضویت</button>')
        });
    });

    $(document).on('click', 'a[data-toggle="pill"]', function () {
        $("#pills-signup-tab").removeClass("active  bg-danger text-white").addClass("bg-gray");
        $("#pills-signin-tab").addClass("active  bg-danger");
        $("#modal-title").html("ورود به سایت")
        $(".modal-footer").html(' <button id="btn-signin" type="button" class="btn btn-block btn-danger" data-save="modal-ajax"><i class="fa fa-sign-in"></i> ورود </button>');
    });

    placeholder.on('click', 'button[data-save="modal-ajax"]', function () {
        var form;
        var IsValid;
        var btnId = $(this).attr('id');
        if (btnId == "btn-register") {
            form = $(this).parents(".modal").find('#pills-signout form');
        }
        else if (btnId == "btn-signin") {
            form = $(this).parents(".modal").find('#pills-signin form');
        }
        else {
            form = $(this).parents(".modal").find('form');
        }

        var actionUrl = form.attr('action');
        var dataToSend = new FormData(form.get(0));
        $.ajax({
            url: actionUrl, type: "post", data: dataToSend, processData: false, contentType: false, error: function () {
                ShowSweetErrorAlert();
            },
            beforeSend: function () { $('#modal-placeholder').after('<div class="preloader d-flex align-items-center justify-content-center"><div class="lds-ellipsis"><div></div><div></div><div></div><div></div></div></div>'); },
        }).done(function (data) {
            if (btnId == "btn-register") {
                $('#pills-signout').html(data);
                IsValid = $('#pills-signout').find("input[name='IsValid']").val() === "True";
                if (IsValid) {
                    $.ajax({ url: '/Admin/Base/Notification', error: function () { ShowSweetErrorAlert(); } }).done(function (notification) {
                        ShowSweetSuccessAlert(notification)
                    });
                    placeholder.find(".modal").modal('hide');
                }
            }

            else if (btnId == "btn-signin") {
                if (data == "success") {
                    window.location.href = '/Account/Profile/';
                }
                else if (data == "requiresTwoFactor") {
                    alert("requires");
                }

                else {
                    $('#pills-signin').html($("#pills-signin", data));
                }
            }
            $('.preloader').remove();
        });
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

$(document).on('click', 'a[data-toggle="tab"]', function () {
    var url = $(this).data('url');
    var id = $(this).attr('id');
    var contentDivId = "#MostViewedNewsDiv";
    var loadingDivId = "#nav-mostViewedNews";
    if ($(this).hasClass("most-talk")) {
        contentDivId = "#MostTalkNewsDiv";
        loadingDivId = "#nav-mostTalkNews";
    }

    $.ajax({
        url: url,
        beforeSend: function () { $(loadingDivId).html("<p class='text-center mb-5 mt-3'><span style='font-size:18px;font-family: Vazir_Medium;'>در حال بارگزاری اطلاعات خبر </span><img src='/icons/LoaderIcon.gif'/></p>") },
        error: function () {
            ShowSweetErrorAlert();
        }
    }).done(function (result) {
        $(contentDivId).html(result);
        $(contentDivId + " a").removeClass("active");
        $("#" + id).addClass("active");
    });
});

$(document).on('click', 'button[data-save="Ajax"]', function () {
    var form = $(".newsletter-widget").find('form');
    var actionUrl = form.attr('action');
    var dataToSend = new FormData(form.get(0));

    $.ajax({
        url: actionUrl, type: "post", data: dataToSend, processData: false, contentType: false, error: function () {
            ShowSweetErrorAlert();
        }
    }).done(function (data) {
        var newForm = $("form", data);
        $(".newsletter-widget").find("form").replaceWith(newForm);
        var IsValid = newForm.find("input[name='IsValid']").val() === "True";
        if (IsValid) {
            $.ajax({ url: '/Admin/Base/Notification', error: function () { ShowSweetErrorAlert(); } }).done(function (notification) {
                ShowSweetSuccessAlert(notification)
            });
        }
    });
});

function ShowCommentForm(parentCommentId, newsId) {
    $.ajax({
        url: "/Admin/Comments/SendComment?parentCommentId=" + parentCommentId + "&&newsId=" + newsId,
        beforeSend: function () { $("#comment-" + parentCommentId).after("<p class='text-center mb-5 mt-3'><span style='font-size:18px;font-family: Vazir_Medium;'> لطفا منتظر بماند  </span><img src='/icons/LoaderIcon.gif'/></p>") },
        error: function () {
            ShowSweetErrorAlert();
        }
    }).done(function (result) {
        $("#comment-" + parentCommentId).next().replaceWith("");
        $("#comment-" + parentCommentId).after("<hr/>" + result);
        $("#btn-" + parentCommentId).html("لغو پاسخ");
        $("#btn-" + parentCommentId).attr("onclick", "HideCommentForm('" + parentCommentId + "','" + newsId + "')");
    });
}
function HideCommentForm(parentCommentId, newsId) {
    $("#comment-" + parentCommentId).next().replaceWith("");
    $("#comment-" + parentCommentId).next().replaceWith("");
    $("#btn-" + parentCommentId).html("پاسخ");
    $("#btn-" + parentCommentId).attr("onclick", "ShowCommentForm('" + parentCommentId + "')");
}

function SendComment(parentCommentId) {
    var form = $("#reply-" + parentCommentId).find('form');
    var actionUrl = form.attr('action');
    var dataToSend = new FormData(form.get(0));
    var loaderAfter = "#comment-" + parentCommentId;
    if ($("#comment-" + parentCommentId).length == 0) {
        loaderAfter = "#reply-"
    }
    $.ajax({
        url: actionUrl, type: "post", data: dataToSend, processData: false, contentType: false, error: function () {
            ShowSweetErrorAlert();
        },
        beforeSend: function () {
            $(".vizew-btn").attr("disabled", true);
            $(loaderAfter).after("<p class='text-center mb-5 mt-3'><span style='font-size:18px;font-family: Vazir_Medium;'> در حال ارسال دیدگاه  </span><img src='/icons/LoaderIcon.gif'/></p>")
        },
        complete: function () {
            $(".vizew-btn").attr("disabled", false);
            $(loaderAfter).next().replaceWith("");
        }
    }).done(function (data) {
        var newForm = $("form", data);
        $("#reply-" + parentCommentId).find("form").replaceWith(newForm);
        var IsValid = newForm.find("input[name='IsValid']").val() === "True";
        if (IsValid) {
            $("#comment-" + parentCommentId).next().replaceWith("");
            $("#comment-" + parentCommentId).next().replaceWith("");
            $.ajax({ url: '/Admin/Base/Notification', error: function () { ShowSweetErrorAlert(); } }).done(function (notification) {
                ShowSweetSuccessAlert(notification)
            });
            $("#Name").val("");
            $("#Email").val("");
            $("#Desription").val("");
        }
    });
}

