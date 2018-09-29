///<reference path="~/Scripts/ExtJS/ext-all.js" />

var Global_Tracking_Rep = "";

//#region 1. Get chart data and get last record

function PolmRep_ReviewParas()
{
    try {
        var Cell = Ext.getCmp("Cmbo_Cell_Rep").getRawValue();
        var selectDateFrom = Ext.util.Format.date(Ext.getCmp('Date_start').getValue(), 'Y-m-d');

        Ext.getCmp('status_Sub').setText("");

        var viewBag = {
            Model: "801_Data",
            Title: "Polm get chart data and last record",
            User: glbLogin_getCookie("userEN"),
            SignalR_ID: Global_SignalR_ID,
            Para1: Cell,
            Para2: selectDateFrom,
        };

        var Url_Client = jQuery("#Url_PolmRep_Operation").val();

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

                Global_Tracking_Rep = viewBag.List_Track;

                Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
                if (viewBag.Message.indexOf("Success") == 0) {
                    ProcessDataFromServer_ViewData(viewBag);
                }
                else
                {
                    Ext.getCmp("grid_RepData").store.removeAll();

                    Ext.getCmp("ChartRepWave").store.removeAll();//loadData([], false); //this is ok

                    //Ext.getCmp("grid_RepData").store.loadData([], false);
                    //Ext.getCmp("grid_RepData").store.loadData([], false); //this is ok
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

Ext.define('model_PolmRep', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Line', type: 'string' },
        { name: 'Machine', type: 'string' },
        { name: 'Project', type: 'string' },
        { name: 'Program', type: 'string' },
        { name: 'Parameter', type: 'string' },
        { name: 'Val_Cen', type: 'string' },
        { name: 'Val_Min', type: 'string' },
        { name: 'Val_Max', type: 'string' },
        { name: 'Val_Act', type: 'string' },
        { name: 'Time', type: 'string' },
        { name: 'Comment', type: 'string' },
        { name: 'Status', type: 'string' },
        { name: 'TEMPDAYS', type: 'string' },
    ]
});

Ext.define('model_PolmChart', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Time', type: 'string' },
        { name: 'Line1', type: 'int' },
        { name: 'Line2', type: 'int' },
        { name: 'Line3', type: 'int' },
        { name: 'Remark1', type: 'string' },
        { name: 'Remark2', type: 'string' },
        { name: 'Remark3', type: 'string' },
    ]
});

function ProcessDataFromServer_ViewData(viewBag)
{
    try {

        if (viewBag.jsonOut != null && viewBag.jsonOut != "")
        {

            var PolmRep = Ext.decode(viewBag.jsonOut);

            var store = new Ext.data.Store({
                model: "model_PolmRep",
                id: "Gridstore",
                data: PolmRep,
                autoLoad: true
            });
            Ext.getCmp("grid_RepData").bindStore(store);
        }

        if (viewBag.ParaRet1 != null)
        {
            var chartOption = Ext.decode(viewBag.ParaRet1);

            //#region reconfig chart
            Ext.getCmp("txtField_Name_L1").setValue(chartOption.Line1_Name);
            Ext.getCmp("txtField_Name_L2").setValue(chartOption.Line2_Name);
            Ext.getCmp("txtField_Name_L3").setValue(chartOption.Line3_Name);
            Ext.getCmp("txtField_Name_L4").setValue(chartOption.Line4_Name);


            var chartAll = Ext.getCmp("ChartRepWave");
            var chartAxisLabel = chartAll.getAxis("axisLabel");

            chartAxisLabel.setMinimum(chartOption.nMin);
            chartAxisLabel.setMaximum(chartOption.nMax);
            chartAxisLabel.setMajorTickSteps(chartOption.nInterval);

            var chartSrs = chartAll.getSeries();
            chartSrs[1].setHidden(false);
            chartSrs[2].setHidden(false);
            chartSrs[3].setHidden(false);
            if (chartOption.LineQty == 1)
            {
                chartSrs[1].setHidden(true);
                chartSrs[2].setHidden(true);
                chartSrs[3].setHidden(true);
            }
            else if (chartOption.LineQty == 2)
            {
                chartSrs[2].setHidden(true);
                chartSrs[3].setHidden(true);
            }
            else if (chartOption.LineQty == 3)
            {
                chartSrs[3].setHidden(true);
            }
            //#endregion

        }

        if (viewBag.jsonData != null)
        {

            Ext.getCmp("ChartRepWave").store.removeAll();

            var PolmChart = Ext.decode(viewBag.jsonData);
            var storeChart = new Ext.data.Store({
                model: "model_PolmChart",
                id: "Gridstore",
                data: PolmChart,
                autoLoad: true
            });
            Ext.getCmp("ChartRepWave").bindStore(storeChart);
        }


    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion


//#region 2. A. Get specified record B. doubl click item - marker   
function MarkerClick_GetDetailedData(markerItem)
{
    try {
        //#region Check if running
        var bRun_OnOff = Ext.getCmp("chkWaveRun").getValue();
        if (bRun_OnOff) {
            Ext.Msg.show({
                title: 'Confirm', msg: "Need close Running, Do you want to close?", buttons: Ext.Msg.YESNO, icon: Ext.MessageBox.QUESTION,
                fn: function (btnId) {
                    if (btnId == "yes") {
                        Ext.getCmp("chkWaveRun").setValue(false);
                    }
                    else
                        return;
                }
            });
            return;
        }
        //#endregion

        var lineField = markerItem.field;
        var line = "";
        if (lineField == "Line1") {
            line = Ext.getCmp("txtField_Name_L1").getValue();
        }
        else if (lineField == "Line2")
        {
            line = Ext.getCmp("txtField_Name_L2").getValue();
        }
        else if (lineField == "Line3")
        {
            line = Ext.getCmp("txtField_Name_L3").getValue();
        }
        else if (lineField == "Line4")
        {
            line = Ext.getCmp("txtField_Name_L4").getValue();
        }

        var record = markerItem.record;
        var cell = Ext.getCmp("Cmbo_Cell_Rep").getRawValue(); //.getValue();
        var index = markerItem.index;
        var selectDateFrom = Ext.util.Format.date(Ext.getCmp('Date_start').getValue(), 'Y-m-d');

        var viewBag = {
            Model: "803_item",
            Title: "Polm get detail hour data",
            User: glbLogin_getCookie("userEN"),
            Para1: cell,
            Para2: line,
            Para3: index,
            Para4: Ext.encode(record.data),
            Para5: selectDateFrom,
        };

        Global_Tracking_Rep = "";

        var Url_Client = jQuery("#Url_PolmRep_Operation").val();

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

                Global_Tracking_Rep = viewBag.List_Track;
                Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
                if (viewBag.Message.indexOf("Success") == 0) {
                    ProcessDataFromServer_RepDetailedData(viewBag);
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

function ProcessDataFromServer_RepDetailedData(viewBag) {
    try {
        var PolmRep = Ext.decode(viewBag.jsonOut);
        //Ext.getCmp("grid_OverView").store.removeAll();

        var store = new Ext.data.Store({
            model: "model_PolmRep",
            id: "Gridstore",
            data: PolmRep,
            autoLoad: true
        });
        Ext.getCmp("grid_RepData").bindStore(store);


    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion


//#region 3. Chart continue run
function Chart_Run_Switch()
{
    try {

        var bRun_OnOff = Ext.getCmp("chkWaveRun").getValue();
        if (bRun_OnOff) {
            Ext.getCmp('my-status').setStatus({ text: "Switch ON for Chart continue get data " });
            Chart_Run_GetData();

            PolmRep_SetCompDisable(true, true, true, false);
            Ext.getCmp("Date_start").setValue(new Date());
        }
        else {
            Ext.getCmp('status_Sub').setText("Int=OFF");
            Ext.getCmp('my-status').setStatus({ text: "Switch OFF for Chart continue get data" });
            PolmRep_SetCompDisable(false, false, false, false);
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function Chart_Run_GetData()
{
    try {
        //var Cell = Ext.getCmp("Cmbo_Cell_Rep").getValue();
        var Cell = Ext.getCmp("Cmbo_Cell_Rep").getRawValue();


        var viewBag = {
            Model: "805_ConRun",  //和801_Data差不多
            Title: "持续更新现在的数据",
            User: glbLogin_getCookie("userEN"),
            Para1: Cell,
        };

        var Url_Client = jQuery("#Url_PolmRep_Operation").val();

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
                    ProcessDataFromServer_ConRun(viewBag);
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

function ProcessDataFromServer_ConRun(viewBag)
{
    try {
        var PolmRep = Ext.decode(viewBag.jsonOut);
        var store = new Ext.data.Store({
            model: "model_PolmRep",
            id: "Gridstore",
            data: PolmRep,
            autoLoad: true
        });
        Ext.getCmp("grid_RepData").bindStore(store);

        if (viewBag.jsonData != null) {
            var PolmChart = Ext.decode(viewBag.jsonData);
            var storeChart = new Ext.data.Store({
                model: "model_PolmChart",
                id: "Gridstore",
                data: PolmChart,
                autoLoad: true
            });
            Ext.getCmp("ChartRepWave").bindStore(storeChart);
        }

        var bRun_OnOff = Ext.getCmp("chkWaveRun").getValue();
        if (bRun_OnOff) {
            var timeout = 2000;
            if (viewBag.ParaRet1 != "") {
                timeout = parseInt(viewBag.ParaRet1) * 1000;

                Ext.getCmp('status_Sub').setText("Int=" + viewBag.ParaRet1);
            }
            var timer = $.timer(timeout, function () {
                timer.stop();

                Chart_Run_GetData();
            });
        }
        else {
            Ext.getCmp('status_Sub').setText("Int=OFF");
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion

//#region additonal function   1. Save as excel 2. Show Tracking , 3. Set Enable, disable

function SaveExcel_Rep_Wave()
{
    try {
        //#region Output SAP update status
        var grid = Ext.getCmp("grid_RepData");
        //var fileName = $("#viewBag_PID").val() + ".xlsx";
        var fileName = "POLM_records.xlsx";

        var fileTitle = "POLM Report";// (" + $("#viewBag_PID").val() + ")";
        grid.saveDocumentAs({
            type: 'xlsx',
            title: fileTitle,
            fileName: fileName
        });
        //#endregion

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}


function PolmRep_ShowTracking()
{
    try {
        if (Global_Tracking_Rep) {
            if (Global_Tracking_Rep.length > 0) {
                var All_Data = "";
                for (var i = 0; i < Global_Tracking_Rep.length; i++) {
                    All_Data = All_Data + Global_Tracking_Rep[i] + "\n\r";
                }
                ShowTracking_Rep(All_Data, "Tracking");
            }
            else {
                Ext.Msg.show({ title: "Tracking", msg: "当前Tracking没有任何消息.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            }
        }
        else {
            Ext.Msg.show({ title: "Tracking", msg: "当前Tracking没有任何消息.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }

}

function ShowTracking_Rep(trackingTxt, title) {

    try {

        var label_Title = Ext.create('Ext.form.Label',
        {
            text: title, width: 100,
        });


        var txtTrack = new Ext.form.TextArea({
            id: 'txtTrack', fieldLabel: "", labelWidth: 0, width: '100%', grow: true, anchor: '100%',
        });

        var Panel_Tracking = Ext.create("Ext.panel.Panel", {
            collapsible: false, title: "",
            //collapseDirection: "right",
            //headerPosition: "left",
            //layout: 'column',
            border: false,
            items: [{
                width: "100%",
                xtype: 'fieldset',
                border: false,
                autoEl: { tag: 'center' },
                items: [label_Title, txtTrack]
            },
            ]
        });


        Ext.define('MyApp.view.MyWindow', {
            id: 'myAppWind',
            extend: 'Ext.window.Window',
            height: 400, //'100%', //View_Height, // 334,
            width: '75%', //View_Width, //540,
            layout: {
                type: 'border',   //不能省，否则有些异常
            },
            border: false,
            style: 'position:fixed; right:0;bottom:0;',
            title: 'Tracking information',
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
                        bodyPadding: 2,
                        region: 'center',
                        width: "100%",
                        height: "100%",
                        scrollable: true,
                        border: true,
                        items: [
                            Panel_Tracking
                        ]
                    }]
                });

                me.callParent(arguments);
            },
        });

        var Global_imgWindow = Ext.create('MyApp.view.MyWindow');
        //imgWindow.y = 130;  //OK=设置此窗口的Y位置
        Global_imgWindow.show();

        Ext.getCmp("txtTrack").setValue(trackingTxt);

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function PolmRep_SetCompDisable(bCell, bBtnReview, bDateSel, bChkRunning)
{
    try {

        Ext.getCmp("Cmbo_Cell_Rep").setDisabled(bCell);
        Ext.getCmp("btn_ReviewParas").setDisabled(bBtnReview);
        Ext.getCmp("Date_start").setDisabled(bDateSel);
        Ext.getCmp("chkWaveRun").setDisabled(bChkRunning);

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion



//#region email owner
function onMenuItemClick_emailInform(grid, record, item, index, e)
{
    try {
        //#region get failed record
        Global_Tracking_Rep = "";
        var store = grid.store;
        var data = [];
        store.each(function (recordEach) {
            var status = recordEach.get("Status");
            if (status.indexOf("fail") >= 0) {
                data.push(recordEach.data);
            }
        });
        //#endregion
        if (data.length > 0)
        {
            var paraData = Ext.encode(data);

            var viewBag = {
                Model: "401_manual",
                User: glbLogin_getCookie("userEN"),
                Para1: paraData,
            };
            //#endregion

            var Url_Client = jQuery("#Url_PolmRep_Operation").val();

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

                    Global_Tracking_Rep = viewBag.List_Track;
                    Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
                    //if (viewBag.Message.indexOf("Success") == 0) {
                    //    ProcessDataFromServer_XXX(viewBag);
                    //}
                },
                error: function (data, status, e)//服务器响应失败处理函数
                {
                    //关闭spinner
                    spinner.spin();
                    alert(e);
                }
            });

        }
        else
        {
            Ext.Msg.show({ title: arguments.callee.name, msg: "没有fail的记录.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
        }

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion









