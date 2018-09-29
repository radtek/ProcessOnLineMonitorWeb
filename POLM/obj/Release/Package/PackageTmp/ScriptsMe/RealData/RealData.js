///<reference path="~/Scripts/ExtJS/ext-all.js" />



Ext.onReady(function () {
    try {

        CreateGUI_RealTime();

    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
});


function CreateGUI_RealTime()
{
    try {
        //#region real time GUI

        //#region columns obsolete
        var columns_RealTime_obsolete = [
            { xtype: 'rownumberer', header: "#", width: 40, align: 'left' },
            {
                header: "<div style='font-size: 11px;'>L_Preheater1</div>", dataIndex: "L_PRETER1", width: 80, height: 30, menuDisabled: true, editor: {},
            },
            {
                header: "<div style='font-size: 11px;'>L_Preheater2</div>", dataIndex: "L_PRETER2", width: 80, menuDisabled: true, editor: {}, //renderer: renderFormat,
            },
            {
                header: "<div style='font-size: 11px;'>L_Preheater3</div>", dataIndex: "L_PRETER3", width: 80, menuDisabled: true, editor: {}, hidden: true,
            },
            {
                header: "<div style='font-size: 11px;'>U_Preheater1</div>", dataIndex: "U_PRETER1", width: 80, menuDisabled: true, editor: {},//renderer: renderColor 
            },
            {
                header: "<div style='font-size: 11px;'>U_Preheater2</div>", dataIndex: "U_PRETER2", width: 80, menuDisabled: true, editor: {},   
            },
            {
                header: "<div style='font-size: 11px;'>solder pot temp</div>", dataIndex: "SOLDER_POT_T", width: 80, menuDisabled: true, editor: {},  //renderer: renderUpdate, tdCls: 'tip',
            },
            {
                header: "<div style='font-size: 11px;'>Speed</div>", dataIndex: "CONV_SPEED", width: 50, menuDisabled: true, editor: {}, //renderer: renderUpdate, tdCls: 'tip',
            },
            {
                header: "<div style='font-size: 11px;'>Width</div>", dataIndex: "CONV_WIDTH", width: 60, menuDisabled: true, editor: {}, //renderer: renderUpdate, tdCls: 'tip',
            },
            {
                header: "<div style='font-size: 11px;'>Lead Clearance</div>", dataIndex: "LEAD_CLEAR", width: 80, menuDisabled: true, editor: {}, //renderer: renderUpdate, tdCls: 'tip',
            },
            {
                header: "<div style='font-size: 11px;'>Flux Flowrate</div>", dataIndex: "FLUX_FLOWRATE", width: 80, menuDisabled: true, editor: {}, //renderer: renderUpdate, tdCls: 'tip',
            },
            {
                header: "<div style='font-size: 11px;'>Program</div>", dataIndex: "PROG_NAME", width: 120, menuDisabled: true, editor: {}, //renderer: renderUpdate, tdCls: 'tip',
            },
            {
                header: "<div style='font-size: 11px;'>COMMENTS</div>", dataIndex: "COMMENTS", width: 100, menuDisabled: true, editor: {}, //renderer: renderUpdate, tdCls: 'tip',
            },

            {
                header: "<div style='font-size: 11px;'>Log Time</div>", dataIndex: "LOGTIME", width: 140, menuDisabled: true, editor: {}, //renderer: renderUpdate, tdCls: 'tip',
            },

        ];
        //#endregion
        //#region columns
        var columns_RealTime = [
            { xtype: 'rownumberer', header: "#", width: 30, align: 'left' },
            //{
            //    xtype: 'checkcolumn', header: "<div style='font-size: 11px;'>chk</div>", dataIndex: "Check", width: 30, menuDisabled: true, stopSelection: true, sortable: false,
            //},
            {
                header: "<div style='font-size: 11px;'>Machine</div>", dataIndex: "Machine", width: 100, height: 30, menuDisabled: true, editor: {},
            },
            {
                header: "<div style='font-size: 11px;'>Program</div>", dataIndex: "ProgName", width: 80, menuDisabled: true, editor: {},// renderer: renderFormat,
            },
            {
                text: "<div style='font-size: 12px;font-weight:bold'>Fluxer</div>", menuDisabled: true,
                columns: [
                {
                    header: "<div style='font-size: 11px;'>Bd_W</div>", dataIndex: "Flux_BdWid", width: 40, menuDisabled: true, editor: {}, hidden: false,
                },
                {
                    header: "<div style='font-size: 11px;'>Speed</div>", dataIndex: "Flux_ConvSpd", width: 45, menuDisabled: true, editor: {},//renderer: renderColor 
                },
                {
                    header: "<div style='font-size: 11px;'>Nozzle</div>", dataIndex: "Flux_NozSpd", width: 45, menuDisabled: true, editor: {},//renderer: renderColor 
                },
                {
                    header: "<div style='font-size: 11px;'>Volumn</div>", dataIndex: "Flux_Volumn", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Spray</div>", dataIndex: "Flux_NozSpray", width: 40, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Power</div>", dataIndex: "Flux_Power", width: 45, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Pressure</div>", dataIndex: "Flux_Pres", width: 55, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>BiModel</div>", dataIndex: "Flux_BiModel", width: 40, menuDisabled: true, editor: {},
                },
                ]
            },
            {
                text: "<div style='font-size: 12px;font-weight:bold'>Pre-heat</div>", menuDisabled: true,
                columns: [
                {
                    header: "<div style='font-size: 11px;'>Low1</div>", dataIndex: "Heat_Low1", width: 50, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Low2</div>", dataIndex: "Heat_Low2", width: 50, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Low3</div>", dataIndex: "Heat_Low3", width: 50, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Upp1</div>", dataIndex: "Heat_Upp1", width: 50, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Upp2</div>", dataIndex: "Heat_Upp2", width: 50, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Upp3</div>", dataIndex: "Heat_Upp3", width: 50, menuDisabled: true, editor: {},
                },
                ]
            },
            {
                text: "<div style='font-size: 12px;font-weight:bold'>Solder pot</div>", menuDisabled: true,
                columns: [
                    {
                        header: "<div style='font-size: 11px;'>TEMP</div>", dataIndex: "SP_Temp", width: 50, menuDisabled: true, editor: {},
                    },
                    {
                        header: "<div style='font-size: 11px;'>Contour</div>", dataIndex: "SP_ConWave", width: 60, menuDisabled: true, editor: {},
                    },
                ]
            },
            {
                text: "<div style='font-size: 12px;font-weight:bold'>Converyor</div>", menuDisabled: true,
                columns: [
                        {
                            header: "<div style='font-size: 11px;'>Clear</div>", dataIndex: "SP_LdClear", width: 45, menuDisabled: true, editor: {},
                        },
                        {
                            header: "<div style='font-size: 11px;'>Speed</div>", dataIndex: "Conv_Speed", width: 45, menuDisabled: true, editor: {},
                        },
                        {
                            header: "<div style='font-size: 11px;'>Width</div>", dataIndex: "Conv_Width", width: 45, menuDisabled: true, editor: {},
                        },
                ]
            },
            //{
            //    text: "<div style='font-size: 12px;font-weight:bold'>Other</div>", menuDisabled: true,
            //    columns: [
            //            {
            //                header: "<div style='font-size: 11px;'>Ni</div>", dataIndex: "Other_Ni", width: 30, menuDisabled: true, editor: {},
            //            },
            //            {
            //                header: "<div style='font-size: 11px;'>Remark</div>", dataIndex: "Remark", width: 50, menuDisabled: true, editor: {},
            //            },
            //    ]
            //},
            //{
            //    header: "<div style='font-size: 11px;'>Comments</div>", dataIndex: "Comments", width: 120, menuDisabled: true, editor: {},
            //},
        ];
        //#endregion


        //#region Date hour select
        var date = new Date();
        date.setDate(date.getDate() - 0);

        var Date_start = Ext.create('Ext.form.field.Date', {
            id: 'dtPickerFrom',
            fieldLabel: 'DAY Select',
            labelAlign: 'right',
            cls: 'iconDate24',
            maxValue: new Date(),
            width: 200, labelWidth: 90,
            format: 'm/d/Y',
            value: date, //new Date(),	
            //renderTo: 'tdQueryDate',
            listeners: {
                //change: function (thisD, newValue, oldValue, eOpts) {
                //    debugger;
                //    Date_end.setMinValue(newValue);
                //}
            }
        });
        //#endregion

        //#region too bar
        var toolbar_Bot_RealData = {
            id: 'toolbar_Bot_RealData',
            xtype: 'toolbar',
            dock: 'bottom',
            hidden: false,
            items: [
                {
                    //#region Cmbo_Filter_Wave
                    id: "txt_QueryProgram", xtype: "textfield", fieldLabel: "Program Query:", labelWidth: 90, width: 250, emptyText: 'XXX',			
                    listeners:			
                       {
                           scope: this,
                           change: function (field, newValue, oldValue, eOpts) {
                               debugger;
                               var grid = Ext.getCmp('grid_RealTime');
                               grid.store.clearFilter();

                               if (newValue) {
                                   var matcher = new RegExp(Ext.String.escapeRegex(newValue), "i");
                                   grid.store.filter({
                                       filterFn: function (record) {
                                           return matcher.test(record.get('ProgName'))
                                       }
                                   });
                               }
                           }
                       }
                   //#endregion
                },
                //{
                //    text: "Output", id: "rep", xtype: "button", iconCls: "iconExcel16",
                //    handler: onToolBtnClick,
                //},
            ]
        };

        //#endregion

        var grid_RealTime = Ext.create("Ext.grid.Panel", {
            id: "grid_RealTime",
            title: "View Real Time Data",
            width: '100%',  //forceFit: true,   
            columnLines: true,
            height: 370,  //autoHeight: true, //height: 250, //
            store: new Ext.data.ArrayStore(),
            renderTo: "divRealData",
            columns: columns_RealTime,
            collapsible: true,
            plugins: [
                { ptype: "cellediting", },
                { ptype: 'gridexporter' }
            ],
            tbar: [
                {
                    id: "btn_GetList", scale: "medium", text: "<div style='font-weight:bolder; font-size:11px;'><b>View Data</b></div>", xtype: "button",
                    iconCls: "iconRefresh16", handler: onToolBtnClick_RT //Home_GetRealTimeData
                },
                '->',
                Date_start,
                '-',
                {
                    id: 'timeFrom', xtype: 'timefield', fieldLabel: 'Time from', width: 150, labelWidth: 65,
                    format: 'H:i', minValue: '0:00', maxValue: "23:00", increment: 60, anchor: '100%',
                    value: Ext.Date.format(new Date(), 'H'),
                    listeners: {
                        change: function (thisD, newValue, oldValue, eOpts) {
                            Ext.getCmp("timeTo").setMinValue(newValue);
                            //newValue.setHours(newValue.getHours() + 1);
                            //Ext.getCmp("timeTo").setValue(newValue);
                        }
                    }
                },
                {
                    id: 'timeTo', xtype: 'timefield', fieldLabel: 'to', width: 110, labelWidth: 18, padding: '0 2 0 5',
                    format: 'H:i', minValue: '0:00', maxValue: "23:00", increment: 60, anchor: '100%',
                    value: Ext.Date.format(new Date(), 'H'),
                    listeners: {
                        afterrender: function (thisTimtTo, eOpts ) {
                            var dateC = new Date();
                            dateC.setHours(dateC.getHours() + 1);
                            var timeToValue = Ext.Date.format(dateC, 'H');
                            thisTimtTo.setMinValue(timeToValue);
                            thisTimtTo.setValue(timeToValue);
                        }
                    },
                },

            ],
            dockedItems: [
                toolbar_Bot_RealData,
            ],
            viewConfig: {
                stripeRows: false,//在表格中显示斑马线
                enableTextSelection: true
            },
            listeners: {
                //celldblclick: GridCellDoubleClick,// function (table, td, cellIndex, record, tr, rowIndex, e, eOpts) {},
                //render: function (grid) {   //////////////////////////////////////////////显示grid cell tip 
                //    var view = grid.getView();
                //    grid.tip = Ext.create('Ext.tip.ToolTip', {
                //        target: view.getId(),
                //        delegate: view.itemSelector + ' .tip',
                //        trackMouse: true,
                //        listeners: {
                //            beforeshow: function (tip) {
                //                //debugger;
                //                var tipGridView = tip.target.component;
                //                var record = tipGridView.getRecord(tip.triggerElement);
                //                var colname = tipGridView.getHeaderCt().getHeaderAtIndex(tip.triggerElement.cellIndex).dataIndex;
                //                if (colname == "Train") {
                //                    var sIntrod = record.get("Train");
                //                    if (sIntrod != "") {
                //                        var tipValue = "<div style='font-size: 11px;'>右键点击->查看Guide,显示介绍:<span style='color:red;'>" + sIntrod + "</span></div>";
                //                        tip.update(tipValue);
                //                    }
                //                    else {
                //                        var tipValue = "<div>暂时无此内容介绍</div>";
                //                        tip.update(tipValue);
                //                    }
                //                }
                //                else if (colname == "View") {
                //                    var Link = record.get("Link");
                //                    if (Link != "") {
                //                        var tipValue = "<div style='font-size: 11px;'>点击此,打开网页:<span style='color:red;'>" + Link + "</span></div>";
                //                        tip.update(tipValue);
                //                    }
                //                    else {
                //                        var tipValue = "<div>暂时无此网页</div>";
                //                        tip.update(tipValue);
                //                    }
                //                }
                //            }
                //        }
                //    });
                //},
                destroy: function (view) {
                    delete view.tip;
                },
                afterrender: {
                    fn: function () {
                        InitilizeValue_RealData();
                    }
                },

            },
        })

        grid_RealTime.header.titleCmp.textEl.selectable();

        //#endregion

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}


function onToolBtnClick_RT(item) {
    try {
        var sText = item.text;
        var sID = item.id;
        if (sID == "btn_GetList") {   
            Home_GetRealTimeData();  
        }
    }
    catch (err) {
            Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
        }
}



Ext.define('Wave_RT', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'L_PRETER1', type: 'string' },
        { name: 'L_PRETER2', type: 'string' },
        { name: 'L_PRETER3', type: 'string' },
        { name: 'U_PRETER1', type: 'string' },
        { name: 'L_PRETER2', type: 'string' },
        { name: 'SOLDER_POT_T', type: 'string' },
        { name: 'CONV_SPEED', type: 'string' },
        { name: 'CONV_WIDTH', type: 'string' },
        { name: 'LEAD_CLEAR', type: 'string' },
        { name: 'FLUX_FLOWRATE', type: 'string' },
        { name: 'PROG_NAME', type: 'string' },
        { name: 'LOGTIME', type: 'string' },
        { name: 'COMMENTS', type: 'string' },
    ]
});

function InitilizeValue_RealData()
{

}









