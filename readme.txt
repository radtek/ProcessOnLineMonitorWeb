The following errors occurred while attempting to load the app.
- The OwinStartup attribute discovered in assembly 'jwangAsp' referencing startup type 'jwangAsp.Startup' conflicts with the attribute in assembly 'plistReport' referencing startup type 'plistReport.Startup' because they have the same FriendlyName ''. Remove or rename one of the attributes, or reference the desired type directly.
To disable OWIN startup discovery, add the appSetting owin:AutomaticAppStartup with a value of "false" in your web.config.
To specify the OWIN startup Assembly, Class, or Method, add the appSetting owin:AppStartup with the fully qualified startup class or configuration method name in your web.config.

----------->
Clear your bin folder and obj folder.Rebuild the project again and run