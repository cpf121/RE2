//闭包（避免全局污染）
//加上分号（防止压缩出错）
;
(function (window, jQuery, undefined) {
    var config = {};
    var userId = "";

    var events = {
        CheckData: function () {
            if ($.trim($("#surname").val()) == '') {
                $("#surname").css('border-color', 'red');
                return false;
            }
            else {
                $('#surname').css('border-color', '#e3e6f3');
            }
            if ($.trim($("#givenname").val()) = '') {
                $("#givenname").css('border-color', 'red');
                return false;
            }
            else {
                $('#givenname').css('border-color', '#e3e6f3');
            }
            if ($.trim($("#jobNumber").val()) = '') {
                $("#jobNumber").css('border-color', 'red');
                return false;
            }
            else {
                $('#jobNumber').css('border-color', '#e3e6f3');
            }
            if ($.trim($("#jobNumber").val()) = '') {
                $("#jobNumber").css('border-color', 'red');
                return false;
            }
            else {
                $('#jobNumber').css('border-color', '#e3e6f3');
            }
            if ($.trim($("#phoneNum").val()) == '') {
                $("#phoneNum").css('border-color', 'red');
                return false;
            }
            else {
                $('#phoneNum').css('border-color', '#e3e6f3');
            }
            if ($.trim($("#title").val()) == '') {
                $("#title").css('border-color', 'red');
                return false;
            }
            else {
                $('#title').css('border-color', '#e3e6f3');
            }
            return true;
        },
        Save: function () {
            var flag = events.CheckData();
            if (!flag) {
                return;
            }
            var params = {
                UserId: userId,
                BUSurname: $.trim($("#surname").val()),
                BUGivenname: $.trim($("#givenname").val()),
                BUJobNumber: $.trim($("#jobNumber").val()),
                BUSex: $("input[name='sex-radios']:checked").val(),
                BUAvatars: $.trim($("#avatars").val()),
                BUPhoneNum: $.trim($("#phoneNum").val()),
                BUEmail: $.trim($("#email").val()),
                DepartId: $("#depart option:selected").val(),
                BUTitle: $.trim($("#title").val()),
                BUIsValid: $("input[name='valid-radios']:checked").val(),
            },
            var url = basePath + culture + '/Home/UsrSave';
            $(".modal-confirm").attr("disabled", "disabled");
            $.post(url, params, function (data) {
                if (data.IsSuccess) {
                    if (data.Code != null) {
                        //window.top.    TODO:关闭页面并且刷新页面
                    } else {
                        //TODO:编辑成功
                    }
                } else {
                    //TODO:操作失败
                }
            });
        },
        InitUser: function () {
            $.ajax({
                type: 'post',
                url: basePath + culture + '/Home/InitAdUser',
                dataType: "json",
                data: {
                    userId: userId
                },
                success: function (data) {
                    $("#surname").val(data.BUSurname);
                    $("#givenname").val(data.BUGivenname);
                    $("#jobNumber").val(data.BUJobNumber);
                    $("input[value='" + data.BUSex + "']").prop('checked', true);
                    $("input[value='" + data.BUIsValid + "']").prop('checked', true);
                    $("#avatars").val(data.BUAvatars);
                    $("#phoneNum").val(data.BUPhoneNum);
                    $("#email").val(data.BUEmail);
                    $("#depart option[value='" + data.DepartId + "']").prop('selected', true);
                    $("#title").val(data.BUTitle);
                }
            });
        },
        Cancel: function () {
            //TODO
        },
    };

    var page = {
        Init: function (fig) {
            config = fig;
            userId = fig.userId;
            page.InitEvents();
        },
        InitEvents: function () {
            if (userId) //编辑
            {
                events.InitUser();
            } else {
                //新增
            }

            $(".modal-confirm").on('click', events.Save);
            $(".modal-dismiss").on('click', events.Cancel)
        }
    }

    window.PageInit = page.Init;
})(window, jQuery)