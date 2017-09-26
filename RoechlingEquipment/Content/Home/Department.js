
//闭包（避免全局污染）
//加上分号（防止压缩出错）
;
(function (window, jQuery, undefined) {
    var config = {};
    var _instance;

    var events = {
        getDepartList: function () {
            var that = this;
            $.ajax({
                url: basePath + culture+ '/Home/DepartmentManagerList',
                type: 'post',
                success: function (data) {
                    $('#firstUL').html(data);
                    $('#btnAdd_0').show();
                    $("#tableWrapper li").not("li:eq(0)").hide();
                }
            });
        },

        //节点点击事件：显示或隐藏下级节点
        InputEvent: function (i) {
            var children = $("li[name='li_" + i + "']");
            var span = $("span[id='span_" + i+"']");
            $("input[type=button]").hide();
            $("a[id^='btnUpdateValid_").hide()
            //先隐藏所有按钮，然后显示该行的按钮
            $("input[id='btnsib_"+ i+"']").show(); //新增平级子模块按钮     
            $("input[id='btnAdd_" + i+"']").show(); //新增下级子模块按钮    
            $("input[id='btn_" + i+"']").show(); //编辑按钮      
            $("a[id='btnUpdateValid_" + i + "']").show(); //无效/有效按钮
            var modal = $("#modal-template").html();
            $(".btn-secondary-setunvalid,.btn-secondary-setvalid").magnificPopup({
                items: {
                    src: modal,
                    type: 'inline'
                }
            });
            if (children.is(":visible")) {
                children.hide('fast');
                span.attr('title', '展开子指标').find(' > i').addClass('icon-plus-sign').removeClass('icon-minus-sign');
            } else {
                children.show('fast');
                span.attr('title', '折叠子指标').find(' > i').addClass('icon-minus-sign').removeClass('icon-plus-sign');
            }
        },
        //部门搜索选中回调
        ChooseParticularNode: function () {
            $(".parent_li").hide();
            var departmentId = $("#DIName").find("input").attr("id") || '';
            var span = $("#span_" + departmentId);
            if (departmentId != "") {
                if (span.text() != "") {
                    $(".selectedSpan").removeClass('selectedSpan');
                    span.addClass("selectedSpan");
                    var li = span.parent();
                    li.show('fast');
                    events.ExpandParent(departmentId);
                } else {
                    new PNotify({
                        title: '',
                        text: 'There is no Department in this organization',
                        type: 'info',
                        addclass: 'ui-pnotify-no-icon',
                        icon: false
                    });
                }
            } else {
                $(".selectedSpan").removeClass('selectedSpan');
            }
        },
        //展开节点
        ExpandParent: function (i) {
            var span = $("span[id='span_" + i + "']");
            var spanParent = span.parent().parent().siblings("span");
            var liParent = spanParent.parent();
            liParent.show('fast');
            spanParent.attr('title', '折叠子指标').find(' > i').addClass('icon-minus-sign').removeClass('icon-plus-sign');

            var index = $(spanParent).attr('id').indexOf('_') + 1;
            var parentid = spanParent.attr('id').substr(index, spanParent.attr('id').length - index);

            if (parentid != 0) {
                events.ExpandParent(parentid);
            }
        },
        //添加节点
        AddNode:function(Code, parentId, DepartName) {
            var rethtml = ` <li class="parent_li" name="li_` + parentId + `">
                                         <span title="展开子部门" id="span_`+ Code + `" data-id="` + Code + `" style="padding:0px 8px;"><font>` + DepartName + `</font></span>
                                            <input type="button" class="btn bk-margin-5 btn-secondary-current  btn-primary" value="新建平级" id="btnsib_`+ Code + `"  style="display:none" />
                                            <input type="button" class="btn bk-margin-5 btn-secondary-next  btn-success" value="新增下级" id="btnAdd_`+ Code + `" data-id="` + parentId + `" style="display:none" />
                                            <input type="button" class="btn bk-margin-5 btn-secondary-edit btn-info" value="编辑"  id="btn_`+ Code + `"  data-id="` + Code + `" style="display:none" />
                                                <a class="btn bk-margin-5 btn-secondary-setunvalid btn-warning"  id="btnUpdateValid_`+ Code + `" style="display:none">停用</a>
                                        <ul style="float: none">
                                       </ul>
                                    </li>`;
            $("span[id='span_" + parentId + "']").nextAll("ul").append(rethtml);
            events.closePopWin();
            new PNotify({
                title: 'Success!',
                text: '添加部门成功',
                type: 'success'
            });
        },
        AddNewNodeFalse: function (data) {
            events.closePopWin();
            new PNotify({
                title: 'False!',
                text: data,
                type: 'error'
            });
        },
        //新建下级部门
        CreateNextDepartment: function () {
            var o = $(this);
            var index = o.attr('id').indexOf('_') + 1;
            var id = o.attr('id').substr(index, o.attr('id').length - index);
            o.magnificPopup({
                items: {
                    src: basePath + culture + '/Home/DepartmentCreate?departmentId=' + id + '&level=0'
                },
                type: 'iframe',
                iframe: {
                    markup: '<div class="mfp-iframe-scaler myiframe"> ' +
                    '<div class="mfp-close"></div>' +
                    '<iframe class="mfp-iframe" frameborder="0" allowfullscreen>            </iframe>' +
                    '</div>'
                },
            });
            _instance = $.magnificPopup.instance;
        },
        //新建平级部门
        CreateOrdinaryDepartment: function () {
            var o = $(this);
            var index = o.attr('id').indexOf('_') + 1;
            var id = o.attr('id').substr(index, o.attr('id').length - index);
            o.magnificPopup({
                items: {
                    src: basePath + culture + '/Home/DepartmentCreate?departmentId=' + id + '&level=1'
                },
                type: 'iframe'
            });
            _instance = $.magnificPopup.instance;
        },
        //编辑部门
        DepartmentEdit: function () {
            var o = $(this);
            var index = o.attr('id').indexOf('_') + 1;
            var id = o.attr('id').substr(index, o.attr('id').length - index);
            o.magnificPopup({
                items: {
                    src: basePath + culture + '/Home/DepartmentEdit?id=' + id 
                },
                type: 'iframe'
            });
            _instance = $.magnificPopup.instance;
        },
        closePopWin: function () {
            events.closeWin(_instance);
        },
        closeWin: function (popWin) {
            popWin.close();
        },
        EditSuccess: function () {
            new PNotify({
                title: 'Success!',
                text: '编辑部门成功',
                type: 'success'
            });
        },
        //启用
        UpdateDepartValid: function () {
            var o = $(this);
            var index = o.attr('id').indexOf('_') + 1;
            var id = o.attr('id').substr(index, o.attr('id').length - index);
            var name = $("span[id='span_" + id + "'] font").html();
            
            $("#modal-conform").on("click", function () {
                var params = {
                    Id: id,
                };
                var url = basePath + culture + '/Home/UpdateDepartmentValid';
                $.post(url, params, function (data) {
                    if (data.IsSuccess == true) {
                        $.magnificPopup.close();
                        new PNotify({
                            title: 'Success!',
                            text: 'Enable Department Success',
                            type: 'success'
                        });
                        $("span[id='span_" + id + "']").parent().remove();
                        var rethtml = ` <li class="parent_li" name="li_` + data.Code + `">
                                         <span title="展开子部门" id="span_`+ params.Id + `" data-id="` + params.Id + `" style="padding:0px 8px;"><font>` + name + `</font></span>
                                            <input type="button" class="btn bk-margin-5 btn-secondary-current  btn-primary" value="新建平级" id="btnsib_`+ params.Id + `"  style="display:none" />
                                            <input type="button" class="btn bk-margin-5 btn-secondary-next  btn-success" value="新增下级" id="btnAdd_`+ params.Id + `" data-id="` + data.Code + `" style="display:none" />
                                            <input type="button" class="btn bk-margin-5 btn-secondary-edit btn-info" value="编辑"  id="btn_`+ params.Id + `"  data-id="` + data.Code + `" style="display:none" />
                                                <a type="button" class="btn bk-margin-5 btn-secondary-setunvalid btn-warning"  id="btnUpdateValid_`+ params.Id + `" style="display:none">停用</a>
                                        <ul style="float: none">
                                       </ul>
                                    </li>`;
                        $("span[id='span_" + data.Code + "']").nextAll("ul").append(rethtml);
                    }
                    else
                    {
                        $.magnificPopup.close();
                        new PNotify({
                            title: 'error!',
                            text: data.Message,
                            type: 'error',
                            addclass:'stack-topleft'
                        });
                    }
                });
            });
        },
        //停用
        UpdateDepartUnValid: function () {
            var o = $(this);
            var index = o.attr('id').indexOf('_') + 1;
            var id = o.attr('id').substr(index, o.attr('id').length - index);
            var name = $("span[id='span_" + id + "'] font").html();
            
            $("#modal-conform").on("click", function () {
                var params = {
                    Id: id,
                };
                var url = basePath + culture + '/Home/UpdateDepartmentValid';
                $.post(url, params, function (data) {
                    if (data.IsSuccess == true) {
                        $.magnificPopup.close();
                        new PNotify({
                            title: 'Success!',
                            text: 'Enable Department Success',
                            type: 'success'
                        });
                        $("span[id='span_" + id + "']").parent().remove();
                        var rethtml = ` <li class="parent_li" name="li_` + $(".inValid:eq(0)").attr('data-id') + `">
                                           <span class="inValid" title="无效部门" id="span_` + id + `" data-id="` + id + `" data-isdmmanager="0"><font>` + name + `</font></span>
                                  <a class="btn btnnew btn-secondary-setvalid btn-primary " id="btnUpdateValid_`+ id + `" style="display:none">启用</a>
                                 <ul style="float: none"></ul>
                                </li>`;
                        $(".inValid:eq(0)").nextAll('ul').append(rethtml);
                    }
                    else {
                        $.magnificPopup.close();
                        new PNotify({
                            title: 'error!',
                            text: data.Message,
                            type: 'error',
                            addclass: 'stack-topleft'
                        });
                    }
                });
            });
        },
    };

    var page = {
        Init: function (fig) {
            config = fig;
            page.InitEvents();
        },

        InitEvents: function () {
            events.getDepartList();

            //根目录单击
            $("#tableWrapper ul:eq(0) li").on('click', 'span', function () {
                var o = $(this);
                var id = o.attr('id');
                var valueId = id.substr(5, id.length - 4);
                events.InputEvent(valueId);
            });

            $("#DIName").select2({
                placeholder: "请输入部门名称",
                allowClear: true,
                templateSelection: function (repo) {
                    events.ChooseParticularNode();
                    return repo.text;
                },
            });
            
            //新增下级部门
            $('#tableWrapper').on('click', '.btn-secondary-next', events.CreateNextDepartment);
            //新增平级部门
            $('#tableWrapper').on('click', '.btn-secondary-current', events.CreateOrdinaryDepartment);
            //编辑部门
            $('#tableWrapper').on('click', '.btn-secondary-edit', events.DepartmentEdit);
            //启用部门
            $('#tableWrapper').on('click', '.btn-secondary-setvalid', events.UpdateDepartValid);
            //停用部门
            $('#tableWrapper').on('click', '.btn-secondary-setunvalid', events.UpdateDepartUnValid);
        },
    }

    window.PageInit = page.Init;
    window.AddNewNode = events.AddNode;
    window.ClosePopWin = events.closePopWin;
    window.AddNewNodeFalse = events.AddNewNodeFalse;
    window.EditSuccess = events.EditSuccess;
})(window, jQuery)