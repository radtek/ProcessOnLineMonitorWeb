///<reference path="~/ExtJS4/ext/ext-all.js" />


Ext.onReady(function () {
    DestroyAll_user();

    CreateGUI_UserOp();

    GetUserFromServer();
});

function DestroyAll_user() {
    try {
        if (Ext.getCmp("userGrid")) {
            Ext.getCmp("userGrid").destroy();
        }


    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}



function CreateGUI_UserOp()
{
    try {
        debugger;
        Ext.QuickTips.init();

        //#region 建立Grid user maintenace
        //#region columns
        function renderUpdate(value, cellmeta, record, rowIndex, columnIndex, store) {
            if (record.dirty == true) {
                return "<input type='button' value='UpdateThis' style='font-size:9px;color:red'" + ">";
            }
        }

        function renderEN(value, cellmeta, record, rowIndex, columnIndex, store) {
            if(value == "28066666" || value == "28088888"){
                record.setDisabled(true);
            }
        }

        var columns = [
            { xtype: 'rownumberer', header: "#", width: 40, align: 'left' },
            { header: "<div style='font-size: 12px;'>Project</div>", dataIndex: "PROJECT", width: '10%', tdCls: 'x-change-cell', menuDisabled: true },
            {
                header: "<div style='font-size: 12px;'>EN</div>", dataIndex: "EN", width: '10%', tdCls: 'x-change-cellHand', menuDisabled: true,
                editor: {
                    xtype: "numberfield",
                },
                //renderer: renderEN,
            },
            {
                header: "<div style='font-size: 12px;'>Name</div>", dataIndex: "NAME", width: '20%', tdCls: 'x-change-cellHand', menuDisabled: true, editor: {},
            },
            {
                header: "<div style='font-size: 12px;'>Password</div>", dataIndex: "PW", width: '25%', tdCls: 'x-change-cellHand',
                menuDisabled: true, editor: { inputType: 'password' },
                renderer: function (value, metaData, record, rowIdx, colIdx, store) {
                    //debugger;
                    var t = new String("*").repeat(value.length);
                    return t;
                },
            },
            //{ header: "<div style='font-size: 11px;'>id</div>", dataIndex: "id", width: '5%', menuDisabled: true },
            {
                header: "Update", width: '8%',  renderer: renderUpdate,
            },
        ];
        //#endregion
        var store = new Ext.data.ArrayStore();

        var grid = Ext.create("Ext.grid.Panel", {
            id: "userGrid", title: "ODC User Maintenance", width: '100%', columnLines: true, tore: store, height:200,//autoHeight: true,
            renderTo: "divgridUserMnt", columns: columns,
            selType: 'cellmodel',
            plugins: [
                { ptype: "cellediting", }
            ],
            tbar: [
               { text: "AddUser", xtype: "button", iconCls: "iconUser16", handler: onToolBtnClick }, "->",
               { text: "Done", xtype: "button", iconCls: "iconOK16", handler: onToolBtnClick },
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
                                    Ext.Msg.show({title: 'Confirm', msg: "Do you want to delete this user ?", buttons: Ext.Msg.YESNO, icon: Ext.MessageBox.QUESTION,
                                        fn: function (btnId) {
                                            if (btnId == "yes") {
                                                onMenuItemClick(grid, record, item, index, e, "delete");
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
                    if (colIdx == 5) {
                        if (record.dirty) {
                            GridUpdateBtnClick(record, rowIdx);
                        }
                    }
                },
            },

        })
        //#endregion

        //#region 处理特殊情况  Fires before cell editing is triggered. Return false from event handler to stop the editing.
        grid.on('beforeedit', function (editor, e, eOpts) {
            //debugger;
            var EN = e.record.get("EN");
            if(EN == "28066666" || EN == "28088888"){
                return false;//this will disable the cell editing
            }    
        });
        //#endregion

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
    ]
});
//#endregion

function onToolBtnClick(item) {
    //debugger;
    try {
        var sText = item.text;
        var sID = item.id;
        if (sText == "AddUser") {
            var newRecord = new UserRecord({
                //"id": "",
                "PROJECT": Ext.getCmp("cmbProject").getValue(),
                "EN":"280",
                "NAME": "",
                "PW": "",
            });
            var grid = Ext.getCmp("userGrid");
            grid.store.add(newRecord);

            Ext.getCmp('userGrid').getView().focusRow(newRecord);
        }
        else if (sText == "Done")  //////////////////////////////////////维护完成
        {
            //window.location.reload();
            GetLoginData();
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#region Get user from server and bind
function GetUserFromServer() {
    try
    {
        debugger;
        var vProject = Ext.getCmp("cmbProject").getValue();

        var MoBag = {
            //UserPw: user + "|" + pw,
            Project: vProject,
        };
        var url_Query = $("#Url_UserData").val();  //Account/UserOperation_Data
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

                BindGridData(data);
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
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function BindGridData(data)
{
    try {
        debugger;
        var viewBag2 = Ext.decode(data);
        var viewBag = JSON.parse(data);
        var Result = viewBag.Message;
        if(Result.indexOf("Success") == 0)
        {
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

//#region GridUpdateBtnClick
function GridUpdateBtnClick(record, rowIdx) {
    debugger;
    try {
        //#region check if meet regquire
        if (Additional_CheckInput(record) == false){
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

function GridUpdate_Result(data)
{
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
            else
            {
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
function onMenuItemClick(grid, record, item, rowIdx, e, action) {
    try {
        debugger;
        var userName = record.get("Name");
        var EN = record.get("EN").trim();
        if (userName == "admin" || userName == "user" || EN == "28066666" || EN == "28088888")
        {
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


