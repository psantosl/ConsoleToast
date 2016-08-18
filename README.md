# ConsoleToast
Display Windows Toast messages from a Console app.

Read this blogpost for more info: http://blog.plasticscm.com/2016/08/how-to-send-windows-toast-notifications.html

![alt tag](https://raw.githubusercontent.com/psantosl/ConsoleToast/master/img/consoletoast-animation.gif)

## What are Toasts
They are the pop up messages that show up on Windows since version 8. Apps can create their own. Normally they are a pain, and a great source of distraction, but sometimes it is good to show some info to the user.

## What it is about
Displaying Toast messages on Windows is quite simple if you are developing Windows Store apps. But when you try from a Desktop app, it becomes slightly more difficult.

This is the info to do it from Desktop Apps: https://code.msdn.microsoft.com/windowsdesktop/sending-toast-notifications-71e230a2 but I didn't find a way to do it from Console apps.

It is not difficult, in fact, it is exactly the same as Desktop apps, the only thing is that you have to be very careful setting ups the references, that's all.

## References are the key
I explain how to setup the references in one of the commits. It is they key part to use Toast (and other Windows.Core APIs) from the Desktop/Console applications. Check https://msdn.microsoft.com/en-us/library/windows/apps/jj856306.aspx for more info.


