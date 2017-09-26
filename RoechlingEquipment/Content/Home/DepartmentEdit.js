//闭包（避免全局污染）
//加上分号（防止压缩出错）
;
(function (window, jQuery, undefined) {
    var config = {};

    //事件类
    var events = {
        CheckData: function () {
            if ($.trim($('#departmentName').val()) == '') {
                $('#departmentName').css('border-color', 'red');
                return false;
            }
            else {
                $('#txtdeptName').css('border-color', '#e3e6f3');
            }
            return true;
        },
        save: function () {
            var flag = events.CheckData();
            if (!flag) {
                return;
            }
            var parms = {
                DepartName: $("#departmentName").val(),
                ParentId: $("#ParentId").val(),
                IsValid: $("input[name='isvalid-radios']:checked").val(),
                DepartDesc: $("#DepartDesc").val(),
                DepartId: $("#Id").val()
            }
            var url = basePath + culture + '/Home/DepartmentSave';
            $(".modal-confirm").attr("disabled", "disabled");
            $.post(url, parms, function (data) {
                if (data.IsSuccess) {
                    if (data.Code != null) {
                        window.top.AddNewNode(data.Code, parms.ParentId, parms.DepartName);
                    } else
                    {
                        window.top.EditSuccess();
                        window.parent.location.reload();
                    }
                } else {
                    window.top.AddNewNodeFalse(data.Message);
                }
            });
        },
        cancel: function () {
            window.top.ClosePopWin();
        }
    };

    //初始化类
    var page = {
        //初始化
        Init: function (fig) {
            config = fig;
            page.InitEvents();
        },
        //初始化事件
        InitEvents: function () {
            $(".modal-confirm").on('click', events.save);
            $(".modal-dismiss").on('click', events.cancel)
        },
    };

    window.PageInit = page.Init;
})(window, jQuery)