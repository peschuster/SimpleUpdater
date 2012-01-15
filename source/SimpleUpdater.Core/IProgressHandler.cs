using System;

namespace SimpleUpdater.Core
{
    internal interface IProgressHandler
    {
        void Reset();

        void OnProgressChanged(object sender, ProgressChangedEventArgs e);

        void SetVersion(Version version);

        void SetText(string text);
    }
}
