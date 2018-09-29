var Global_Height_Machine = 230;

Ext.onReady(function () {
    try {
        Create_SingalR();

        CreateGUI_UploadWI();

        CreateGUI_Excel_ListSheet();
        CreateGUI_Oven_Wave();
        CreateGUI_LoadConent();
    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
});

/*
1. load=>upload wi into db directly => startItemXlsFile_Update (605_UpPara3)
*/
//#region Crete GUI ----- 1. load excel list
function CreateGUI_UploadWI()
{
    try {
        //#region CreateGUI_UploadWI

        function renderUpdate(value, metaData, record, rowIndex, columnIndex, store, view) {
            //debugger;
            var para = view.panel.id;// rowIndex + "|" + view.panel.id;
            if (value == "update") {
                return "<input type='button' value='load" + //record.get("Update") +
                    "' style='font-size:10px;font-weight:bold; width:60px;height:20px; ' onclick='startItemXlsFile_Update(" + rowIndex + ',' + para + ")'" + ">";  //',' + view.panel.id +
            }

            return "<input type='button' value='" + record.get("View") +
                "'style='font-size:10px;font-weight:bold; width:60px;height:20px; ' onclick='startItemClickXlsFile(" + rowIndex + ',' + para + ")'" + ">";  //',' + view.panel.id +
        }

        //#region columns
        var columns_UploadWI = [
            { xtype: 'rownumberer', header: "#", width: 40, align: 'left' },
            {
                xtype: 'checkcolumn', header: "<div style='font-size: 11px;'>chk</div>", dataIndex: "Check", width: 40, menuDisabled: true, stopSelection: true, sortable: false, 
            },
            {
                header: "<div style='font-size: 11px;'>Category</div>", dataIndex: "Category", width: 100, height: 30, menuDisabled: true, editor: {},
            },
            {
                header: "<div style='font-size: 11px;'>Doc#</div>", dataIndex: "Doc", width: 150, menuDisabled: true, editor: {},// renderer: renderFormat,
            },
            {
                header: "<div style='font-size: 11px;'>Rev</div>", dataIndex: "Rev", width: 50, menuDisabled: true, editor: {}, hidden: false,
            },
            {
                header: "<div style='font-size: 11px;'>Remark</div>", dataIndex: "Remark", flex: 1, menuDisabled: true, editor: {},//renderer: renderColor 
            },
            {
                header: "<div style='font-size: 11px;'>Project</div>", dataIndex: "Project", width: 50, menuDisabled: true, editor: {},//renderer: renderColor 
            },
            {
                header: "<div style='font-size: 11px;'>Eff Data</div>", dataIndex: "DateTime", width: 120, menuDisabled: true, //editor: {},   
            },
            {
                header: "<div style='font-size: 11px;'>View</div>", dataIndex: "View", width: 100, menuDisabled: true, renderer: renderUpdate, tdCls: 'tip',
            },
            {
                header: "<div style='font-size: 11px;'>Update</div>", dataIndex: "Update", width: 100, menuDisabled: true, renderer: renderUpdate, tdCls: 'tip',
            },
            {
                header: "<div style='font-size: 11px;'>File</div>", dataIndex: "File", width: 120, menuDisabled: true, hidden: true,
            },
        ];
        //#endregion

        //#region tool bar

        var toolbar1 = {
            id: 'top_gridWI_1',
            xtype: 'toolbar',
            dock: 'top', // bottom, right, left
            items: [
                {
                    id: "Cmbo_Machine", scale: "medium", xtype: "combo", fieldLabel: "Machine:", labelWidth: 40, width: 150, iconCls: "", padding: '0 5,0,5', valueField: 'value', displayField: 'value', //handler: onToolBtnClick_UploadWI,
                    tpl: Ext.create('Ext.XTemplate',
                        '<ul class="x-list-plain"><tpl for=".">',
                            '<li role="option" class="x-boundlist-item" style="font-weight: bold;">{value}</li>',
                        '</tpl></ul>'
                    ),
                    displayTpl: Ext.create('Ext.XTemplate',// template for the content inside text field	
                        '<tpl for=".">',
                            //'<div style="font-weight: bold;">' +
                            '{value}', //+ 
                            //'</div>',
                        '</tpl>'
                    ),
                    listeners: {
                        select: function (combo, record, eOpts) {
                            //#region
                            var mcSelect = record.get('value');
                            //var mcSelect = Ext.getCmp("Cmbo_Machine").getValue();
                            if (mcSelect == "Wave") {
                                Ext.getCmp("TabAll_WI").child('#tabSheets').tab.show();
                                Ext.getCmp("TabAll_WI").child('#tabWave').tab.show();
                                Ext.getCmp("TabAll_WI").child('#tabReflow').tab.hide();
                                Ext.getCmp("TabAll_WI").child('#tabDek').tab.hide();
                            }
                            else if (mcSelect == "Reflow") {
                                Ext.getCmp("TabAll_WI").child('#tabSheets').tab.show();
                                Ext.getCmp("TabAll_WI").child('#tabWave').tab.hide();
                                Ext.getCmp("TabAll_WI").child('#tabReflow').tab.show();
                                Ext.getCmp("TabAll_WI").child('#tabDek').tab.hide();
                            }
                            else if (mcSelect == "Dek") {
                                Ext.getCmp("TabAll_WI").child('#tabSheets').tab.show();
                                Ext.getCmp("TabAll_WI").child('#tabWave').tab.hide();
                                Ext.getCmp("TabAll_WI").child('#tabReflow').tab.hide();
                                Ext.getCmp("TabAll_WI").child('#tabDek').tab.show();
                            }
                            //#endregion
                        },
                    }

                },
                '-',
                {
                    id: 'fileBrowseFieldId', xtype: "filefield", fieldLabel: 'Excel file', fieldName: 'file', labelWidth: 70, width: 500, allowBlank: false, buttonText: 'Select...'
                },
                '-',
                {  //call wiFile_Upload();
                    id: "btn_AddFile", scale: "medium", text: "<div style='font-weight:bolder; font-size:11px;'><b>Upload This WI</b></div>", xtype: "button", iconCls: "iconUpload20",
                    handler: onToolBtnClick_UploadWI   //wiFile_Upload();
                },
                '',
                {
                    id: "btn_WITracking", scale: "medium", text: "<div style='font-weight:normal; font-size:11px;'>Tracking</div>", xtype: "button", iconCls: "iconDoc16",
                    handler: function () { onMenuItemClick_ShowTracking(); }
                },
                '->',
                {    //wiFile_LoadList
                    id: "btn_LoadWIList", scale: "medium", xtype: "button", iconCls: "iconExcel16",  // baseCls: 'buttonStd', baseCls效果不好
                    text: "<div style='font-weight:bold; font-size:12px; '>List WI</div>",
                    handler: onToolBtnClick_UploadWI, //wiFile_LoadList();
                },
            ],

        };

        var toolbar_Bot = {
            id: 'bot_gridWI_1',
            xtype: 'toolbar',
            dock: 'bottom',
            hidden: false,
            items: [
                {   //////wiFile_LoadList
                    id: "btn_LoadWIList", scale: "medium", xtype: "button", iconCls: "iconExcel16",  // baseCls: 'buttonStd', baseCls效果不好
                    text: "<div style='font-weight:bold; font-size:12px; '>Load WI Excel</div>",
                    handler: onToolBtnClick_UploadWI
                },
                //'->',
                //{
                //    id: "btn_SAP", scale: "medium", xtype: "button", iconCls: "iconSAP24",
                //    //text: "<div style='font-weight:bold; font-size:12px; color:#FF6600;'>Start</div>", //颜色不一样
                //    text: "<div style='font-weight:bold; font-size:13px;'>SAP</div>",
                //    handler: onToolBtnClick_UploadWI
                //},
            ]
        };

        //#endregion

        var grid_xlsList = Ext.create("Ext.grid.Panel", {
            id: "grid_xlsList",
            title: "Upload WI",
            width: '100%',  //forceFit: true,   
            columnLines: true,
            height: Global_Height_Machine,  //autoHeight: true, //height: 250, //
            store: new Ext.data.ArrayStore(),
            renderTo: "divUploadWI",
            columns: columns_UploadWI,
            collapsible: true,
            plugins: [
                { ptype: "cellediting", },
            ],
            dockedItems: [
                toolbar1,
                //toolbar_Bot,
            ],
            viewConfig: {
                stripeRows: false,//在表格中显示斑马线
                enableTextSelection: true
            },
            listeners: {
                render: function (grid) {   //////////////////////////////////////////////显示grid cell tip 
                    var view = grid.getView();
                    grid.tip = Ext.create('Ext.tip.ToolTip', {
                        target: view.getId(),
                        delegate: view.itemSelector + ' .tip',
                        trackMouse: true,
                        listeners: {
                            beforeshow: function (tip) {
                                //debugger;
                                var tipGridView = tip.target.component;
                                var record = tipGridView.getRecord(tip.triggerElement);
                                var colname = tipGridView.getHeaderCt().getHeaderAtIndex(tip.triggerElement.cellIndex).dataIndex;
                                //tip.update(record.get(colname));

                                var sFileName = record.get("File");
                                var colIndex = tip.triggerElement.cellIndex;
                                if (colname == "View") {
                                    var tipValue = "<div style='font-size: 13px;'><b>点击读取= <span style='color:red;'>" + sFileName + "</span> 的工作表<b></div>";
                                    tip.update(tipValue);
                                }
                                else if (colname == "Update") {
                                    var tipValue = "<div style='font-size: 13px;'><b>点击更新此Excel的表到数据中 <span style='color:red;'></span><b></div>";
                                    tip.update(tipValue);
                                }

                            }
                        }
                    });
                },
                destroy: function (view) {
                    delete view.tip;
                },
                afterrender: {
                    fn: function () {
                        Init_grid_UploadWI();
                    }
                },
                itemcontextmenu: function (grid, record, item, index, e) {
                    //debugger;	
                    if (Ext.getCmp("uploadWI_contextMenu"))
                        Ext.getCmp("uploadWI_contextMenu").destroy();

                    var contextMenu = Ext.create('Ext.menu.Menu', {
                        id: 'uploadWI_contextMenu',
                        width: 150,
                        items: [
                            {
                                text: 'Delete This Excel', iconCls: "iconCut16",
                                handler: function () {
                                    onMenuItemClick_DelExcel(grid, record, item, index, e);
                                }
                            },
                            '-',
                            {
                                text: 'Show Tracking', iconCls: "iconDoc16",
                                handler: function () {
                                    onMenuItemClick_ShowTracking(grid, record, item, index, e);
                                }
                            },
                        ]
                    });
                    e.stopEvent();
                    contextMenu.showAt(e.getXY());
                }
            },
        })

        grid_xlsList.header.titleCmp.textEl.selectable();
        //#endregion

        new Ext.tip.ToolTip({ target: "btn_LoadWIList", trackMouse: true, html: "<div style='font-size: 13px;'>点击获取excel文件</div>" });

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion

//#region Create GUI ---- 2. list work sheet
function CreateGUI_Excel_ListSheet() {
    try {
        //#region CreateGUI_Excel_ListSheet
        function renderWSList(value, metaData, record, rowIndex, columnIndex, store, view) {
            //debugger;
            var para = view.panel.id;// rowIndex + "|" + view.panel.id;
            return "<input type='button' value='" + record.get("View") +
                "'style='font-size:10px;font-weight:bold; width:60px;height:20px; ' onclick='startItemXlsSheet(" + rowIndex + ',' + para + ")'" + ">";  //',' + view.panel.id +
        }

        //#region columns
        var columns_SheetList = [
            { xtype: 'rownumberer', header: "#", width: 40, align: 'left' },
            {
                header: "<div style='font-size: 11px;'>FileName</div>", dataIndex: "File", width: 350, height: 30, menuDisabled: true, editor: {},
            },
            {
                header: "<div style='font-size: 11px;'>WorkSheet</div>", dataIndex: "Sheet", width: 300, height: 30, menuDisabled: true, editor: {},
                renderer: function (value, metaData, record) {
                    //debugger;				
                    if (value) {
                        metaData.style = 'color:#FF6600; font-weight:bolder';
                    }
                    else {
                        metaData.style = 'color:black';
                    }
                    return value;
                },
            },
            {
                header: "<div style='font-size: 11px;'>Line</div>", dataIndex: "Line", width: 100, menuDisabled: true, editor: {},// renderer: renderFormat,
            },
            {
                header: "<div style='font-size: 11px;'>Remark</div>", dataIndex: "Remark", flex: 1, menuDisabled: true, editor: {},// renderer: renderFormat,
            },
            {
                header: "<div style='font-size: 11px;'>Read</div>", dataIndex: "View", width: 100, width: 100, menuDisabled: true, editor: {}, renderer: renderWSList, tdCls: 'tip',
            },
        ];
        //#endregion

        //#region tool bar
        var toolbar_ListSheet = {
            id: 'toolbar_ListSheet',
            xtype: 'toolbar',
            dock: 'top', // bottom, right, left
            items: [

                {
                    id: "btn_Wave1", scale: "medium", text: "<div style='font-weight:bolder; font-size:11px;'><b>Upload This WI</b></div>", xtype: "button", iconCls: "iconCateList16",
                    handler: onToolBtnClick_UploadWI
                },
                '->',
                {
                    id: "btn_Wave2", scale: "medium", xtype: "button", iconCls: "iconExcel16",  // baseCls: 'buttonStd', baseCls效果不好
                    text: "<div style='font-weight:bold; font-size:12px; '>Load WI Excel</div>",
                    handler: onToolBtnClick_UploadWI
                },
            ]
        };
        //#endregion

        var grid_WI_WorkSheet = Ext.create("Ext.grid.Panel", {
            id: "grid_WI_WorkSheet",
            title: "WI worksheet",
            width: '100%',  //forceFit: true,   
            columnLines: true,
            height: Global_Height_Machine,  //autoHeight: true, //height: 250, //
            store: new Ext.data.ArrayStore(),
            //renderTo: "divUploadWI",
            columns: columns_SheetList,
            collapsible: true,
            plugins: [
                { ptype: "cellediting", },
            ],
            dockedItems: [
                //toolbar_ListSheet,
                //toolbar_Bot,
            ],
            viewConfig: {
                stripeRows: false,//在表格中显示斑马线
                enableTextSelection: true
            },
            listeners: {
                render: function (grid) {   //////////////////////////////////////////////显示grid cell tip 
                    var view = grid.getView();
                    grid.tip = Ext.create('Ext.tip.ToolTip', {
                        target: view.getId(),
                        delegate: view.itemSelector + ' .tip',
                        trackMouse: true,
                        listeners: {
                            beforeshow: function (tip) {
                                //debugger;
                                var tipGridView = tip.target.component;
                                var record = tipGridView.getRecord(tip.triggerElement);
                                var colname = tipGridView.getHeaderCt().getHeaderAtIndex(tip.triggerElement.cellIndex).dataIndex;
                                //tip.update(record.get(colname));

                                var sFileName = record.get("Sheet");
                                var colIndex = tip.triggerElement.cellIndex;

                                var tipValue = "<div style='font-size: 13px;'><b>点击读取表=<span style='color:red;'>" + sFileName + "</span> 的内容<b></div>";
                                tip.update(tipValue);
                            }
                        }
                    });
                },
                afterrender: {
                    fn: function () {
                        //Init_grid_UploadWI();
                    }
                },
                itemcontextmenu: function (grid, record, item, index, e) {
                    //debugger;	
                    var contextMenu = Ext.create('Ext.menu.Menu', {
                        width: 150,
                        items: [
                            {
                                text: 'Reset This WorkSheet', iconCls: "iconCut16",
                                handler: function () {
                                    onMenuItemClick_Wave_WS(grid, record, item, index, e);
                                }
                            },
                        ]
                    });
                    e.stopEvent();
                    contextMenu.showAt(e.getXY());
                }
            },
        })

        //grid_WI_WorkSheet.header.titleCmp.textEl.selectable();
        //#endregion
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion

//#region Create GUI ---- 3. read excel content(all parameters) Oven -- Wave
function CreateGUI_Oven_Wave() {
    try {
        //#region CreateGUI_UploadWI
        ///////////////////////////////////////////选择WorkSheet, read sheet 内容
        function renderUpdate_Wave(value, metaData, record, rowIndex, columnIndex, store, view) {
            //debugger;
            var para = view.panel.id;// rowIndex + "|" + view.panel.id;
            return "<input type='button' value='" + record.get("View") +
                "'style='font-size:10px;font-weight:bold; width:60px;height:20px; ' onclick='startItemClickXlsFile(" + rowIndex + ',' + para + ")'" + ">";  //',' + view.panel.id +
        }

        //#region columns
        Ext.tip.QuickTipManager.init();
        var columns_Wave = [
            { xtype: 'rownumberer', header: "#", width: 30, align: 'left' },
            {
                xtype: 'checkcolumn', header: "<div style='font-size: 11px;'>chk</div>", dataIndex: "Check", width: 30, menuDisabled: true, stopSelection: true, sortable: false,
            },
            {
                header: "<div style='font-size: 11px;'>Model</div>", dataIndex: "ModelName", width: 100, height: 30, menuDisabled: true, editor: {},
            },
            {
                header: "<div style='font-size: 11px;'>Program</div>", dataIndex: "ProgName", width: 70, menuDisabled: true, editor: {},// renderer: renderFormat,
            },
            {
                text: "<div style='font-size: 12px;font-weight:bold'>Fluxer</div>", menuDisabled: true,
                columns: [
                {
                    header: "<div style='font-size: 11px;'>Bd_W</div>", dataIndex: "Flux_BdWid", width: 40, menuDisabled: true, editor: {}, hidden: false, tooltip: '1_Board_Width(mm)',
                },

                {
                    id: "col_BdW_x", header: "<div style='font-size: 11px;'>Bd_W_x</div>", dataIndex: "Flux_BdWid_Max", width: 60, menuDisabled: true, editor: {}, hidden: false, 
                },
                {
                    header: "<div style='font-size: 11px;'>Bd_W_n</div>", dataIndex: "Flux_BdWid_Min", width: 60, menuDisabled: true, editor: {}, hidden: false,
                },
                {
                    header: "<div style='font-size: 11px;'>Speed</div>", dataIndex: "Flux_ConvSpd", width: 45, menuDisabled: true, editor: {}, tooltip: '2_Conveyor_speed(m/min)',
                },
                {
                    header: "<div style='font-size: 11px;'>Speed_x</div>", dataIndex: "Flux_ConvSpd_Max", width: 60, menuDisabled: true, editor: {},//renderer: renderColor 
                },
                {
                    header: "<div style='font-size: 11px;'>Speed_n</div>", dataIndex: "Flux_ConvSpd_Min", width: 60, menuDisabled: true, editor: {},//renderer: renderColor 
                },
                {
                    header: "<div style='font-size: 11px;'>Nozzle</div>", dataIndex: "Flux_NozSpd", width: 45, menuDisabled: true, editor: {}, tooltip: '3_Nozzle_Speed(mm/s)',
                },
                {
                    header: "<div style='font-size: 11px;'>Nozzle_x</div>", dataIndex: "Flux_NozSpd_Max", width: 55, menuDisabled: true, editor: {},//renderer: renderColor 
                },
                {
                    header: "<div style='font-size: 11px;'>Nozzle_n</div>", dataIndex: "Flux_NozSpd_Min", width: 55, menuDisabled: true, editor: {},//renderer: renderColor 
                },
                {
                    header: "<div style='font-size: 11px;'>Volumn</div>", dataIndex: "Flux_Volumn", width: 40, menuDisabled: true, editor: {}, tooltip: '4_Flux_Volume(ml/m)',
                },
                {
                    header: "<div style='font-size: 11px;'>Vol_x</div>", dataIndex: "Flux_Volumn_Max", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Vol_n</div>", dataIndex: "Flux_Volumn_Min", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Spray</div>", dataIndex: "Flux_NozSpray", width: 40, menuDisabled: true, editor: {}, tooltip: '5_Nozzle_Spray_Width(mm)',
                },
                {
                    header: "<div style='font-size: 11px;'>Spray_x</div>", dataIndex: "Flux_NozSpray_Max", width: 55, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Spray_n</div>", dataIndex: "Flux_NozSpray_Min", width: 55, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Power</div>", dataIndex: "Flux_Power", width: 45, menuDisabled: true, editor: {}, tooltip: '6_Power(W)',
                },
                {
                    header: "<div style='font-size: 11px;'>Power_x</div>", dataIndex: "Flux_Power_Max", width: 55, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Power_n</div>", dataIndex: "Flux_Power_Min", width: 55, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Pressure</div>", dataIndex: "Flux_Pres", width: 50, menuDisabled: true, editor: {}, tooltip: '7_Pressure(kpa)',
                },
                {
                    header: "<div style='font-size: 11px;'>Pres_x</div>", dataIndex: "Flux_Pres_Max", width: 60, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Pres_n</div>", dataIndex: "Flux_Pres_Min", width: 60, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>BiModel</div>", dataIndex: "Flux_BiModel", width: 40, menuDisabled: true, editor: {}, tooltip: '8_Bi-direction_Model ',
                },
                ]
            },
            {
                text: "<div style='font-size: 12px;font-weight:bold'>Pre-heat</div>", menuDisabled: true,
                columns: [
                {
                    header: "<div style='font-size: 11px;'>Low1</div>", dataIndex: "Heat_Low1", width: 40, menuDisabled: true, editor: {}, tooltip: '9_Lower_p/h1(℃)',
                },
                {
                    header: "<div style='font-size: 11px;'>Low1_x</div>", dataIndex: "Heat_Low1_Max", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Low1_n</div>", dataIndex: "Heat_Low1_Min", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Low2</div>", dataIndex: "Heat_Low2", width: 40, menuDisabled: true, editor: {}, tooltip: '10_Lower_p/h2(℃)',
                },
                {
                    header: "<div style='font-size: 11px;'>Low2_x</div>", dataIndex: "Heat_Low2_Max", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Low2_n</div>", dataIndex: "Heat_Low2_Min", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Low3</div>", dataIndex: "Heat_Low3", width: 40, menuDisabled: true, editor: {}, tooltip: '11_Lower_p/h3(℃)',
                },
                {
                    header: "<div style='font-size: 11px;'>Low3_x</div>", dataIndex: "Heat_Low3_Max", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Low3_n</div>", dataIndex: "Heat_Low3_Min", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Upp1</div>", dataIndex: "Heat_Upp1", width: 40, menuDisabled: true, editor: {}, tooltip: '12_Upper_p/h1(℃)',
                },
                {
                    header: "<div style='font-size: 11px;'>Upp1_x</div>", dataIndex: "Heat_Upp1_Max", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Upp1_n</div>", dataIndex: "Heat_Upp1_Min", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Upp2</div>", dataIndex: "Heat_Upp2", width: 40, menuDisabled: true, editor: {}, tooltip: '13_Upper_p/h2(℃)',
                },
                {
                    header: "<div style='font-size: 11px;'>Upp2_x</div>", dataIndex: "Heat_Upp2_Max", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Upp2_n</div>", dataIndex: "Heat_Upp2_Min", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Upp3</div>", dataIndex: "Heat_Upp3", width: 40, menuDisabled: true, editor: {}, tooltip: '14_Upper_p/h3(℃)',
                },
                {
                    header: "<div style='font-size: 11px;'>Upp3_x</div>", dataIndex: "Heat_Upp3_Max", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Upp3_n</div>", dataIndex: "Heat_Upp3_Min", width: 40, menuDisabled: true, editor: {}, 
                }
                ]
            },
            {
                text: "<div style='font-size: 12px;font-weight:bold'>Solder pot</div>", menuDisabled: true,
                columns: [
                    {
                        header: "<div style='font-size: 11px;'>TEMP</div>", dataIndex: "SP_Temp", width: 50, menuDisabled: true, editor: {}, tooltip: '15_Solder_TEMP(℃',
                    },
                    {
                        header: "<div style='font-size: 11px;'>TEMP_x</div>", dataIndex: "SP_Temp_Max", width: 50, menuDisabled: true, editor: {},
                    },
                    {
                        header: "<div style='font-size: 11px;'>TEMP_n</div>", dataIndex: "SP_Temp_Min", width: 60, menuDisabled: true, editor: {},
                    },
                    {
                        header: "<div style='font-size: 11px;'>Contour</div>", dataIndex: "SP_ConWave", width: 50, menuDisabled: true, editor: {}, tooltip: '16_Contour_WAVE(rpm)',
                    },
                    {
                        header: "<div style='font-size: 10px;'>Contour_x</div>", dataIndex: "SP_ConWave_Max", width: 65, menuDisabled: true, editor: {},
                    },
                    {
                        header: "<div style='font-size: 10px;'>Contour_n</div>", dataIndex: "SP_ConWave_Min", width: 65, menuDisabled: true, editor: {},
                    },
                ]
            },
            {
                text: "<div style='font-size: 12px;font-weight:bold'>Converyor</div>", menuDisabled: true, tooltip: 'Converyor Parameter',
                columns: [
                        {
                            header: "<div style='font-size: 11px;'>Clear</div>", dataIndex: "SP_LdClear", width: 40, menuDisabled: true, editor: {}, tooltip: '17_Lead_Clearance(mm)',
                        },
                        {
                            header: "<div style='font-size: 11px;'>Clear_x</div>", dataIndex: "SP_LdClear_Max", width: 55, menuDisabled: true, editor: {},
                        },
                        {
                            header: "<div style='font-size: 11px;'>Clear_n</div>", dataIndex: "SP_LdClear_Min", width: 55, menuDisabled: true, editor: {}, 
                        },
                        {
                            header: "<div style='font-size: 11px;'>Speed</div>", dataIndex: "Conv_Speed", width: 40, menuDisabled: true, editor: {}, tooltip: '18_Speed(m/min)',
                        },
                        {
                            header: "<div style='font-size: 11px;'>Speed_x</div>", dataIndex: "Conv_Speed_Max", width: 60, menuDisabled: true, editor: {},
                        },
                        {
                            header: "<div style='font-size: 11px;'>Speed_n</div>", dataIndex: "Conv_Speed_Min", width: 60, menuDisabled: true, editor: {},
                        },
                        {
                            header: "<div style='font-size: 11px;'>Width</div>", dataIndex: "Conv_Width", width: 45, menuDisabled: true, editor: {}, tooltip: '19_Width(mm)',
                        },
                        {
                            header: "<div style='font-size: 11px;'>Width_x</div>", dataIndex: "Conv_Width_Max", width: 65, menuDisabled: true, editor: {},
                        }, 
                        {
                            header: "<div style='font-size: 11px;'>Width_n</div>", dataIndex: "Conv_Width_Min", width: 65, menuDisabled: true, editor: {},
                        },
                ]
            },
            {
                text: "<div style='font-size: 12px;font-weight:bold'>Other</div>", menuDisabled: true,
                columns: [
                        {
                            header: "<div style='font-size: 11px;'>Ni</div>", dataIndex: "Other_Ni", width: 30, menuDisabled: true, editor: {}, tooltip: 'Nitrogen',
                        },
                        {
                            header: "<div style='font-size: 11px;'>Remark</div>", dataIndex: "Remark", width: 50, menuDisabled: true, editor: {},
                        },

                ]
            },
            {
                header: "<div style='font-size: 11px;'>Comments</div>", dataIndex: "Comments", width: 120, menuDisabled: true, editor: {},
            },
        ];
        //#endregion

        //#region tool bar
        var txtField_Wave = new Ext.form.TextField({
            id: 'txtField_Wave', fieldLabel: "Sheet:", labelWidth: 40, width:100, anchor: '98%',  //必须要有，否则不能无效		
            grow: true, emptyText: "?", value: "", width: 200, selectOnFocus: false, hidden: true,
        });

        var toolbar_Wave = {
            id: 'toolbar_Wave',
            xtype: 'toolbar',
            dock: 'top', // bottom, right, left
            items: [
                { //UploadWI_Method.js=>Wave_Upload_Paras();
                    id: "btn_UpdateParas_Wave", scale: "medium", text: "<div style='font-weight:bolder; font-size:11px;'><b>Upload Parameter</b></div>", xtype: "button",
                    iconCls: "iconUpload20", handler: onToolBtnClick_UploadWI
                },
                txtField_Wave,
                '->',
                {
                    id: "btn_Wave_ShowAll", scale: "medium", xtype: "button", iconCls: "iconColumn32",  // baseCls: 'buttonStd', baseCls效果不好
                    text: "<div style='font-weight:bold; font-size:12px; '>Show All</div>",
                    handler: onToolBtnClick_UploadWI   //Wave_ShowAllParameters()
                },
            ]
        };

        var toolbar_Bot_Wave = {
            id: 'bot_gridWI_1',
            xtype: 'toolbar',
            dock: 'bottom',
            hidden: false,
            items: [
                {   //wiFile_LoadList()
                    id: "btn_LoadWIList", scale: "medium", xtype: "button", iconCls: "iconExcel16",  // baseCls: 'buttonStd', baseCls效果不好
                    text: "<div style='font-weight:bold; font-size:12px; '>Load WI Excel</div>",
                    handler: onToolBtnClick_UploadWI
                },
                //'->',
                //{
                //    id: "btn_SAP", scale: "medium", xtype: "button", iconCls: "iconSAP24",
                //    //text: "<div style='font-weight:bold; font-size:12px; color:#FF6600;'>Start</div>", //颜色不一样
                //    text: "<div style='font-weight:bold; font-size:13px;'>SAP</div>",
                //    handler: onToolBtnClick_UploadWI
                //},
            ]
        };
        //#endregion

        var grid_WI_Wave = Ext.create("Ext.grid.Panel", {
            id: "grid_WI_Wave",
            title: "Wave Parameter",
            width: '100%',  //forceFit: true,   
            columnLines: true,
            height: Global_Height_Machine,  //autoHeight: true, //height: 250, //
            store: new Ext.data.ArrayStore(),
            //renderTo: "divUploadWI",
            columns: columns_Wave,
            collapsible: true,
            plugins: [
                { ptype: "cellediting", },
            ],
            dockedItems: [
                toolbar_Wave,
                //toolbar_Bot,
            ],
            viewConfig: {
                stripeRows: false,//在表格中显示斑马线
                enableTextSelection: true
            },
            listeners: {
                afterrender: {
                    fn: function () {
                        Init_grid_grid_WI_Wave();
                    }
                },
                headerclick: function (ct, column, e, t, eOpts) {//选择是否全选
                    toolFun_Header(column, t);
                },

            },
        })
        //grid_WI_Wave.header.titleCmp.textEl.selectable();
        //#endregion

        new Ext.tip.ToolTip({ target: "btn_LoadWIList", trackMouse: true, html: "<div style='font-size: 13px;'>点击获取excel文件</div>" });

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion

//#region Crete GUI ----- 4. read excel content -- all
function CreateGUI_LoadConent() {
    try {

        Ext.create('Ext.tab.Panel', {
            id: 'TabAll_WI',
            title: 'Read Parameters and upload',
            renderTo: 'divUploadWIContent',
            width: '100%',
            height: 350,
            cls: 'mytab',
            activeTab: 0,
            items: [
                {
                    id: 'tabSheets',
                    title: '<p><div style="font-size: 14px; color:black">Excel Sheet List</div></p>',  
                    iconCls: 'iconSheets_24',
                    items: [Ext.getCmp("grid_WI_WorkSheet")],
                },
                {
                    id: 'tabWave',
                    title: '<p><div style="font-size: 14px; color:black">Wave WI Content</div></p>',  
                    iconCls: 'iconElectra32',
                    items: [Ext.getCmp("grid_WI_Wave")], 
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
        Ext.getCmp("TabAll_WI").header.titleCmp.textEl.selectable();

        Ext.getCmp("TabAll_WI").child('#tabReflow').tab.hide();
        Ext.getCmp("TabAll_WI").child('#tabDek').tab.hide();
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endregion

function onToolBtnClick_UploadWI(item)
{
    try {
        var sText = item.text;
        var sID = item.id;
        if (sID == "btn_AddFile") {
            wiFile_Upload();
        }
        else if (sID == "btn_LoadWIList") {
            wiFile_LoadList();
        }
        else if (sID == "Cmbo_Machine") {

        }
        else if (sID == "btn_UpdateParas_Wave") {
            Wave_Upload_Paras2();
        }
        else if (sID == "btn_Wave_ShowAll") {
            Wave_ShowAllParameters();
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function onMenuItemClick_Wave_WS(grid, record, item, index, e) {
    debugger;	
    try	
    {	
        var WS = record.get("Sheet");
        var Line = record.get("Line");

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

        //#region Part_Desc


        //#endregion

    }
    catch (err) {	
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });	
    }	
}

//#region initilize grid
function Init_grid_UploadWI()
{
    try {

        //#region 绑定Machine	
        var list_Option = ['Wave', 'Reflow','Dek'];
        for (var i = 0, c = list_Option.length; i < c; i++) {
            list_Option[i] = [list_Option[i]];
        }

        var storeCmb = new Ext.data.ArrayStore({
            data: list_Option,
            fields: ['value']
        });

        if (list_Option.length > 0) {
            Ext.getCmp("Cmbo_Machine").bindStore(storeCmb);
            Ext.getCmp("Cmbo_Machine").setValue(list_Option[0]);
        }
        //#endregion	

        var user = glbLogin_getCookie("userEN")
        if (user == "28036240" || user == "28036000") {
            Ext.getCmp("btn_WITracking").show();
        }
        else {
            Ext.getCmp("btn_WITracking").hide();
        }


    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function Init_grid_grid_WI_Wave()
{
    try {
        var grid = Ext.getCmp("grid_WI_Wave");
        //var btn = Ext.getCmp("btn_Wave_ShowAll");

        //var columns = grid.getColumns();
        var columns = grid.getColumnManager().getColumns();
        for (var i = 0; i < columns.length; i++)
        {
           var column = columns[i];
           var di = column.dataIndex;
           if (di.indexOf("_M") > 0) {
               column.hide();
           }
        }

        //if (column.isVisible()) {
        //    column.hide();
        //    btn.setText('Show All');
        //}
        //else {
        //    column.show();
        //    btn.setText('Hide All');
        //}
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function toolFun_Header(column, t) {
    try {
        var text = column.text;
        var dataIndex = column.dataIndex;
        if (dataIndex == "Check") {
            var Checked_All = false;
            var store = Ext.getCmp("grid_WI_Wave").getStore();
            if (store) {
                if (store.data.length > 0) {

                    var record = store.getAt(0);
                    var checked = record.get("Check");
                    if (checked == true) {
                        Checked_All = true;
                    }

                    for (var i = 0; i < store.data.length; i++) {
                        var record = store.getAt(i);
                        record.set("Check", !Checked_All);
                        record.commit();
                    }
                }
            }
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#endregion











