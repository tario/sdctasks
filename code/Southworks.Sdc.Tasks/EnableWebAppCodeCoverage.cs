namespace Southworks.Sdc.Tasks
{
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// This task in inteded to update the Web Application Code Coverage configuration allowing 
    /// your project to run code coverage from the command line using MSTest.exe.
    /// </summary>
    public class EnableWebAppCodeCoverage : Task
    {
        /// <summary>
        /// Holds a reference to Visual Studio Team Test configuration namespace.
        /// </summary>
        private static XNamespace documentNamespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2006";

        /// <summary>
        /// Gets or sets the list of test run configuration files.
        /// </summary>
        [Required]
        public ITaskItem[] TestRunConfigurations { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>A value indicating whether the execution was successful.</returns>
        public override bool Execute()
        {
            foreach (var testRunConfig in this.TestRunConfigurations)
            {
                XDocument document = XDocument.Load(testRunConfig.ItemSpec);

                var codeCoverageNode = document.Root.Element(documentNamespace + "CodeCoverage");

                if (codeCoverageNode != null)
                {
                    var aspNetNode = codeCoverageNode.Element(documentNamespace + "AspNet");

                    if (aspNetNode != null && aspNetNode.Nodes().Count() > 0)
                    {
                        var items = aspNetNode.Elements(documentNamespace + "AspNetCodeCoverageItem");

                        var nodes = from n in items
                                    select new XElement(documentNamespace + "CodeCoverageItem", new XAttribute("binaryFile", string.Format(CultureInfo.InvariantCulture, @"{0}\bin\{0}.dll", n.Attribute("name").Value)), new XAttribute("pdbFile", string.Format(CultureInfo.InvariantCulture, @"{0}\bin\{0}.pdb", n.Attribute("name").Value)));

                        aspNetNode.Remove();

                        var regularNode = codeCoverageNode.Element(documentNamespace + "Regular");

                        if (regularNode == null)
                        {
                            regularNode = new XElement(documentNamespace + "Regular");
                            codeCoverageNode.Add(regularNode);
                        }

                        regularNode.Add(nodes);
                    }

                    document.Save(testRunConfig.ItemSpec);
                }
            } 
            
            return true;
        }
    }
}