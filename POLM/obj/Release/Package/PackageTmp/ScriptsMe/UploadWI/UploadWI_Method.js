///<reference path="~/Scripts/ExtJS/ext-all.js" />

var Global_Tracking;

//#region 1. WI upload excel to server
function wiFile_Upload() {
    try {
        debugger;

        Ext.getCmp('my-status').setStatus({ text: "Upload file ..." });

        //Global_SAP_Update_Track = Global_SAP_Update_Track + "开始 call sapFile_Upload @ " + new Date() + "|";

        var Url_Client = $("#Url_UploadWI_uploadFiles").val();  //UploadWI/uploadFiles

        var file = Ext.getCmp("fileBrowseFieldId").fileInputEl.dom.files[0],
             reader;
        if (file === undefined || !(file instanceof File)) {
            Ext.getCmp('my-status').setStatus({ text: "Please select file" });
            return;
        }
        var dataFile = new FormData();
        dataFile.append('file', file);

        //var selectDateFrom = Ext.util.Format.date(Ext.getCmp('dtPickerFrom').getValue(), 'Y-m-d');
        //dataFile.append("para1", "");
        dataFile.append("para1", Ext.getCmp("Cmbo_Machine").getValue());


        $.ajax({
            url: Url_Client, type: 'post', data: dataFile, dataType: "json",
            processData: false,  // tell jQuery not to process the data
            contentType: false,   // tell jQuery not to set contentType
            beforeSend: function () {
                //异步请求时spinner出现
                $("#divMySpin").text("");
                var target = $("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (data) {
                //关闭spinner
                spinner.spin();

                Global_Tracking = data.List_Track;
                //console.log("Success to return @ " + new Date());
                ProcessDataFromServer_Upload(data);
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

function ProcessDataFromServer_Upload(viewBag)
{
    try {
        Ext.getCmp('my-status').setStatus({ text: viewBag.Message });

        if (viewBag.Message.indexOf("Success") == 0) {
            Ext.Msg.show({
                title: 'Confirm', msg: viewBag.Message + ", 重新读取excel清单？", buttons: Ext.Msg.YESNO, icon: Ext.MessageBox.QUESTION,
                fn: function (btnId) {
                    if (btnId == "yes") {
                        wiFile_LoadList();
                    }
                    else
                        return;
                }
            });
        }
        else {
            Ext.Msg.show({
                title: 'Confirm', msg: "Fail to upload, do you want to view log?", buttons: Ext.Msg.YESNO, icon: Ext.MessageBox.QUESTION,
                fn: function (btnId) {
                    if (btnId == "yes") {
                        //ShowTracking(viewBag.Message, "upload File");
                        onMenuItemClick_ShowTracking();
                    }
                    else
                        return;
                }
            });
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion

//#region 2. load excel list

function wiFile_LoadList()
{
    try {
        //#region wiFile_LoadList
        debugger;
        var viewBag = {
            Model: "501_xlsList",
            Title: "WI Load excel list",
            PcName: "",
            Para1: Ext.getCmp("Cmbo_Machine").getValue(),
        };
        Ext.getCmp('my-status').setStatus({ text: "Getting xls file ..." });

        Ext.getCmp("grid_xlsList").store.loadData([], false);
        Ext.getCmp("grid_xlsList").store.loadData([]); //this is ok			
        //Ext.getCmp("grid_xlsCont").store.loadData([], false);
        //Ext.getCmp("grid_xlsCont").store.loadData([]); //this is ok			

        Global_Tracking = "";

        var Url_Client = $("#Url_UploadWI_Operation").val();

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


                ProcessDataFromServer_xlsList(data);
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
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}


Ext.define('Xls_List', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Category', type: 'string' },
        { name: 'Doc', type: 'string' },
        { name: 'Rev', type: 'string' },
        { name: 'Remark', type: 'string' },
        { name: 'DateTime', type: 'string' },
        { name: 'View', type: 'string' },
        { name: 'File', type: 'string' },

    ]
});

function ProcessDataFromServer_xlsList(viewBag)
{
    try {
        //#region Part_Desc
        Ext.getCmp('my-status').setStatus({ text: viewBag.Message });

        Global_Tracking = viewBag.List_Track;
        if (viewBag.Message.indexOf("Success") == 0) {

            var XlsList = Ext.decode(viewBag.jsonOut);
            var store = new Ext.data.Store({
                model: "Xls_List",
                id: "Gridstore",
                data: XlsList,
                autoLoad: true
            });

            Ext.getCmp("grid_xlsList").bindStore(store);
        }
        //#endregion
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion

//#region 3. Get work sheet 
function startItemClickXlsFile(nRow)
{
    try {

        var store = Ext.getCmp("grid_xlsList").store.getAt(nRow);
        var Category = store.get("Category");
        var XlsFilName = store.get("File");

        Ext.getCmp("grid_WI_WorkSheet").store.loadData([], false);
        Ext.getCmp("grid_WI_WorkSheet").store.loadData([]); //this is ok	


        if (XlsFilName == "") {
            Ext.Msg.show({ title: arguments.callee.name, msg: "Fail to get excel file name.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return;
        }

        var grid = Ext.getCmp("grid_xlsList");
        var storeFile = grid.getStore();
        for (var i = 0; i < storeFile.data.length; i++) {
            var recordONE = storeFile.getAt(i);
            if (i == nRow) {
                recordONE.set("Check", true);
                recordONE.commit();
            }
            else {
                recordONE.set("Check", false);
                recordONE.commit();
            }
        }


        var Url_Client = $("#Url_UploadWI_Operation").val();

        var viewBag = {
            Model: "503_xlsSheet",
            User: glbLogin_getCookie("userEN"),
            Para1: Category,
            Para2: XlsFilName,
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

                ProcessDataFromServer_xlsSheet(data);
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

Ext.define('SheetList', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'File', type: 'string' },
        { name: 'Sheet', type: 'string' },
        { name: 'Rev', type: 'string' },
        { name: 'Remark', type: 'string' },
        { name: 'View', type: 'string' },

    ]
});

function ProcessDataFromServer_xlsSheet(viewBag)
{
    try {

        Ext.getCmp('my-status').setStatus({ text: viewBag.Message });

        if (viewBag.Message.indexOf("Success") == 0) {

            var xlsList = Ext.decode(viewBag.jsonOut);
            var store = new Ext.data.Store({
                model: "SheetList",
                id: "sheetList",
                data: xlsList,
                autoLoad: true
            });

            Ext.getCmp("grid_WI_WorkSheet").bindStore(store);

            Ext.getCmp('TabAll_WI').setActiveTab('tabSheets');
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion

//#region 4. Reading select work sheet 读取工作表
function startItemXlsSheet(nRow)
{
    try {
        var store = Ext.getCmp("grid_WI_WorkSheet").store.getAt(nRow);
        var XlsFilName = store.get("File");
        var Category = Ext.getCmp("Cmbo_Machine").getValue();
        var SheetName = store.get("Sheet");
        var Line = store.get("Line");
        if (Line == null)
            Line = "";
        
        if (SheetName == "") {
            Ext.Msg.show({ title: arguments.callee.name, msg: "Fail to get work sheet name.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return;
        }
        if (Line == "") {
            Ext.Msg.show({ title: arguments.callee.name, msg: "没有获取到 线别 名字", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return;
        }
        if (XlsFilName == "") {
            Ext.Msg.show({ title: arguments.callee.name, msg: "Fail to get excel file name.", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return;
        }
        Ext.getCmp('my-status').setStatus({ text: "Reading worksheet " + SheetName + " ..." });

        Ext.getCmp('TabAll_WI').setActiveTab('tabWave');

        var Url_Client = $("#Url_UploadWI_Operation").val();

        var viewBag = {
            Model: "505_xlsCont",
            User: glbLogin_getCookie("userEN"),
            Para1: Category,
            Para2: XlsFilName,
            Para3: SheetName,
            Para4: Line,
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

                ProcessDataFromServer_xlsCont(data);
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

function ProcessDataFromServer_xlsCont(viewBag)
{
    try {
        //#region ProcessDataFromServer_xlsCont
        Ext.getCmp('my-status').setStatus({ text: viewBag.Message });

        if (viewBag.Message.indexOf("Success") == 0) {

            var Xls_Wave_Cont = Ext.decode(viewBag.jsonOut);
            var store = new Ext.data.Store({
                model: "Xls_Wave_Cont",
                id: "Xls_Wave_Cont",
                data: Xls_Wave_Cont,
                autoLoad: true
            });

            Ext.getCmp("grid_WI_Wave").bindStore(store);

            Ext.getCmp('TabAll_WI').setActiveTab('tabWave');
            
            Ext.getCmp("txtField_Wave").setValue(viewBag.Para3 + "|" + viewBag.Para4); //保存工作表名字

        }
        else {
            Ext.getCmp("txtField_Wave").setValue(""); //保存工作表名字

            Ext.getCmp("grid_WI_Wave").store.loadData([], false);
            Ext.getCmp("grid_WI_Wave").store.loadData([]); //this is ok		
        }
        //#endregion
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

Ext.define('Xls_Wave_Cont', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'ModelName', type: 'string' },
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

//#endregion


//#region 5.1 Wave upload all parameter to server 更新参数到服务器

//#region new method, all data 一次性更新
function Wave_Upload_Paras2() {
    try {
        var Machine = "", DocNum = "", DocRev = "", Project = "", FileName = "";

        var grid = Ext.getCmp("grid_xlsList");
        if (grid.store.data.length > 0) {
            for (var i = 0; i < grid.store.data.length; i++) {
                var recordONE = grid.store.getAt(i);
                var checked = recordONE.get("Check");
                if (checked) {
                    Machine = recordONE.get("Category");
                    DocNum = recordONE.get("Doc");
                    DocRev = recordONE.get("Rev");
                    Project = recordONE.get("Project");
                    FileName = recordONE.get("Remark");
                    break;
                }
            }
        }
        else {
            Ext.Msg.show({ title: arguments.callee.name, msg: "No ONE row data", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return;
        }

        //var paraData = Ext.encode(record.data);
        var rowIndex = 0;
        var record = Ext.getCmp("grid_WI_Wave").store.getAt(rowIndex);
        var bIsChecked = record.get("Check");
        var store = Ext.getCmp("grid_WI_Wave").store;

        //var paraData = Ext.encode(grid.store.data);
        var jsonData = Ext.encode(Ext.pluck(store.data.items, 'data'));
        var viewBag = {
            Model: "603_UpPara2",
            User: glbLogin_getCookie("userEN"),
            Para1: rowIndex,
            Para2: Machine,
            Para3: Project,
            Para4: FileName,
            Para5: Ext.getCmp("txtField_Wave").getValue(), //获取工作表名字
            Para6: DocNum,
            Para7: DocRev,
            Para8: jsonData,
            Para9: bIsChecked,
        };
        var Url_Client = $("#Url_UploadWI_Operation").val();

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

                Global_Tracking = viewBag.List_Track;

                Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
                //ProcessDataFromServer_UploadPara(data);
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


//#region old method , 1个个的更新，速度慢
function Wave_Upload_Paras()
{
    try {
        var Machine="", DocNum = "", DocRev="", Project="", FileName="";

        var grid = Ext.getCmp("grid_xlsList");
        if (grid.store.data.length > 0) {
            for (var i = 0; i < grid.store.data.length; i++) {
                var recordONE = grid.store.getAt(i);
                var checked = recordONE.get("Check");
                if (checked) {
                    Machine = recordONE.get("Category");
                    DocNum = recordONE.get("Doc");
                    DocRev = recordONE.get("Rev");
                    Project = recordONE.get("Project");
                    FileName = recordONE.get("Remark");
                    break;
                }
            }
        }
        else {
            Ext.Msg.show({ title: arguments.callee.name, msg: "No ONE row data", buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            return;
        }

        //var paraData = Ext.encode(record.data);
        var rowIndex = 0;
        var record = Ext.getCmp("grid_WI_Wave").store.getAt(rowIndex);
        var bIsChecked = record.get("Check");

        var paraData = Ext.encode(record.data);
        var viewBag = {
            Model: "601_UpPara",
            User: glbLogin_getCookie("userEN"),
            Para1: rowIndex,
            Para2: Machine,
            Para3: Project,
            Para4: FileName,
            Para5: Ext.getCmp("txtField_Wave").getValue(), //获取工作表名字
            Para6: DocNum,
            Para7: DocRev,
            Para8: paraData,
            Para9: bIsChecked,
        };
        var Url_Client = $("#Url_UploadWI_Operation").val();

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

                ProcessDataFromServer_UploadPara(data);
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

function ProcessDataFromServer_UploadPara(viewBag)
{
    try {
        Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
        if (viewBag.Message.indexOf("Success") == 0) {
            var rowIndex = viewBag.Para1;
            var record = Ext.getCmp("grid_WI_Wave").store.getAt(rowIndex);
            record.set("Comments", viewBag.Message);
            record.commit();

            Wave_Upload_Paras_Continue(viewBag);
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//继承上面的Wave_Upload_Paras 参数清单
function Wave_Upload_Paras_Continue(viewBag) {
    try {

        var rowIndex = parseInt(viewBag.Para1);
        rowIndex = rowIndex + 1;

        var gridStore = Ext.getCmp("grid_WI_Wave").store;
        if (gridStore.data.length > rowIndex) {
            //#region gridStore
            var record = gridStore.getAt(rowIndex);
            var bIsChecked = record.get("Check");
            var paraData = Ext.encode(record.data);

            //循环利用前面的viewBag, update参数
            viewBag.Para1 = rowIndex;
            viewBag.Para8 = paraData;
            viewBag.Para9 = bIsChecked;

            //var viewBag = {
            //    Model: "601_UpPara",
            //    Para1: rowIndex,
            //    Para8: paraData,
            //    Para9: bIsChecked,
            //};
            var Url_Client = $("#Url_UploadWI_Operation").val();

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

                    ProcessDataFromServer_UploadPara(data);
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
        else {
            Ext.getCmp('my-status').setStatus({ text: "All rows updated."});
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion 

//#endregion

//#region 5.2 Wave Show all prameter
function Wave_ShowAllParameters()
{
    try {
        var grid = Ext.getCmp("grid_WI_Wave");
        var btn = Ext.getCmp("btn_Wave_ShowAll");

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
                if (bShow){
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

//#region Direct read xls and update server
function startItemXlsFile_Update(nRow)
{
    try {
        var store = Ext.getCmp("grid_xlsList").store.getAt(nRow);
        var Category = store.get("Category");
        var XlsFilName = store.get("File");
        var Project = store.get("Project");
        var Doc_Num = store.get("Doc");
        var Doc_Rev = store.get("Rev");
        var Doc_EffDate = store.get("DateTime");


        var grid = Ext.getCmp("grid_xlsList");
        var storeFile = grid.getStore();
        for (var i = 0; i < storeFile.data.length; i++) {
            var recordONE = storeFile.getAt(i);
            if (i == nRow) {
                recordONE.set("Check", true);
                recordONE.commit();
            }
            else {
                recordONE.set("Check", false);
                recordONE.commit();
            }
        }

        var viewBag = {
            Model: "605_UpPara3",
            User: glbLogin_getCookie("userEN"),
            Project: Project,
            Para1: Category,
            Para2: XlsFilName,
            Para3: Doc_Num,
            Para4: Doc_Rev,
            Para5: Doc_EffDate,
        };

        var Url_Client = jQuery("#Url_UploadWI_Operation").val();

        jQuery.ajax({
            url: Url_Client, type: 'post', data: viewBag, dataType: "json",
            beforeSend: function () {
                //异步请求时spinner出现
                $("#divMySpin").text("");
                var target = $("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (viewBag) {
                //关闭spinner
                spinner.spin();

                Global_Tracking = viewBag.List_Track;
                Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
                //ProcessDataFromServer_XXX(viewBag);
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


//#region additional -- 1)delete excel file
function onMenuItemClick_DelExcel(grid, record, item, index, e)
{
    try {
        var MaType = record.get("Category");
        var FileName = record.get("Remark");
        
        var viewBag = {
            Model: "507_xlsDel",
            User: glbLogin_getCookie("userEN"),
            Para1: MaType,
            Para2: FileName,
        };
        Ext.getCmp('my-status').setStatus({ text: "Deleting file ..." });

        var Url_Client = $("#Url_UploadWI_Operation").val();
        $.ajax({
            url: Url_Client, type: 'post', data: viewBag, dataType: "json",
            beforeSend: function () {
                //异步请求时spinner出现
                $("#divMySpin").text("");
                var target = $("#divMySpin").get(0);
                spinner.spin(target);
            },
            success: function (viewBag) {
                //关闭spinner
                spinner.spin();

                Ext.getCmp('my-status').setStatus({ text: viewBag.Message });

                Ext.Msg.show({
                    title: 'Confirm', msg: viewBag.Message  + ", 重新读取excel清单？", buttons: Ext.Msg.YESNO, icon: Ext.MessageBox.QUESTION,
                    fn: function (btnId) {
                        if (btnId == "yes") {
                            wiFile_LoadList();
                        }
                        else
                            return;
                    }
                });


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

function onMenuItemClick_ShowTracking()
{
    try {
        if (Global_Tracking) {
            if (Global_Tracking.length > 0) {
                var All_Data = "";
                for (var i = 0; i < Global_Tracking.length; i++) {
                    All_Data = All_Data + Global_Tracking[i] + "\n\r";
                }
                ShowTracking(All_Data, "Tracking");
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

function ShowTracking(trackingTxt, title) {

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

//#endregion
















