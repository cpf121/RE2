(function () {

    'use strict';

    //验证消息扩展
    $.extend($.validator.messages, {
        required: "please input the value"
    });
    // validation summary
    var $summaryForm = $("#material-form");
    $summaryForm.validate({
        errorPlacement: function (error, element) {
            $(element).parent().after(error);
        },
        wrapper: "div",
        showErrors: function (errorMap, errorList) {
            // 遍历错误列表
            for (var obj in errorMap) {
                // 自定义错误提示效果
                $('#' + obj).parent().addClass('has-error');
            }
            // 此处注意，一定要调用默认方法，这样保证提示消息的默认效果
            this.defaultShowErrors();
        },
        success: function (label) {
            $(label).parent().prev().removeClass('has-error');
        }
    });


    // checkbox, radio and selects
    //$("#chk-radios-form, #selects-form").each(function () {
    //    $(this).validate({
    //        highlight: function (element) {
    //            $(element).closest('.form-group').removeClass('has-success').addClass('has-error');
    //        },
    //        success: function (element) {
    //            $(element).closest('.form-group').removeClass('has-error');
    //        }
    //    });
    //});



    $("#addOrUpdateClick").click(function () {
        //TODO:验证
        if (!($("#summary-form").valid())) return;
        var id = $("#MIId").val() || 0;
        var params = {
            Id: $("#MIId").val() || 0,
            MICustomerPart: $("#MICustomerPart").val(),
            MIProductName: $("#MIProductName").val(),
            MICustomer: $("#MICustomer").val(),
            MIPicture: $("#MIPicture").val(),
            MIIsValid: $("input[name='MIIsValid']:checked").val(),
            MIWorkOrder: $("#MIWorkOrder").val(),
            MIMaterial: $("#MIMaterial").val(),
            MIMaterialText: $("#MIMaterialText").val(),
            MITool: $("#MITool").val(),
            MITotalQty: $("#MITotalQty").val()
        }
        $.ajax({
            type: "Post",
            url: "AddOrUpdate",
            async: false,
            data: JSON.stringify(params),
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (id > 0) {
                    $("#MIId").val(0);
                    alert("更新成功");
                }
                else {
                    alert("新增成功" + data.data.Id);
                }
            }
        });
    });
    $("#Search").click(function () {
        SearchMaterialList(1);
    });
    SearchMaterialList(1);
}).apply(this, [jQuery]);

function SearchMaterialList(pageIndex) {
    var search = {
        CustomerPart: $("#CustomerPart").val() || "",
        ProductName: $("#ProductName").val() || "",
        WorkOrder: $("#WorkOrde").val() || "",
        CurrentPage: pageIndex
    }
    $.ajax({
        type: "Post",
        url: "Search",
        async: false,
        data: JSON.stringify(search),
        contentType: "application/json;charset=utf-8",
        dataType: "html",
        success: function (data) {
            $("#searchResult").html(data);
            resizeFrameAsyn();
        }
    });
}

function EditMaterial(Id) {
    $.ajax({
        type: "GET",
        url: "GetOneMaterial",
        data: { materialId: Id },
        dataType: "json",
        success: function (data) {
            $("#tabEditMaterial").trigger('click');
            //todo 填充内容
            $("#MIId").val(data.Id);
            $("#MICustomerPart").val(data.MICustomerPart);
            $("#MIProductName").val(data.MIProductName);
            $("#MICustomer").val(data.MICustomer);
            $("#MIPicture").val(data.MIPicture);
            $("#MIWorkOrder").val(data.MIWorkOrder);
            $("#MIMaterial").val(data.MIMaterial);
            $("#MIMaterialText").val(data.MIMaterialText);
            $("#MITool").val(data.MITool);
            $("#MITotalQty").val(data.MITotalQty);
            resizeFrameAsyn();
        }
    });
}