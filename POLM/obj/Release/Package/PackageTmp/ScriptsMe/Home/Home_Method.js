///<reference path="~/Scripts/ExtJS/ext-all.js" />



//#region Get category list from server

function Home_GetList_Category() {
    try {
        //debugger;
        var viewBag = {
            Model: "101_Cagegory",
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

                if (viewBag.Message.indexOf("Success") == 0) {
                    //#region Get all list
                    var SoftList = Ext.decode(viewBag.jsonOut);

                    Ext.getCmp("grid_OverView").store.removeAll();

                    var store = new Ext.data.Store({
                        model: "Record_List",
                        id: "Gridstore",
                        data: SoftList,
                        autoLoad: true
                    });
                    
                    Ext.getCmp("grid_OverView").bindStore(store);
                    //#endregion

                    //#region Get category and fill list
                    var CategoryList = Ext.decode(viewBag.jsonData);

                    var list_Option = CategoryList;
                    for (var i = 0, c = list_Option.length; i < c; i++) {
                        list_Option[i] = [list_Option[i]];
                    }

                    var storeCmb = new Ext.data.ArrayStore({
                        data: list_Option,
                        fields: ['value']
                    });

                    if (list_Option.length > 0) {
                        Ext.getCmp("Cmbo_Category").bindStore(storeCmb);
                        Ext.getCmp("Cmbo_Category").setValue(list_Option[0]);
                    }
                    //#endregion
                }

                //#endregion
            },
            error: function (data, status, e)//服务器响应失败处理函数
            {
                //关闭spinner
                spinner.spin();
                Ext.Msg.show({ title: arguments.callee.name, msg: e, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            }
        });

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion

//#region startItemClick
function startItemClick(rowIndex, para) {
    try {
        debugger;
        var record = Ext.getCmp("grid_OverView").store.getAt(rowIndex);

        var item_Category = record.get("Category");
        var item_Cont = record.get("Item");
        var webAddress = record.get("Link");
        var item_ID = record.get("iid");

        if  (item_ID == "101" ) { //(item_Cont == "Real Time Data") {
            var newUrl = $("#Url_Page_RealData").val(); //newUrl = "~/RealData";
            window.location.assign(newUrl)
        }
        else if  (item_ID == "103" ) {  //(item_Cont == "WI Review") {
            var newUrl = $("#Url_Page_WIReview").val(); //newUrl = "~/RealData";
            window.location.assign(newUrl)
        }
        else if  (item_ID == "102" ) { //(item_Cont == "Upload WI") {
            var newUrl = $("#Url_Page_UploadWI").val(); //newUrl = "~/CompareParas";
            window.location.assign(newUrl);
        }
        else if (item_ID == "104") {// "Process On-Line Monitor Report") {
            var newUrl = $("#Url_Page_PolmRep").val(); //newUrl = "~/CompareParas";
            window.location.assign(newUrl);
        }
        //window.open(webAddress);   //在另外的窗口打开
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion



//#region Get list for this machine
function Home_GetMachineItems()
{
    try {
        var machine = Ext.getCmp("Cmbo_Category").getValue();
        var viewBag = {
            Model: "103_Items",
            Para1: machine,
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
                //#region bind category combobox
                var viewBag = data;
                Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
                if (viewBag.Message.indexOf("Success") == 0) {
                    //#region Get all list
                    var SoftList = Ext.decode(viewBag.jsonOut);

                    Ext.getCmp("grid_OverView").store.removeAll();

                    var store = new Ext.data.Store({
                        model: "Record_List",
                        id: "Gridstore",
                        data: SoftList,
                        autoLoad: true
                    });

                    Ext.getCmp("grid_OverView").bindStore(store);
                    //#endregion
                }
                //#endregion
            },
            error: function (data, status, e)//服务器响应失败处理函数
            {
                //关闭spinner
                spinner.spin();
                Ext.Msg.show({ title: arguments.callee.name, msg: e, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            }
        });

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion

