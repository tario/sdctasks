namespace Southworks.Sdc.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Build.Utilities;
    using System.IO;
    using Southworks.Sdc.Tasks.Helpers;
    using Microsoft.Build.Framework;

    /// <summary>
    /// This class contains the logic for generating a self-extracting exe file.
    /// </summary>
    public class GenerateSFX : ToolTask
    {
        /// <summary>
        /// Holds the command execution format.
        /// </summary>
        private const string CommandExecutionFormat = "{0} {1} {2} {3}";

        /// <summary>
        /// Holds the default win rar verb.
        /// </summary>
        private const string DefaultWinRarVerb = "a";

        /// <summary>
        /// Gets or sets a value indicating whether the archived file should be deleted.
        /// </summary>
        public bool DeleteFilesAfterArchiving { get; set; }

        /// <summary>
        /// Gets or sets the source directory.
        /// </summary>
        [Required]
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the SFX File Name.
        /// </summary>
        [Required]
        public string SFXFileName { get; set; }

        /// <summary>
        /// Gets or sets the scripted file location.
        /// </summary>
        public string ScriptCommentFile { get; set; }

        /// <summary>
        /// Gets or sets the image file.
        /// </summary>
        public string ImageFile { get; set; }

        /// <summary>
        /// Gets or sets the icon file.
        /// </summary>
        public string IconFile { get; set; }

        /// <summary>
        /// Gets the ToolName.
        /// </summary>
        protected override string ToolName
        {
            get { return "winrar.exe"; }
        }

        /// <summary>
        /// Gets or sets the full path where the tool is located.
        /// </summary>
        /// <returns>A System.String contaiing the WinRar.exe file.</returns>
        protected override string GenerateFullPathToTool()
        {
            string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WinRAR"), this.ToolName);
            return path;
        }

        /// <summary>
        /// Generates the command-line command to generate the self extracting rar with the given parameters.
        /// </summary>
        /// <returns>A System.String containing the full SFX instruction.</returns>
        protected override string GenerateCommandLineCommands()
        {
            ExtendedCommandLineBuilder builder = new ExtendedCommandLineBuilder();
            builder.AppendSwitchIfTrue("-df ", this.DeleteFilesAfterArchiving);
            builder.AppendSwitch(@"-m5 -mdg -r -s -sfx -ep1 -iadm");
            builder.AppendSwitchIfNotNull("-z", this.ScriptCommentFile);
            builder.AppendSwitchIfNotNull("-iimg", this.ImageFile);
            builder.AppendSwitchIfNotNull("-iicon", this.IconFile);

            string command = string.Format(CommandExecutionFormat, DefaultWinRarVerb, builder.ToString(), this.SFXFileName, this.SourceDirectory);

            return command;
        }
    }
}