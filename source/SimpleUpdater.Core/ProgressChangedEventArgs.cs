using System;

namespace SimpleUpdater.Core
{
    internal class ProgressChangedEventArgs : EventArgs
    {
        public double Progress { get; private set; }

        public ProgressChangedEventArgs(double progress)
        {
            this.Progress = progress;
        }
    }
}