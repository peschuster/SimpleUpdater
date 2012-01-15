using System;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace SimpleUpdater.Core
{
    internal class InstallationManager
    {
        private const string PreUpdateFile = "PreUpdate.exe";

        private const string PostUpdateFile = "PostUpdate.exe";

        private string[] reservedFiles = new[] { PreUpdateFile, PostUpdateFile };

        private readonly IUpdateContext context;

        private readonly ILogger logger;

        public InstallationManager(IUpdateContext context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public void Install(FileInfo file, Version version)
        {
            DirectoryInfo destination = this.context.TempDirectory.CreateSubdirectory(version.ToString());

            try
            {
                ZipManager zipper = new ZipManager();
                zipper.Unzip(file.FullName, destination.FullName);

                string preUpdatePath = Path.Combine(destination.FullName, PreUpdateFile);
                if (File.Exists(preUpdatePath))
                {
                    Process preUpdate = new Process
                    {
                        StartInfo = new ProcessStartInfo(preUpdatePath, String.Concat("\"", this.context.ApplicationDirectory.FullName, "\""))
                    };

                    preUpdate.Start();
                    preUpdate.WaitForExit(15000);
                }

                this.MoveFiles(destination, this.context.ApplicationDirectory);

                string postUpdatePath = Path.Combine(destination.FullName, PostUpdateFile);
                if (File.Exists(postUpdatePath))
                {
                    Process postUpdate = new Process
                    {
                        StartInfo = this.GetProcessInfo(postUpdatePath, String.Concat("\"", this.context.ApplicationDirectory.FullName, "\""))
                    };

                    postUpdate.Start();
                    
                    postUpdate.BeginOutputReadLine();
                    this.logger.Info("PostUpdate: {0}", postUpdate.StandardOutput.ReadToEnd());

                    postUpdate.WaitForExit(15000);                    
                }
            }
            finally
            {
                destination.Delete(true);
            }

        }

        private ProcessStartInfo GetProcessInfo(string file, string arguments)
        {
            return new ProcessStartInfo(file, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
            };
        }

        private void MoveFiles(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (var file in source.GetFiles())
            {
                if (this.reservedFiles.Contains(file.Name))
                    continue;

                FileInfo destinationFile = file.Rebase(destination);

                if (destinationFile.Exists)
                {
                    destinationFile.Delete();
                    this.logger.Info("Replaced file {0}", file.Name);
                }

                file.MoveTo(destinationFile.FullName);
            }

            foreach (var dir in source.GetDirectories())
            {
                DirectoryInfo destinationDir = destination.Subdirectory(dir.Name);

                if (!destinationDir.Exists)
                {
                    destinationDir.Create();
                    this.logger.Info("Created directory {0}.", destinationDir);
                }

                this.MoveFiles(dir, destinationDir);
            }
        }
    }
}
