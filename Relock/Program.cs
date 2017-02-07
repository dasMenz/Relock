using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
/* be sure this program is compiled for the right system architectur like x86 or x64 otherwise the file will not be found cause the os redirect the system path based on this!
* http://stackoverflow.com/a/29057814
* https://msdn.microsoft.com/en-us/library/windows/desktop/aa384187%28v=vs.85%29.aspx
*/
namespace Relock {
    class Program {
        static void Main( string[] args ) {
            if(args.Length > 0) {
                string drive_letter = args[0].Replace( "\"", "" ).Replace( "\\", "" );
                bool mb_exists = File.Exists( FindExePath( "manage-bde.exe" ) );

                ProcessStartInfo pInfo = new ProcessStartInfo( FindExePath( "manage-bde.exe" ) );
                pInfo.Arguments = "-lock " + drive_letter;
                pInfo.WindowStyle = ProcessWindowStyle.Hidden;
                pInfo.CreateNoWindow = true;
                int exitCode;

                using(Process p = Process.Start( pInfo )) {
                    p.WaitForExit();

                    exitCode = p.ExitCode;
                }
                if(exitCode == 0) {
                    MessageBox.Show( "Drive \"" + drive_letter.ToUpper() + "\" successfully locked", "Relock", MessageBoxButtons.OK, MessageBoxIcon.Information );
                } else {
                    MessageBox.Show( "Drive \"" + drive_letter.ToUpper() + "\" could not be locked\r\nExit Code: " + exitCode, "Relock", MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
            } else {
                // ask to update registry
                if(MessageBox.Show( "You did not supply a drive letter. I assume you want to update your registry?", "Relock", MessageBoxButtons.OKCancel, MessageBoxIcon.Information ) == DialogResult.OK) {
                    updateRegistry();
                } else {
                    // show message about default useage
                    MessageBox.Show( "The program requires a drive letter as first parameter.", "Relock", MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
            }
        }
        public static string FindExePath( string exe ) {
            exe = Environment.ExpandEnvironmentVariables( exe );
            if(!File.Exists( exe )) {
                if(Path.GetDirectoryName( exe ) == String.Empty) {
                    foreach(string test in ( Environment.GetEnvironmentVariable( "PATH" ) ?? "" ).Split( ';' )) {
                        string path = test.Trim();
                        if(!String.IsNullOrEmpty( path ) && File.Exists( path = Path.Combine( path, exe ) ))
                            return Path.GetFullPath( path );
                    }
                }
                throw new FileNotFoundException( new FileNotFoundException().Message, exe );
            }
            return Path.GetFullPath( exe );
        }
        public static void updateRegistry() {
            string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Classes\\Drive\\shell\\relock-bde";

            string vDefault = (string) Registry.GetValue( keyName, "", null );   // default value of the key
            string vHasLUAShield = (string) Registry.GetValue( keyName, "HasLUAShield", null );
            string vAppliesTo = (string) Registry.GetValue( keyName, "AppliesTo", null );
            string vMultiSelectModel = (string) Registry.GetValue( keyName, "MultiSelectModel", null );

            const string _vDefault = "Relock this drive";
            const string _vHasLUAShield = "";
            const string _vAppliesTo = "System.Volume.BitLockerProtection:=System.Volume.BitLockerProtection#On OR System.Volume.BitLockerProtection:=System.Volume.BitLockerProtection#Encrypting OR System.Volume.BitLockerProtection:=System.Volume.BitLockerProtection#Suspended";
            const string _vMultiSelectModel = "Single";

            try {
                // setup the context menu entries
                if(vDefault != _vDefault) {
                    Registry.SetValue( keyName, "", _vDefault );
                }
                if(vHasLUAShield != _vHasLUAShield) {
                    Registry.SetValue( keyName, "HasLUAShield", _vHasLUAShield );
                }
                if(vAppliesTo != _vAppliesTo) {
                    Registry.SetValue( keyName, "AppliesTo", _vAppliesTo );
                }
                if(vMultiSelectModel != _vMultiSelectModel) {
                    Registry.SetValue( keyName, "MultiSelectModel", _vMultiSelectModel );
                }

                // setup the command regedit
                string commandKeyName = keyName + "\\command";

                string vCommandValue = (string) Registry.GetValue( commandKeyName, "", null );
                string _vCommandValue = System.Reflection.Assembly.GetExecutingAssembly().Location;

                if(vCommandValue.Replace( " %1", "" ) != _vCommandValue) {
                    Registry.SetValue( commandKeyName, "", _vCommandValue + " %1" );
                }
            } catch(Exception ex) {
                MessageBox.Show( ex.Message, "Error while updating registry", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }
    }
}
