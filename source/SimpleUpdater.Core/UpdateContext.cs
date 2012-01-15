using System;
using System.IO;

namespace SimpleUpdater.Core
{
    internal class UpdateContext : IUpdateContext
    {
        /// <summary>
        /// Gets the temp directory.
        /// </summary>
        /// <value>The temp directory.</value>
        public DirectoryInfo TempDirectory { get; set; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public Version ApplicationVersion { get; set; }

        /// <summary>
        /// Gets the application directory.
        /// </summary>
        /// <value>The application directory.</value>
        public DirectoryInfo ApplicationDirectory { get; set; }

        /// <summary>
        /// Gets or sets the application title.
        /// </summary>
        /// <value>The application title.</value>
        public string ApplicationTitle { get; set; }
    }
}
