(function () {

    'use strict';

    // basic
    $("#form").validate({
        highlight: function (label) {
            $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
        },
        success: function (label) {
            $(label).closest('.form-group').removeClass('has-error');
            label.remove();
        },
        errorPlacement: function (error, element) {
            var placement = element.closest('.input-group');
            if (!placement.get(0)) {
                placement = element;
            }
            if (error.text() !== '') {
                placement.after(error);
            }
        }
    });
    //验证消息扩展
    $.extend($.validator.messages, {
        required: "please input the value"
    });
    // validation summary
    //var $summaryForm = $("#summary-form");
    //$summaryForm.validate({

    //    errorPlacement: function (error, element) {
    //        $(element).parent().after(error);
    //    },
    //    wrapper: "div",
    //    showErrors: function (errorMap, errorList) {
    //        // 遍历错误列表
    //        for (var obj in errorMap) {
    //            // 自定义错误提示效果
    //            $('#' + obj).parent().addClass('has-error');
    //        }
    //        // 此处注意，一定要调用默认方法，这样保证提示消息的默认效果
    //        this.defaultShowErrors();
    //    },
    //    success: function (label) {
    //        $(label).parent().prev().removeClass('has-error');
    //    }
    //});

    $("#proInfoTab").trigger("click");

}).apply(this, [jQuery]);

