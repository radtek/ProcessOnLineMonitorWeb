///<reference path="~/Scripts/ExtJS/ext-all.js" />


function Review_Wave_GetParameters()
{
    try {
        //#region Init Parmeter
        var Project = Ext.getCmp("Cmbo_Project").getValue();
        var Line = Ext.getCmp("Cmbo_Line").getValue();
        if (Project == null || Project == "") {
            Ext.Msg.show({ title: arguments.callee.name, msg: "请选择项目名", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return;
        }
        if (Line == null || Line == "") {
            Ext.Msg.show({ title: arguments.callee.name, msg: "请选择线别", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return;
        }
        //#endregion

        var Url_Client = $("#Url_UploadWI_Operation").val();

        var viewBag = {
            Model: "703_Paras",
            Title: "Get WorkI parameter for project,line",
            User: glbLogin_getCookie("userEN"),
            Para1: "Wave",
            Project: Project,
            Para2: Line,
        };

        $.ajax({
            url: Url_Client, type: 'post', data: viewBag, dataType: "json",
            beforeSend: function () {
                //异步请求时spinner出现
                $("#divMySpin").text("");
                var target = $("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (data) {
                //关闭spinner
                spinner.spin();

                ProcessDataFromServer_ReviewWI(data);
            },
            error: function (data, status, e)//服务器响应失败处理函数
            {
                //关闭spinner
                spinner.spin();
                alert(e);
            }
        });

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

Ext.define('POLM_WI_OV', {
    extend: 'Ext.data.Model',
    fields: [
        //{ name: 'ID', type: 'string' },
        //{ name: 'MCTYPE', type: 'string' },
        { name: 'OV_ID', type: 'string' },
        { name: 'WORKSHEET', type: 'string' },
        { name: 'DOCNUM', type: 'string' },
        { name: 'DOCREV', type: 'string' },
        { name: 'MODEL', type: 'string' },
        { name: 'PROGRAM', type: 'string' },
        { name: 'PARA_NAME', type: 'string' },
        { name: 'VAL_CEN', type: 'string' },
        { name: 'VAL_MAX', type: 'string' },
        { name: 'VAL_MIN', type: 'string' },
    ]
});


function ProcessDataFromServer_ReviewWI(viewBag)
{
    try {
        Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
        if (viewBag.Message.indexOf("Success") == 0) {
            if (viewBag.jsonOut && viewBag.jsonOut != "")
            {
                var POLM_WI_OV = Ext.decode(viewBag.jsonOut);
                var store = new Ext.data.Store({
                    model: "POLM_WI_OV",
                    id: "POLM_WI_OV",
                    data: POLM_WI_OV,
                    autoLoad: true
                });

                Ext.getCmp("grid_Review_Wave").bindStore(store);
            }
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#region 绑定 line after selecting project
function Project_Select_BindLine(SelProject)
{
    try {
        var viewBag = {
            Model: "701_PojLine",
            User: glbLogin_getCookie("userEN"),
            Para1: "Wave",
            Para2: SelProject,
        };

        var Url_Client = jQuery("#Url_UploadWI_Operation").val();

        jQuery.ajax({
            url: Url_Client, type: 'post', data: viewBag, dataType: "json",
            beforeSend: function () {
                //异步请求时spinner出现
                jQuery("#divMySpin").text("");
                var target = jQuery("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (viewBag) {
                //关闭spinner
                spinner.spin();

                Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
                if (viewBag.Message.indexOf("Success") == 0) {
                    //#region 绑定Line
                    var list_Option2 = JSON.parse(viewBag.ParaRet2);
                    for (var i = 0, c = list_Option2.length; i < c; i++) {
                        list_Option2[i] = [list_Option2[i]];
                    }

                    var storeCmbLine = new Ext.data.ArrayStore({
                        data: list_Option2,
                        fields: ['value']
                    });

                    if (list_Option2.length > 0) {
                        Ext.getCmp("Cmbo_Line").bindStore(storeCmbLine);

                        Ext.getCmp("Cmbo_Line").setValue(list_Option2[0]);
                    }
                    //#endregion
                }
            },
            error: function (data, status, e)//服务器响应失败处理函数
            {
                //关闭spinner
                spinner.spin();
                alert(e);
            }
        });

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion


//#region Temporary WI
function Review_Wave_TemporyWI() {
    try {
        //var columns = Ext.getCmp("grid_Review_Wave").getColumns();
        var column = Ext.getCmp("grid_Review_Wave").columnManager.getHeaderByDataIndex('VAL_Temp');
        var column_Update = Ext.getCmp("grid_Review_Wave").columnManager.getHeaderByDataIndex('Update');
        var column_TEMPDAYS = Ext.getCmp("grid_Review_Wave").columnManager.getHeaderByDataIndex('TEMPDAYS');
        
        if (column.hidden) {
            column.show();
            column_Update.show();
            column_TEMPDAYS.show();
        }
        else {
            column.hide();
            column_Update.hide();
            column_TEMPDAYS.hide();
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion


//#region Review WorkI update 
function ReviewWI_TemporyUpdate_Click(rowIndex)
{
    try
    {
        var record = Ext.getCmp("grid_Review_Wave").store.getAt(rowIndex);
        var paraData = Ext.encode(record.data);

        //#region ReviewWI_Click
        var viewBag = {
            Model: "705_TempWIUpdate",
            Title: "Tempory WorkI for parameter update",
            User: glbLogin_getCookie("userEN"),
            Para1: Ext.getCmp("Cmbo_Project").getValue(),
            Para2: Ext.getCmp("Cmbo_Line").getValue(),
            jsonData: paraData,
        };

        var Url_Client = jQuery("#Url_UploadWI_Operation").val();

        jQuery.ajax({
            url: Url_Client, type: 'post', data: viewBag, dataType: "json",
            beforeSend: function ()
            {
                //异步请求时spinner出现
                jQuery("#divMySpin").text("");
                var target = jQuery("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (viewBag)
            {
                //关闭spinner
                spinner.spin();

                Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
                if (viewBag.Message.indexOf("Success") == 0)
                {
                    //ProcessDataFromServer_XXX(viewBag);
                    var timeout = 1000;
                    var timer = $.timer(timeout, function ()
                    {
                        timer.stop();

                        Review_Wave_GetParameters();
                    });

                    
                }
            },
            error: function (data, status, e)//服务器响应失败处理函数
            {
                //关闭spinner
                spinner.spin();
                alert(e);
            }
        });


        //#endregion

    }
    catch (err)
    {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion


//#region 暂时不用

function Review_Wave_ShowAllParameters() {
    try {
        var grid = Ext.getCmp("grid_Review_Wave");
        var btn = Ext.getCmp("btn_Review_Wave_ShowAll");

        var columns = grid.getColumns();
        //var columns = grid.getColumnManager().getColumns();
        var c_col_BdW_x = Ext.getCmp("col_BdW_x"); // columns["col_BdW_x"];
        var bShow = false;
        if (c_col_BdW_x.isVisible()) {
            //c_col_BdW_x.hide();
            bShow = false;
            btn.setText('Show All');
        }
        else {
            //c_col_BdW_x.show();
            btn.setText('Hide All');
            bShow = true;
        }

        for (var i = 0; i < columns.length; i++) {
            var column = columns[i];
            if (column.dataIndex.indexOf("_M") > 0) {
                if (bShow) {
                    column.show();
                }
                else {
                    column.hide();
                }
            }
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion


