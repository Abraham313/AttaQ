using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Security.Principal;
using System.Diagnostics;

namespace AttaQ
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // 관리자 권한으로 실행
            if(IsAdmin() == false)
            {
                try
                {
                    ProcessStartInfo procinfo = new ProcessStartInfo();
                    procinfo.UseShellExecute = true;
                    procinfo.FileName = Application.ExecutablePath;
                    procinfo.WorkingDirectory = Environment.CurrentDirectory;
                    procinfo.Verb = "runas";
                    Process.Start(procinfo);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

                return;
            }



            // Log File 있는지 확인하고 그에 맞는 폼 실행
            FileInfo finfo = new FileInfo("C:\\WINDOWS\\log.Q");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (finfo.Exists)
            {
                Application.Run(new Attack());
            }
            else
            {
                Application.Run(new Start());
            }
        }
        public static bool IsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if(null != identity)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }
    }
}
