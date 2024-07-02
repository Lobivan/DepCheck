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
            string solutionDir = Path.GetDirectoryName(sol.FullPath);
            solutionDir = Path.GetDirectoryName(solutionDir);
            await VS.MessageBox.ShowWarningAsync(solutionDir);

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/k C:\\Users\\79607\\Downloads\\dependency-check\\bin\\dependency-check.bat -o {solutionDir} -s {solutionDir}";
            proc.StartInfo = startInfo;
            proc.Start();
            proc.WaitForExit();
            System.Diagnostics.Process.Start(solutionDir + "\\dependency-check-report.html");
        }
    }
}
