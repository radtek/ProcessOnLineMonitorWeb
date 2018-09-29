///<reference path="~/ExtJS6/ext-all.js" />

var Global_imgWindow;

function LoginShow()
{
    try {
        if (Global_imgWindow)
        {
            Global_imgWindow.destroy();
        }


        var View_Width = 300; // $("#ImgView_Width").val();
        var View_Height = 350; // $("#ImgView_Height").val();

        //#region Panel --- Project
        var jsonProject = new Ext.data.ArrayStore();
        var cmbProject = new Ext.form.ComboBox({
            id: "cmbProject", fieldLabel: "Select Project", labelWidth: 90, width: 220, editable: false,//renderTo: "divAcnt_Project",
            store: jsonProject,
            listeners: {
                afterRender: function (combo) {
                    //debugger
                    //var firstValue = jsonProject[0];
                    //combo.setValue(firstValue);//同时下拉框会将与name为firstValue值对应的 text显示
                },
                select: function (combo, records, eOpts) {
                    GetLoginData_User();
                }
            },
        });


        var Panel_Project = Ext.create("Ext.panel.Panel", {
            collapsible: false, title: "", //flex: 1,
            collapseDirection: "left",
            headerPosition: "left",
            layout: 'column',
            border: false,
            margin: '5 0 0 0',
            height: 22,
            items: [{
                width: "90%",
                xtype: 'fieldset',
                border: false,
                autoEl: { tag: 'left' },
                items: [cmbProject]
            },
            ]
        });
        //#endregion


        //#region Panel --- User
        var jsonUser = new Ext.data.ArrayStore();
        var cmbUser = new Ext.form.ComboBox({
            //id: "cmbUser", fieldLabel: "<img src='../Images/Login/user_key_16px.png' /> User:", labelStyle: 'width:80px', width: 225, store: jsonUser, // iconCls: 'iconUserSelect16',
            id: "cmbUser", fieldLabel: "User:", labelAlign: 'right', labelWidth: 90, width: 220, store: jsonUser, cls: 'iconUserSelect16',
            //renderTo: "divAcnt_User", 
            listeners: {
                afterRender: function (combo) {
                    //debugger
                    //var firstValue = jsonUser[0];
                    //combo.setValue(firstValue);//同时下拉框会将与name为firstValue值对应的 text显示
                },
                select: function (combo, records, eOpts) {
                    //debugger;
                    txtPW.setValue("");
                }
            },
        });

        //#region 创建密码框
        var txtPW = new Ext.form.TextField({
            //id: 'txtPW', fieldLabel: "<img src='../Images/Login/Password_16x.png' /> Password:", labelStyle: 'width:80px', anchor: '100%',
            id: 'txtPW', fieldLabel: "Password:", labelAlign:'right',labelWidth: 90, anchor: '95%', cls: 'iconPW16',
            emptyText: "password", value: "", width: 220, inputType: 'password', enableKeyEvents: true,
            //renderTo: "divAct_PW", 
            listeners: {
                keyup: function (obj, e) {
                    //debugger;
                    if (e.getKey() == 13) {   ////e.getKey()是获取按键的号码，13代表是回车键  
                        loginWithUserPw(); //InputKeyin(obj, e);
                    }
                }
            }
        });
        //#endregion

        //#region  login button / logout button  / Maintenance
        var btn_login = Ext.create('Ext.button.Button',
        {
            id: 'btn_login', text: "login", scale: "medium", iconCls: "iconLogin24", width: 80, margin: '2 5 2 5',//renderTo: "divAct_login",
            handler: function () {
                ////////////////////login action
                loginWithUserPw()
            }
        });

        ///////////////////////////////////////////////////////////////// for logout
        var btn_logout = Ext.create('Ext.button.Button',
        {
            id: 'btn_logout', text: "logout", scale: "medium", iconCls: "iconLogout16", width: 80, margin: '2 5 2 5',
            handler: function () {
                ////////////////////logout action
                logoutWithUser();
            }
        });

        //////////////////////////////////////////////////////////////////////////////////////////////////////for add user

        var btn_addUser = Ext.create('Ext.button.Button',
        {
            id: 'btn_addUser', text: "add", scale: "medium", iconCls: "iconUser16", width: 80, margin: '2 5 2 5', 
            disabled: true, handler: function () {   
                ////////////////////maintenance action
                AddNewUser();
            }
        });

        debugger;

        var labelMessage = new Ext.form.Label({
            id: 'labelMessage', text: "", html:"", width: '100%',
            //border: 5,
            //style: {
            //    borderColor: 'red',
            //    borderStyle: 'solid'
            //}
        });

        //#endregion

        var Panel_User = Ext.create("Ext.panel.Panel", {
            collapsible: false, flex: 2, title: "",
            collapseDirection: "left",
            headerPosition: "left",
            layout: 'vbox',
            margin: '10 0 0 0',
            border: false,
            items: [
            {
                xtype: 'fieldset',
                border: false,
                autoEl: { tag: 'left' },
                items: [cmbUser]
            }, {
                xtype: 'fieldset',
                border: false,
                autoEl: { tag: 'left' },
                items: [txtPW]
            }, {
                xtype: 'fieldset',
                border: false,
                autoEl: { tag: 'left' },
                items: [btn_login, btn_logout, btn_addUser]
            }, {
                xtype: 'fieldset',
                border: false,
                autoEl: { tag: 'left' },
                width: '100%',
                items: [labelMessage]
            }
        ]
        });
        //#endregion

        var panel_space = new Ext.Panel({
            //scroll: "vertical",
            padding: '0 0 0 0',
            margin: '0 0 0 0',
            border: false,
            html: "<hr>",
            height: 30,
        });

        //#region 建立Grid user maintenace
        //#region columns
        function renderUpdate(value, cellmeta, record, rowIndex, columnIndex, store) {
            if (record.dirty == true) {
                return "<input type='button' value='UpdateThis' style='font-size:9px;'" + ">";
            }
        }

        function renderEN(value, cellmeta, record, rowIndex, columnIndex, store) {
            if (value == "28066666" || value == "28088888") {
                record.setDisabled(true);
            }
        }

        var columns = [
            { xtype: 'rownumberer', header: "#", width: 40, align: 'left' },
            { header: "<div style='font-size: 12px;'>PRJ</div>", dataIndex: "PROJECT", width: 40, tdCls: 'x-change-cell', menuDisabled: true },
            {
                header: "<div style='font-size: 12px;'>EN</div>", dataIndex: "EN", width: 70, tdCls: 'x-change-cellHand', menuDisabled: true,
                editor: {
                    xtype: "numberfield",
                },
                //renderer: renderEN,
            },
            {
                header: "<div style='font-size: 12px;'>Name</div>", dataIndex: "NAME", width: 100, tdCls: 'x-change-cellHand', menuDisabled: true, editor: {},
            },

            {
                header: "<div style='font-size: 12px;'>Password</div>", dataIndex: "PW", width: 100, tdCls: 'x-change-cellHand',
                menuDisabled: true, editor: { inputType: 'password' },
                renderer: function (value, metaData, record, rowIdx, colIdx, store) {
                    //debugger;
                    var t = new String("*").repeat(value.length);
                    return t;
                },
            },
            {
                header: "<div style='font-size: 12px;'>Remark</div>", dataIndex: "REMARK", width: 100, tdCls: 'x-change-cellHand', menuDisabled: true, editor: {},
                editor: new Ext.form.field.ComboBox({
                    typeAhead: true,
                    triggerAction: '',
                    store: ['', 'ME', 'MFG', 'IPQA'],
                })

            },
            {
                header: "Update", flex: 1, renderer: renderUpdate,
            },
        ];
        //#endregion
        var store = new Ext.data.ArrayStore();

        var userGrid = Ext.create("Ext.grid.Panel", {
            id: "userGrid", title: "ODC User Maintenance", width: '100%', columnLines: true, tore: store, height: 200,//autoHeight: true,
            columns: columns, //renderTo: "divgridUserMnt",
            selType: 'cellmodel',
            plugins: [
                { ptype: "cellediting", }
            ],
            tbar: [
               { text: "AddUser", xtype: "button", iconCls: "iconUser16", handler: login_onToolBtnClick }, "->",
               { text: "Done", xtype: "button", iconCls: "iconOK16", handler: login_onToolBtnClick },
            ],
            viewConfig: {
                stripeRows: false,//在表格中显示斑马线
                enableTextSelection: true, //可以复制单元格文字
                listeners: {
                }
            },
            listeners: {
                //#region item menu
                itemcontextmenu: function (grid, record, item, index, e) {
                    //debugger;
                    var contextMenu = Ext.create('Ext.menu.Menu', {
                        width: 100,
                        items: [
                            {
                                text: '删除此用户', iconCls: "iconCut16", handler: function () {
                                    Ext.Msg.show({
                                        title: 'Confirm', msg: "Do you want to delete this user ?", buttons: Ext.Msg.YESNO, icon: Ext.MessageBox.QUESTION,
                                        fn: function (btnId) {
                                            if (btnId == "yes") {
                                                onMenuItemClick_CommonLogin(grid, record, item, index, e, "delete");
                                            }
                                        }
                                    });
                                }
                            }
                        ]
                    });
                    e.stopEvent();
                    contextMenu.showAt(e.getXY());
                },
                //#endregion
                cellclick: function (view, cell, colIdx, record, row, rowIdx, e) {
                    //debugger;
                    if (colIdx == 6) {
                        if (record.dirty) {
                            GridUpdateBtnClick(record, rowIdx);
                        }
                    }
                },
            },

        })
        //#endregion

        //#region panel --- confirm
        var panel_space1 = new Ext.Panel({
            //scroll: "vertical",
            padding: '0 0 0 0',
            margin: '0 0 0 0',
            border: false,
            html: "<hr>",
            height: 30,
        });

        var btn_Pass = Ext.create('Ext.button.Button',
        {
            id: 'btn_Pass', text: "<b>OK</b>", scale: "medium", iconCls: "iconOK16", width: 60, height: 30, tooltip: "",
            handler: function () {
                Global_imgWindow.close();

                //////////////////设置聚焦点
                Ext.getCmp("txt_QuerySO").focus(true);
            }
        });

        //new Ext.tip.ToolTip({ target: btn_Pass, trackMouse: true, html: "<div style='font-size: 13px;'>点击设置此外观图片符合要求Pass</div>" });

        var btn_Fail = Ext.create('Ext.button.Button',
        {
            id: 'btn_Fail', text: "Cancel", scale: "medium", width: 60, height: 30, //tooltip: "", iconCls: "iconCancel16",
            handler: function () {
                Global_imgWindow.close();
            }
        });

        var Panel_Confirm = Ext.create("Ext.panel.Panel", {
            collapsible: false, flex: 2, title: "",
            collapseDirection: "right", width: '100%',
            headerPosition: "left",
            layout: 'column',
            border: false,
            items: [{
                width: "50%",
                xtype: 'fieldset',
                border: false,
                autoEl: { tag: 'center' },
                items: [btn_Pass]
            }, {
                width: "50%",
                xtype: 'fieldset',
                border: false,
                autoEl: { tag: 'center' },
                items: [btn_Fail]
            }]
        });

        //#endregion


        Ext.define('MyApp.view.MyWindow', {
            extend: 'Ext.window.Window',
            height: View_Height, // 334,
            width: View_Width, //540,
            layout: {
                type: 'border',   //不能省，否则有些异常
            },
            iconCls: 'iconUserKey16',
            border: false,
            style: 'position:fixed; right:0;bottom:0;',
            title: 'User Login',
            //minimizable: true,
            maximizable: true,
            scrollable: true,  //scroll 必须要和下面的一起使用才有效
            initComponent: function () {
                var me = this;
                Ext.applyIf(me,
                {
                    items: [{
                        id: "myWindowForm",
                        xtype: 'form',
                        bodyPadding: 0,
                        region: 'center',
                        width: "100%",
                        height: "100%",
                        margin: '0 0 0 0',
                        scrollable: true,
                        border: true,
                        //style: {'background-color': 'blue' },
                        //layout: { type: "vbox", align: 'stretch' },  //align: 'center'
                        items: [
                            Panel_Project,
                            panel_space,
                            Panel_User,
                            userGrid,
                            panel_space1,
                            Panel_Confirm,
                            //{
                            //    xtype: 'gridpanel',
                            //    id: 'winGrid',
                            //    forceFit: true,
                            //    store: storeWindow,
                            //    columns: columns_Img,
                            //    border: false,
                            //},
                        ]
                    }]
                });

                me.callParent(arguments);
            }
        });


        Global_imgWindow = Ext.create('MyApp.view.MyWindow');
        //imgWindow.y = 130;  //OK=设置此窗口的Y位置
        Global_imgWindow.show();

        userGrid.hide();

        GetLoginData_Project();
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}


//#region common function
String.prototype.repeat = function (n) {
    var _this = this;
    var result = '';
    for (var i = 0; i < n; i++) {
        result += _this;
    }
    //alert(result);
    return result;
}

Ext.define('UserRecord', {
    extend: 'Ext.data.Model',
    fields: [
        //{ name: 'id', type: 'string' },
        { name: 'PROJECT', type: 'string' },
        { name: 'EN', type: 'string' },
        { name: 'NAME', type: 'string' },
        { name: 'PW', type: 'string' },
        { name: 'REMARK', type: 'string' },
    ]
});
//#endregion

//#region GetLoginData --> Project list
function GetLoginData_Project() {
    //debugger;
    try {
        Ext.getCmp('my-status').setStatus({ text: "Getting project list ..." });

        Ext.getCmp("cmbProject").setDisabled(true);
        Ext.getCmp("cmbUser").setDisabled(true);

        var viewBag = {
            Model: "login_Project",
            PcName: "",
            Para1: "",
        };

        var url_loginData = $("#Url_Common_Login").val();  //AccountMe/loginData

        $.ajax({
            url: url_loginData, type: 'GET', data: viewBag, dataType: "json",
            beforeSend: function () {
                //异步请求时spinner出现
                $("#divMySpin").text("");
                var target = $("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (data) {
                spinner.spin();  //关闭spinner
                
                FillinData_Project(data);
            },
            complete: function (data) {
                spinner.spin();  //关闭spinner
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                Ext.Msg.show({ title: 'Error', msg: errorThrown, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
                Ext.getCmp('my-status').setStatus({ text: errorThrown });

            }
        });
    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
        Ext.getCmp('my-status').setStatus({ text: err.message });
    }
}

function FillinData_Project(viewBag)
{
    try {
        debugger;
        if(viewBag.Message.indexOf("Success") == 0)
        {
            var jsonProject = JSON.parse(viewBag.jsonProject);

            var store = jsonProject;
            var grid = Ext.getCmp("cmbProject");
            grid.bindStore(store);
            Ext.getCmp("cmbProject").setDisabled(false);

        }

        Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion


//#region GetLoginData -- user list
function GetLoginData_User() {
    //debugger;
    try {
        Ext.getCmp("cmbUser").setDisabled(true);

        var Project = Ext.getCmp("cmbProject").getValue();
        if (Project == "") {
            Ext.Msg.show({ title: 'Warnning', msg: "没有获取到项目名字，请选择项目名字.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return;
        }

        Ext.getCmp('my-status').setStatus({ text: "Getting user list for project=" + Project + " ..." });
        Ext.getCmp("cmbUser").setValue("");

        var viewBag = {
            Model: "login_User",
            PcName: "",
            Project: Project,
        };

        var url_loginData = $("#Url_Common_Login").val();  //AccountMe/loginData

        $.ajax({
            url: url_loginData, type: 'GET', data: viewBag, dataType: "json",
            beforeSend: function () {
                //异步请求时spinner出现
                $("#divMySpin").text("");
                var target = $("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (data) {
                spinner.spin();  //关闭spinner
                
                FillinData_UserList(data);
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


function FillinData_UserList(viewBag) {
    try {
        debugger;
        if (viewBag != null) {
            if (viewBag.Message.indexOf("Success") == 0) {
                var jsonUser = Ext.decode(viewBag.jsonUser); // JSON.parse(viewBag.jsonUser);

                var store = jsonUser;
                var grid = Ext.getCmp("cmbUser");
                grid.bindStore(store);
            }

            Ext.getCmp("cmbUser").setDisabled(false);
            Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
        }
        else
        {
            Ext.getCmp('my-status').setStatus({ text: "Fail to get data from server" });
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion


//#region for login
function loginWithUserPw() {
    try {
        debugger;
        var user = Ext.getCmp("cmbUser").getValue();
        var pw = Ext.getCmp("txtPW").getValue();
        if (pw == "") {
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
            url: url_Query, type: 'GET', data: MoBag, dataType: "json",
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
        var result = data;// Ext.decode(data);
        // $("#labelMessage").text(result);
        var msgShow = '<span style="font-size:11px; text-align:left; font-weight:500" >' + result + '</span>'
        Ext.getCmp("labelMessage").setHtml(msgShow);

        var userEN = "";
        var userDepart = "";
        if (result.indexOf("|") > 0) {
            userEN = result.split('|')[1];
            userDepart = result.split('|')[2];
        }

        if (result.indexOf("Success") == 0) {
            Ext.getCmp("txtPW").setValue("");

            var vProject = Ext.getCmp("cmbProject").getValue();
            //set status
            Ext.getCmp('my-status').setStatus({ text: "Success to login" });
            Ext.getCmp('status_User').setText(user + "(" + userEN + ")");
            Ext.getCmp('status_Project').setText("project:(" + vProject + ")");
            Ext.getCmp('status_Depart').setText(userDepart);

            

            glbLogin_setCookie("username", user, 0);
            glbLogin_setCookie("userEN", userEN, 0);
            glbLogin_setCookie("project", vProject, 0);
            glbLogin_setCookie("userdepart", userDepart, 0);

            //显示增加用户
            if (user == "admin") {
                Ext.getCmp('btn_addUser').enable();
            }
            else {
                Ext.getCmp('btn_addUser').disable();
                $("#divUserOperation").empty();
            }


            Ext.getCmp("btn_Pass").focus(true);
            ResetGUIAfterLogout();

        }
        else {
            glbLogin_setCookie("username", "", 0);
            glbLogin_setCookie("userEN", "", 0);
            glbLogin_setCookie("userdepart", "", 0);
            //隐藏增加用户
            Ext.getCmp('btn_addUser').disable(); //hide();
            $("#divUserOperation").empty();
        }
    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function ResetGUIAfterLogout() {
    try {

        var grid_Torque = Ext.getCmp("grid_Torque");
        if (grid_Torque) {
            //grid_Torque.store.clearData();
            //grid_Torque.store.removeAll();

            var store = grid_Torque.getStore();
            if (store) {
                store.clearData();
                store.removeAll();

                store.loadData([], false);
            }
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion

//#region for logout
function logoutWithUser() {
    try {
        debugger;
        glbLogin_setCookie("username", "", 0);
        glbLogin_setCookie("userEN", "", 0);
        glbLogin_setCookie("userdepart", "", 0);
        glbLogin_setCookie("project", "", 0);


        //set status
        Ext.getCmp('my-status').setStatus({ text: "Success to logout" });
        Ext.getCmp('status_User').setText("");
        Ext.getCmp('status_Depart').setText("");

        //隐藏增加用户
        Ext.getCmp('btn_addUser').disable(); //.hide();
        $("#divUserOperation").empty();

        Ext.getCmp("userGrid").hide();

        var msgShow = '<span style="font-size:11px; text-align:left; font-weight:500" >' + 'Success to logout' + '</span>'
        Ext.getCmp("labelMessage").setHtml(msgShow);
    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion

//#region for add new user
function AddNewUser() {
    debugger;
    var vProject = Ext.getCmp("cmbProject").getValue();

    var MoBag = {
        //UserPw: user + "|" + pw,
        Project: vProject,
    };
    var url_Query = $("#Url_UserData").val();  //Account/UserOperation
    $.ajax({
        url: url_Query, type: 'GET', data: MoBag, dataType: "json",
        beforeSend: function () {
            //异步请求时spinner出现
            $("#divMySpin").text("");
            var target = $("#divMySpin").get(0);
            spinner.spin(target);
        },
        success: function (data) {
            spinner.spin();  //关闭spinner

            fillinData_Maintenance(data);
            //$("#divUserOperation").empty().append(data);
        },
        complete: function (data) {
            spinner.spin();  //关闭spinner
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            Ext.Msg.show({ title: 'Error', msg: errorThrown, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
        }
    });

}

function fillinData_Maintenance(viewBag)
{
    try {
        debugger;
        //var viewBag2 = Ext.decode(data);
        //var viewBag = JSON.parse(data);
        var Result = viewBag.Message;
        if (Result.indexOf("Success") == 0) {

            Ext.getCmp("userGrid").show();
            var jsonUsers = Ext.decode(viewBag.jsonUser);  // viewBag.jsonUser;// JSON.parse(data.jsonUser);
            //#region 

            var store = new Ext.data.Store({
                model: "UserRecord",
                id: "Gridstore",
                data: jsonUsers, //Arrydata,
                autoLoad: true
            });
            //#endregion

            var grid = Ext.getCmp("userGrid");
            grid.bindStore(store);
        }

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion

function login_onToolBtnClick(item) {
    //debugger;
    try {
        var sText = item.text;
        var sID = item.id;
        if (sText == "AddUser") {
            var newRecord = new UserRecord({
                //"id": "",
                "PROJECT": Ext.getCmp("cmbProject").getValue(),
                "EN": "280",
                "NAME": "",
                "PW": "",
                "REMARK":"",
            });
            var grid = Ext.getCmp("userGrid");
            grid.store.add(newRecord);

            Ext.getCmp('userGrid').getView().focusRow(newRecord);
        }
        else if (sText == "Done")  //////////////////////////////////////维护完成
        {
            //window.location.reload();
            GetLoginData_User();

            Ext.getCmp('userGrid').hide();
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}


//#region additional function
function Additional_CheckInput(record) {
    try {
        debugger;
        var Name = record.get("NAME").trim();
        if (Name == "") {
            Ext.Msg.show({ title: arguments.callee.name, msg: "用户名不能为空.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return false;
        }
        var EN = record.get("EN").trim();
        if (EN == "") {
            Ext.Msg.show({ title: arguments.callee.name, msg: "工号不能为空.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return false;
        }
        var PW = record.get("PW").trim();
        if (PW == "") {
            Ext.Msg.show({ title: arguments.callee.name, msg: "密码不能为空.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return false;
        }

        if (Name == "admin" || Name == "user" || EN == "28066666" || EN == "28088888") {
            Ext.Msg.show({ title: "update", msg: Name + " 用户信息不能更改.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return false;
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
    return true;
}
//#endregion

//#region menu item click
function onMenuItemClick_CommonLogin(grid, record, item, rowIdx, e, action) {
    try {
        debugger;
        var userName = record.get("Name");
        var EN = record.get("EN").trim();
        if (userName == "admin" || userName == "user" || EN == "28066666" || EN == "28088888") {
            Ext.Msg.show({ title: "delete", msg: userName + " 用户不能删除.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            //record.cancelEdit();
            return;
        }

        Ext.getCmp('my-status').setStatus({ text: "delete this user ..." });
        $('body').addClass('waiting');

        var paraData = Ext.encode(record.data);
        var vProject = Ext.getCmp("cmbProject").getValue();
        var MoBag = {
            Project: vProject,
            jsonDt: paraData,
            MntModel: action,
            Para1: rowIdx,
        };
        var url_Query = $("#Url_UserAction").val();  //Account/UserOperation_Action
        $.ajax({
            url: url_Query, type: 'GET', data: MoBag, dataType: "json",
            beforeSend: function () {
                //异步请求时spinner出现
                $("#divMySpin").text("");
                var target = $("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (data) {
                spinner.spin();  //关闭spinner

                GridUpdate_Result(data);
            },
            complete: function (data) {
                spinner.spin();  //关闭spinner
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                Ext.getCmp('my-status').setStatus({ text: err.message });
                $('body').removeClass('waiting');
                Ext.Msg.show({ title: arguments.callee.name, msg: errorThrown, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            }
        });

    }
    catch (err) {
        Ext.getCmp('my-status').setStatus({ text: err.message });
        $('body').removeClass('waiting');
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }

}
//#endregion


//#region user update into server
function GridUpdateBtnClick(record, rowIdx) {
    debugger;
    try {
        //#region check if meet regquire
        if (Additional_CheckInput(record) == false) {
            return;
        }

        Ext.getCmp('my-status').setStatus({ text: "maintenance user ..." });
        $('body').addClass('waiting');

        //#endregion  link server
        var paraData = Ext.encode(record.data);
        var vProject = Ext.getCmp("cmbProject").getValue();
        var MoBag = {
            Project: vProject,
            jsonDt: paraData,
            MntModel: "update",
            Para1: rowIdx,
            Para2: record.id,
        };

        var url_Query = $("#Url_UserAction").val();  //Account/UserOperation_Action
        $.ajax({
            url: url_Query, type: 'GET', data: MoBag, dataType: "json",
            beforeSend: function () {
                //异步请求时spinner出现
                $("#divMySpin").text("");
                var target = $("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (data) {
                spinner.spin();  //关闭spinner

                GridUpdate_Result(data);
            },
            complete: function (data) {
                spinner.spin();  //关闭spinner
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                Ext.getCmp('my-status').setStatus({ text: err.message });
                $('body').removeClass('waiting');
                Ext.Msg.show({ title: arguments.callee.name, msg: errorThrown, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            }
        });

    }
    catch (err) {
        Ext.getCmp('my-status').setStatus({ text: err.message });
        $('body').removeClass('waiting');
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function GridUpdate_Result(data) {
    try {

        var result = data.Message; // Ext.decode(data.Message);
        //$("#divMntMsg").text(result);
        debugger;
        if (result.indexOf("Success") == 0) {
            if (data.MntModel == "delete") {
                var grid = Ext.getCmp("userGrid");
                var store = grid.getStore();
                var record = store.getAt(data.Para1);
                store.remove(record);

                //grid.store.removeAt(data.Para1);
                ////store.removeAt(data.Para1);
                //grid.store.sync();
                ////Ext.getCmp('userGrid').getView().refresh();
                ////grid.reconfigure(store);
            }
            else {
                var grid = Ext.getCmp("userGrid");
                var record = grid.store.getAt(data.Para1);
                //var record = store.getById(store.findExact('id',data.Para2));  //grid.store.getById(data.Para2); //此方法无效，需进一步验证
                record.set("id", data.Para2);
                record.commit();
            }
        }

        Ext.getCmp('my-status').setStatus({ text: result });
        $('body').removeClass('waiting');
    }
    catch (err) {
        Ext.getCmp('my-status').setStatus({ text: err.message });
        $('body').removeClass('waiting');
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion


