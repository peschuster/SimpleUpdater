using System;
using System.IO;

namespace SimpleUpdater.Core
{
    /// <summary>
    /// Context of update operation
    /// </summary>
    internal interface IUpdateContext
    {
        /// <summary>
        /// Temporary directory for downloads, etc.
        /// </summary>
        DirectoryInfo TempDirectory { get; }

        /// <summary>
        /// Installed version of the application.
        /// </summary>
        Version ApplicationVersion { get; }

        /// <summary>
        /// Directory of the application.
        /// </summary>
        DirectoryInfo ApplicationDirectory { get; }
        /// <summary>
        /// Gets the application title.
        /// </summary>
        /// <value>The application title.</value>
        string ApplicationTitle { get; }
    }
}
