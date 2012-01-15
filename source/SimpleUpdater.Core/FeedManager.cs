using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SimpleUpdater.Core.Import;

namespace SimpleUpdater.Core
{
    internal class FeedManager
    {
        private readonly XmlRsaSignature signature;

        public FeedManager(string publicKey)
        {
            this.signature = new XmlRsaSignature(publicKey);            
        }

        public List<VersionEntry> GetNewVersions(Uri address, Version appVersion)
        {
            UpdateInfoWrapper wrapper = new UpdateInfoWrapper(signature);

            string content;
            using (var client = new WebClient())
            {
                content = client.DownloadString(address);
            }

            if (wrapper.Load(content))
            {
                var versions = wrapper.Data.Versions
                    .Where(x => x.Version > appVersion)
                    .OrderByDescending(x => x.Version);

                return this.FilterVersions(versions, appVersion);
            }

            return null;
        }

        private List<VersionEntry> FilterVersions(IEnumerable<VersionEntry> versions, Version appVersion)
        {
            List<VersionEntry> result = new List<VersionEntry>();

            if (versions.Count() == 0 || versions.First().Version <= appVersion)
                return result;

            VersionEntry current = versions.First();
            result.Add(current);
            while (current.Required > appVersion)
            {
                current = versions.Single(x => x.Version == current.Required);
                result.Add(current);
            }

            return result;
        }
    }
}
