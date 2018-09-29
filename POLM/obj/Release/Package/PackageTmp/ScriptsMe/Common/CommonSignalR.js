
//#region Global ---- SignalR
//Asp.net SignalR
var Global_serverHub = null;
//最近一次的login的工号
var Global_LastEN = "";
var Global_SignalR_ID = "";
//#endregion

function Create_SingalR()
{
    try
    {
        Ext.getCmp('status_SR').setText("");
        Global_serverHub = $.connection.AuthorizeMsgHub;

        function init()
        {
            //SignalR_Connect();
            var user = glbLogin_getCookie("userEN");
            Global_serverHub.server.connectSignalR(user);
        }

        Global_serverHub.client.publishMsg = function (viewBag)
        {
            if (viewBag.Title == "sub")
                Ext.getCmp('status_Sub').setText(viewBag.Message);
            else
            {
                Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
            }
        }

        //Global_serverHub.client.publishMsgSub = function (viewBag)
        //{
        //    Ext.getCmp('my-status').setStatus({ text: viewBag.Message });
        //}

        Global_serverHub.client.onConnected = function (id, srMessage, userName)  //userName以后使用
        {
            Global_SignalR_ID = id;

            Ext.getCmp('status_SR').setText(srMessage);
        }

        $.connection.hub.disconnected(function ()
        {
            console.log("connection.hub.disconnected @" + new Date());
            setTimeout(function ()
            {
                $.connection.hub.start();
            }, 2000); // Restart connection after 2 seconds.
        });


        // Start the connection
        $.connection.hub.start().done(init);

    }
    catch (err)
    {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}

//#region obsolete
//function SignalR_Connect() {
//    try {
//        Ext.getCmp('status_SR').setText("");
//        var user = glbLogin_getCookie("userEN");

//        Global_serverHub.server.connect(user, user).done(function (initMsg)
//        {
//            Ext.getCmp('status_SR').setText(initMsg);
//        });

//        //var user = glbLogin_getCookie("userEN");
//        //if (user) {
//        //    Global_serverHub.server.addGroup(user).done(function (initMsg) {
//        //        Ext.getCmp('status_SR').setText(initMsg);
//        //    });
//        //} else {
//        //    Global_serverHub.server.initMessage("").done(function (initMsg)
//        //    {
//        //        Global_SignalR_ID = initMsg;
//        //        //debugger;
//        //        Ext.getCmp('status_SR').setText(initMsg);
//        //    });
//        //}
//    }
//    catch (err) {
//        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
//    }
//}

//function SignalR_Logout() {
//    try {
//        Ext.getCmp('status_SR').setText("");
//        if (Global_LastEN) {
//            Global_serverHub.server.leaveGroup(Global_LastEN).done(function (leaveMsg) {
//                Ext.getCmp('status_SR').setText(leaveMsg);
//            });
//        }
//        else {
//            Ext.getCmp('status_SR').setText("err");
//        }
//    }
//    catch (err) {
//        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
//    }
//}

//function SignalR_Connect_Detect() {
//    try {
//        var SR = Ext.getCmp('status_SR').getValue();
//        if (SR == "") {
//            Create_SingalR();
//        }
//    }
//    catch (err) {
//        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
//    }
//}
//#endregion
