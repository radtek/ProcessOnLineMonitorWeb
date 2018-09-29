///<reference path="~/Scripts/ExtJS62/ext-all.js" />

var ticker = null;

Ext.onReady(function () {
    try {

        CreateGUI_Home();

        Create_SingalR();
    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
});

function CreateGUI_Home()
{
    try {
        //#region Create GUI for Content Category
        Ext.QuickTips.init();

        //#region 建立Grid -- software list
        function renderUpdate(value, metaData, record, rowIndex, columnIndex, store, view) {
            //debugger;
            var para = view.panel.id;
            return "<input type='button' value='" + record.get("View") +
                "'style='font-size:12px;font-weight:bold; width:100%; height:25px ' onclick='startItemClick(" + rowIndex + ',' + para + ")'" + ">";  //',' + view.panel.id +  height:40px;
        }

        function renderFormat(value, metaData, record, rowIndex, colIndex) {
            //if (colIndex == Global_Grid_Column) {
            //    metaData.style = 'background-color: #FF9900;';
            //}
            //else {
            //    metaData.style = 'background-color: white;';
            //}
            metaData.style = 'font-weight:bolder;font-size: 12px;';
            return value;
        }

        //#region columns
        var columns_Cate = [
            { xtype: 'rownumberer', header: "#", width: 40, align: 'left' },
            {
                header: "<div style='font-size: 11px;'>Category</div>", dataIndex: "Category", width: 100, height: 30, menuDisabled: true, //editor: {},
            },
            {
                header: "<div style='font-size: 11px;'>Item</div>", dataIndex: "Item", width: 150, menuDisabled: true, editor: {}, renderer: renderFormat,
            },
            {
                header: "<div style='font-size: 11px;'>Link</div>", dataIndex: "Link", width: 200, menuDisabled: true, editor: {}, hidden: true,
            },
            {
                header: "<div style='font-size: 11px;'>Description</div>", dataIndex: "Desc", flex: 1, menuDisabled: true, editor: {},//renderer: renderColor 
            },
            {
                header: "<div style='font-size: 11px;'>Remark</div>", dataIndex: "Train", width: 200, menuDisabled: true, //editor: {},   
            },
            {
                header: "<div style='font-size: 11px;'>View</div>", dataIndex: "View", width: 100, menuDisabled: true, renderer: renderUpdate, tdCls: 'tip',
            },
            //{ header: "<div style='font-size: 11px;'>Introduction</div>", dataIndex: "Train", width: 120, menuDisabled: true, hidden: false, tdCls: 'tip', },
            { header: "<div style='font-size: 11px;'>id</div>", dataIndex: "iid", width: 50, menuDisabled: true, hidden: true },
        ];
        //#endregion

        //#region barcode
        var txtField_s = new Ext.form.TextField({
            id: 'txtBarcode_s', fieldLabel: "<b>Barcode</b>", scale: "medium", labelWidth: 80, labelAlign: 'right', anchor: '98%', cls: "iconBarCode16",
            grow: true, emptyText: "?", value: "", width: 200, selectOnFocus: true, enableKeyEvents: true, padding: '0 5,0,5',
            listeners: {
                keyup: function (obj, e) {
                    if (e.getKey() == 13) {   ////e.getKey()是获取按键的号码，13代表是回车键  
                        setBarcode();
                    }
                }
            },
        });
        //#endregion

        var grid_Global_Index = 0;
        var grid_Global_record = null;

        var grid_OverView = Ext.create("Ext.grid.Panel", {
            id: "grid_OverView",
            title: "Category List",
            width: '100%',  //forceFit: true,   
            columnLines: true,
            height: 370,  //autoHeight: true, //height: 250, //
            store: new Ext.data.ArrayStore(),
            renderTo: "divContentCategory",
            columns: columns_Cate,
            //selType: 'cellmodel',
            collapsible: true,
            plugins: [
                { ptype: "cellediting", },
                { ptype: 'gridexporter' }
            ],
            tbar: [
                {
                    id: "Cmbo_Category", scale: "medium", xtype: "combo", fieldLabel: "设备类别:", labelWidth: 60, width: 180, iconCls: "", padding: '0 5,0,5', valueField: 'value', displayField: 'value', // handler: onToolBtnClick
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

                },
                '-',
                {
                    id: "btn_GetList", scale: "medium", text: "<div style='font-weight:bolder; font-size:11px;'><b>Get List</b></div>", xtype: "button", iconCls: "iconCateList16", handler: onToolBtnClick
                },
            ],
            viewConfig: {
                stripeRows: false,//在表格中显示斑马线
                enableTextSelection: true
            },
            listeners: {
                celldblclick: GridCellDoubleClick,// function (table, td, cellIndex, record, tr, rowIndex, e, eOpts) {},
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
                                if (colname == "Train") {
                                    var sIntrod = record.get("Train");
                                    if (sIntrod != "") {
                                        var tipValue = "<div style='font-size: 11px;'>右键点击->查看Guide,显示介绍:<span style='color:red;'>" + sIntrod + "</span></div>";
                                        tip.update(tipValue);
                                    }
                                    else {
                                        var tipValue = "<div>暂时无此内容介绍</div>";
                                        tip.update(tipValue);
                                    }
                                }
                                else if (colname == "View") {
                                    var Link = record.get("Link");
                                    if (Link != "") {
                                        var tipValue = "<div style='font-size: 11px;'>点击此,打开网页:<span style='color:red;'>" + Link + "</span></div>";
                                        tip.update(tipValue);
                                    }
                                    else {
                                        var tipValue = "<div>暂时无此网页</div>";
                                        tip.update(tipValue);
                                    }
                                }
                            }
                        }
                    });
                },
                destroy: function (view) {
                    delete view.tip;
                },
                itemcontextmenu: function (grid, record, item, index, e, eOpts) {
                    //debugger;
                    //Ext.getCmp("myConMenu").destroy;
                    grid_Global_Index = index;
                    grid_Global_record = record;

                    var contextMenu = Ext.create('Ext.menu.Menu', {
                        id: 'myConMenu',
                        width: 100,
                        items: [
                            {
                                id: "menuViewGuide", text: '查看Guide', iconCls: "iconDoc16",
                                handler: function () {
                                    onMenuItemClick(grid, record, item, index, e, "menuViewGuide");
                                }
                            },
                        ],
                        listeners: {
                            beforeshow: function menuItemUpdate(thisMenu, eOpts, record, index) { //显示有问题, setDisabled(true) --> 无效
                                //debugger;
                                var thisTrain = grid_Global_record.get("Train");
                                if (thisTrain == "") {
                                    Ext.getCmp("menuViewGuide").setDisabled(false);
                                }
                                else {
                                    Ext.getCmp("menuViewGuide").setDisabled(false);
                                }

                            }
                        },
                    });
                    e.stopEvent();
                    contextMenu.showAt(e.getXY());
                },
                afterrender: {
                    fn: function () {
                        Home_GetList_Category();
                    }
                },
            },
        })
        //#endregion

        Ext.QuickTips.register({ target: 'btn_GetList', title: 'Category', text: '点击从服务器获取相应的清单', });

        grid_OverView.header.titleCmp.textEl.selectable();
        //#endregion
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

function onToolBtnClick(item) {
    try {
        var sText = item.text;
        var sID = item.id;
        if (sID == "btn_GetList") {
            Home_GetMachineItems();
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////单元格双击的操作 -------暂时不用
function GridCellDoubleClick(grid, td, cellIndex, record, tr, rowIndex, e, eOpts) {
    try {
        //debugger;

        var colName = grid.getHeaderCt().getHeaderAtIndex(cellIndex).dataIndex;
        if (colName == "EN") {

        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}
//#endreigon
Ext.define('Record_List', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Category', type: 'string' },
        { name: 'Item', type: 'string' },
        { name: 'Link', type: 'string' },
        { name: 'Desc', type: 'string' },
        { name: 'Remark', type: 'string' },
        { name: 'View', type: 'string' },
        { name: 'iid', type: 'string' },
        { name: 'Train', type: 'string' },
    ]
});






