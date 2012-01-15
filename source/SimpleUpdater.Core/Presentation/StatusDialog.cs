using System;
using System.Globalization;
using System.Windows.Forms;

namespace SimpleUpdater.Core.Presentation
{
    public partial class StatusDialog : Form
    {
        public StatusDialog()
        {
            InitializeComponent();
        }

        public string AppTitle
        {
            get { return lblAppTitle.Text; }
            set { lblAppTitle.Text = value; }
        }

        public void SetVersion(Version version)
        {
            lblVersionInfo.Text = String.Format(CultureInfo.CurrentCulture, "Ein Update auf Version {0} wird durchgeführt.", version);
        }

        public bool IsMarquee
        {
            get { return progressBar.Style == ProgressBarStyle.Marquee; }
            set { progressBar.Style = value ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous; }
        }

        public void SetValue(double value)
        {
            if (value > 1 || value < 0)
                throw new ArgumentOutOfRangeException("value");

            progressBar.Value = (int)((progressBar.Maximum - progressBar.Minimum) * value + progressBar.Minimum);
        }

        public string Description
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }
    }
}
