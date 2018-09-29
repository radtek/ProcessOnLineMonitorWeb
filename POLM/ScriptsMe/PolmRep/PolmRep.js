///<reference path="~/Scripts/ExtJS/ext-all.js" />
/*
POLM 的 Chart Report

*/
Ext.onReady(function () {
    try {
        Create_SingalR();
        /*
        //POLM的数据部分的GUI
        //包含子功能 （使用 contextMenu <itemcontextmenu>） 
        1. 手工 email 到 process owner
        2. 设置 tempory WorkI
        3. link 到 Config页面
        */
        CreateGUI_PolmRepData();

        //POLM的Chart部分的GUI
        CreateGUI_PolmRepChart();

        //AdjustFormHeight_Report();
    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
});


/*
afterrender: 1s过后，afterrender自动执行PolmRep_ReviewParas
*/
function CreateGUI_PolmRepChart()
{
    try {
        var store_Sample = Ext.create('Ext.data.Store', {
            fields: [{
                name: 'Time',
                //type: 'date',
                //dateFormat: 'Y-m-d'
            }, {
                name: 'Line1',
                type: 'int',
            }, {
                name: 'Line2',
                type: 'int',
            },{
                name: 'Line3',
                type: 'int',
            }, {
                name: 'data1',
                type: 'int',
            }],
            data: [{
                "Time": "00:30",
                "data1": 10,
                "Line1": 30,
                "Line2": 60,
                "Line3": 90,
            }, {
                "Time": "00:40",
                "data1": 10,
                "Line1": 30,
                "Line2": 60,
                "Line3": 90,
            }, {
                "Time": "00:45",
                "data1": 10,
                "Line1": 30,
                "Line2": 60,
                "Line3": 90,
            }, {
                "Time": "00:50",
                "data1": 10,
                "Line1": 25,
                "Line2": 60,
                "Line3": 90,
            }, {
                "Time": "00:55",
                "data1": 10,
                "Line1": 30,
                "Line2": 60,
                "Line3": 90,
            }, {
                "Time": "01:00",
                "data1": 10,
                "Line1": 30,
                "Line2": 60,
                "Line3": 90,
            }, {
                "Time": "01:05",
                "data1": 10,
                "Line1": 30,
                "Line2": 60,
                "Line3": 90,
            }, {
                "Time": "01:10",
                "data1": 10,
                "Line1": 30,
                "Line2": 55,
                "Line3": 90,
            }, {
                "Time": "01:15",
                "data1": 10,
                "Line1": 30,
                "Line2": 60,
                "Line3": 90,
            }, {
                "Time": "01:20",
                "data1": 10,
                "Line1": 30,
                "Line2": 60,
                "Line3": 90,
            }, {
                "Time": "01:25",
                "data1": 10,
                "Line1": 30,
                "Line2": 60,
                "Line3": 90,
            }, {
                "Time": "01:30",
                "data1": 10,
                "Line1": 25,
                "Line2": 60,
                "Line3": 85,
            }]
        });

        if (Ext.getCmp("panel_Chart"))
        {
            Ext.getCmp("panel_Chart").destroy();
        }

        Ext.create('Ext.panel.Panel', {
            id: 'panel_Chart',
            title: 'Process On-Line Monitor -- Wave',
            width: '100%',
            renderTo: "divChartOV",// Ext.getBody(),
            //tbar: [
            //    {
            //        id: "btn_CTN", scale: "medium", text: "<div style='font-weight:normal; font-size:11px;'>CTN</div>", xtype: "button",
            //        iconCls: "iconOdcDb16", //handler: onToolBtnClick
            //    },
            //],
            items: [{
                id: 'ChartRepWave',
                xtype: 'cartesian',
                width: '100%',
                height: 250,
                store: new Ext.data.ArrayStore(), //store_Sample,//
                plugins: {
                    chartitemevents: {
                        moveEvents: true
                    }
                },
                captions: {
                    title: {
                        text: 'Cell 1 Wave Parameter On-Line Monitor',
                        style: {
                            fontSize: '18px',
                            fontWeight: 'bold',
                            fontFamily: 'Verdana, Aria, sans-serif'
                        },
                    },
                    //subtitle: {
                    //    text: 'from 2007 to 2017'
                    //},
                    //credits: {
                    //    text: 'Source: bls.gov'
                    //},
                },
                //sprites: [{ //任意位置添加图片
                //    type: 'text',
                //    text: 'Wave Parameter On-Line Monitor',
                //    font: '22px Helvetica',
                //    width: 100,
                //    height: 30,
                //    x: 40, // the sprite x position
                //    y: 10  // the sprite y position
                //}],
                insetPadding: {
                    top: 0,
                    left: 10,
                    right: 20,
                    bottom: 10
                },
                innerPadding: {
                    left: 15,
                    right: 10
                },
                //legend: {
                //    docked: 'right'
                //},
                axes: [
                {
                    id: 'axisLabel',
                    type: 'numeric',
                    fields: ['Line1', 'Line2', 'Line3', 'Line4'],
                    //fields: ['Remark1', 'Remark2', 'Remark3', 'Remark4'],
                    position: 'left',
                    grid: false,
                    //title: "Wave Line",
                    titleMargin: 20,
                    label: {
                        fontSize: 14,
                        fillStyle: 'blue',
                        fontWeight: 'bold',
                    },
                    minimum: 0,
                    majorTickSteps: 10,
                    maximum: 100,
                    renderer: function (axis, label, layoutContext, lastLabel)
                    {
                        return onAxisLabelRender(axis, label, layoutContext);
                    }
                },
                {
                    type: 'category',
                    fields: 'Time',
                    //title: 'Number of Hours',
                    title: {
                        text: 'Number of Hours',
                        style: {
                            fontSize: '18px',
                            fontWeight: 'bold',
                            fontFamily: 'Verdana, Aria, sans-serif'
                        },
                    },
                    position: 'bottom',
                    //dateFormat: 'd',
                    grid: false,
                    label: {
                        rotate: {
                            degrees: 0
                        },
                        fontSize: 10,
                        fillStyle: 'black',
                        fontWeight: 'bold',
                    },
                }
                ],
                series: [
                {
                    //#region line 1  ==> show line 3
                    id: 'srs1',
                    type: 'line',
                    xField: 'Time',
                    yField: 'Line1',
                    style: {
                        lineWidth: 2,
                        stroke: "black",
                    },
                    marker: {
                        radius: 8,
                        lineWidth: 2,
                        fillStyle: 'rgb(178,210,51)'
                    },
                    label: {
                        field: 'Remark1',
                        display: 'over',
                        hidden: true,
                        renderer: function (text) {
                            if (text.indexOf("Pass") > 0)  { // "30") {
                                return "pass";
                            }
                            else
                                return "fail";
                        }
                    },
                    highlight: {
                        fillStyle: '#FF9900',
                        radius: 10,
                        lineWidth: 2,
                        strokeStyle: '#FF9900'
                    },
                    tooltip: {
                        trackMouse: true,
                        style: 'background: #fff',
                        showDelay: 0,
                        dismissDelay: 0,
                        hideDelay: 0,
                        renderer: function (toolTip, record, ctx) {
                            var toolVal = record.get('Remark1')
                            toolTip.setHtml(toolVal); //record.get('name') + ': ' + record.get('data1') + ' views');
                        }
                    },
                    renderer: function (sprite, config, rendererData, index) {
                        //#region renderer
                        var store = rendererData.store;
                        if (store)
                        {
                            var storeItems = store.getData().items;
                            var changes = {};

                            if (storeItems.length > 0)
                            {
                                var currentRecord = storeItems[index];
                                if (currentRecord)
                                {
                                    switch (config.type)
                                    {
                                        case 'marker':
                                            changes.strokeStyle = "rgb(178,210,51)";
                                            changes.fillStyle = "rgb(178,210,51)";
                                            var line3Val = currentRecord.data['Remark1'];
                                            if (line3Val.indexOf("Fail") > 0)
                                            {
                                                changes.strokeStyle = "red";
                                                changes.fillStyle = "red";
                                            }
                                            break;
                                    }
                                }
                            }
                            return changes;
                        }
                        //#endregion
                    },
                    listeners: {//itemdblclick ( series, item, event, eOpts ) 
                        itemdblclick: function(series, item, event, eOpts) {
                            //#region markers
                            if (item.category == "markers") {
                                MarkerClick_GetDetailedData(item);
                            }
                            //#endregion
                        }
                    }
                    //#endregion
                },
                {
                    //#region line 2  ==> show line 6
                    id: 'srs1',
                    type: 'line',
                    xField: 'Time',
                    yField: 'Line2',
                    style: {
                        lineWidth: 2,
                        stroke: "black",
                    },
                    marker: {
                        radius: 8,
                        lineWidth: 2,
                        fillStyle: 'rgb(178,210,51)'
                    },
                    label: {
                        field: 'Remark2',
                        display: 'over',
                        hidden: true,
                        renderer: function (text)
                        {
                            if (text.indexOf("Pass") > 0)
                            { 
                                return "pass";
                            }
                            else
                                return "fail";
                        }
                    },
                    highlight: {
                        fillStyle: '#FF9900',
                        radius: 10,
                        lineWidth: 2,
                        strokeStyle: '#FF9900'
                    },
                    tooltip: {
                        trackMouse: true,
                        style: 'background: #fff',
                        showDelay: 0,
                        dismissDelay: 0,
                        hideDelay: 0,
                        renderer: function (toolTip, record, ctx)
                        {
                            var toolVal = record.get('Remark2')
                            toolTip.setHtml(toolVal); //record.get('name') + ': ' + record.get('data1') + ' views');
                        }
                    },
                    renderer: function (sprite, config, rendererData, index)
                    {
                        //#region renderer
                        var store = rendererData.store,
                            storeItems = store.getData().items,
                            currentRecord = storeItems[index],
                            changes = {};
                        if (currentRecord)
                        {
                            switch (config.type)
                            {
                                case 'marker':
                                    changes.strokeStyle = "rgb(178,210,51)";
                                    changes.fillStyle = "rgb(178,210,51)";
                                    var line3Val = currentRecord.data['Remark2'];
                                    if (line3Val.indexOf("Fail") > 0)
                                    {
                                        changes.strokeStyle = "red";
                                        changes.fillStyle = "red";
                                    }
                                    break;
                            }
                        }
                        return changes;
                        //#endregion
                    },
                    listeners: {//itemdblclick ( series, item, event, eOpts ) 
                        itemdblclick: function (series, item, event, eOpts)
                        {
                            //#region markers
                            if (item.category == "markers")
                            {
                                MarkerClick_GetDetailedData(item);
                            }
                            //#endregion
                        }
                    }
                    //#endregion
                },
                {
                    //#region line 3  ==> show line 7
                    id: 'srs3',
                    hidden: true,
                    type: 'line',
                    xField: 'Time',
                    yField: 'Line3',
                    style: {
                        lineWidth: 2,
                        stroke: "black",
                    },
                    marker: {
                        radius: 8,
                        lineWidth: 2,
                        fillStyle: 'rgb(178,210,51)'
                    },
                    label: {
                        field: 'Remark3',
                        display: 'over',
                        hidden: true,
                        renderer: function (text) {
                            if (text.indexOf("Pass") > 0)
                            {
                                return "pass";
                            }
                            else
                                return "fail";
                        }
                    },
                    highlight: {
                        fillStyle: '#FF9900',
                        radius: 10,
                        lineWidth: 2,
                        strokeStyle: '#FF9900'
                    },
                    tooltip: {
                        trackMouse: true,
                        style: 'background: #fff',
                        showDelay: 0,
                        dismissDelay: 0,
                        hideDelay: 0,
                        renderer: function (toolTip, record, ctx) {
                            var toolVal = record.get('Remark3')
                            toolTip.setHtml(toolVal); //record.get('name') + ': ' + record.get('data1') + ' views');
                        }
                    },
                    renderer: function (sprite, config, rendererData, index) {
                        var store = rendererData.store,
                            storeItems = store.getData().items,
                            currentRecord = storeItems[index],
                            changes = {};
                        if (currentRecord)
                        {
                            switch (config.type)
                            {
                                case 'marker':
                                    changes.strokeStyle = "rgb(178,210,51)";
                                    changes.fillStyle = "rgb(178,210,51)";
                                    var line3Val = currentRecord.data['Remark3'];
                                    if (line3Val.indexOf("Fail") > 0)
                                    {
                                        changes.strokeStyle = "red";
                                        changes.fillStyle = "red";
                                    }
                                    break;
                                    //case 'line':
                                    //    changes.strokeStyle = (isUp ? 'cornflowerblue' : 'tomato');
                                    //    changes.fillStyle = (isUp ? 'rgba(100, 149, 237, 0.4)' : 'rgba(255, 99, 71, 0.4)');
                                    //    break;
                            }
                        }
                        return changes;
                    },
                    listeners: {//itemdblclick ( series, item, event, eOpts ) 
                        itemdblclick: function (series, item, event, eOpts) {
                            //#region markers
                            if (item.category == "markers") {
                                MarkerClick_GetDetailedData(item);
                            }
                            //#endregion
                        }
                    }
                    //#endregion
                },
                {
                    //#region line 4  ==> show line 8
                     id: 'srs4',
                     type: 'line',
                     xField: 'Time',
                     yField: 'Line4',
                     style: {
                         lineWidth: 2,
                         stroke: "black",
                     },
                     marker: {
                         radius: 8,
                         lineWidth: 2,
                         fillStyle: 'rgb(178,210,51)'
                     },
                     label: {
                         field: 'Remark4',
                         display: 'over',
                         hidden: true,
                         renderer: function (text) {
                             if (text.indexOf("Pass") > 0)
                             {
                                 return "pass";
                             }
                             else
                                 return "fail";
                         }
                     },
                     highlight: {
                         fillStyle: '#FF9900',
                         radius: 10,
                         lineWidth: 2,
                         strokeStyle: '#FF9900'
                     },
                     tooltip: {
                         trackMouse: true,
                         style: 'background: #fff',
                         showDelay: 0,
                         dismissDelay: 0,
                         hideDelay: 0,
                         renderer: function (toolTip, record, ctx) {
                             var toolVal = record.get('Remark4')
                             toolTip.setHtml(toolVal); //record.get('name') + ': ' + record.get('data1') + ' views');
                         }
                     },
                     renderer: function (sprite, config, rendererData, index) {
                         var store = rendererData.store,
                             storeItems = store.getData().items,
                             currentRecord = storeItems[index],
                             changes = {};
                         if (currentRecord)
                         {
                             switch (config.type)
                             {
                                 case 'marker':
                                     changes.strokeStyle = "rgb(178,210,51)";
                                     changes.fillStyle = "rgb(178,210,51)";
                                     var line3Val = currentRecord.data['Remark4'];
                                     if (line3Val.indexOf("Fail") > 0)
                                     {
                                         changes.strokeStyle = "red";
                                         changes.fillStyle = "red";
                                     }
                                     break;
                             }
                         }
                         return changes;
                     },
                     listeners: {//itemdblclick ( series, item, event, eOpts ) 
                         itemdblclick: function (series, item, event, eOpts) {
                             //#region markers
                             if (item.category == "markers") {
                                 MarkerClick_GetDetailedData(item);
                             }
                             //#endregion
                         }
                     }
                    //#endregion
                }
                ],
                listeners: { // Listen to itemclick events on all series.
                    afterrender: {
                        fn: function () {
                            var timer = $.timer(1000, function () {
                                timer.stop();
                                PolmRep_ReviewParas();
                            });
                        }
                    },

                    //#region itemclick
                    //itemdbclick: function (chart, item, event) {
                    //    //console.log('itemclick', item.category, item.field);
                    //    if (item.category == "markers") {
                    //        MarkerClick_GetDetailedData(item);
                    //    }
                    //}
                    //#endregion
                }
            }]
        });
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function onAxisLabelRender(axis, label, layoutContext) {
    try {
        // Custom renderer overrides the native axis label renderer.
        // Since we don't want to do anything fancy with the value
        // ourselves except appending a '%' sign, but at the same time
        // don't want to loose the formatting done by the native renderer,
        // we let the native renderer process the value first.
        //return layoutContext.renderer(label) + '%';
        if (label == "30")
        {
            var lineNum = Ext.getCmp("txtField_Name_L1").getValue();
            return "L" + lineNum;
        }
        else if (label == "60")
        {
            var lineNum = Ext.getCmp("txtField_Name_L2").getValue();
            return "L" + lineNum;
        }
        else if (label == "90")
        {
            var lineNum = Ext.getCmp("txtField_Name_L3").getValue();
            return "L" + lineNum;
        }
        else if (label == "120")
        {
            var lineNum = Ext.getCmp("txtField_Name_L4").getValue();
            return "L" + lineNum;
        }
        return "";


        //#region backp
        //var cellNum = Ext.getCmp("Cmbo_Cell_Rep").getValue();
        //if (cellNum.indexOf("1") >= 0)
        //{
        //    //#region for cell 1
        //    if (label == "30")
        //    {
        //        return "L3";
        //    }
        //    else if (label == "60")
        //    {
        //        return "L6";
        //    }
        //    else if (label == "90")
        //    {
        //        return "L7";
        //    }
        //    else if (label == "120")
        //    {
        //        return "L8";
        //    }
        //    //#endregion
        //}
        //else if (cellNum.indexOf("2") >= 0)
        //{
        //    //#region for cell 2
        //    if (label == "30")
        //    {
        //        return "L16";
        //    }
        //    //#endregion
        //}
        //else if (cellNum.indexOf("3") >= 0)
        //{
        //    //#region for cell 3
        //    if (label == "30")
        //    {
        //        return "L22";
        //    }
        //    //#endregion
        //}
        //return "";
        //#endregion
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }

};

function CreateGUI_PolmRepData() {
    try {
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
            var columns_Wave = [
                { xtype: 'rownumberer', header: "#", width: 30, align: 'left' },
                //{
                //    header: "<div style='font-size: 11px;'>OV_ID</div>", dataIndex: "OV_ID", width: 60, menuDisabled: true, editor: {}, hidden: true,
                //},
                {
                    header: "<div style='font-size: 11px;'>Line</div>", dataIndex: "Line", width: 30, menuDisabled: true, editor: {},// renderer: renderFormat,
                },
                {
                    header: "<div style='font-size: 11px;'>Machine#</div>", dataIndex: "Machine", width: 110, menuDisabled: true, editor: {}, hidden: false,
                },
                {
                    header: "<div style='font-size: 11px;'>Customer</div>", dataIndex: "Project", width: 50, menuDisabled: true, editor: {}, hidden: false,
                },
                {
                    header: "<div style='font-size: 11px;'>Program</div>", dataIndex: "Program", width: 100, menuDisabled: true, editor: {},//renderer: renderColor 
                },
                {
                    header: "<div style='font-size: 11px;'>Parameter</div>", dataIndex: "Parameter", width: 110, menuDisabled: true, editor: {}, tdCls: 'tipRep',
                },
                {
                    header: "<div style='font-size: 11px;'>Val_Cen</div>", dataIndex: "Val_Cen", width: 70, menuDisabled: true, editor: {},
                },
                {
                    header: "<div style='font-size: 11px;'>Min</div>", dataIndex: "Val_Min", width: 70, menuDisabled: true, editor: {}, hidden: false, tdCls: 'tipRep',
                    renderer: function (value, metaData, record, rowIndex, columnIndex, store, view)
                    {
                        //#region renderer
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
                    header: "<div style='font-size: 11px;'>Max</div>", dataIndex: "Val_Max", width: 70, menuDisabled: true, editor: {}, hidden: false, tdCls: 'tipRep',
                    renderer: function (value, metaData, record, rowIndex, columnIndex, store, view)
                    {
                        //#region renderer
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
                    header: "<div style='font-size: 11px;'>Actual</div>", dataIndex: "Val_Act", width: 70, menuDisabled: true, editor: {}, hidden: false,
                },
                {
                    header: "<div style='font-size: 11px;'>DateTime</div>", dataIndex: "Time", width: 120, menuDisabled: true, editor: {}, hidden: false,
                },
                {
                    header: "<div style='font-size: 11px;'>Owner</div>", dataIndex: "Owner", width: 80, menuDisabled: true, editor: {}, hidden: false,
                },
                {
                    header: "<div style='font-size: 11px;'>Comment</div>", dataIndex: "Comment", width: 80, menuDisabled: true, editor: {}, hidden: false,
                },
                {
                    header: "<div style='font-size: 11px;'>Status</div>", dataIndex: "Status", width: 80, menuDisabled: true, editor: {}, hidden: false,
                    renderer: function (value, metaData, record) {
                        if (value) {
                            if (value == "pass") {
                                metaData.style = 'color:black; font-weight:bolder';
                            }
                            else if (value == "fail") {
                                metaData.style = 'color:red; font-weight:bolder';
                            }
                        }
                        else {
                            metaData.style = 'color:black';
                        }
                        return value;
                    },

                },
                {
                    header: "<div style='font-size: 11px;'>TEMPDAYS</div>", dataIndex: "TEMPDAYS", width: 80, menuDisabled: true, editor: {}, hidden: true, 
                },

            ];
            //#endregion

            //#region tool bar
            var cellStore = Ext.create('Ext.data.Store', {
                fields: ['name', 'desc'],
                data: [
                    { "name": "Cell 1", "desc": "ALL" },
                    { "name": "Cell 1", "desc": "L3" },
                    //{ "name": "Cell 1", "desc": "L6" },
                    { "name": "Cell 1", "desc": "L7" },
                    { "name": "Cell 1", "desc": "L8" },
                    { "name": "Cell 2", "desc": "ALL" },
                    { "name": "Cell 2", "desc": "L19" },
                    { "name": "Cell 3", "desc": "ALL" },
                    { "name": "Cell 3", "desc": "L24" },
                    { "name": "Cell 3", "desc": "L29" },
                ]
            });

            var txtField_Wave = new Ext.form.TextField({
                id: 'txtField_Wave', fieldLabel: "Sheet:", labelWidth: 40, width: 100, anchor: '98%',  //必须要有，否则不能无效		
                grow: true, emptyText: "?", value: "", width: 200, selectOnFocus: false, hidden: true,
            });

            //#region line name store
            var txtField_Name_L1 = new Ext.form.TextField({
                id: 'txtField_Name_L1', fieldLabel: "L1:", labelWidth: 15, width: 60,
                grow: true, emptyText: "", value: "", selectOnFocus: false, hidden: true,
            });
            var txtField_Name_L2 = new Ext.form.TextField({
                id: 'txtField_Name_L2', fieldLabel: "L2:", labelWidth: 15, width: 60,
                grow: true, emptyText: "", value: "", selectOnFocus: false, hidden: true,
            });
            var txtField_Name_L3 = new Ext.form.TextField({
                id: 'txtField_Name_L3', fieldLabel: "L3:", labelWidth: 15, width: 60,
                grow: true, emptyText: "", value: "", selectOnFocus: false, hidden: true,
            });
            var txtField_Name_L4 = new Ext.form.TextField({
                id: 'txtField_Name_L4', fieldLabel: "L4:", labelWidth: 15, width: 60,
                grow: true, emptyText: "", value: "", selectOnFocus: false, hidden: true,
            });
            //#endregion
            //#region DATE SELECT

            var Date_start = Ext.create('Ext.form.field.Date', {
                id: 'Date_start',
                fieldLabel: 'DAY',
                labelAlign: 'right',
                cls: 'iconDate24',
                maxValue: new Date(),
                width: 140, labelWidth: 35,
                format: 'm/d/Y',
                value: new Date(),	
            });

            //#endregion

            var toolbar_PolmRep = {
                id: 'toolbar_Wave',
                xtype: 'toolbar',
                dock: 'top', // bottom, right, left
                items: [
                    {
                        //#region  Cell
                        id: "Cmbo_Cell_Rep", scale: "medium", xtype: "combo", iconCls: "", fieldLabel: "选择车间:", width: 160, labelWidth: 55,
                        editable: false, store: cellStore, //cls: 'combo',		
                        queryModel: "local",
                        valueField: "desc",
                        tpl: Ext.create('Ext.XTemplate',
                            '<tpl for=".">',
                                //'<div class="x-boundlist-item" style="font-weight: normal;">{name} {desc}</div>',
                                //'<div class="{[this.getClass(values)]}">{name} {desc}</div>',
                                '<div class="x-boundlist-item" style="{[this.getClass(values)]}">{name} {desc}</div>',
                            '</tpl>', {
                                getClass: function (rec)
                                {
                                    //return rec.desc == 'ALL' ? 'x-boundlist-item x-highlighted-item' : 'x-boundlist-item';
                                    return rec.desc == 'ALL' ? 'font-weight: bolder; color: blue;' : 'font-weight: normal;';
                                }
                            }
                        ),
                        displayTpl: Ext.create('Ext.XTemplate',
                            '<tpl for=".">',
                                '{name} {desc}',
                            '</tpl>'
                        ),
                        //defaultValue: "Cell 1 ALL",
                        listeners: {
                            select: function (combo, record, eOpts) {
                                
                                CreateGUI_PolmRepChart();
                            },
                            afterRender: function (combo) {	
                                //debugger;	
                                var firstValue = cellStore.data.items[0];	
                                //var dispVal = firstValue.data.name + ' ' + firstValue.data.desc;	
                                //combo.setValue(firstValue.data.desc);//同时下拉框会将与name为firstValue值对应的 text显示	
                                //combo.setValue(cellStore.getAt('1').get('desc'));	
                                combo.setValue(firstValue);
                            },	
                        },
                        //#endregion 
                    },
                    '-',
                    Date_start,
                    '-',
                    {
                        id: "btn_ReviewParas", scale: "medium", text: "<div style='font-weight:bolder; font-size:11px;'><b>View Data</b></div>", xtype: "button", iconCls: "iconData32",
                        handler: onToolBtnClick_RepData  //PolmRep_ReviewParas()
                    },
                    txtField_Wave,
                    txtField_Name_L1, txtField_Name_L2, txtField_Name_L3, txtField_Name_L4,
                    '->',
                    {
                        id: "chkWaveRun", xtype: "checkbox", width: 60, labelWidth: 30, boxLabel: "Running", checked: false,//scale: "small",
                        handler: function () { Chart_Run_Switch() },
                    },
                ]
            };

            var filter_PF = Ext.create('Ext.data.Store', {
                fields: ['name', 'desc'],
                data: [
                    { "name": "ALL", "desc": "1" },
                    { "name": "pass", "desc": "1" },
                    { "name": "fail", "desc": "2" },
                    { "name": "NA", "desc": "3" },
                ]
            });

            var toolbar_Bot_Wave = {
                id: 'toolbar_Wave_Bot',
                xtype: 'toolbar',
                dock: 'bottom',
                hidden: false,
                items: [
                    {
                        //#region Cmbo_Filter_Wave
                        id: "Cmbo_Filter_Wave", scale: "medium", xtype: "combo", iconCls: "", fieldLabel: "过滤 pass fail:", width: 150, labelWidth: 80,
                        editable: false, store: filter_PF, //cls: 'combo',		
                        queryModel: "local",
                        valueField: "name",
                        tpl: Ext.create('Ext.XTemplate',
                            '<tpl for=".">',
                                '<div class="x-boundlist-item" style="font-weight: bold;">{name}</div>',
                            '</tpl>'
                        ),
                        displayTpl: Ext.create('Ext.XTemplate',
                            '<tpl for=".">',
                                '{name}',
                            '</tpl>'
                        ),
                        listeners:
                        {
                            scope: this,
                            change: function (field, newValue, oldValue, eOpts) {
                                if (newValue == "ALL") {
                                    var grid = Ext.getCmp('grid_RepData');
                                    grid.store.clearFilter();
                                }
                                else {
                                    var grid = Ext.getCmp('grid_RepData');
                                    grid.store.clearFilter();

                                    if (newValue) {
                                        var matcher = new RegExp(Ext.String.escapeRegex(newValue), "i");
                                        grid.store.filter({
                                            filterFn: function (record) {
                                                return matcher.test(record.get('Status'))
                                            }
                                        });
                                    }
                                }
                            },
                        },
                        //#endregion
                    },
                    '->',
                    {
                        id: "btn_RepTracking", scale: "small", text: "<div style='font-weight:normal; font-size:11px;'>Tracking</div>", xtype: "button", iconCls: "iconDoc16",
                        handler: function () { PolmRep_ShowTracking(); }
                    },
                    {
                        id: "btn_xlsSave", scale: "small", text: "<div style='font-weight:normal; font-size:11px;'>Export</div>", xtype: "button", iconCls: "iconMenuExcel16",
                        handler: function () { SaveExcel_Rep_Wave(); }
                    },
                ]
            };
            //#endregion

            var grid_RepData = Ext.create("Ext.grid.Panel", {
                id: "grid_RepData",
                title: "Wave Parameter",
                width: '100%',  //forceFit: true,   
                columnLines: true,
                height: 300,  //autoHeight: true, //height: 250, //
                store: new Ext.data.ArrayStore(),
                renderTo: "divRepData",
                columns: columns_Wave,
                collapsible: true,
                plugins: [
                    {
                        ptype: "cellediting",
                        ptype: 'gridexporter',
                    },
                ],
                dockedItems: [
                    toolbar_PolmRep,
                    toolbar_Bot_Wave,
                ],
                viewConfig: {
                    stripeRows: false,//在表格中显示斑马线
                    enableTextSelection: true
                },
                listeners: {
                    afterrender: {
                        fn: function () {
                            Init_grid_RepData_Wave();
                        }
                    },
                    headerclick: function (ct, column, e, t, eOpts) {//选择是否全选
                        //toolFun_Header(column, t);
                    },
                    itemcontextmenu: function (grid, record, item, index, e) {
                        if (Ext.getCmp("polmRep_contextMenu"))
                            Ext.getCmp("polmRep_contextMenu").destroy();

                        var contextMenu = Ext.create('Ext.menu.Menu', {
                            id: 'polmRep_contextMenu',
                            width: 150,
                            items: [
                                {
                                    text: 'e-Mail owner', iconCls: "iconemail16",
                                    handler: function () {
                                        onMenuItemClick_emailInform(grid, record, item, index, e);
                                    }
                                },
                                '-',
                                {
                                    text: 'TemporyWI', iconCls: "iconLink16",
                                    handler: function () {
                                        var newUrl = $("#Url_Page_WIReview").val();
                                        window.open(newUrl);
                                        //window.location.assign(newUrl)
                                    }
                                },
                                '-',
                                {
                                    text: 'Config Owner', iconCls: "iconConfig16",
                                    handler: function ()
                                    {
                                        var newUrl = $("#Url_PolmConfig").val();
                                        window.open(newUrl);
                                    }
                                }
                            ]
                        });
                        e.stopEvent();
                        contextMenu.showAt(e.getXY());
                    },
                    render: function (grid)
                    {   //////////////////////////////////////////////显示grid cell tip 	
                        //#region 显示grid cell tip 
                        var view = grid.getView();
                        grid.tip = Ext.create('Ext.tip.ToolTip', {
                            target: view.getId(),
                            delegate: view.itemSelector + ' .tipRep',
                            trackMouse: true,
                            listeners: {
                                beforeshow: function (tip)
                                {
                                    //#region tooltip
                                    var tipGridView = tip.target.component;
                                    var colname = tipGridView.getHeaderCt().getHeaderAtIndex(tip.triggerElement.cellIndex).dataIndex;
                                    //tip.update(record.get(colname));	
                                    var colIndex = tip.triggerElement.cellIndex;
                                    var record = tipGridView.getRecord(tip.triggerElement);

                                    if (colname == "Val_Max" || colname == "Val_Min")
                                    {
                                        //#region for Val_Max	
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
                                        //#endregion
                                    }
                                    else if (colname == "Parameter")
                                    {
                                        //#region Parameter
                                        var tipValue = "";
                                        var paraValue = record.get(colname);
                                        if (paraValue == "Flux_BdWid")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=<span style='color:red;'>1_Board_Width(mm)</span>, Fluxer=<span style='color:blue;'>Width=</span></div>";
                                        }
                                        else if (paraValue == "Flux_ConvSpd")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>2_Conveyor_speed(m/min)</span>, Fluxer=><span style='color:blue;'>Conv=</span></div>";
                                        }
                                        else if (paraValue == "Flux_NozSpd")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>3_Nozzle_Speed(mm/s)</span>, Fluxer=><span style='color:blue;'>StrokeFactor=</span></div>";
                                        }
                                        else if (paraValue == "Flux_Volumn")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>4_Flux_Volume(ml/m)</span>, Fluxer=><span style='color:blue;'>FluxPres=</span>(/10)</div>";
                                        }
                                        else if (paraValue == "Flux_NozSpray")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>5_Nozzle_Spray_Width(mm)</span>, Fluxer=><span style='color:blue;'>NozWidth=</span></div>";
                                        }
                                        else if (paraValue == "Flux_Power")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>6_Power(W)</span>, Fluxer=><span style='color:blue;'>Ultra=</span>(/10)</div>";
                                        }
                                        else if (paraValue == "Flux_Pres")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>7_Pressure(kpa)</span>, Fluxer=><span style='color:blue;'>AirPres=</span>(/10 1psi=6.895kPa)</div>";
                                        }
                                        else if (paraValue == "Flux_BiModel")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>8_Bi-direction_Model</span>, Fluxer=><span style='color:blue;'>BiDirection=</span></div>";
                                        }
                                        else if (paraValue == "Heat_Low1")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>9_Lower_p/h1(℃)</span>, Wave=><span style='color:blue;'>Lower Preheater 1</span>(℉=32+℃ ×1.8)</div>";
                                        }
                                        else if (paraValue == "Heat_Low2")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>10_Lower_p/h2(℃)</span>, Wave=><span style='color:blue;'>Lower Preheater 2</span>(℉=32+℃ ×1.8)</div>";
                                        }
                                        else if (paraValue == "Heat_Low3")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>11_Lower_p/h3(℃)</span>, Wave=><span style='color:blue;'>Lower Preheater 3</span>(℉=32+℃ ×1.8)</div>";
                                        }
                                        else if (paraValue == "Heat_Upp1")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>12_Upper_p/h1(℃)</span>, Wave=><span style='color:blue;'>Upper Preheater 1</span>(℉=32+℃ ×1.8)</div>";
                                        }
                                        else if (paraValue == "Heat_Upp2")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>13_Upper_p/h2(℃)</span>, Wave=><span style='color:blue;'>Upper Preheater 2</span>(℉=32+℃ ×1.8)</div>";
                                        }
                                        else if (paraValue == "Heat_Upp3")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>14_Upper_p/h3(℃)</span>, Wave=><span style='color:blue;'>Upper Preheater 3</span>(℉=32+℃ ×1.8)</div>";
                                        }
                                        else if (paraValue == "SP_Temp")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>15_Solder_TEMP(℃)</span>, Wave=><span style='color:blue;'>Solder Temperature=</span>(摄氏度(℃)=（华氏度(℉)-32）÷1.8)</div>";
                                        }
                                        else if (paraValue == "SP_ConWave")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>16_Contour_WAVE(rpm)</span>, Wave=><span style='color:blue;'>Lambda Wave=</span></div>";
                                        }
                                        else if (paraValue == "SP_LdClear")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>17_Lead_Clearance(mm)</span>, Wave=><span style='color:blue;'>Lead Clearance=</span>(1英寸(in)=25.4mm)</div>";
                                        }
                                        else if (paraValue == "Conv_Speed")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>18_Speed(m/min)</span>, Wave=><span style='color:blue;'>Conveyor Speed=</span>(1英尺(ft)=0.3048米(m))</div>";
                                        }
                                        else if (paraValue == "Conv_Width")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>19_Width(mm)</span>, Wave=><span style='color:blue;'>Conveyor Width=</span>(1英寸(in)=0.0254米(m))</div>";
                                        }
                                        else if (paraValue == "Other_Ni")
                                        {
                                            tipValue = "<div style='font-size: 11px;'>WI=><span style='color:red;'>Nitrogen</span>, Wave=><span style='color:blue;'>N2 Lambda Wave/Tunnel</span>(1=on, 0=off)</div>";
                                        }

                                        tip.update(tipValue);
                                        //#endregion
                                    }
                                    //#endregion
                                }
                            }
                        });
                        //#endregion
                    },

                },
            })

            grid_RepData.header.titleCmp.textEl.selectable();
            //#endregion
        }
        catch (err) {
            Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
        }

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function Init_grid_RepData_Wave()
{
    try {
        PolmRep_SetCompDisable(false, false, false, false);

        var user = glbLogin_getCookie("userEN")
        if (user == "28036240" || user == "28036000") {
            Ext.getCmp("btn_RepTracking").show();
        }
        else {
            Ext.getCmp("btn_RepTracking").hide();
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function onToolBtnClick_RepData(item) {
    try {
        var sText = item.text;
        var sID = item.id;
        if (sID == "btn_GetList") {
            Home_GetRealTimeData();
        }
        else if(sID == "btn_ReviewParas") {
            PolmRep_ReviewParas();
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function AdjustFormHeight_Report() {
    try {
        debugger;
        var bodyHeight = $('#divBody').height(); //$('body').height();
        var progTitle = $("#prgTitle").height();
        var footerHeight = $("#contFooter").height();

        var contAll_Height = bodyHeight - progTitle - 12;// - footerHeight;
        $("#contAll").height(contAll_Height);

        var bodyHeightAll = $("#contAll").height();
        var MainHeight = bodyHeightAll - footerHeight - 12;

        $("#contMain").height(MainHeight);

        var divTitle = $("#divTitle").height();
        var divGrid_Main = MainHeight - divTitle;

        var panel_Chart = $("#divChartOV").height();

        var h_grid_RepData = divGrid_Main - panel_Chart;
        Ext.getCmp("grid_RepData").setHeight(h_grid_RepData);

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}



























