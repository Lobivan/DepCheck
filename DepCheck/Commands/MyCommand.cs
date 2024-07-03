using EnvDTE;
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
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = cmdString;
            proc.StartInfo = startInfo;
            proc.Start();
            proc.WaitForExit();

            System.Diagnostics.Process.Start(solutionDir + "\\dependency-check-report.html");
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
