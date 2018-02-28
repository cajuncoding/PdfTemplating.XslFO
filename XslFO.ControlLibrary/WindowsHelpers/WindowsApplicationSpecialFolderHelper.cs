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
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;


namespace TE.Library {
    public enum TE_ConfigFolderType {
        UserRoamingFolder,
        UserLocalFolder,
        MachineLocalFolder,
        MachineTempFolder
    }

    /// <summary>
    /// Simplifies the creation of custom folders inside of Environment.Configuration folders (ie. CommonApplicationData) folder
    /// and setting of permissions for all users.
    /// </summary>
    public class WindowsApplicationSpecialFolderHelper {
        private string _applicationFolder;
        private string _companyFolder;
        private string _rootDirectory = String.Empty;

        /// <summary>
        /// Creates a new instance of this class creating the specified company and application folders
        /// if they don't already exist and optionally allows write/modify to all users.
        /// </summary>
        /// <param name="companyFolder">The name of the company's folder (normally the company name).</param>
        /// <param name="applicationFolder">The name of the application's folder (normally the application name).</param>
        /// <remarks>If the application folder already exists then permissions if requested are NOT altered.</remarks>
        public WindowsApplicationSpecialFolderHelper(string companyFolder, string applicationFolder)
            : this(companyFolder, applicationFolder, false) {
        }

        /// <summary>
        /// Creates a new instance of this class creating the specified company and application folders
        /// if they don't already exist and optionally allows write/modify to all users.
        /// </summary>
        /// <param name="companyFolder">The name of the company's folder (normally the company name).</param>
        /// <param name="applicationFolder">The name of the application's folder (normally the application name).</param>
        /// <param name="allUsers">true to allow write/modify to all users; otherwise, false.</param>
        /// <remarks>If the application folder already exists then permissions if requested are NOT altered.</remarks>
        public WindowsApplicationSpecialFolderHelper(string companyFolder, string applicationFolder, bool allUsers)
            : this(TE_ConfigFolderType.MachineTempFolder, companyFolder, applicationFolder, allUsers) {
        }

        /// <summary>
        /// Creates a new instance of this class creating the specified company and application folders
        /// if they don't already exist and optionally allows write/modify to all users.
        /// </summary>
        /// <param name="companyFolder">The name of the company's folder (normally the company name).</param>
        /// <param name="applicationFolder">The name of the application's folder (normally the application name).</param>
        /// <remarks>If the application folder already exists then permissions if requested are NOT altered.</remarks>
        public WindowsApplicationSpecialFolderHelper(TE_ConfigFolderType enumConfigFolderType, string companyFolder, string applicationFolder)
            : this(enumConfigFolderType, companyFolder, applicationFolder, false) {
        }

        /// <summary>
        /// Creates a new instance of this class creating the specified company and application folders
        /// if they don't already exist and optionally allows write/modify to all users.
        /// </summary>
        /// <param name="companyFolder">The name of the company's folder (normally the company name).</param>
        /// <param name="applicationFolder">The name of the application's folder (normally the application name).</param>
        /// <param name="allUsers">true to allow write/modify to all users; otherwise, false.</param>
        /// <remarks>If the application folder already exists then permissions if requested are NOT altered.</remarks>
        public WindowsApplicationSpecialFolderHelper(TE_ConfigFolderType enumConfigFolderType, string companyFolder, string applicationFolder, bool allUsers) {
            //NOTE:  For more detailed explanation of the Following see:
            //       http://blogs.msdn.com/b/patricka/archive/2010/03/18/where-should-i-store-my-data-and-configuration-files-if-i-target-multiple-os-versions.aspx
            switch (enumConfigFolderType) {
                case TE_ConfigFolderType.UserLocalFolder:
                    this._rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    break;
                case TE_ConfigFolderType.UserRoamingFolder:
                    this._rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    break;
                case TE_ConfigFolderType.MachineLocalFolder:
                    this._rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    break;
                case TE_ConfigFolderType.MachineTempFolder:
                    this._rootDirectory = Path.GetTempPath();
                    break;
            }

            this._applicationFolder = applicationFolder;
            this._companyFolder = companyFolder;

            CreateFolders(allUsers);
        }

        /// <summary>
        /// Convert the Helper into the String Path for the Application Config Folder initialized.
        /// </summary>
        /// <returns></returns>
        public override String ToString() {
            return this.ApplicationFolderPath;
        }

        /// <summary>
        /// Convert the Helper into a DirectoryInfo reference for the Application Config Folder initialized.
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo ToDirectoryInfo() {
            return this.ApplicationFolderDirectory;
        }

        /// <summary>
        /// Gets the path of the application's data folder.
        /// </summary>
        public String ApplicationFolderPath
        {
            get { return Path.Combine(CompanyFolderPath, _applicationFolder); }
        }

        /// <summary>
        /// Gets the Dircotry for the application's data folder.
        /// </summary>
        public DirectoryInfo ApplicationFolderDirectory {
            get { return new DirectoryInfo(Path.Combine(CompanyFolderPath, _applicationFolder)); }
        }

        /// <summary>
        /// Gets the path of the company's data folder.
        /// </summary>
        public string CompanyFolderPath {
            get { return Path.Combine(_rootDirectory, _companyFolder); }
        }

        /// <summary>
        /// Gets the Directory for the company's data folder.
        /// </summary>
        public DirectoryInfo CompanyFolderDirectory
        {
            get { return new DirectoryInfo(Path.Combine(_rootDirectory, _companyFolder)); }
        }

        private void CreateFolders(bool allUsers)
        {
            DirectoryInfo directoryInfo;
            DirectorySecurity directorySecurity;
            AccessRule rule;
            SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);

            if (!Directory.Exists(CompanyFolderPath)) {
                directoryInfo = Directory.CreateDirectory(CompanyFolderPath);
                bool modified;
                directorySecurity = directoryInfo.GetAccessControl();
                rule = new FileSystemAccessRule(
                    securityIdentifier,
                    FileSystemRights.Write |
                    FileSystemRights.ReadAndExecute |
                    FileSystemRights.Modify,
                    AccessControlType.Allow);
                directorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out modified);
                directoryInfo.SetAccessControl(directorySecurity);
            }

            if (!Directory.Exists(ApplicationFolderPath)) {
                directoryInfo = Directory.CreateDirectory(ApplicationFolderPath);
                if (allUsers) {
                    bool modified;
                    directorySecurity = directoryInfo.GetAccessControl();
                    rule = new FileSystemAccessRule(
                        securityIdentifier,
                        FileSystemRights.Write |
                        FileSystemRights.ReadAndExecute |
                        FileSystemRights.Modify,
                        InheritanceFlags.ContainerInherit |
                        InheritanceFlags.ObjectInherit,
                        PropagationFlags.InheritOnly,
                        AccessControlType.Allow);
                    directorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out modified);
                    directoryInfo.SetAccessControl(directorySecurity);
                }
            }
        }

    }
}