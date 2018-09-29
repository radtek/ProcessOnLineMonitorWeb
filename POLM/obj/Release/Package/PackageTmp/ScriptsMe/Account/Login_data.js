///<reference path="~/ExtJS65/ext-all.js" />


Ext.onReady(function () {
    
    DestroyAll();
    CreateGUI_LoginData();
});

function DestroyAll()
{
    try {
        if (Ext.getCmp("txtPW")) {
            Ext.getCmp("txtPW").destroy();
        }

        if (Ext.getCmp("cmbUer")) {
            Ext.getCmp("cmbUer").destroy();
        }

        if (Ext.getCmp("btn_login")) {
            Ext.getCmp("btn_login").destroy();
        }

        if (Ext.getCmp("btn_logout")) {
            Ext.getCmp("btn_logout").destroy();
        }

        if (Ext.getCmp("btn_addUser")) {
            Ext.getCmp("btn_addUser").destroy();
        }

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function CreateGUI_LoginData() {
    try {
        //debugger;
        Ext.QuickTips.init();

        //#region 创建密码框
        var txtPW = new Ext.form.TextField({
            id: 'txtPW', fieldLabel: "输入密码:", labelStyle: 'width:80px', anchor: '100%',
            emptyText: "password", value: "", width: 225, inputType: 'password',
            renderTo: "divAct_PW", enableKeyEvents: true,
            listeners: {
                keyup: function (obj, e) {
                    //debugger;
                    if (e.getKey() == 13) {   ////e.getKey()是获取按键的号码，13代表是回车键  
                        InputKeyin(obj, e);
                    }
                }
            }
        });

        Ext.QuickTips.register({ target: 'txtPW', title: 'User', text: '输入用户的密码', });
        //#endregion

        //#region 创建用户名
        var jsonUser = JSON.parse($("#loginUserData").val());

        var cmbUer = new Ext.form.ComboBox({
            id: "cmbUer", fieldLabel: "选择用户:", labelStyle: 'width:80px', width: 225,
            renderTo: "divAcnt_User", store: jsonUser,
            listeners: {
                afterRender: function (combo) {
                    //debugger
                    var firstValue = jsonUser[0];
                    combo.setValue(firstValue);//同时下拉框会将与name为firstValue值对应的 text显示
                },
                select: function (combo, records, eOpts) {
                    //debugger;
                    txtPW.setValue("");
                    //console.log(records[0].get('name'));
                    //console.log(records[0].get('abbr'));
                }
            },
        });

        Ext.QuickTips.register({ target: 'cmbUer', title: 'User', text: '在清单中选择用户名', });
        //#endregion

        //#region  login button / logout button  / Maintenance
        var btn_login = Ext.create('Ext.button.Button',
        {
            id: 'btn_login', text: "<b>login登录</b>", scale: "medium", iconCls: "iconLogout24", width: 100,renderTo: "divAct_login",
            handler: function () {
                ////////////////////login action
                loginWithUserPw()
            }
        });

        Ext.QuickTips.register({ target: 'btn_login', title: 'login', text: '点击登录,验证密码是否正确', });

        ///////////////////////////////////////////////////////////////// for logout
        var btn_logout = Ext.create('Ext.button.Button',
        {
            id: 'btn_logout', text: "<b>logout注销</b>", scale: "medium", iconCls: "iconLogin24", width: 100, renderTo: "divAct_logout",
            handler: function () {
                ////////////////////logout action
                logoutWithUser();
            }
        });

        Ext.QuickTips.register({ target: 'btn_logout', title: 'login', text: '点击注销当前用户', });

        //////////////////////////////////////////////////////////////////////////////////////////////////////for add user

        var btn_addUser = Ext.create('Ext.button.Button',
        {
            id: 'btn_addUser', text: "<b>维护用户</b>", scale: "medium", iconCls: "iconUser16", width: 100, renderTo: "divAct_addUser",
            hidden: true, handler: function () {
                ////////////////////logout action
                AddNewUser();
            }
        });

        Ext.QuickTips.register({ target: 'btn_addUser', title: 'user', text: '点击添加新用户', });

        //#endregion

    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }

}

//#region for login
function loginWithUserPw() {
    try {
        //debugger;
        var user = Ext.getCmp("cmbUer").getValue();
        var pw = Ext.getCmp("txtPW").getValue();
        if(pw == "")
        {
            Ext.Msg.show({ title: 'PW', msg: "请输入用户名(" + user + ")的密码", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return;
        }
        var vProject = Ext.getCmp("cmbProject").getValue();

        var MoBag = {
            UserPw: user + "|" + pw,
            Project: vProject,
        };
        var url_Query = $("#Url_UserPwConfirm").val();  //AccountMe/UserPwConfirm
        $.ajax({
            url: url_Query, type: 'GET', data: MoBag, dataType: "html",
            beforeSend: function () {
                //异步请求时spinner出现
                $("#divMySpin").text("");
                var target = $("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (data) {
                spinner.spin();  //关闭spinner

                LoginSuccess(data, user);
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


function LoginSuccess(data, user) {
    try {
        debugger;
        var result = JSON.parse(data);
        $("#divAct_Result").text(result);

        var userEN = "";
        if( result.indexOf("|")> 0)
        {
            userEN = result.split('|')[1];
        }

        if (result.indexOf("Success") == 0) {
            Ext.getCmp("txtPW").setValue("");

            var vProject = Ext.getCmp("cmbProject").getValue();
            //set status
            Ext.getCmp('my-status').setStatus({ text: "Success to login" });
            Ext.getCmp('status_User').setText(user + "(" + userEN + ")");
            Ext.getCmp('status_Project').setText("project:(" + vProject + ")");


            glbLogin_setCookie("username", user, 0);
            glbLogin_setCookie("userEN", userEN, 0);
            glbLogin_setCookie("project", vProject, 0);
            //显示增加用户
            if (user == "admin") {
                Ext.getCmp('btn_addUser').show();
            }
            else {
                Ext.getCmp('btn_addUser').hide();
                $("#divUserOperation").empty();
            }
               
        }
        else
        {
            glbLogin_setCookie("username", "", 0);
            glbLogin_setCookie("userEN", "", 0);

            //隐藏增加用户
            Ext.getCmp('btn_addUser').hide();
            $("#divUserOperation").empty();
        }
    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion

//#region for logout
function logoutWithUser()
{
    try{
        glbLogin_setCookie("username", "", 0);
        glbLogin_setCookie("userEN", "", 0);
        //set status
        Ext.getCmp('my-status').setStatus({ text: "Success to logout" });
        Ext.getCmp('status_User').setText("");

        //隐藏增加用户
        Ext.getCmp('btn_addUser').hide();
        $("#divUserOperation").empty();
    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion

//#region CTN扫入，处理
function InputKeyin(obj, e)
{
    try {
        //debugger;
        loginWithUserPw();
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion

//#region for add new user
function AddNewUser()
{
    var vProject = Ext.getCmp("cmbProject").getValue();

    var MoBag = {
        //UserPw: user + "|" + pw,
        Project: vProject,
    };
    var url_Query = $("#Url_UserOperation").val();  //Account/UserOperation
    $.ajax({
        url: url_Query, type: 'GET', data: MoBag, dataType: "html",
        beforeSend: function () {
            //异步请求时spinner出现
            $("#divMySpin").text("");
            var target = $("#divMySpin").get(0);
            spinner.spin(target);
        },
        success: function (data) {
            spinner.spin();  //关闭spinner

            $("#divUserOperation").empty().append(data);
        },
        complete: function (data) {
            spinner.spin();  //关闭spinner
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            Ext.Msg.show({ title: 'Error', msg: errorThrown, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
        }
    });

}

//#endregion