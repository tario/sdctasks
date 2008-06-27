namespace Southworks.Sdc.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Southworks.Sdc.Tasks.Helpers;

    /// <summary>
    /// This task is used to wrap the logic of calling the MSTest command line tool
    /// in order to run the tests created with Visual Studio Professional edition or higher.
    /// </summary>
    public class RunMSTest : ToolTask
    {
        /// <summary>
        /// Holds the reference to the location of the base name node on the test run configuration file.
        /// </summary>
        private const string BaseNameNodeString = "//Tests/TestRunConfiguration/testRunNamingScheme/baseName";
        
        /// <summary>
        /// Holds a reference to the InstallDir key name on the string dictionary.
        /// </summary>
        private const string InstallDirValue = "InstallDir";

        /// <summary>
        /// Holds a refence to the location of the TestRunNamingScheme node location on the test run configuration file.
        /// </summary>
        private const string TestRunNamingSchemeNodeString = "//Tests/TestRunConfiguration/testRunNamingScheme";
        
        /// <summary>
        /// Holds a refence to the location of the TimeStamp node location on the test run configuration file.
        /// </summary>
        private const string TimeStampNodeString = "//Tests/TestRunConfiguration/testRunNamingScheme/appendTimeStamp";
        
        /// <summary>
        /// Holds a refence to the location of the UseDefault node location on the test run configuration file.
        /// </summary>
        private const string UseDefaultNodeString = "//Tests/TestRunConfiguration/testRunNamingScheme/useDefault";
        
        /// <summary>
        /// Holds a refence to the Visual Studio 2005 installation registry node for x86.
        /// </summary>
        private const string Win32KeyNameVS2005 = @"SOFTWARE\Microsoft\VisualStudio\8.0";

        /// <summary>
        /// Holds a refence to the Visual Studio 2008 installation registry node for x86.
        /// </summary>
        private const string Win32KeyNameVS2008 = @"SOFTWARE\Microsoft\VisualStudio\9.0";
        
        /// <summary>
        /// Holds a refence to the Visual Studio 2005 installation registry node for x64.
        /// </summary>
        private const string Win64KeyNameVS2005 = @"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\8.0";
        
        /// <summary>
        /// Holds a refence to the Visual Studio 2008 installation registry node for x86
        /// </summary>
        private const string Win64KeyNameVS2008 = @"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\9.0";
        
        /// <summary>
        /// Holds the DeatailsList property value.
        /// </summary>
        private ITaskItem[] detailList;

        /// <summary>
        /// Holds the FinalResultFile property value.
        /// </summary>
        private string finalResultFile;

        /// <summary>
        /// Holds the NoIsolation property value.
        /// </summary>
        private bool noIsolation;

        /// <summary>
        /// Holds the values in form of a string dictionary.
        /// </summary>
        private Dictionary<string, string> stringDictionary;

        /// <summary>
        /// Holds the TestContainer property value.
        /// </summary>
        private ITaskItem[] testContainers;

        /// <summary>
        /// Holds the TestList property value.
        /// </summary>
        private ITaskItem[] testList;

        /// <summary>
        /// Holds the TestListLists property value.
        /// </summary>
        private ITaskItem[] testListLists;

        /// <summary>
        /// Holds the ToolPath property value.
        /// </summary>
        private string toolPath;

        /// <summary>
        /// Holds the UniqueName property value.
        /// </summary>
        private bool uniqueName;

        /// <summary>
        /// Creates an instance of RunMSTest.
        /// </summary>
        public RunMSTest()
        {
            this.stringDictionary = new Dictionary<string, string>();
            this.uniqueName = true;
            this.finalResultFile = string.Empty;
            this.toolPath = this.GetVSInstallDir();
        }

        /// <summary>
        /// Gets or sets the test run details.
        /// </summary>
        public ITaskItem[] Details
        {
            get { return this.detailList; }
            set { this.detailList = value; }
        }

        /// <summary>
        /// Gets or sets the test run flavor.
        /// </summary>
        public string Flavor
        {
            get { return this.GetKey("Flavor"); }
            set { this.stringDictionary["Flavor"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the test should run without isolation (single thread).
        /// </summary>
        public bool NoIsolation
        {
            get { return this.noIsolation; }
            set { this.noIsolation = value; }
        }

        /// <summary>
        /// Gets or sets the platform to be used on the test run.
        /// </summary>
        public string Platform
        {
            get { return this.GetKey("Platform"); }
            set { this.stringDictionary["Platform"] = value; }
        }

        /// <summary>
        /// Gets or sets the publish results for the tests results (VSTS TFS only).
        /// </summary>
        public string Publish
        {
            get { return this.GetKey("Publish"); }
            set { this.stringDictionary["Publish"] = value; }
        }

        /// <summary>
        /// Gets or sets the publish build for the tests results (VSTS TFS only).
        /// </summary>
        public string PublishBuild
        {
            get { return this.GetKey("PublishBuild"); }
            set { this.stringDictionary["PublishBuild"] = value; }
        }

        /// <summary>
        /// Gets or sets the publish file for the tests results (VSTS TFS only).
        /// </summary>
        public string PublishResultsFile
        {
            get { return this.GetKey("PublishResultsFile"); }
            set { this.stringDictionary["PublishResultsFile"] = value; }
        }

        /// <summary>
        /// Gets or sets the results file name for the test run.
        /// </summary>
        [Required]
        public string ResultsFile
        {
            get { return this.GetKey("ResultsFile"); }
            set { this.stringDictionary["ResultsFile"] = value; }
        }

        /// <summary>
        /// Gets or sets the test run configuration.
        /// </summary>
        public string RunConfig
        {
            get { return this.GetKey("RunConfig"); }
            set { this.stringDictionary["RunConfig"] = value; }
        }

        /// <summary>
        /// Gets or sets the team project used to publish the test results (VSTS TFS only).
        /// </summary>
        public string TeamProject
        {
            get { return this.GetKey("TeamProject"); }
            set { this.stringDictionary["TeamProject"] = value; }
        }

        /// <summary>
        /// Gets or sets the containers that have the tests to run.
        /// </summary>
        public ITaskItem[] TestContainers
        {
            get { return this.testContainers; }
            set { this.testContainers = value; }
        }

        /// <summary>
        /// Gets or sets the lists of test to run.
        /// </summary>
        public ITaskItem[] TestLists
        {
            get { return this.testListLists; }
            set { this.testListLists = value; }
        }

        /// <summary>
        /// Gets or sets the test metadata file.
        /// </summary>
        public string TestMetaData
        {
            get { return this.GetKey("TestMetaData"); }
            set { this.stringDictionary["TestMetaData"] = value; }
        }

        /// <summary>
        /// Gets or sets the tests name to run.
        /// </summary>
        public ITaskItem[] Tests
        {
            get { return this.testList; }
            set { this.testList = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the test should be executed with a unique name.
        /// </summary>
        public bool UniqueName
        {
            get { return this.uniqueName; }
            set { this.uniqueName = value; }
        }

        /// <summary>
        /// Gets the ToolName (executable file) for MSTest.exe.
        /// </summary>
        protected override string ToolName
        {
            get { return "MSTEST.EXE"; }
        }

        /// <summary>
        /// Generates the command-line command to launch MSTests with the given parameters.
        /// </summary>
        /// <returns>A System.String containing the full MSTest instruction.</returns>
        protected override string GenerateCommandLineCommands()
        {
            ExtendedCommandLineBuilder builder = new ExtendedCommandLineBuilder();
            builder.AppendSwitch("/nologo");
            builder.AppendSwitchIfNotNull("/testmetadata:", this.GetKey("TestMetaData"));
            builder.AppendSwitchesIfNotNull("/testcontainer:", this.testContainers);
            builder.AppendSwitchIfNotNull("/runconfig:", this.GetKey("RunConfig"));
            builder.AppendSwitchIfNotNull("/resultsfile:", this.finalResultFile);
            builder.AppendSwitchesIfNotNull("/testlist:", this.testListLists);
            builder.AppendSwitchesIfNotNull("/test:", this.testList);
            builder.AppendSwitchIfTrue("/noisolation", this.noIsolation);
            builder.AppendSwitchesIfNotNull("/detail:", this.detailList);
            builder.AppendSwitchIfNotNull("/publish:", this.GetKey("Publish"));
            builder.AppendSwitchIfNotNull("/publishbuild:", this.GetKey("PublishBuild"));
            builder.AppendSwitchIfNotNull("/publishresultsfile:", this.GetKey("PublishResultsFile"));
            builder.AppendSwitchIfNotNull("/teamproject:", this.GetKey("TeamProject"));
            builder.AppendSwitchIfNotNull("/platform:", this.GetKey("Platform"));
            builder.AppendSwitchIfNotNull("/flavor:", this.GetKey("Flavor"));
            return builder.ToString();
        }

        /// <summary>
        /// Generates the full path to the MSTest.exe tool.
        /// </summary>
        /// <returns>A System.String containing the path to the tool.</returns>
        protected override string GenerateFullPathToTool()
        {
            return this.toolPath;
        }

        /// <summary>
        /// Validates the given parameters before running the tests.
        /// </summary>
        /// <returns>A value indicating whether the parameters are valid or not.</returns>
        protected override bool ValidateParameters()
        {
            this.finalResultFile = this.GetKey("ResultsFile");

            if (!TaskHelpers.CheckFilePath(this.finalResultFile, this.Log))
            {
                return false;
            }

            this.finalResultFile = Path.GetFullPath(this.finalResultFile);
            bool flag = TaskHelpers.SafeCreateDirectory(Path.GetDirectoryName(this.finalResultFile), this.Log);

            if (flag)
            {
                this.finalResultFile = AddExtensionIfNecessary(this.finalResultFile);
                string key = this.GetKey("RunConfig");
                if (this.uniqueName && string.IsNullOrEmpty(key))
                {
                    this.finalResultFile = AddTimeStampToFilename(this.finalResultFile, false);
                }
                else if (!string.IsNullOrEmpty(key))
                {
                    this.finalResultFile = this.BuildResultFileFromRunConfig(this.finalResultFile);
                }
            }

            return flag;
        }

        /// <summary>
        /// Adds the ".trx" extension to the results file if not provided.
        /// </summary>
        /// <param name="file">The test results file name.</param>
        /// <returns>A System.String containing the full tests results name including the extension.</returns>
        private static string AddExtensionIfNecessary(string file)
        {
            if (!Path.HasExtension(file))
            {
                return Path.ChangeExtension(file, ".trx");
            }

            return file;
        }

        /// <summary>
        /// Adds a TimeStamp to the file name according to the test run full date-time.
        /// </summary>
        /// <param name="file">The file name of the current test run.</param>
        /// <param name="useUnderscores">A value indicating whether to use underscores to add the time stamp.</param>
        /// <returns>A System.String with the test results file name including the time stamp.</returns>
        private static string AddTimeStampToFilename(string file, bool useUnderscores)
        {
            string directoryName = Path.GetDirectoryName(file);

            if (!string.IsNullOrEmpty(directoryName))
            {
                directoryName = directoryName + @"\";
            }

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
            string extension = Path.GetExtension(file);
            string str4 = string.Empty;

            if (useUnderscores)
            {
                str4 = "_";
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} {2}{1}{2}", fileNameWithoutExtension, DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss", DateTimeFormatInfo.InvariantInfo), str4);
            StringBuilder builder2 = new StringBuilder();
            builder2.AppendFormat("{0}{1}{2}", directoryName, builder.ToString(), extension);
            return builder2.ToString();
        }

        /// <summary>
        /// Generates the test results file name from the Test Run Configuration file.
        /// </summary>
        /// <remarks>This file name is used in case that the results file is not provided.</remarks>
        /// <param name="file">The Test Run Configuration file.</param>
        /// <returns>A System.String containing the test result file name.</returns>
        private string BuildResultFileFromRunConfig(string file)
        {
            string directoryName = Path.GetDirectoryName(file);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
            string extension = Path.GetExtension(file);
            XmlDocument document = new XmlDocument();
            document.Load(this.GetKey("RunConfig"));
            XmlNode node = document.SelectSingleNode("//Tests/TestRunConfiguration/testRunNamingScheme/appendTimeStamp");
            Console.WriteLine("timeStampNode: " + node);
            XmlNode node2 = document.SelectSingleNode("//Tests/TestRunConfiguration/testRunNamingScheme/useDefault");
            Console.WriteLine("useDefaultNode: " + node2);
            string innerText = null;
            XmlNode node3 = document.SelectSingleNode("//Tests/TestRunConfiguration/testRunNamingScheme/baseName");
            Console.WriteLine("baseNameNode: " + node3);

            if (null != node3)
            {
                innerText = node3.InnerText;
            }

            bool flag = false;

            if (node != null)
            {
                flag = Convert.ToBoolean(node.InnerText, CultureInfo.InvariantCulture);
            }

            Console.WriteLine("useTimeStamp: " + flag);
            bool flag2 = false;

            if (node2 != null)
            {
                flag2 = Convert.ToBoolean(node2.InnerText, CultureInfo.InvariantCulture);
            }

            Console.WriteLine("useTimeStamp: " + flag);
            if (flag2)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("{0} {1}_{2}", fileNameWithoutExtension, Environment.UserName, Environment.MachineName);
                fileNameWithoutExtension = builder.ToString();
            }
            else if (!string.IsNullOrEmpty(innerText))
            {
                StringBuilder builder2 = new StringBuilder();
                builder2.AppendFormat("{0} {1}", fileNameWithoutExtension, innerText);
                fileNameWithoutExtension = builder2.ToString();
            }

            if (flag)
            {
                bool useUnderscores = false;

                if (!(string.IsNullOrEmpty(innerText) || flag2))
                {
                    useUnderscores = true;
                }

                fileNameWithoutExtension = AddTimeStampToFilename(fileNameWithoutExtension, useUnderscores);
            }

            StringBuilder builder3 = new StringBuilder();
            builder3.AppendFormat(@"{0}\{1}{2}", directoryName, fileNameWithoutExtension, extension);
            return builder3.ToString();
        }

        /// <summary>
        /// Retrieves the Visual Studio installation path from the Registry Key.
        /// </summary>
        /// <param name="regKey">The registry key to be used according to the current platform.</param>
        /// <returns>A System.String containing the Visual Studio installation path.</returns>
        private string GetInstallDirFromRegKey(string regKey)
        {
            string str = null;
            RegistryKey key = null;
            try
            {
                key = Registry.LocalMachine.OpenSubKey(regKey);
                if (null == key)
                {
                    return str;
                }

                string str2 = key.GetValue("InstallDir", string.Empty) as string;

                if (!string.IsNullOrEmpty(str2))
                {
                    str = str2 + "MSTEST.EXE";
                }
            }
            finally
            {
                if (null != key)
                {
                    key.Close();
                }
            }

            return str;
        }

        /// <summary>
        /// Retrieves a key from the string dictionary used to store the curren properties.
        /// </summary>
        /// <param name="key">The key to be retrieved.</param>
        /// <returns>A System.String with the property value.</returns>
        private string GetKey(string key)
        {
            string str = string.Empty;

            if (!this.stringDictionary.TryGetValue(key, out str))
            {
                return null;
            }

            return str;
        }

        /// <summary>
        /// Gets the Visual Studio installation location.
        /// </summary>
        /// <returns>A System.String containing the Visual Studio installtion dir.</returns>
        private string GetVSInstallDir()
        {
            string installDirFromRegKey = null;

            installDirFromRegKey = this.GetInstallDirFromRegKey(@"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\9.0");

            if (string.IsNullOrEmpty(installDirFromRegKey))
            {
                installDirFromRegKey = this.GetInstallDirFromRegKey(@"SOFTWARE\Microsoft\VisualStudio\9.0");
            }

            if (string.IsNullOrEmpty(installDirFromRegKey))
            {
                installDirFromRegKey = this.GetInstallDirFromRegKey(@"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\8.0");
            }

            if (string.IsNullOrEmpty(installDirFromRegKey))
            {
                installDirFromRegKey = this.GetInstallDirFromRegKey(@"SOFTWARE\Microsoft\VisualStudio\8.0");
            }

            if (string.IsNullOrEmpty(installDirFromRegKey))
            {
                throw new ApplicationException("MSTest is not installed on this machine.");
            }

            return installDirFromRegKey;
        }
    }
}