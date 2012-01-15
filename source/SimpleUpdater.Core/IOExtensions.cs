using System.IO;

namespace SimpleUpdater.Core
{
    /// <summary>
    /// Extensions for System.IO namespace.
    /// </summary>
    internal static class IOExtensions
    {
        /// <summary>
        /// Returns a subdirectory handle with specified name for the directory.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static DirectoryInfo Subdirectory(this DirectoryInfo dir, string name)
        {
            return new DirectoryInfo(Path.Combine(dir.FullName, name));
        }

        /// <summary>
        /// Returns handle to file underneath directory.
        /// </summary>
        /// <param name="dir">The parent directory.</param>
        /// <param name="name">The file name.</param>
        /// <returns></returns>
        public static FileInfo SubFile(this DirectoryInfo dir, string name)
        {
            return new FileInfo(Path.Combine(dir.FullName, name));
        }

        /// <summary>
        /// Rebases the file into the specified directory (virtually).
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="destination">The destination directory.</param>
        /// <returns></returns>
        public static FileInfo Rebase(this FileInfo file, DirectoryInfo destination)
        {
            return new FileInfo(Path.Combine(destination.FullName, file.Name));
        }
    }
}
