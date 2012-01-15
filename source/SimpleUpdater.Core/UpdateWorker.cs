using System;
using System.ComponentModel;

namespace SimpleUpdater.Core
{
    internal class UpdateWorker : IDisposable
    {
        private readonly BackgroundWorker worker;

        private Action action;

        public event RunWorkerCompletedEventHandler WorkCompleted;

        public UpdateWorker(Action action)
        {
            this.worker = new BackgroundWorker();
            this.worker.DoWork += OnWork;
            this.worker.RunWorkerCompleted += OnCompleted;

            this.action = action;
        }

        private void OnWork(object sender, DoWorkEventArgs e)
        {
            if (this.action != null)
            {
                this.action();
            }
        }

        private void OnCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.WorkCompleted != null)
            {
                this.WorkCompleted(this, e);
            }
        }

        public void Start()
        {
            this.worker.RunWorkerAsync();
        }

        public void Dispose()
        {
            if (this.worker != null)
            {
                this.worker.Dispose();
            }
        }
    }
}
