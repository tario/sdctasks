namespace Southworks.Sdc.Tasks
{
    using System.Text.RegularExpressions;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System.IO;
    using System.Globalization;

    /// <summary>
    /// This tasks updates an specific number of fields of the AssemblyInfo.cs file.
    /// </summary>
    public class UpdateAssemblyInfo : Task
    {
        /// <summary>
        /// Holds the RegEx pattern used to look for the specified field on the file.
        /// </summary>
        private const string NeedleFormPattern = "{0}\\(\"[^\"]*\"\\)";

        /// <summary>
        /// Holds the RegEx pattern used to replace the file with the desired value.
        /// </summary>
        private const string ReplacementFormPattern = "{0}(\"{1}\")";
        
        /// <summary>
        /// Gets or sets the assembly product to be used on the assembly information file.
        /// </summary>
        public string AssemblyProduct { get; set; }

        /// <summary>
        /// Gets or sets the assembly company (a.k.a manufacturer) to be used on the assembly information file. 
        /// </summary>
        public string AssemblyCompany { get; set; }

        /// <summary>
        /// Gets or sets assembly copyright notice to be used on the assembly information file.
        /// </summary>
        public string AssemblyCopyright { get; set; }

        /// <summary>
        /// Gets or sets the assemby information files to be updated.
        /// </summary>
        [Required]
        public ITaskItem[] Include { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>A value indicating whether the execution was succesful or not.</returns>
        public override bool Execute()
        {
            foreach (var item in this.Include)
            {
                string fileContent = File.ReadAllText(item.ItemSpec);

                if (!string.IsNullOrEmpty(this.AssemblyProduct))
                {
                    Regex assemblyProductExpression = UpdateAssemblyInfo.GenerateNeedleExpression("AssemblyProduct");
                    string assemblyProductReplacement = UpdateAssemblyInfo.GenerateReplacementExpression("AssemblyProduct", this.AssemblyProduct);

                    fileContent = assemblyProductExpression.Replace(fileContent, assemblyProductReplacement);
                }

                if (!string.IsNullOrEmpty(this.AssemblyCompany))
                {
                    Regex assemblyCompanyExpression = UpdateAssemblyInfo.GenerateNeedleExpression("AssemblyCompany");
                    string assemblyCompanyReplacement = UpdateAssemblyInfo.GenerateReplacementExpression("AssemblyCompany", this.AssemblyCompany);

                    fileContent = assemblyCompanyExpression.Replace(fileContent, assemblyCompanyReplacement);
                }

                if (!string.IsNullOrEmpty(this.AssemblyCopyright))
                {
                    Regex assemblyCopyrightExpression = UpdateAssemblyInfo.GenerateNeedleExpression("AssemblyCopyright");
                    string assemblyCopyrightReplacement = UpdateAssemblyInfo.GenerateReplacementExpression("AssemblyCopyright", this.AssemblyCopyright);

                    fileContent = assemblyCopyrightExpression.Replace(fileContent, assemblyCopyrightReplacement);
                }

                File.WriteAllText(item.ItemSpec, fileContent);
            }

            return true;
        }

        /// <summary>
        /// Generates the needle expresion by taking the desired key and building a regular expresion
        /// that looks for that property on the assembly infomation file.
        /// </summary>
        /// <param name="keyName">The desired key to be updated.</param>
        /// <returns>A regular expresion that matches the desired key on the AssemblyInfo.cs.</returns>
        private static Regex GenerateNeedleExpression(string keyName)
        {
            string regEx = string.Format(CultureInfo.InvariantCulture, NeedleFormPattern, keyName);
            return new Regex(regEx);
        }

        /// <summary>
        /// Generates the replacement text for the needle to be used on the file.
        /// </summary>
        /// <param name="keyName">The desired key to be updated.</param>
        /// <param name="value">The value that will be used to update the property.</param>
        /// <returns>A System.String value that contains the replacement text to be used.</returns>
        private static string GenerateReplacementExpression(string keyName, string value)
        {
            string replacement = string.Format(CultureInfo.InvariantCulture, ReplacementFormPattern, keyName, value);
            return replacement;
        }
    }
}
