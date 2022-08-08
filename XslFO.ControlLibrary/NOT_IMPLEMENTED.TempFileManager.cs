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

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using PdfTemplating.SystemIOCustomExtensions;
//using System.Diagnostics;
//using PdfTemplating.SystemCustomExtensions;

//namespace XslFO.ControlLibrary
//{
//    public class TempFileManager : IDisposable
//    {
//        private Dictionary<String, FileInfo> _tempFilesIndex = new Dictionary<String, FileInfo>();
//        private String _tempFolderPath = Path.GetTempPath().TrimEnd('\\');

//        public void Remove(FileInfo tempFileInfo)
//        {
//            if(_tempFilesIndex.ContainsKey(tempFileInfo.FullName))
//            {
//                _tempFilesIndex.Remove(tempFileInfo.FullName);
//            }
//        }

//        public void Add(FileInfo tempFileInfo)
//        {
//            _tempFilesIndex[tempFileInfo.FullName] = tempFileInfo;
//        }

//        private static readonly String tempFileFullNameFormat = @"{0}\{1}";
//        /// <summary>
//        /// Create a new Temp file using the specified File Name, not including Path (i.e. MyTempFile.txt)
//        /// </summary>
//        /// <param name="fileName"></param>
//        /// <returns></returns>
//        public FileInfo Create(String fileName)
//        {
//            //Create a Unique Temp File Name
//            FileInfo tempFileInfo = null;

//            //Loop until a Completely Unique File is created via GUID identifier values.
//            String tempFileName = fileName.ToFileSystemSafeString(true);
//            String tempFileFullName = String.Format(tempFileFullNameFormat, _tempFolderPath, tempFileName);
            
//            //Validate the Temp File Status
//            tempFileInfo = new FileInfo(tempFileFullName);
//            if(tempFileInfo.Exists)
//            {
//                throw new ArgumentException(String.Format("The specified File Name [{0}] already exists and cannot be used as a temp file.", tempFileName));
//            }

//            //Finally add the unique temp file to the index
//            this.Add(tempFileInfo);

//            return tempFileInfo;
//        }
        
//        /// <summary>
//        /// Create a truly unique file name given the FileName format that should have a single placeholder for a GUID identifier (i.e. MyTempFile_{0}.txt)
//        /// </summary>
//        /// <param name="fileNameGuidFormat"></param>
//        /// <returns></returns>
//        public FileInfo CreateUnique(String fileNameGuidFormat)
//        {
//            //Create a Unique Temp File Name
//            FileInfo tempFileInfo = null;
//            int maxAttempts = 1000;

//            //Loop until a Completely Unique File is created via GUID identifier values.
//            do
//            {
//                String tempFileName = String
//                                        .Format(fileNameGuidFormat, Guid.NewGuid())
//                                        .TrimStart('\\')
//                                        .ToFileSystemSafeString(true);

//                String tempFileFullName = String.Format(tempFileFullNameFormat, _tempFolderPath, tempFileName);
//                tempFileInfo = new FileInfo(tempFileFullName);
//            }
//            while ((maxAttempts--) > 0 && tempFileInfo == null && tempFileInfo.Exists);
            
//            //Finally add the unique temp file to the index
//            this.Add(tempFileInfo);

//            return tempFileInfo;
//        }
        
//        public IList<FileInfo> GetFiles()
//        {
//            return _tempFilesIndex.Values.ToList();
//        }

//        public FileInfo GetFile(string fileFullNameKey)
//        {
//            return _tempFilesIndex[fileFullNameKey];
//        }

//        public void PurgeFiles()
//        {
//            List<String> successfulPurgeList = new List<String>();
//            List<String> failedItemList = new List<String>();

//            foreach (KeyValuePair<String, FileInfo> indexItem in _tempFilesIndex)
//            {
//                FileInfo tempFileInfo = indexItem.Value;
//                if (tempFileInfo.Exists)
//                {
//                    //If it exists then attempt to Purge, otherwise remove it from the Manager
//                    try
//                    {
//                        tempFileInfo.Delete();
//                        successfulPurgeList.Add(indexItem.Key);
//                    }
//                    catch
//                    {
//                        List<Process> lockingProcs = tempFileInfo.GetLockingProcesses();
//                        String errorType = "[UNKNOWN]";
//                        if (lockingProcs.Count > 0)
//                        {
//                            errorType = String.Format("[LOCKED {{0}}]", String.Join(",", lockingProcs.Select(p => p.ProcessName)));
//                        }

//                        failedItemList.Add(String.Format("{0} {1}", errorType, tempFileInfo.FullName).Trim());
//                    }
//                }
//                else
//                {
//                    successfulPurgeList.Add(indexItem.Key);
//                }
//            }

//            //Clean up any items that were actually successful!
//            foreach (String itemKey in successfulPurgeList)
//            {
//                _tempFilesIndex.Remove(itemKey);
//            }

//            if (failedItemList.Count > 0)
//            {
//                throw new InvalidOperationException(String.Format("Unable to delete the following Files being managed: {0}", String.Join(", ", failedItemList)));
//            }

//        }

//        #region IDisposable Members

//        public void Dispose()
//        {
//            try
//            {
//                this.PurgeFiles();
//            }
//            catch
//            {
//            }
//        }

//        #endregion

//    }
//}
