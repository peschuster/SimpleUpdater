using System;
using SimpleUpdater.Core.Presentation;
using System.Windows.Forms;

namespace SimpleUpdater.Core
{
    internal class StatusDialogAdapter : IProgressHandler
    {
        private readonly StatusDialog dialog;

        public StatusDialogAdapter(StatusDialog dialog)
        {
            this.dialog = dialog;
        }

        public void Reset()
        {
            dialog.Invoke((MethodInvoker)delegate
            {
                dialog.IsMarquee = true;
                dialog.SetValue(0);
                dialog.Description = String.Empty;
            });
        }

        public void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e == null)
                return;
            
            double progress = Math.Max(0, Math.Min(e.Progress, 1));

            dialog.Invoke((MethodInvoker)delegate 
            {
                dialog.IsMarquee = false;
                dialog.SetValue(progress); 
            });
        }

        public void SetText(string text)
        {
            dialog.Invoke((MethodInvoker)delegate { dialog.Description = text; });
        }

        public void SetVersion(Version version)
        {
            dialog.Invoke((MethodInvoker)delegate { dialog.SetVersion(version); });
        }
    }
}
