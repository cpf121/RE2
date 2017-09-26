//闭包（避免全局污染）
//加上分号（防止压缩出错）
;
(function (window, jQuery, undefined) {
    var config = {};
    var basepath = '';
    var culture = 'en-US';
    //自定义封装方法
    var custMethods = {
        //查询
        SearchResult: function (index) {

        }
    };

    //事件类
    var events = {
        submitEvent: function () {
            var url = $(".login").attr("action");
            var ajaxObj = $.ajax({
                url: url,
                type: "POST",
                data: { account: $("#username").val(), password: $("#password").val(), DBType: $("#DBType option:selected").val() },
                dataType: "json",
                timeout: 20000,
                async: true,
                success: function (data) {
                    if (!data.Success) {
                        $("#loginHelp").html(data.Message);
                    }
                    else {
                        var language = $("#selectLaguage").children('option:selected').val();
                        window.location.href = basepath + "/" + language + "/Home/Index";
                    }
                }
            });
        },

    };

    //初始化类
    var page = {
        //初始化
        Init: function (fig) {
            config = fig;
            basepath = config.basepath;
            culture = config.culture;
            page.InitEvents();
            page.InitControl();
        },
        //初始化事件
        InitEvents: function () {
            //   $("#btnSearch").click(events.SearchEvent);
            $("#selectLaguage").on("change", function () {
                var language = $(this).children('option:selected').val();
                window.location.href = basepath + "/" + language + "/login.html";
            });

            $(".login :submit").on("click", function (event) {
                event.preventDefault();
                if ($("#username").val().length == 0) {
                    $("#usernameHelp").html("The account is required");
                    return;
                }
                if ($("#password").val().length == 0) {
                    $("#passwordHelp").html("The password is required");
                    return;
                }
                events.submitEvent();
            });

            $("#username").blur(function () {
                if ($("#username").val().length == 0) {
                    $("#usernameHelp").html("The account is required");
                }
            });

            $("#password").blur(function () {
                if ($("#password").val().length == 0) {
                    $("#passwordHelp").html("The password is required");
                }
            });
        },
        InitControl: function () {
            $("#selectLaguage").find("option[value=" + culture + "]").prop("selected", true);
        }
    };

    //全局变量，请勿重新定义（多个JS文件需要区分）
    window.PageInit = page.Init;

})(window, jQuery)