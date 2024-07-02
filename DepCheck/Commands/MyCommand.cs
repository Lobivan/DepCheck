using EnvDTE;
using System.Diagnostics;
using System.IO;

namespace DepCheck
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var sol = await VS.Solutions.GetActiveProjectAsync();
            if (sol == null)
            {
                await VS.MessageBox.ShowWarningAsync("No active project!");
                return;
            }
            string solutionDir = Path.GetDirectoryName(sol.FullPath);
            solutionDir = Path.GetDirectoryName(solutionDir);

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";

            char cmdPrefix = 'c';
            var opts = await Options.GetLiveInstanceAsync();
            if (opts.ShowTrminal == TerminalState.showAndKeepWhenDone)
            {
                cmdPrefix = 'k';
            }
            else if (opts.ShowTrminal == TerminalState.doNotShow)
            {
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            startInfo.Arguments = $"/{cmdPrefix} C:\\Users\\79607\\Downloads\\dependency-check\\bin\\dependency-check.bat -o {solutionDir} -s {solutionDir}";
            proc.StartInfo = startInfo;
            proc.Start();
            proc.WaitForExit();
            System.Diagnostics.Process.Start(solutionDir + "\\dependency-check-report.html");
        }
    }
}
