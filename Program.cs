using System;
using System.IO;
using System.Diagnostics;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

using MS.WindowsAPICodePack.Internal;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace ConsoleToast
{
    class Program
    {
        static void Main(string[] args)
        {
            ShortCutCreator.TryCreateShortcut("ConsoleToast.App", "ConsoleToast");

            Console.WriteLine("Type 'exit' to quit. ENTER to show a notification");

            while (Console.ReadLine() != "exit")
            {
                ShowImageToast(
                    "ConsoleToast.App",
                    DateTime.Now.ToLongTimeString() + " title with image",
                    "this is a message",
                    Path.GetFullPath("plasticlogo.png"));
                /*ShowTextToast(
                    "ConsoleToast.App",
                    DateTime.Now.ToLongTimeString() + "title",
                    "this is a message");*/
            }
        }

        static void ShowTextToast(string appId, string title, string message)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(
                ToastTemplateType.ToastText02);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);

            ToastEvents events = new ToastEvents();

            toast.Activated += events.ToastActivated;
            toast.Dismissed += events.ToastDismissed;
            toast.Failed += events.ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId
            // on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(appId).Show(toast);
        }

        static void ShowImageToast(string appId, string title, string message, string image)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(
                ToastTemplateType.ToastImageAndText02);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            // Specify the absolute path to an image
            String imagePath = "file:///" + image;
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);

            ToastEvents events = new ToastEvents();

            toast.Activated += events.ToastActivated;
            toast.Dismissed += events.ToastDismissed;
            toast.Failed += events.ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId
            // on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(appId).Show(toast);
        }

        class ToastEvents
        {
            internal void ToastActivated(ToastNotification sender, object e)
            {
                Console.WriteLine("User activated the toast");
            }

            internal void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs e)
            {
                String outputText = "";
                switch (e.Reason)
                {
                    case ToastDismissalReason.ApplicationHidden:
                        outputText = "The app hid the toast using ToastNotifier.Hide";
                        break;
                    case ToastDismissalReason.UserCanceled:
                        outputText = "The user dismissed the toast";
                        break;
                    case ToastDismissalReason.TimedOut:
                        outputText = "The toast has timed out";
                        break;
                }

                Console.WriteLine(outputText);
            }

            internal void ToastFailed(ToastNotification sender, ToastFailedEventArgs e)
            {
                Console.WriteLine("The toast encountered an error.");
            }
        }

        static class ShortCutCreator
        {
            // In order to display toasts, a desktop application must have
            // a shortcut on the Start menu.
            // Also, an AppUserModelID must be set on that shortcut.
            // The shortcut should be created as part of the installer.
            // The following code shows how to create
            // a shortcut and assign an AppUserModelID using Windows APIs.
            // You must download and include the Windows API Code Pack
            // for Microsoft .NET Framework for this code to function

            internal static bool TryCreateShortcut(string appId, string appName)
            {
                String shortcutPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData) +
                    "\\Microsoft\\Windows\\Start Menu\\Programs\\" + appName + ".lnk";
                if (!File.Exists(shortcutPath))
                {
                    InstallShortcut(appId, shortcutPath);
                    return true;
                }
                return false;
            }

            static void InstallShortcut(string appId, string shortcutPath)
            {
                // Find the path to the current executable
                String exePath = Process.GetCurrentProcess().MainModule.FileName;
                IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

                // Create a shortcut to the exe
                VerifySucceeded(newShortcut.SetPath(exePath));
                VerifySucceeded(newShortcut.SetArguments(""));

                // Open the shortcut property store, set the AppUserModelId property
                IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

                using (PropVariant applicationId = new PropVariant(appId))
                {
                    VerifySucceeded(newShortcutProperties.SetValue(
                        SystemProperties.System.AppUserModel.ID, applicationId));
                    VerifySucceeded(newShortcutProperties.Commit());
                }

                // Commit the shortcut to disk
                IPersistFile newShortcutSave = (IPersistFile)newShortcut;

                VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
            }

            static void VerifySucceeded(UInt32 hresult)
            {
                if (hresult <= 1)
                    return;

                throw new Exception("Failed with HRESULT: " + hresult.ToString("X"));
            }
        }
    }
}