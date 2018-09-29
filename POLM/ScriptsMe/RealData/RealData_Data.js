///<reference path="~/Scripts/ExtJS62/ext-all.js" />



function Home_GetRealTimeData() {
    try {
        //#region real time get data
        var dateSelect =  Ext.util.Format.date(Ext.getCmp('dtPickerFrom').getValue(), 'Y-m-d');//Ext.getCmp("dtPickerFrom").getValue();
        var dateSelect_TimeFrom = Ext.util.Format.date(Ext.getCmp('timeFrom').getValue(), 'H'); //Ext.getCmp("timeFrom").getValue();
        var dateSelect_TimeTo = Ext.util.Format.date(Ext.getCmp('timeTo').getValue(), 'H'); //Ext.getCmp("timeTo").getValue();



        var viewBag = {
            Model: "301_RealTime",
            Para1: dateSelect,
            Para2: dateSelect_TimeFrom,
            Para3: dateSelect_TimeTo,
        };

        var Url_Client = $("#Url_Home_Operation").val();

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
                debugger
                //#region bind category combobox
                var viewBag = data;
                Home_GetRealTimeData_Fill(viewBag);
                //#endregion
            },
            error: function (data, status, e)//服务器响应失败处理函数
            {
                //关闭spinner
                spinner.spin();
                Ext.Msg.show({ title: arguments.callee.name, msg: e, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            }
        });

        //#endregion
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

Ext.define('Wave_RT_Full', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Machine', type: 'string' },
        { name: 'ProgName', type: 'string' },
        { name: 'Flux_BdWid', type: 'string' },
        { name: 'Flux_ConvSpd', type: 'string' },
        { name: 'Flux_NozSpd', type: 'string' },
        { name: 'Flux_Volumn', type: 'string' },
        { name: 'Flux_Power', type: 'string' },
        { name: 'Flux_NozSpray', type: 'string' },
        { name: 'Flux_Pres', type: 'string' },
        { name: 'Flux_BiModel', type: 'string' },
        { name: 'Heat_Low1', type: 'string' },
        { name: 'Heat_Low2', type: 'string' },
        { name: 'Heat_Low3', type: 'string' },
        { name: 'Heat_Upp1', type: 'string' },
        { name: 'Heat_Upp2', type: 'string' },
        { name: 'Heat_Upp3', type: 'string' },
        { name: 'SP_Temp', type: 'string' },
        { name: 'SP_ConWave', type: 'string' },
        { name: 'SP_LdClear', type: 'string' },
        { name: 'Conv_Speed', type: 'string' },
        { name: 'Conv_Width', type: 'string' },
        { name: 'Other_Ni', type: 'string' },
        { name: 'Remark', type: 'string' },
    ]
});


function Home_GetRealTimeData_Fill(viewBag) {
    try {
        Ext.getCmp('my-status').setStatus({ text: viewBag.Message });

        if (viewBag.Message.indexOf("Success") == 0) {
            //#region Part_Desc
            var rtData = Ext.decode(viewBag.jsonOut);

            Ext.getCmp("grid_RealTime").store.removeAll();

            var store = new Ext.data.Store({
                model: "Wave_RT_Full",
                id: "Gridstore_rt",
                data: rtData,
                autoLoad: true
            });
            //#endregion
            Ext.getCmp("grid_RealTime").bindStore(store);

            //#endregion
        }
        else
        {
            Ext.getCmp("grid_RealTime").store.removeAll();
        }

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}