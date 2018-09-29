///<reference path="~/ExtJS4/ext/ext-all.js" />

Ext.onReady(function () {
    CreateGUI();
});


function CreateGUI() {
    try {
        //debugger;
        Ext.QuickTips.init();

        var projectList = $("#loginProject").val()

        var jsonProject = JSON.parse(projectList);

        var cmbProject = new Ext.form.ComboBox({
            id: "cmbProject", fieldLabel: "选择项目名字:", width: 200,
            renderTo: "divAcnt_Project", store: jsonProject,
            listeners: {
                afterRender: function (combo) {
                    //debugger
                    var firstValue = jsonProject[0];
                    combo.setValue(firstValue);//同时下拉框会将与name为firstValue值对应的 text显示
                },
                select: function (combo, records, eOpts) {
                    GetLoginData();
                    //console.log(records[0].get('name'));
                    //console.log(records[0].get('abbr'));
                }
            },
        });

        Ext.QuickTips.register({ target: 'cmbProject', title: 'Project', text: '选择项目名字', });
    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}


//#region GetLoginData
function GetLoginData() {
    //debugger;
    try {

        var Project = Ext.getCmp("cmbProject").getValue();
        if (Project == "")
        {
            Ext.Msg.show({ title: 'Warnning', msg: "没有获取到项目名字，请选择项目名字.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return;
        }

        var Para = "Para=" + Project;
        var url_loginData = $("#Url_loginData").val();  //Account/loginData

        $.ajax({
            url: url_loginData, type: 'GET', data: Para, dataType: "html",
            beforeSend: function () {
                //异步请求时spinner出现
                $("#divMySpin").text("");
                var target = $("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (data) {
                spinner.spin();  //关闭spinner
                $('#divAcnt_login').empty().append(data);
            },
            complete: function (data) {
                spinner.spin();  //关闭spinner
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                Ext.Msg.show({ title: 'Error', msg: errorThrown, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            }
        });
    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion
