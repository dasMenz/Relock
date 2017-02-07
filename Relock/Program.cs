using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
/* be sure this program is compiled for the right system architectur like x86 or x64 otherwise the file will not be found cause the os redirect the system path based on this!
 * http://stackoverflow.com/a/29057814
 * https://msdn.microsoft.com/en-us/library/windows/desktop/aa384187%28v=vs.85%29.aspx
 */
namespace Relock
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string drive_letter = args[0].Replace("\"", "");
                bool mb_exists = File.Exists(FindExePath("manage-bde.exe"));

                ProcessStartInfo pInfo = new ProcessStartInfo(FindExePath("manage-bde.exe"));
                pInfo.Arguments = "-lock " + drive_letter;
                pInfo.WindowStyle = ProcessWindowStyle.Hidden;
                pInfo.CreateNoWindow = true;
                int exitCode;

                using (Process p = Process.Start(pInfo))
                {
                    p.WaitForExit();

                    exitCode = p.ExitCode;
                }
                if (exitCode == 0)
                {
                    MessageBox.Show("Drive \"" + drive_letter.ToUpper() + "\" successfully locked");
                }
                else
                {
                    MessageBox.Show("Drive \"" + drive_letter.ToUpper() + "\" could not be locked\r\nExit Code: " + exitCode);
                }
            } else
            {
                MessageBox.Show("The program requires a drive letter as first parameter.");
            }
        }
        public static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                            return Path.GetFullPath(path);
                    }
                }
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            return Path.GetFullPath(exe);
        }
    }
}
