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

            var opts = await Options.GetLiveInstanceAsync();
            char cmdPrefix = 'c';
            if (opts.ShowTrminal == TerminalState.showAndKeepWhenDone)
            {
                cmdPrefix = 'k';
            }
            else if (opts.ShowTrminal == TerminalState.doNotShow)
            {
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            if (File.Exists(opts.DCPath) == false)
            {
                await VS.MessageBox.ShowWarningAsync($"File {opts.DCPath} does not exist!", 
                    "Change DC executable in Tools > Options > DepCheck > Paths > Path to DC");
                return;
            }
            if (opts.ReportPath == "")
                opts.ReportPath = solutionDir;
            if (Directory.Exists(opts.ReportPath) == false)
            {
                await VS.MessageBox.ShowWarningAsync($"File {opts.ReportPath} does not exist!",
                    "Change Report destination in Tools > Options > DepCheck > Paths > Path to DC Report");
                return;
            }

            startInfo.Arguments = $"/{cmdPrefix} {opts.DCPath} -o {opts.ReportPath} -s {solutionDir}";
            proc.StartInfo = startInfo;
            proc.Start();
            proc.WaitForExit();
            System.Diagnostics.Process.Start(solutionDir + "\\dependency-check-report.html");
        }
    }
}
