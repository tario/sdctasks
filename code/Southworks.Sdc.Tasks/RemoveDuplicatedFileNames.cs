namespace Southworks.Sdc.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Build.Utilities;
    using Microsoft.Build.Framework;

    /// <summary>
    /// This task removes the duplicated file names from an item collection.
    /// </summary>
    public class RemoveDuplicatedFileNames : Task
    {
        /// <summary>
        /// Gets or sets the list of files to be filtered.
        /// </summary>
        [Required]
        public ITaskItem[] Input { get; set; }
        
        /// <summary>
        /// Gets or sets the list of filtered files containing unique file names (path independent).
        /// </summary>
        [Output]
        public ITaskItem[] FilteredItems { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>A value indicating whether the execution was sucessful or not.</returns>
        public override bool Execute()
        {
            List<ITaskItem> output = new List<ITaskItem>();

            foreach (ITaskItem file in this.Input)
            {
                if (output.Where(f => string.Compare(System.IO.Path.GetFileName(file.ItemSpec), System.IO.Path.GetFileName(f.ItemSpec), true) == 0).Count() < 1)
                {
                    output.Add(file);
                }
            }

            this.FilteredItems = output.ToArray();

            return true;
        }
    }
}
