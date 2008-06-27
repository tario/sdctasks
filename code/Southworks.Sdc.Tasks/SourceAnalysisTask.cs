namespace Southworks.Sdc.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Microsoft.SourceAnalysis;

    /// <summary>
    /// This task runs the source analysis check for a given file set.
    /// </summary>
    public sealed class SourceAnalysisTask : Task
    {
        /// <summary>
        /// Holds a reference to the MSBuildErrorCode used in case of task failure.
        /// </summary>
        private const string MSBuildErrorCode = null;

        /// <summary>
        /// Holds a reference to the MSBuildSubCategory used in case of task failure.
        /// </summary>
        private const string MSBuildSubCategory = null;

        /// <summary>
        /// Holds a reference to the addins paths.
        /// </summary>
        private ITaskItem[] inputAdditionalAddinPaths = new ITaskItem[0];

        /// <summary>
        /// Holds the inputCacheResults property value.
        /// </summary>
        private bool inputCacheResults;

        /// <summary>
        /// Holds the inputDefineConstants property value.
        /// </summary>
        private string[] inputDefineConstants = new string[0];

        /// <summary>
        /// Holds the inputForceFullAnalysis property value.
        /// </summary>
        private bool inputForceFullAnalysis;

        /// <summary>
        /// Holds the value of the inputOverrideSettingsFile property value.
        /// </summary>
        private ITaskItem inputOverrideSettingsFile;

        /// <summary>
        /// Holds the value of inputProjectFullPath property value.
        /// </summary>
        private ITaskItem inputProjectFullPath;

        /// <summary>
        /// Holds the value of the inputSourceFile property value.
        /// </summary>
        private ITaskItem[] inputSourceFiles = new ITaskItem[0];

        /// <summary>
        /// Holds the value of the inputTreatErrorsAsWarnings property value.
        /// </summary>
        private bool inputTreatErrorsAsWarnings;

        /// <summary>
        /// Holds a reference to the current task execution state.
        /// </summary>
        private bool succeeded = true;

        /// <summary>
        /// Holds the value of the outputFile property value.
        /// </summary>
        private string outputFile;

        /// <summary>
        /// Gets or sets the additional addin paths.
        /// </summary>
        public ITaskItem[] AdditionalAddinPaths
        {
            get { return this.inputAdditionalAddinPaths; }
            set { this.inputAdditionalAddinPaths = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cache results.
        /// </summary>
        public bool CacheResults
        {
            get { return this.inputCacheResults; }
            set { this.inputCacheResults = value; }
        }

        /// <summary>
        /// Gets or sets the defined constants.
        /// </summary>
        public string[] DefineConstants
        {
            get { return this.inputDefineConstants; }
            set { this.inputDefineConstants = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a full check should be performed.
        /// </summary>
        public bool ForceFullAnalysis
        {
            get { return this.inputForceFullAnalysis; }
            set { this.inputForceFullAnalysis = value; }
        }

        /// <summary>
        /// Gets or sets a the override settings file.
        /// </summary>
        public ITaskItem OverrideSettingsFile
        {
            get { return this.inputOverrideSettingsFile; }
            set { this.inputOverrideSettingsFile = value; }
        }

        /// <summary>
        /// Gets or sets project full path.
        /// </summary>
        public ITaskItem ProjectFullPath
        {
            get { return this.inputProjectFullPath; }
            set 
            {
                Param.RequireNotNull(value, "ProjectFullPath");
                this.inputProjectFullPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the source files to be analyzed.
        /// </summary>
        public ITaskItem[] SourceFiles
        {
            get { return this.inputSourceFiles; }
            set 
            {
                Param.RequireNotNull(value, "SourceFiles");
                this.inputSourceFiles = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether MSBuild should treat errors as warnings.
        /// </summary>
        public bool TreatErrorsAsWarnings
        {
            get { return this.inputTreatErrorsAsWarnings; }
            set { this.inputTreatErrorsAsWarnings = value; }
        }

        /// <summary>
        /// Gets or sets the output file for the source analsyis violation check.
        /// </summary>
        public string OutputFile
        {
            get { return this.outputFile; }
            set { this.outputFile = value; }
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>A value indicating whether the execution was successfuly or not.</returns>
        public override bool Execute()
        {
            string settings = null;
            
            if ((this.inputOverrideSettingsFile != null) && (this.inputOverrideSettingsFile.ItemSpec.Length > 0))
            {
                settings = this.inputOverrideSettingsFile.ItemSpec;
            }
            
            List<string> addinPaths = new List<string>();
            
            foreach (ITaskItem item in this.inputAdditionalAddinPaths)
            {
                addinPaths.Add(item.GetMetadata("FullPath"));
            }

            string location = Assembly.GetExecutingAssembly().Location;
            addinPaths.Add(Path.GetDirectoryName(location));
            SourceAnalysisConsole console = new SourceAnalysisConsole(settings, this.inputCacheResults, this.OutputFile, addinPaths, false);
            Configuration configuration = new Configuration(this.inputDefineConstants);
            CodeProject project = new CodeProject(this.inputProjectFullPath.ItemSpec.GetHashCode(), this.inputProjectFullPath.ItemSpec, configuration);

            foreach (ITaskItem item2 in this.inputSourceFiles)
            {
                console.Core.Environment.AddSourceCode(project, item2.ItemSpec, null);
            }

            try
            {
                console.OutputGenerated += new EventHandler<OutputEventArgs>(this.OnOutputGenerated);
                console.ViolationEncountered += new EventHandler<ViolationEventArgs>(this.OnViolationEncountered);
                CodeProject[] projects = new CodeProject[] { project };
                console.Start(projects, this.inputForceFullAnalysis);
            }
            finally
            {
                console.OutputGenerated -= new EventHandler<OutputEventArgs>(this.OnOutputGenerated);
                console.ViolationEncountered -= new EventHandler<ViolationEventArgs>(this.OnViolationEncountered);
            }

            return this.succeeded;
        }

        /// <summary>
        /// Logs an entry when the output has been generated.
        /// </summary>
        /// <param name="sender">The event firer.</param>
        /// <param name="e">The output event args containing the message to be logged.</param>
        private void OnOutputGenerated(object sender, OutputEventArgs e)
        {
            lock (this)
            {
                this.Log.LogMessage(e.Output.Trim(), new object[0]);
            }
        }

        /// <summary>
        /// Handles the ViolationEncounterered event.
        /// </summary>
        /// <param name="sender">The event firer.</param>
        /// <param name="e">The encountered violation.</param>
        private void OnViolationEncountered(object sender, ViolationEventArgs e)
        {
            if (!e.Warning && !this.inputTreatErrorsAsWarnings)
            {
                this.succeeded = false;
            }

            string file = string.Empty;

            if (((e.SourceCode != null) && (e.SourceCode.Path != null)) && (e.SourceCode.Path.Length > 0))
            {
                file = e.SourceCode.Path;
            }
            else if (((e.Element != null) && (e.Element.Document != null)) && ((e.Element.Document.SourceCode != null) && (e.Element.Document.SourceCode.Path != null)))
            {
                file = e.Element.Document.SourceCode.Path;
            }

            lock (this)
            {
                if (e.Warning || this.inputTreatErrorsAsWarnings)
                {
                    this.Log.LogWarning(null, null, null, file, e.LineNumber, 1, 0, 0, e.Message, new object[0]);
                }
                else
                {
                    this.Log.LogError(null, null, null, file, e.LineNumber, 1, 0, 0, e.Message, new object[0]);
                }
            }
        }        
    }
}

