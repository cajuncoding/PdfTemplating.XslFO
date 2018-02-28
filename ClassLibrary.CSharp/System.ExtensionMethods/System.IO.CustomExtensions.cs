/*
Copyright 2012 Brandon Bernard

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
//using System.Configuration;
using System.CustomExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.IO.IsolatedStorage;

namespace System.IO.CustomExtensions
{
	public static class SystemStreamReaderCustomExtensions
	{
		/// <summary>
		/// Safely, resets the position to the Beginning of the stream (Position 0 / SeekOrigin.Begin) regardless of the Stream type and support for Seeking.  Streams that
		/// do not support Seek functionality will simply retain their original positioning before the call.
		/// </summary>
		/// <param name="objThis"></param>
		/// <returns></returns>
		public static StreamReader Reset(this StreamReader objThis)
		{
			return objThis.Reset(SeekOrigin.Begin);
		}

		/// <summary>
		/// Safely, resets the position to the specificid Location (SeekOrigin.Begin/SeekOrigin.End) regardless of the Stream type and support for Seeking.  Streams that
		/// do not support Seek functionality will simply retain thier original positioning before the call.
		/// </summary>
		/// <param name="objThis"></param>
		/// <param name="enumOrigin"></param>
		/// <returns></returns>
		public static StreamReader Reset(this StreamReader objThis, SeekOrigin enumOrigin)
		{
			//Reset the Underlying base Stream (via existing Extension Methods)
			objThis.BaseStream.Reset(enumOrigin);

			//Return the current Object to support Safe Chaining!
			return objThis;
		}

		/// <summary>
		/// Reads All data from the StreamReader into a string; from beginning to end.
		/// </summary>
		/// <param name="objThis"></param>
		/// <returns></returns>
		public static String ReadAll(this StreamReader objThis)
		{
			String output = String.Empty;
			if (objThis != null)
			{
				Stream stream = objThis.BaseStream;
				if (stream.CanRead)
				{
					output = objThis.Reset().ReadToEnd();
				}
			}
			return output;
		}
	}

	public static class SystemStreamCustomExtensions
	{
		/// <summary>
		/// Returns a StreamReader for the Stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static StreamReader GetReader(this Stream stream)
		{
			return new StreamReader(stream);
		}

		/// <summary>
		/// Returns a StreamReader for the Stream using the specified Encoding.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="objEncoding"></param>
		/// <returns></returns>
		public static StreamReader GetReader(this Stream stream, Encoding objEncoding)
		{
			//NOTE:  This code is SAFE code and will return an empty Stream() if needed so that it
			//       can be chained with other extension methods, safely.
			return new StreamReader(stream ?? new MemoryStream(), objEncoding);
		}

		/// <summary>
		/// Reads All data from the Stream into a string; from beginning to end. Uses default UTF-8 encoding. Saves the step of producing a StreamReader.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static string ReadString(this Stream stream)
		{
			return stream.ReadString(false);
		}

		/// <summary>
		/// Reads All data from the Stream into a string; from beginning to end. Uses default UTF-8 encoding. Saves the step of producing a StreamReader.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static string ReadString(this Stream stream, bool bDisposeOfStream)
		{
			return stream.ReadString(Encoding.Default, bDisposeOfStream);
		}

		/// <summary>
		/// Reads All data from the Stream into a string using the specified Encoding; from beginning to end. Saves the step of producing a StreamReader.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="objEncoding"></param>
		/// <returns></returns>
		public static string ReadString(this Stream stream, Encoding objEncoding)
		{
			return stream.ReadString(objEncoding, false);
		}
		
		/// <summary>
		/// Reads All data from the Stream into a string using the specified Encoding; from beginning to end. Saves the step of producing a StreamReader.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="objEncoding"></param>
		/// <returns></returns>
		public static string ReadString(this Stream stream, Encoding objEncoding, bool bDisposeOfStream)
		{
			string output = String.Empty;
			try
			{
				if (stream != null)
				{
					if (bDisposeOfStream)
					{
						using (Stream streamToRead = stream)
						using (StreamReader reader = streamToRead.GetReader(objEncoding))
						{
							output = reader.ReadAll();
						}
					}
					else
					{
						output = stream.GetReader(objEncoding).ReadAll();
					}
				}
			}
			finally
			{
				if (bDisposeOfStream)
				{
					stream.Dispose();
				}
			}
			return output;
		}

		/// <summary>
		/// Reads the Stream as Binary Content and return a Byte Array of data.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static byte[] ReadBinary(this Stream stream)
		{
			using (MemoryStream memoryStream = stream.ToMemoryStream())
			{
				return memoryStream.ToArray();
			}
		}

		/// <summary>
		/// Safely, resets the position to the Beginning of the stream (Position 0 / SeekOrigin.Begin) regardless of the Stream type and support for Seeking.  Streams that
		/// do not support Seek functionality will simply retain thier original positioning before the call.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static Stream Reset(this Stream stream)
		{
			return stream.Reset(SeekOrigin.Begin);
		}

		/// <summary>
		/// Safely, resets the position to the specificid Location (SeekOrigin.Begin/SeekOrigin.End) regardless of the Stream type and support for Seeking.  Streams that
		/// do not support Seek functionality will simply retain thier original positioning before the call.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="enumOrigin"></param>
		/// <returns></returns>
		public static Stream Reset(this Stream stream, SeekOrigin enumOrigin)
		{
			if (stream == null) throw new ArgumentNullException("Stream object is null; Custom Extension Stream.Reset() cannot process a null stream.");

			//Attempt to Seek to the Beginning of the Stream if possible and necessary.
			//NOTE:  Some Streams do not support Seek functionality  such as WebRequest/WebResponse Streams.
			if (stream.CanSeek)
			{
				switch (enumOrigin)
				{
					case SeekOrigin.Begin:
						if (stream.Position != 0)
						{
							stream.Seek(0, SeekOrigin.Begin);
						}
						break;
					case SeekOrigin.End:
						if (stream.Position != stream.Length)
						{
							stream.Seek(0, SeekOrigin.End);
						}
						break;
					case SeekOrigin.Current:
						break;
				}
			}

			//Return the current Object to support Safe Chaining!
			return stream;
		}

		 /// <summary>
		/// Convert the Stream to a MemoryStream regardless of the current type of Stream or the current position in the Stream.  Supports custom size for the Read Buffer.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="bufferByteSize"></param>
		/// <returns></returns>
		public static MemoryStream ToMemoryStream(this Stream stream)
		{
			MemoryStream memoryStream = null;

			if (stream != null && stream.CanRead)
			{
				//NOTE: If the Stream is Not inherited from MemoryStream then we must transpose the stream
				//      otherwise we can simply cast the Stream.
				stream.Reset();
				if (!stream.GetType().IsAssignableFrom(typeof(MemoryStream)))
				{
					memoryStream = new MemoryStream();
					stream.CopyTo(memoryStream);
				}
				else
				{
					memoryStream = (MemoryStream)stream;
				}
			}
			else
			{
				memoryStream = new MemoryStream();
			}

			return memoryStream;
		}

		#region .Net 3.5 (and less) custom CopyTo implementation Methods -- NOT REQUIRED in .Net 4.0

		//BBernard
		//NOTE:  This is NOT required in .Net 4.0 which provides it's own internal version of the CopyTo Method!
		private const int _defaultBufferByteSize = 32768; //8192;
		public static void CopyTo(this Stream stream, Stream copyToStream)
		{
			stream.CopyTo(copyToStream, _defaultBufferByteSize);
		}

		public static void CopyTo(this Stream stream, Stream copyToStream, int bufferByteSize)
		{
			byte[] buffer = new byte[bufferByteSize];
			int read = 0;
			while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
			{
				copyToStream.Write(buffer, 0, read);
			}
		}

		#endregion

		public static void WriteFile(this Stream stream, FileInfo outputFileInfo)
		{
			if (stream == null)
			{
				throw new EndOfStreamException(String.Format("The stream is null or empty; a valid stream with must be specified to write out as a file."));
			}

			//Render the Pdf Output to the defined File Stream
			using (FileStream outputFileStream = outputFileInfo.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.None))
			{
				//BBernard - 04/06/2012
				//Implemented optimization to not 'copy the buffer' here, but using the optimal writing process similar to the above ToMemoryStream conversion.
				//byte[] bytes = objThis.ReadBinary();
				//outputFileStream.Write(bytes, 0, bytes.Length);
				stream.CopyTo(outputFileStream);
			}

			//Refresh the FileInfo to reflect that the file now exists!
			outputFileInfo.Refresh();
		}
	}


	/// <summary>
	/// Allows launching of file in the following known Process contexts.
	/// </summary>
	public enum LaunchInApp
	{
		Excel,
		Word,
		Notepad,
		DefaultAssociatedApp
	}

	public static class SystemFileInfoCustomExtensions
	{
		public static XDocument OpenXDocument(this FileInfo fileInfo)
		{
			return fileInfo.OpenXDocument(LoadOptions.None);
		}

		//public static System.Configuration.Configuration OpenExeConfigurationFile(this FileInfo fileInfo)
		//{
		//	System.Configuration.Configuration config = null;
		//	if (fileInfo.ExistsSafely())
		//	{
		//		ExeConfigurationFileMap configMap = new ExeConfigurationFileMap()
		//		{
		//			ExeConfigFilename = fileInfo.FullName
		//		};

		//		config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
		//	}
		//	return config;
		//}

		public static XDocument OpenXDocument(this FileInfo fileInfo, LoadOptions loadOptions)
		{
			XDocument xDoc = null;
			using (FileStream xmlStream = fileInfo.OpenRead())
			using (StreamReader streamReader = xmlStream.GetReader())
			{
				xDoc = XDocument.Load(streamReader, loadOptions);
			}
			return xDoc;
		}

		//public static bool IsLocked(this FileInfo objThis)
		//{
		//    return (objThis.GetLockingProcesses().Count > 0);
		//}

		//public static List<Process> GetLockingProcesses(this FileInfo objThis)
		//{
		//    String filePath = objThis.FullName;

		//    IntPtr blankPtr = new IntPtr(0);
		//    var lockingProcesses = (from Process proc in Process.GetProcesses()
		//                            where proc.MainWindowHandle != blankPtr 
		//                                && !proc.HasExited
		//                                && (from ProcessModule pm in proc.Modules
		//                                    where pm.ModuleName == objThis.Name
		//                                    select pm).Count() > 0
		//                            select proc).ToList();

		//    return lockingProcesses;

		//    //Process[] procs = Process.GetProcesses();
		//    //string fileName = Path.GetFileName(filePath);
		//    //foreach (Process proc in procs)
		//    //{
		//    //    if (proc.MainWindowHandle != blankPtr && !proc.HasExited)
		//    //    {
		//    //        foreach (ProcessModule pm in proc.Modules)
		//    //        {
		//    //            if (pm.ModuleName == fileName)
		//    //                return proc.ProcessName;
		//    //        }
		//    //    }
		//    //}

		//    //return null;
		//}

		public static FileInfo Latest(this FileInfo fileInfo)
		{
			if (fileInfo == null) throw new ArgumentException("FileInfo object is null.");
			fileInfo.Refresh();
			return fileInfo;
		}

		public static bool ExistsSafely(this FileInfo fileInfo)
		{
			//NOTE:  Latest() will verify arguments so no need to do so here!
			return fileInfo.Latest().Exists;
		}

		public static FileInfo DeleteSafely(this FileInfo fileInfo)
		{
			if (fileInfo.ExistsSafely())
			{
				//if (!fileInfo.IsLocked())
				//{
				//    fileInfo.Delete();
				//    fileInfo.Refresh();
				//}

				fileInfo.Delete();
			}
			return fileInfo.Latest();
		}

		/// <summary>
		/// Reads All data from the FileInfo into a string; from beginning to end. Uses default UTF-8 encoding.  Saves the step of producing a StreamReader.
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <returns></returns>
		public static String ReadString(this FileInfo fileInfo)
		{
			fileInfo.Refresh();
			using (var stream = fileInfo.OpenText())
			{
				return stream.ReadAll();
			}
		}

		/// <summary>
		/// Launch the file using the Windows associated application.
		/// </summary>
		/// <param name="fileInfo"></param>
		public static void Launch(this FileInfo fileInfo)
		{
			fileInfo.Launch(LaunchInApp.DefaultAssociatedApp);
		}

		/// <summary>
		/// Launch the file using the Specified avaialble known App Contexts.
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <param name="enumLaunchApp"></param>
		public static void Launch(this FileInfo fileInfo, LaunchInApp enumLaunchApp)
		{
			if (fileInfo == null || !fileInfo.ExistsSafely())
			{
				throw new ArgumentNullException("File is null or does not exist; cannot launch the File in [{0}].".FormatArgs(enumLaunchApp.ToString()));
			}

			var startInfo = new ProcessStartInfo()
			{
				UseShellExecute = true,
				WindowStyle = ProcessWindowStyle.Maximized
			};

			switch (enumLaunchApp)
			{
				case LaunchInApp.Excel:
					startInfo.With(i => {
						i.FileName = "Excel";
						i.Arguments = "\"{0}\"".FormatArgs(fileInfo.FullName);
					});
					break;

				case LaunchInApp.Word:
					startInfo.With(i => {
						i.FileName = "Word";
						i.Arguments = "\"{0}\"".FormatArgs(fileInfo.FullName);
					});
					break;

				case LaunchInApp.Notepad:
					startInfo.With(i => {
						i.FileName = "Notepad";
						i.Arguments = "\"{0}\"".FormatArgs(fileInfo.FullName);
					});
					break;

				case LaunchInApp.DefaultAssociatedApp:
				default:
					startInfo.FileName = fileInfo.FullName;
					break;
			}

			//Process.Start(fileInfo.FullName);
			Process.Start(startInfo);
		}

		/// <summary>
		/// Launch the file using the Windows associated application.
		/// </summary>
		/// <param name="fileInfo"></param>
		public static void Start(this FileInfo fileInfo)
		{
			fileInfo.Launch();
		}
	}

	public static class SystemDirectoryInfoCustomExtensions
	{
		public static DirectoryInfo Latest(this DirectoryInfo dir)
		{
			if (dir == null) throw new ArgumentException("DirectoryInfo object is null.");
			dir.Refresh();
			return dir;
		}

		public static FileInfo GetFile(this DirectoryInfo dir, String fileName)
		{
			var filePath = dir.GetAppendedPath(fileName);
			return new FileInfo(filePath);
		}

		public static bool ExistsSafely(this DirectoryInfo dir)
		{
			//Latest() will verify our argument.
			return dir.Latest().Exists;
		}

		public static String GetAppendedPath(this DirectoryInfo dir, String pathOrFileNameToAppend)
		{
			return Path.Combine(dir.FullName, pathOrFileNameToAppend);
		}

		public static bool HasSubdirectory(this DirectoryInfo dir, string searchPattern)
		{
			if (!dir.ExistsSafely()) throw new ArgumentException("Base Directory doesn't exist, can't work with Subdirectories.");
			return dir.GetDirectories(searchPattern).Count() > 0;
		}

		public static bool HasSubdirectory(this DirectoryInfo dir, string searchPattern, SearchOption searchOption)
		{
			if (!dir.ExistsSafely()) throw new ArgumentException("Base Directory doesn't exist, can't work with Subdirectories.");
			return dir.GetDirectories(searchPattern, searchOption).Count() > 0;
		}

		public static DirectoryInfo GetSubdirectory(this DirectoryInfo dir, string directoryName)
		{
			if (!dir.ExistsSafely()) throw new ArgumentException("Base Directory doesn't exist, can't work with Subdirectories.");
			return dir.GetDirectories(directoryName.Trim(@"\")).FirstOrDefault();
		}
		
		public static DirectoryInfo GetOrCreateSubdirectory(this DirectoryInfo dir, string directoryName)
		{
			//GetSubdirectory() will validate arguments.
			var subDir = dir.GetSubdirectory(directoryName);
			if (subDir == null)
			{
				subDir = dir.CreateSubdirectory(directoryName);
			}
			return subDir;
		}

		public static void DeleteFiles(this DirectoryInfo dir, String csvFilePatternList)
		{
			if (!dir.ExistsSafely())
			{
				throw new DirectoryNotFoundException("Cannot delete files from the directory; Directory does not exist [{0}].".FormatArgs(dir.FullName));
			}

			var filePatternList = csvFilePatternList.Split(',').Select(p => p.Trim());
			foreach (String pattern in filePatternList)
			{
				foreach (var file in dir.GetFiles(pattern))
				{
					file.DeleteSafely();
				}
			}
		}
	}

	public static class SystemIsolatedStorageCustomExtensions
	{
		public static Stream ReadTextFileAsStream(this IsolatedStorageFile isoStorage, String filePath)
		{
			using (IsolatedStorageFileStream fileStream = isoStorage.OpenFile(filePath, FileMode.OpenOrCreate, FileAccess.Read))
			{
				return fileStream.ToMemoryStream();
			}
		}

		public static String ReadTextFile(this IsolatedStorageFile isoStorage, String filePath)
		{
			return isoStorage.ReadTextFile(filePath, Encoding.Default);
		}

		public static String ReadTextFile(this IsolatedStorageFile isoStorage, String filePath, Encoding encoding)
		{
			return isoStorage.ReadTextFileAsStream(filePath).ReadString(encoding);
		}

		public static void SaveTextFile(this IsolatedStorageFile isoStorage, String filePath, String textContent)
		{
			isoStorage.SaveTextFile(filePath, textContent, FileMode.Create, Encoding.Default);
		}

		public static void SaveTextFile(this IsolatedStorageFile isoStorage, String filePath, String textContent, FileMode mode)
		{
			isoStorage.SaveTextFile(filePath, textContent, mode, Encoding.Default);
		}
		
		public static void SaveTextFile(this IsolatedStorageFile isoStorage, String filePath, String textContent, FileMode mode, Encoding encoding)
		{
			using (IsolatedStorageFileStream fileStream = isoStorage.OpenFile(filePath, mode, FileAccess.ReadWrite))
			using (var streamWriter = new StreamWriter(fileStream, encoding))
			{
				streamWriter.Write(textContent);
			}
		}

	}
}

