using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using SimpleUpdater.Core.Import;

namespace SimpleUpdater.Core
{
    internal class DownloadManager : IDisposable
    {
        private readonly IUpdateContext context;

        private readonly WebClient client;

        private readonly IProgressHandler handler;

        public event EventHandler<ProgressChangedEventArgs> DownloadProgress;

        private void OnDownloadProgress(long total, long current)
        {
            if (this.DownloadProgress == null)
                return;

            this.DownloadProgress(this, new ProgressChangedEventArgs(total == 0 ? 1 : ((double)current / total)));
        }

        public DownloadManager(IUpdateContext context, IProgressHandler handler)
        {
            this.handler = handler;
            this.context = context;

            this.client = new WebClient();
        }

        public Dictionary<Version, FileInfo> DownloadFiles(IEnumerable<VersionEntry> versions)
        {
            var result = new Dictionary<Version, FileInfo>();

            if (versions == null)
                return result;

            this.DownloadProgress += this.handler.OnProgressChanged;
            this.handler.Reset();

            int count = 0;
            foreach (var item in versions)
            {
                this.handler.SetVersion(item.Version);
                this.handler.SetText(String.Format("Lade Datei {0} von {1} herunter...", ++count, versions.Count()));

                byte[] md5Hash = Convert.FromBase64String(item.Md5Hash);
                FileInfo file = this.DownloadFile(new Uri(item.File), md5Hash);

                result.Add(item.Version, file);
            }

            this.DownloadProgress -= this.handler.OnProgressChanged;
            this.handler.Reset();

            return result;
        }

        private FileInfo DownloadCore(Uri address)
        {
            var webRequest = WebRequest.Create(address);
            webRequest.Credentials = CredentialCache.DefaultCredentials;

            var webResponse = webRequest.GetResponse();

            long fileSize = webResponse.ContentLength;

            string fileName = this.GetFilenameFromUri(address.AbsoluteUri);

            FileInfo localFile = context.TempDirectory.SubFile(fileName);

            using (FileStream localStream = localFile.OpenWrite())
            {
                using (var webStream = client.OpenRead(address))
                {
                    int bytesSize = 0;
                    byte[] buffer = new byte[2048];

                    // Loop through the buffer until the buffer is empty
                    while ((bytesSize = webStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        localStream.Write(buffer, 0, bytesSize);

                        this.OnDownloadProgress(fileSize, localStream.Length);
                    }
                }
            }

            return localFile;
        }

        public FileInfo DownloadFile(Uri url, byte[] md5Hash = null)
        {
            FileInfo localFile = this.DownloadCore(url);

            if (md5Hash != null && !this.CheckMd5Hash(localFile.FullName, md5Hash))
            {
                localFile.Delete();

                throw new InvalidOperationException(String.Format("Invalid file hash: {0}", url));
            }

            return localFile;
        }

        private string GetFilenameFromUri(string uri)
        {
            if (String.IsNullOrEmpty(uri))
                throw new ArgumentNullException("uri");

            uri = uri.Replace('\\', '/');

            Regex split = new Regex(@"(\?|\#)");
            string urlPart = split.Split(uri).First();

            return urlPart.Split('/').Last();
        }

        public bool CheckMd5Hash(byte[] data, byte[] hash)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                byte[] dataHash = md5.ComputeHash(data);

                return dataHash.SequenceEqual(hash);
            }
        }

        public bool CheckMd5Hash(string fileName, byte[] hash)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    byte[] dataHash = md5.ComputeHash(stream);

                    return dataHash.SequenceEqual(hash);
                }
            }
        }
    
        public void Dispose()
        {
            client.Dispose();
        }
    }
}
