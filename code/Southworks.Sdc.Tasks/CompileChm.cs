﻿namespace Southworks.Sdc.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System.Diagnostics;
    using DocxConverter.Log;
    using System.Xml;

    /// <summary>
    /// This task compile the files generated by PrepareDocxToChm task.
    /// </summary>
    public class CompileChm : Task
    {
        /// <summary>
        /// The path for the chm project file generated.
        /// </summary>
        [Required]
        public ITaskItem ChmOutput { get; set; }

        /// <summary>
        /// The chm compiler.
        /// </summary>
        [Required]
        public ITaskItem ChmCompiler { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>A value indicating whether the execution was successful or not.</returns>
        public override bool Execute()
        {
            try
            {
                if (!this.ChmCompiler.ItemSpec.EndsWith("hhc.exe"))
                    this.Logger("false", "The Chm compiler is incorrect");

                new Process { StartInfo = new ProcessStartInfo(this.ChmCompiler.ItemSpec, String.Concat(this.ChmOutput.ItemSpec, "Project.hhp")) } .Start();

                this.Logger("true", "");
                return true;
            }
            catch (Exception ex)
            {
                this.Logger("false", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Log the conversion process.
        /// </summary>
        /// <param name="success">Result of the conversion process.</param>
        /// <param name="exceptionMessage">Error Message if the process fail.</param>
        private void Logger(string success, string exceptionMessage)
        {
            Log log = new Log();
            log.Success = success;
            if (!String.IsNullOrEmpty(exceptionMessage))
            {
                log.Error = exceptionMessage;
            }

            XmlDocument xmlLog = new XmlDocument();
            xmlLog.LoadXml(log.SerializeToXml());
            xmlLog.Save(String.Concat(this.ChmOutput.ItemSpec, "\\CompileLog.xml"));
        }
    }
}
