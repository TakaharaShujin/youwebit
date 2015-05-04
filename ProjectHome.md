**Copy YouWebIt in any directory, run it. You are browsing a website on your desktop.**

## How it works? ##
YouWebIt start a WebServer in the starting Directory, then it start default web browser.
  * No installation needed
  * No specific requirement needed
  * No configuration needed
  * No technical knowledge needed
  * No internet connection needed
YouWebIt search for a frequently used start page name in the current directory then launch your default web browser.
YouWebIt search for index.html, default.html, default.aspx... then it search for .htm, .html, .aspx but it's possible to specify a specific filename by using the "Save config" tray icon menu item.

## Download ##
[Last Release(fixing x64 processor trouble)](http://youwebit.googlecode.com/files/YouWebIt1.1.exe)


## Articles ##
### YouWebIt published on SharpToolbox ###
This project is born to facilitate RIA project managment. Lot of people involved durring this kind of project do not have any webserver on their desktop to run the project.
Developpers are not the only one to be interrested about an RIA project status : Sales team, managers, crea team, direction, customers are too...[Read more here](http://blogs.developpeur.org/max/archive/2008/09/19/youwebit-on-sharptoolbox-boost-your-ria-project-managment.aspx)
### YouWebIt first drop ###
Do you have some Asp.Net ot Static Html website files on your desktop? Are you working on Asp.Net website? Full Static Html? Ajax? Silverlight? Flash website? Flex one?

How do you browse them? Do you setup an IIS website? Do you open Visual Studio then press Ctrl+F5? Blend? Do you know Cassini web sevrer? Do your boss or your mother can do it?

I guess you know theese tools but what about some simple scénario :

You : Hey boss! I just send you my website by mail. What do think about it?
Boss : Are you kidding me? [Read more here](http://blogs.developpeur.org/max/archive/2008/04/20/i-just-publish-youwebit-on-google-code-an-easy-desktop-webserver.aspx)

## YouWebIt TrayIcon Menu ##
![http://blogs.developpeur.org/photos/mdislaire/images/38895/original.aspx?image.jpg](http://blogs.developpeur.org/photos/mdislaire/images/38895/original.aspx?image.jpg)

## Technical info ##
YouWebIt use ASP.Net development server (WebDev.WebHost.dll). WebDev.WebHost.dll is embedded as YouWebIt resource. Thank to Scott Handselman [article](http://www.hanselman.com/blog/NUnitUnitTestingOfASPNETPagesBaseClassesControlsAndOtherWidgetryUsingCassiniASPNETWebMatrixVisualStudioWebDeveloper.aspx) that enable YouWebIt to use WebDev.WebHost.dd without Visual Studio installed.


## Helpful links: ##
  * [WebDev.WebServer for easy website hosting](http://www.alexthissen.nl/blogs/main/archive/2006/12/15/webdev-webserver-for-easy-website-hosting.aspx)
  * [Using WebServer.WebDev For Unit Tests](http://haacked.com/archive/2006/12/12/using_webserver.webdev_for_unit_tests.aspx)
  * [NUnit Unit Testing of ASP.NET Pages, Base Classes, Controls and other widgetry using Cassini (ASP.NET Web Matrix/Visual Studio Web Developer)](http://www.hanselman.com/blog/NUnitUnitTestingOfASPNETPagesBaseClassesControlsAndOtherWidgetryUsingCassiniASPNETWebMatrixVisualStudioWebDeveloper.aspx)
  * [IIS 7.0 Hosting Proof of Concept](http://notgartner.wordpress.com/2004/12/19/iis-70-hosting-proof-of-concept)
  * [TinyWebServer, ideal portable webserver for asp.net project development. By Mehran Ghanizadeh](http://www.codeproject.com/KB/vb/TinyWebServer.aspx)
  * [Light IIS (Run Asp.net without IIS) By mohsen ahmadian](http://www.codeproject.com/KB/aspnet/LightIIS.aspx)

Few icons come from Silk icon set by Mark James, http://www.famfamfam.com/lab/icons/silk

Some code come from http://www.icsharpcode.net/OpenSource/SD/.

Any suggestion are welcome

Hope this help.

[Max](http://blogs.developpeur.org/max/archive/2008/04/20/i-just-publish-youwebit-on-google-code-an-easy-desktop-webserver.aspx)