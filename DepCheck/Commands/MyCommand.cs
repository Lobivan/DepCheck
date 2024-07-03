using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TaskStatusCenter;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DepCheck
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.StatusBar.ShowProgressAsync("Checking options", 1, 3);

            var sol = await VS.Solutions.GetActiveProjectAsync();
            if (sol == null)
            {
                await VS.MessageBox.ShowWarningAsync("No active project!");
                return;
            }
            string solutionDir = Path.GetDirectoryName(sol.FullPath);
            solutionDir = Path.GetDirectoryName(solutionDir);

            string cmdString = await ConstructCommandStringAsync(solutionDir);
            if (cmdString == "")
                return;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = cmdString;

            var opts = await Options.GetLiveInstanceAsync();
            if (opts.ShowTrminal == TerminalState.doNotShow)
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            await VS.StatusBar.ShowProgressAsync("Started CVE check", 2, 3);
            proc.Start();
            proc.WaitForExit();
            await VS.StatusBar.ShowProgressAsync("CVE check Done", 3, 3);

            if (opts.AutoOpenReport)
                System.Diagnostics.Process.Start(solutionDir + "\\dependency-check-report.html");
            else
                await VS.MessageBox.ShowAsync($"Check is comlete, report can be found at {solutionDir}\\dependency-check-report.html");
        }
        private async Task<string> ConstructCommandStringAsync(string solutionDir)
        {
            var opts = await Options.GetLiveInstanceAsync();

            char cmdPrefix = 'c';
            if (opts.ShowTrminal == TerminalState.showAndKeepWhenDone)
                cmdPrefix = 'k';

            if (File.Exists(opts.DCPath) == false)
            {
                await VS.MessageBox.ShowWarningAsync($"File {opts.DCPath} does not exist!",
                    "Change DC executable in Tools > Options > DepCheck > Paths > Path to DC");
                return "";
            }

            if (opts.ReportPath == "")
            {
                opts.ReportPath = solutionDir;
                await opts.SaveAsync();
            }

            if (Directory.Exists(opts.ReportPath) == false)
            {
                await VS.MessageBox.ShowWarningAsync($"File {opts.ReportPath} does not exist!",
                    "Change Report destination in Tools > Options > DepCheck > Paths > Path to DC Report");
                return "";
            }

            string result = $"/{cmdPrefix} {opts.DCPath} -o {opts.ReportPath} -s {solutionDir}";
            return result;
        }
    }
}
