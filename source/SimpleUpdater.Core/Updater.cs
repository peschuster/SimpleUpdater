using System;
using System.IO;
using System.Linq;
using SimpleUpdater.Core.Presentation;
using System.Threading;

namespace SimpleUpdater.Core
{
    public class Updater : IDisposable
    {
        private readonly IUpdateContext context;

        private readonly Uri feedUrl;

        private readonly string publicKey;

        private bool disposed;

        private StatusDialog dialog;

        private StatusDialogAdapter dialogAdapter;

        private UpdateWorker worker;

        public Updater(Version version, string binDir, string feedUrl, string publicKey, string appTitle)
        {
            this.context = new UpdateContext
            {
                ApplicationDirectory = new DirectoryInfo(binDir),
                TempDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())),
                ApplicationVersion = version,
                ApplicationTitle = appTitle,
            };

            this.feedUrl = new Uri(feedUrl);
            this.publicKey = publicKey;
        }

        public StatusDialog UpdateApplication(EventWaitHandle asyncHandle)
        {
            var feedManager = new FeedManager(this.publicKey);
            var newVersions = feedManager.GetNewVersions(feedUrl, this.context.ApplicationVersion);

            if (dialog == null)
            {
                this.dialog = new StatusDialog();
                this.dialog.AppTitle = this.context.ApplicationTitle;

                this.dialogAdapter = new StatusDialogAdapter(dialog);
            }

            worker = new UpdateWorker(
                () =>
                {
                    var downloadManager = new DownloadManager(this.context, dialogAdapter);
                    var files = downloadManager.DownloadFiles(newVersions);

                    var installer = new InstallationManager(this.context, new TraceLogger());
                    foreach (var file in files.OrderBy(f => f.Key))
                    {
                        installer.Install(file.Value, file.Key);
                    }
                });

            worker.WorkCompleted += (object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
                =>
                {
                    if (dialog != null)
                    {
                        dialog.Close();
                        dialog.Dispose();

                        dialogAdapter = null;
                        dialog = null;
                    }

                    if (asyncHandle != null)
                    {
                        asyncHandle.Set();
                    }
                };

            worker.Start();

            return this.dialog;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (context != null && context.TempDirectory != null)
                    {
                        if (context.TempDirectory.Exists)
                            context.TempDirectory.Delete(true);                        
                    }

                    if (dialog != null)
                    {
                        dialogAdapter = null;
                        dialog.Dispose();
                    }
                }

                this.disposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
