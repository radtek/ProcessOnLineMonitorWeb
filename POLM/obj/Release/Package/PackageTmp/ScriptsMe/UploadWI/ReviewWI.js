///<reference path="~/Scripts/ExtJS/ext-all.js" />
var Global_Height_Review = 400;

//#region form remark
/*
参考关于Tempory WorkI:
1. 需要点击Tempory WI 才能出现 列 1）VAL_Temp，2） Update
2. 当选择天数时，则 Val_Max 和 Val_Min 会变成红色，以警示此为Tempory WorkI有效

*/
//#endregion

//#region form initilize and start
Ext.onReady(function () {
    try {

        CreateGUI_Review_Wave();
        CreateGUI_ReviewWI();

        AdjustFormHeight();
    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
});
//#endregion

//#region Create GUI ---- 1. read excel content Oven -- Wave
function CreateGUI_Review_Wave() {
    try {
        //#region CreateGUI_UploadWI
        ///////////////////////////////////////////选择WorkSheet, read sheet 内容
        function renderUpdate_ReviewWI(value, metaData, record, rowIndex, columnIndex, store, view) {
            //debugger;
            var para = view.panel.id;// rowIndex + "|" + view.panel.id;
            return "<input type='button' value='" + record.get("View") +
                "'style='font-size:10px;font-weight:bold; width:60px;height:20px; ' onclick='startItemClickXlsFile(" + rowIndex + ',' + para + ")'" + ">";  //',' + view.panel.id +
        }

        //#region columns
        var columns_Wave = [
            { xtype: 'rownumberer', header: "#", width: 30, align: 'left' },
            {
                header: "<div style='font-size: 11px;'>WorkSheet</div>", dataIndex: "WORKSHEET", width: 170, menuDisabled: true, editor: {},// renderer: renderFormat,
            },
            {
                header: "<div style='font-size: 11px;'>Doc Num</div>", dataIndex: "DOCNUM", width: 80, menuDisabled: true, editor: {}, hidden: false,
            },
            {
                header: "<div style='font-size: 11px;'>Rev</div>", dataIndex: "DOCREV", width: 35, menuDisabled: true, editor: {}, hidden: false,
            },
            {
                header: "<div style='font-size: 11px;'>Model</div>", dataIndex: "MODEL", width: 120, menuDisabled: true, editor: {}, hidden: false,
            },
            {
                header: "<div style='font-size: 11px;'>Program</div>", dataIndex: "PROGRAM", width: 80, menuDisabled: true, editor: {},//renderer: renderColor 
            },
            {
                header: "<div style='font-size: 11px;'>Para_NAME</div>", dataIndex: "PARA_NAME", width: 90, menuDisabled: true, editor: {},//renderer: renderColor 
            },
            {
                header: "<div style='font-size: 11px;'>Desc</div>", dataIndex: "PARA_DESC", width: 160, menuDisabled: true, editor: {},//renderer: renderColor 
            },
            {
                header: "<div style='font-size: 11px;'>Val_Cen</div>", dataIndex: "VAL_CEN", width: 60, menuDisabled: true, editor: {},
                renderer: function (value, metaData, record, rowIndex, columnIndex, store, view)
                {	
                    //#region renderer
                    var TEMPDAYS = record.get("TEMPDAYS");  //参考关于Tempory WorkI
                    if (TEMPDAYS)
                    {
                        TEMPDAYS = TEMPDAYS.trim();
                        if (TEMPDAYS == "0" || TEMPDAYS == "")
                        {
                            //metaData.style = 'background-color: white;';
                        }
                        else
                        {
                            //metaData.style = 'background-color: red;';
                        }
                    }
                    return value;
                    //#endregion
                }
            },
            {
                header: "<div style='font-size: 11px;'>Val_Max</div>", dataIndex: "VAL_MAX", width: 60, menuDisabled: true, editor: {}, hidden: false, tdCls: 'tip',
                renderer: function (value, metaData, record, rowIndex, columnIndex, store, view) {	
                    //#region renderer
                    //var val_Temp = record.get("VAL_Temp");  //参考关于Tempory WorkI
                    //if (val_Temp) {
                    //    val_Temp = val_Temp.trim();
                    //    if (value == "") {
                    //        metaData.style = 'background-color: white;';
                    //    }
                    //    else {
                    //        metaData.style = 'background-color: red;';
                    //    }
                    //}
                    //return value;
                    var TEMPDAYS = record.get("TEMPDAYS");  //参考关于Tempory WorkI
                    if (TEMPDAYS)
                    {
                        TEMPDAYS = TEMPDAYS.trim();
                        if (TEMPDAYS == "0" || TEMPDAYS == "")
                        {
                            metaData.style = 'background-color: white;';

                            //var sTooltip = "数据来源于正式WorkI";
                            //metaData.tdAttr = 'data-qtip="' + sTooltip + '"';
                        }
                        else
                        {
                            metaData.style = 'background-color: red;';

                            //var sTooltip = "数据来源于 临时WorkI";
                            //metaData.tdAttr = 'data-qtip="' + sTooltip + '"';
                        }
                    }
                    return value;

                    //#endregion
                }
            },
            {
                header: "<div style='font-size: 11px;'>Val_Min</div>", dataIndex: "VAL_MIN", width: 60, menuDisabled: true, editor: {}, hidden: false, tdCls: 'tip',
                renderer: function (value, metaData, record, rowIndex, columnIndex, store, view) {	
                    //#region renderer
                    //var val_Temp = record.get("VAL_Temp"); //参考关于Tempory WorkI
                    //if (val_Temp) {
                    //    val_Temp = val_Temp.trim();
                    //    if (value == "") {
                    //        metaData.style = 'background-color: white;';
                    //    }
                    //    else {
                    //        metaData.style = 'background-color: red;';
                    //    }
                    //}
                    //return value;
                    var TEMPDAYS = record.get("TEMPDAYS");  //参考关于Tempory WorkI
                    if (TEMPDAYS)
                    {
                        TEMPDAYS = TEMPDAYS.trim();
                        if (TEMPDAYS == "0" || TEMPDAYS == "")
                        {
                            metaData.style = 'background-color: white;';
                        }
                        else
                        {
                            metaData.style = 'background-color: red;';
                        }
                    }
                    return value;
                    //#endregion
                }
            },
            {
                header: "<div style='font-size: 11px;'>TemDay</div>", dataIndex: "TEMPDAYS", width: 55, menuDisabled: true, editor: {}, hidden: true,
            },
            {
                header: "<div style='font-size: 11px;'>SET WI</div>", dataIndex: "VAL_Temp", width: 70, menuDisabled: true, hidden: true,
                editor: new Ext.form.field.ComboBox({  //参考关于Tempory WorkI
                    typeAhead: true,
                    //triggerAction: '3 day',
                    store: ['3 days', 'No Temp'],  //'2 days', '1 days',
                })
                //editor: {
                //    xtype: 'textfield',
                //    allowBlank: true,
                //    listeners: {
                //        //change: function (field, newValue, oldValue, eOpts)  { //(field, e) {
                //        //    var text = field.value.trim();
                //        //    if (text != "") {
                //        //        var grid = field.up('grid'),
                //        //                plugin = grid.findPlugin('cellediting');
                //        //        var record = plugin.context.record;
                //        //        record.commit();
                //        //    }
                //        //},
                //        //dirtychange: function ( field, isDirty, eOpts ) {
                //        //    var text = field.value.trim();
                //        //    if (text != "") {
                //        //        //var record = field.up('editor').context.record; //field.up("record");
                //        //        //record.commit();
                //        //        var grid = field.up('grid'),
                //        //                plugin = grid.findPlugin('cellediting');
                //        //        //console.log(plugin.context.record);
                //        //        var record = plugin.context.record;
                //        //        record.set("Val_Temp", text);
                //        //        record.commit();
                //        //    }
                //        //}
                //    }
                //},
            },
            {
                header: "<div style='font-size: 11px;'>Update</div>", dataIndex: "Update", width: 70, menuDisabled: true, editor: {}, hidden: true,
                renderer: function (value, metaData, record, rowIndex, columnIndex, store, view) {// (value, metaData, record) {  //change row format	
                    var val_Temp = record.get("VAL_Temp"); //参考关于Tempory WorkI
                    if (val_Temp) {
                        val_Temp = val_Temp.trim();
                        if (val_Temp == "")
                        {
                            return "";
                        }	
                        else
                        {
                            return "<input type='button' value='Update" + 
                        "' style='font-size:10px;font-weight:bold; width:60px;height:20px; ' onclick='ReviewWI_TemporyUpdate_Click(\"" + rowIndex + "\")'" + ">";

                            //return "<input type='button' value='Update' style='font-size:10px; width:60px;height:20px;' onclick='ReviewWI_Click(" + rowIndex + ")'" + ">";
                        }
                    }	
                    return value;	
                },
            },
            {
                header: "<div style='font-size: 11px;'>OV_ID</div>", dataIndex: "OV_ID", width: 60, menuDisabled: true, editor: {}, hidden: true,
            },

        ];
        //#endregion

        //#region tool bar
        var txtField_Wave = new Ext.form.TextField({
            id: 'txtField_Wave', fieldLabel: "Sheet:", labelWidth: 40, width: 100, anchor: '98%',  //必须要有，否则不能无效		
            grow: true, emptyText: "?", value: "", width: 200, selectOnFocus: false, hidden: true,
        });

        var toolbar_Wave = {
            id: 'toolbar_Wave',
            xtype: 'toolbar',
            dock: 'top', // bottom, right, left
            items: [
                {
                    //#region  Project
                    id: "Cmbo_Project", scale: "medium", xtype: "combo", iconCls: "", fieldLabel: "Project:", width: 120, labelWidth: 40,
                    padding: '0 2 0 2', valueField: 'value', displayField: 'value', editable: false, hidden: false,
                    tpl: Ext.create('Ext.XTemplate',
                        '<ul class="x-list-plain"><tpl for=".">',
                            '<li role="option" class="x-boundlist-item" style="font-weight: bold;">{value}</li>',
                        '</tpl></ul>'
                    ),
                    displayTpl: Ext.create('Ext.XTemplate',// template for the content inside text field	
                        '<tpl for=".">',
                            '{value}',
                        '</tpl>'
                    ),
                    listeners: {
                        select: function (combo, record, eOpts) {
                            var selOpt = record.get('value');
                            if (selOpt) {
                                Project_Select_BindLine(selOpt);
                            }
                        },
                        afterrender: function (combo) {
                            //combo.getEl().on('dblclick', function () {
                            //    BindDataForGrid();
                            //});
                        }
                    }
                    //#endregion 
                },
                '-',
                {
                    //#region  Line
                    id: "Cmbo_Line", scale: "medium", xtype: "combo", iconCls: "", fieldLabel: "Line:", width: 100, labelWidth: 30,
                    padding: '0 2 0 2', valueField: 'value', displayField: 'value', editable: false, hidden: false,
                    tpl: Ext.create('Ext.XTemplate',
                        '<ul class="x-list-plain"><tpl for=".">',
                            '<li role="option" class="x-boundlist-item" style="font-weight: bold;">{value}</li>',
                        '</tpl></ul>'
                    ),
                    displayTpl: Ext.create('Ext.XTemplate',// template for the content inside text field	
                        '<tpl for=".">',
                            '{value}',
                        '</tpl>'
                    ),
                    //#endregion 
                },
                {
                    id: "btn_ReviewParas_Wave", scale: "medium", text: "<div style='font-weight:bolder; font-size:11px;'><b>Review Parameter</b></div>", xtype: "button", iconCls: "iconDownload24",
                    handler: onToolBtnClick_Review  //Review_Wave_GetParameters
                },
                txtField_Wave,
                '->',
                {
                    id: "btn_Review_TempWI", scale: "medium", xtype: "button", iconCls: "iconEdit24",  // baseCls: 'buttonStd', baseCls效果不好
                    text: "<div style='font-weight:bold; font-size:12px; '>Tempory WI</div>",
                    handler: function () { Review_Wave_TemporyWI()},
                },
            ]
        };

        var toolbar_Bot_Wave = {
            id: 'toolbar_Wave_Bot',
            xtype: 'toolbar',
            dock: 'bottom',
            hidden: false,
            items: [
                {
                    id: "txt_QueryModel", xtype: "textfield", fieldLabel: "Model查询:", labelWidth: 60, width: 180, emptyText: '',			
                    listeners:			
                   {			
                       scope: this,			
                       change: function (field, newValue, oldValue, eOpts) {			
                           debugger;			
                           var grid = Ext.getCmp('grid_Review_Wave');
                           grid.store.clearFilter();			
			
                           if (newValue) {			
                               var matcher = new RegExp(Ext.String.escapeRegex(newValue), "i");			
                               grid.store.filter({			
                                   filterFn: function (record) {			
                                       return matcher.test(record.get('MODEL'))
                                   }			
                               });			
                           }			
                       }			
                   },
                },
                '-',
                {
                    id: "txt_QueryProgram", xtype: "textfield", fieldLabel: "Program查询:", labelWidth: 80, width: 190, emptyText: '', padding: '0 5 0 5',
                    listeners:
                   {
                       scope: this,
                       change: function (field, newValue, oldValue, eOpts)
                       {
                           debugger;
                           var grid = Ext.getCmp('grid_Review_Wave');
                           grid.store.clearFilter();

                           if (newValue)
                           {
                               var matcher = new RegExp(Ext.String.escapeRegex(newValue), "i");
                               grid.store.filter({
                                   filterFn: function (record)
                                   {
                                       return matcher.test(record.get('PROGRAM'))
                                   }
                               });
                           }
                       }
                   },
                },
            ]
        };
        //#endregion

        Ext.QuickTips.init();

        var grid_Review_Wave = Ext.create("Ext.grid.Panel", {
            id: "grid_Review_Wave",
            title: "Wave Parameter",
            width: '100%',  //forceFit: true,   
            columnLines: true,
            height: 300,  //autoHeight: true, //height: 250, //
            store: new Ext.data.ArrayStore(),
            //renderTo: "divUploadWI",
            columns: columns_Wave,
            collapsible: true,
            plugins: [
                { ptype: "cellediting", },
            ],
            dockedItems: [
                toolbar_Wave,
                toolbar_Bot_Wave,
            ],
            viewConfig: {
                stripeRows: false,//在表格中显示斑马线
                enableTextSelection: true
            },
            listeners: {
                //#region grid event
                afterrender: {
                    fn: function () {
                        Init_grid_Review_Wave();
                    }
                },
                headerclick: function (ct, column, e, t, eOpts) {//选择是否全选
                    //toolFun_Header(column, t);
                },
                edit: function (editor, context, eOpts) {
                    var record = context.record;
                    record.commit();
                },
                render: function (grid)
                {   //////////////////////////////////////////////显示grid cell tip 	
                    //#region 显示grid cell tip 
                    var view = grid.getView();	
                    grid.tip = Ext.create('Ext.tip.ToolTip', {	
                        target: view.getId(),	
                        delegate: view.itemSelector + ' .tip',	
                        trackMouse: true,	
                        listeners: {	
                            beforeshow: function (tip) {	
                                var tipGridView = tip.target.component;	
                                var colname = tipGridView.getHeaderCt().getHeaderAtIndex(tip.triggerElement.cellIndex).dataIndex;	
                                //tip.update(record.get(colname));	
                                var colIndex = tip.triggerElement.cellIndex;	
                                if (colname == "VAL_MAX" || colname == "VAL_MIN")
                                {
                                    //debugger;	
                                    var record = tipGridView.getRecord(tip.triggerElement);

                                    var maxValue = record.get(colname);
                                    var workI_S = "正式WorkI";

                                    var TEMPDAYS = record.get("TEMPDAYS");
                                    if (TEMPDAYS)
                                    {
                                        TEMPDAYS = TEMPDAYS.trim();
                                        if (TEMPDAYS == "0" || TEMPDAYS == "")
                                        {
                                        }
                                        else
                                        {
                                            workI_S = "临时WorkI";
                                            var tipValue = "<div style='font-size: 11px;'>数据来源于 <span style='color:red;'>" + workI_S + "</span>  " + maxValue + "</div>";
                                            tip.update(tipValue);
                                        }
                                    }
                                    else
                                    {
                                        var tipValue = "<div style='font-size: 11px;'>数据来源于 <span style='color:black;'>" + workI_S + "</span>  " + maxValue + "</div>";
                                        tip.update(tipValue);
                                    }
                                }	
                            }	
                        }	
                    });
                    //#endregion
                },	
                destroy: function (view) {	
                    delete view.tip;	
                }	
                //#endregion
            },
        })

        //grid_WI_Wave.header.titleCmp.textEl.selectable();
        //#endregion
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion

//#region Create framework of GUI
function CreateGUI_ReviewWI()
{
    try {

        Ext.create('Ext.tab.Panel', {
            id: 'TabAll_Review',
            title: 'Review WI Parameters',
            renderTo: 'divReviewWIAll',
            width: '100%',
            height: Global_Height_Review,
            cls: 'mytab',
            activeTab: 0,
            items: [
                {
                    id: 'tabWave',
                    title: '<p><div style="font-size: 14px; color:black">Wave WI Content</div></p>',
                    iconCls: 'iconElectra32',
                    items: [Ext.getCmp("grid_Review_Wave")],
                }, {
                    id: 'tabReflow',
                    title: '<p><div style="font-size: 14px;color:black">Reflow WI</div></p>',
                    iconCls: 'iconBTU32',
                    items: []
                },
                {
                    id: 'tabDek',
                    title: '<p><div style="font-size: 14px;color:black">Dek WI</div></p>',  //查询存取
                    iconCls: 'iconDEK32',
                    items: [],
                }
            ],

            listeners: {
                'tabchange': function (tabPanel, tab) {
                    tab.title = '<span style="font-color: #FF0000;">Material In</span>';
                }
            }
        });
        Ext.getCmp("TabAll_Review").header.titleCmp.textEl.selectable();



    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion


function onToolBtnClick_Review(item)
{
    try {
        var sText = item.text;
        var sID = item.id;
        if (sID == "btn_Review_Wave_ShowAll") {
            Review_Wave_ShowAllParameters();
        }
        else if (sID == "btn_ReviewParas_Wave") {
            Review_Wave_GetParameters();
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#region additional function
function Init_grid_Review_Wave() {
    try {
        

        //#region hide some column
        //var grid = Ext.getCmp("grid_Review_Wave");

        //var columns = grid.getColumnManager().getColumns();
        //for (var i = 0; i < columns.length; i++) {
        //    var column = columns[i];
        //    var di = column.dataIndex;
        //    if (di.indexOf("_M") > 0) {
        //        column.hide();
        //    }
        //}
        //#endregion

        //#region get projects
        var viewBag = {
            Model: "701_Project",
            Para1: "Wave",
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
            success: function (viewBag) {
                //关闭spinner
                spinner.spin();

                Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
                if (viewBag.Message.indexOf("Success") == 0) {
                    //debugger;

                    //#region 绑定Project
                    var list_Option = JSON.parse(viewBag.ParaRet1);
                    for (var i = 0, c = list_Option.length; i < c; i++) {
                        list_Option[i] = [list_Option[i]];
                    }

                    var storeCmbCell = new Ext.data.ArrayStore({
                        data: list_Option,
                        fields: ['value']
                    });

                    if (list_Option.length > 0) {
                        Ext.getCmp("Cmbo_Project").bindStore(storeCmbCell);
                    }
                    //#endregion

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
        //#endregion
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}


function AdjustFormHeight() {
    try {
        //debugger;
        var bodyHeight = $('#divBody').height(); //$('body').height();
        var progTitle = $("#prgTitle").height();
        var footerHeight = $("#contFooter").height();

        var contAll_Height = bodyHeight - progTitle -12;// - footerHeight;
        $("#contAll").height(contAll_Height);

        var bodyHeightAll = $("#contAll").height();
        var MainHeight = bodyHeightAll - footerHeight - 12;

        $("#contMain").height(MainHeight);


        var divTitle = $("#divTitle").height();
        var divGrid_Main = MainHeight - divTitle -20;
        Ext.getCmp("TabAll_Review").setHeight(divGrid_Main);

        var hGridWave = divGrid_Main - 90;
        Ext.getCmp("grid_Review_Wave").setHeight(hGridWave);

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion










