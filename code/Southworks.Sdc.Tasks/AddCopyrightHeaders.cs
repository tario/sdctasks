namespace Southworks.Sdc.Tasks
{
    using Microsoft.Build.Utilities;
    using Microsoft.Build.Framework;
    using System.IO;
    using System.Text;

    /// <summary>
    /// This tasks adds the specified copyright header to the code files.
    /// </summary>
    public class AddCopyrightHeaders : Task
    {
        /// <summary>
        /// Gets or sets the path to the copyright file to be appended on code files.
        /// </summary>
        [Required]
        public ITaskItem CopyrightHeaderLocation { get; set; }

        /// <summary>
        /// Gets or sets the list of code files which will be updated by appending the copyright header.
        /// </summary>
        [Required]
        public ITaskItem[] Include { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>A value indicating whether the execution was successful or not.</returns>
        public override bool Execute()
        {
            if (!File.Exists(this.CopyrightHeaderLocation.ItemSpec))
            {
                this.Log.LogError("The copyright header file {0} not found", this.CopyrightHeaderLocation);
                return false;
            }

            string copyrightHeader = File.ReadAllText(this.CopyrightHeaderLocation.ItemSpec);
            foreach (var item in this.Include)
            {
                string fileContent = File.ReadAllText(item.ItemSpec);
                StringBuilder builder = new StringBuilder(copyrightHeader);
                builder.Append(fileContent);
                File.WriteAllText(item.ItemSpec, builder.ToString());
            }
            
            return true;
        }
    }
}
