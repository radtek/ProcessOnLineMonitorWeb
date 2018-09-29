///<reference path="~/ExtJS6/ext-all.js" />


Ext.onReady(function () {
    try
    {
        new Ext.menu.Bar({
            renderTo: 'divMyMenu',
            width: '100%', //'100%',// 600,
            flex: 1,
            columnWidth: 200,
            items: [{
                text: 'Home',
                iconCls: "iconHome16",
                margin: '2 10 2 10',
            },
            //{
            //    text: 'Record',
            //    iconCls: "iconMenuExcel16",
            //    margin: '2 10 2 10',
            //    menu: [
            //        {
            //            text: 'Torque', iconCls: 'iconScrew16x',
            //            listeners: {
            //                click: {
            //                    fn: function (item, e, eOpts) {
            //                        debugger;
            //                        MenuItemClick2(item);
            //                    }
            //                },
            //            }
            //        },
            //        '-',
            //        {
            //            text: 'Odc Data', iconCls: 'iconExcel16',
            //            listeners: {
            //                click: {
            //                    fn: function (item, e, eOpts) {
            //                        debugger;
            //                        MenuItemClick2(item);
            //                    }
            //                },
            //            }
            //        },
            //    ],


            //},
            //{
            //    text: 'History',
            //    iconCls: "iconMenuQ16",
            //    margin: '2 10 2 10',
            //},
            {
                text: 'Login',
                iconCls: "iconMenuUser16",
                margin: '2 2 2 2',
                menu: [{
                    text: 'Logout', iconCls: 'iconLogout20',
                    listeners: {
                        click: {
                            fn: function (item, e, eOpts) {
                                debugger;
                                MenuItemClick2(item);
                            }
                        },
                    }
                }]
            }, {
                text: 'About',
                iconCls: 'iconAbout16',
                margin: '2 10 2 10',
                listeners: {
                    click: {
                        fn: function (item, e, eOpts) {
                            debugger;
                            MenuItemClick2(item);
                        }
                    },
                }
            }],
            listeners: {
                click: {
                    //element: 'el', //bind to the underlying el property on the panel
                    fn: function (menu, item, e, eOpts) {
                        //debugger;
                        //console.log('click el');
                        //Ext.Msg.show({ title: 'Create Menu', msg: 'click el', buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
                        MenuItemClick(item);
                    }
                },
                dblclick: {
                    element: 'body', //bind to the underlying body property on the panel
                    fn: function () { console.log('dblclick body'); }
                }
            }
        });

    }
    catch (err) {
        Ext.Msg.show({ title: 'Create Menu', msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
});


function MenuItemClick(item)
{
    try {
        //debugger;
	    if(item){
            var menuTxt = item.text;
            //Ext.Msg.show({ title: 'Create Menu', msg: txt, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
            if(menuTxt == "Home")
            {
                newUrl = $("#Url_Page_Home").val(); //newUrl = "~/Home";
                //window.location(newUrl);
                window.location.assign(newUrl)
            }
            else if (menuTxt == "Login")
            {
                LoginShow();
            }
            else if (menuTxt == "History") {
                newUrl = $("#Url_Page_Query").val();   // "~/Torque";
                window.location.assign(newUrl)
            }
            else if (menuTxt == "About") {
                newUrl = $("#Url_Page_About").val(); //newUrl = "~/Home";
                //window.location(newUrl);
                window.location.assign(newUrl)
            }
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}


function MenuItemClick2(item) {
    try {
        debugger;
        var menuTxt = item.text;

        //Ext.Msg.show({ title: 'Create Menu', msg: txt, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
        if (menuTxt == "Torque") {
            newUrl = $("#Url_Page_Torque").val();   // "~/Torque";
            window.location.assign(newUrl)
        }
        else if (menuTxt == "Logout")
        {
            //#region for logout
            glbLogin_setCookie("username", "", 0);
            glbLogin_setCookie("userEN", "", 0);
            glbLogin_setCookie("userdepart", "", 0);
	        glbLogin_setCookie("project", "", 0);

            Ext.getCmp('status_User').setText("()");
            Ext.getCmp('status_Project').setText("project:()");
            Ext.getCmp('status_Depart').setText("");

            Ext.getCmp('my-status').setStatus({ text: "Success to logout" });
            //#endregion
        }
        else if (menuTxt == "Odc Data") {
            newUrl = $("#Url_Page_OdcData").val();   // "~/OdcData";
            window.location.assign(newUrl)
        }
    }
    catch (err) {
        Ext.Msg.show({ title: arguments.callee.name, msg: err.message, buttons: Ext.Msg.OK, icon: Ext.MessageBox.WARNING });
    }
}



