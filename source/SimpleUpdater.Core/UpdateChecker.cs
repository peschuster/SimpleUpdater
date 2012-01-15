using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleUpdater.Core
{
    public class UpdateChecker
    {
        private Version version;

        private string binDir;

        private readonly Uri feedUrl;

        private readonly string publicKey;

        private readonly string appTitle;

        public UpdateChecker(Version version, string binDir, string feedUrl, string publicKey, string appTitle)
        {
            this.version = version;
            this.binDir = binDir;
            this.feedUrl = new Uri(feedUrl);
            this.publicKey = publicKey;
            this.appTitle = appTitle;
        }

        /// <exception cref="System.InvalidOperationExcetion"></exception>
        public bool IsUpdateAvailable()
        {
            var feedManager = new FeedManager(this.publicKey);

            var newVersions = feedManager.GetNewVersions(feedUrl, version);

            if (newVersions == null)
                throw new InvalidOperationException("Unable to load feed data.");

            return newVersions.Any();
        }

        public void StartUpdate(bool restart)
        {
            Process.Start(
                Path.Combine(this.binDir, "SimpleUpdater.exe"), 
                String.Join(
                    " ", 
                    new[]
                    {
                        "--version=" + this.version.ToString(),
                        "--binDir=\"" + this.binDir + "\"",
                        "--feedUrl=\"" + this.feedUrl + "\"",
                        "--publicKey=\"" + this.publicKey + "\"",
                        "--current=\"" + Assembly.GetEntryAssembly().Location + "\"",
                        "--appTitle=\"" + this.appTitle + "\"",
                        "--restart=" + restart,
                    }));
        }
    }
}
