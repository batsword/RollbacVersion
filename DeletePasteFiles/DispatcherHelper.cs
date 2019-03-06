/*

* ==============================================================================
*
* Filename: UdpHelper
* Description: 
*
* Version: 1.0
* Created: 2018.11.13
* Compiler: Visual Studio 2015
*
* Author: zyj
* Company: nined
*
* ==============================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;

namespace NDKTV.Utils.Helpers
{
    public static class DispatcherHelper
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
            try { Dispatcher.PushFrame(frame); }
            catch (InvalidOperationException) { }
        }
        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
        public static void ShowText(this DependencyObject obj, string msg)
        {
            TextBox textbox = obj as TextBox;
            if (textbox != null)
            {
                obj.Dispatcher.BeginInvoke(new Action(() =>
                {
                    textbox.Text = msg;

                }));
            }
        }
    }


}
