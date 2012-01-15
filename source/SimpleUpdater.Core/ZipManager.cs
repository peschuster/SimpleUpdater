using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace SimpleUpdater.Core
{
	/// <summary>
	/// Packs and unpacks zip files
	/// </summary>
	internal class ZipManager
	{
		/// <summary>
		/// Creates a zip file out of specified input files
		/// </summary>
		/// <param name="files">input files</param>
		/// <param name="zipFileName">output zip file</param>
		/// <exception cref="System.ArgumentException">No file must exist with specified zip file name.</exception>
		public void Zip(FileInfo[] files, string zipFileName)
		{
			if (File.Exists(zipFileName))
				throw new ArgumentException("Es existiert bereits eine Datei unter diesem Namen.", "zipFileName");

			using (FileStream fs = new FileStream(zipFileName, FileMode.CreateNew))
			{
				using (ZipOutputStream stream = new ZipOutputStream(fs))
				{
					stream.SetLevel(9);

					foreach (FileInfo file in files)
					{
						ZipEntry entry = new ZipEntry(file.Name)
							{
								DateTime = DateTime.Now
							};

						using (FileStream entryFs = file.OpenRead())
						{
							byte[] buffer = new byte[entryFs.Length];

							entryFs.Read(buffer, 0, buffer.Length);
							entry.Size = entryFs.Length;

							stream.PutNextEntry(entry);
							stream.Write(buffer, 0, buffer.Length);
						}
					}

					stream.Finish();
				}
			}
		}

		/// <summary>
		/// Unzips zip files into directory.
		/// Returns unzipped files.
		/// </summary>
		/// <param name="zipFileName">input zip file</param>
		/// <param name="unzipDir">output directory</param>
		/// <returns>Unzipped files</returns>
		public FileInfo[] Unzip(string zipFileName, string unzipDir)
		{
			List<FileInfo> files = new List<FileInfo>();
			
			using (FileStream fs = new FileStream(zipFileName, FileMode.Open))
			{
				using (ZipInputStream zipStream = new ZipInputStream(fs))
				{
					ZipEntry entry = null;
					
					while ((entry = zipStream.GetNextEntry()) != null)
					{
						string destination = Path.Combine(unzipDir, entry.Name);

						if (entry.IsDirectory && !Directory.Exists(destination))
						{
							Directory.CreateDirectory(destination);
						}
						else if (!entry.IsDirectory)
						{
							if (!Directory.Exists(Path.GetDirectoryName(destination)))
								Directory.CreateDirectory(Path.GetDirectoryName(destination));

							FileInfo file = new FileInfo(destination);
							using (FileStream entryFs = file.OpenWrite())
							{
								Byte[] data = new Byte[2048];
								int length = 0;

								while ((length = zipStream.Read(data, entry.Offset, data.Length)) > 0)
								{
									entryFs.Write(data, 0, length);
								}
							}

							files.Add(file);
						}
					}
				}
			}

			return files.ToArray();
		}
	}
}
