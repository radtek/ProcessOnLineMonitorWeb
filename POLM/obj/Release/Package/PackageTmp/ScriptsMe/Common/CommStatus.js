///<reference path="~/ExtJS6/ext-all.js" />

//var Global_User = "";

Ext.onReady(function () {

    try
    {
        new Ext.Panel({
            //title: 'status',
            width: '100%',
            //height: 100,
            renderTo: 'divComStatus',
            bbar: new Ext.ux.StatusBar({
                id: 'my-status',

                defaultText: 'Ready',
                defaultIconCls: 'default-icon',

                text: 'Ready',
                iconCls: 'ready-icon',

                items: [
                        '-',
                        {
                            id: "status_Sub", xtype: "label", text: '', padding: "0 1 0 1", cls: "Status_Cls", hidden: false,
                        },
                        '-',
                        {
                            id: "status_SR", xtype: "label", text: '', padding: "0 1 0 1", cls: "Status_Cls", hidden: false,
                        },
                        '-',
                        {
                            id: "status_Project", xtype: "label", text: 'project:', cls: "Status_Cls",
                        },
                        '-',
                        {
                            id: "status_User", xtype: "label", text: glbLogin_getCookie("username"),
                        },
                        {
                            id: "status_Depart", xtype: "label", text: glbLogin_getCookie("userdepart"), cls: "Status_Cls", hidden: true
                        },
                        '-',
                        {
                            xtype: "label", text: 'Project Name', cls: "Status_Cls", padding: "0 5 0 5",
                        },
                        '-',
                        {
                            xtype: "label", text: 'Ver:1.3', cls: "Status_Cls"
                        },
                ]
            })
        });

    }
    catch (err) {
        Ext.Msg.show({ title: 'Warnning', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
});


function TimerOutEvent()
{

}




function glbLogin_getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i].trim();
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}

function glbLogin_setCookie(name, value, days) {
    debugger;
    if (days == 0)
        days = 2 / 8;

    if (value == "")
        value = "";

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toUTCString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";

    //toGMTString()	请使用 toUTCString() 方法代替。
    //toUTCString()	根据世界时，把 Date 对象转换为字符串。
}



//for different project has different
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function Destroy_All() {
    Destroy_query();
    Destroy_PnIn();
}

function Destroy_query() {
    try {
        if (Ext.getCmp("txtQModel")) {
            Ext.getCmp("txtQModel").destroy();
        }

        if (Ext.getCmp("cmbDate")) {
            Ext.getCmp("cmbDate").destroy();
        }

        if (Ext.getCmp("grid_Q")) {
            Ext.getCmp("grid_Q").destroy();
        }

        if (Ext.getCmp("chkQSN")) {
            Ext.getCmp("chkQSN").destroy();
        }

        
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}


function Destroy_PnIn() {
    try {
        if (Ext.getCmp("txtSN")) {
            Ext.getCmp("txtSN").destroy();
        }

        if (Ext.getCmp("grid_RefDes")) {
            Ext.getCmp("grid_RefDes").destroy();
        }

        if (Ext.getCmp("chkBySN")) {
            Ext.getCmp("chkBySN").destroy();
        }

    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}














