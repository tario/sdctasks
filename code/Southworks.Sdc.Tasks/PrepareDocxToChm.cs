namespace Southworks.Sdc.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Microsoft.Practices.DocxConverter;
    using Microsoft.Practices.DocxAuthoring.ComponentModel;
    using System.Xml;
    using System.Xml.Xsl;
    using System.IO;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Xml.XPath;
    using DocxConverter.Log;
    using System.Security.Permissions;
    using System.Threading;

    /// <summary>
    /// This task use the p&amp;p Documentation Tools to prepare docxs for chm conversion.
    /// </summary>
    public class PrepareDocxToChm : Task
    {
        /// <summary>
        /// The TOC of the chm file.
        /// </summary>
        [Required]
        public ITaskItem MasterToc { get; set; }

        /// <summary>
        /// The path of the configuration file for the html pages.
        /// </summary>        
        public ITaskItem HtmlConfigurationFile { get; set; }

        /// <summary>
        /// The path of the configuration file for the chm.
        /// </summary>        
        public ITaskItem ChmConfigurationFile { get; set; }

        /// <summary>
        /// The output path for the chm file will be generate.
        /// </summary>
        [Required]
        public ITaskItem ChmOutput { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>A value indicating whether the execution was successful or not.</returns>        
        public override bool Execute()
        {
            try
            {
                XmlDocument masterToc = new XmlDocument();
                masterToc.Load(this.MasterToc.ItemSpec);

                this.PrepareConfigFiles();
                this.PrepareDirectories();

                XslCompiledTransform tranform = new XslCompiledTransform();
                XsltArgumentList arguments = new XsltArgumentList();
                arguments.AddParam("configFile", String.Empty, Path.GetFullPath(this.HtmlConfigurationFile.ItemSpec));

                foreach (XmlNode item in masterToc.ChildNodes[1].ChildNodes[0].ChildNodes)
                {
                    XmlDocument mainDocument = OpenXmlHelper.DeleteCommentsAndAcceptRevisions(item.InnerText, String.Empty);
                    IReferenceResolver resolver = new OpenXmlReferenceResolver(item.InnerText, "..\\html\\images", String.Concat(this.ChmOutput.ItemSpec, "\\html", "\\images"), ".png");
                    DocumentConverterEngine converterEngine = new DocumentConverterEngine(mainDocument, resolver, "..\\tools\\DocxConverter\\Microsoft.Practices.DocxConverter.dll.config");

                    IDictionary<string, XmlDocument> converted = converterEngine.SplitAndConvert();

                    foreach (KeyValuePair<string, XmlDocument> pair in converted)
                    {
                        tranform.Load("..\\tools\\DocxConverter\\xsl\\Domain.xsl", XsltSettings.TrustedXslt, new XmlUrlResolver());
                        StreamWriter writer = new StreamWriter(String.Concat(this.ChmOutput.ItemSpec, "\\html\\", pair.Key, ".html"));
                        tranform.Transform(new XmlNodeReader(pair.Value.DocumentElement), arguments, writer);
                    }
                }

                this.GenerateChmProjectFile("HtmlHelp1Toc.xsl", "Toc.hhc", this.ChmConfigurationFile.ItemSpec);
                this.GenerateChmProjectFile("HtmlHelp1Hhp.xsl", "Project.hhp", this.ChmConfigurationFile.ItemSpec);

                this.Logger("true", String.Empty);

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
            xmlLog.Save(String.Concat(this.ChmOutput.ItemSpec, "\\PrepareLog.xml"));
        }

        /// <summary>
        /// Prepare the directories necessary for the chm conversion.
        /// </summary>
        private void PrepareDirectories()
        {
            Directory.CreateDirectory(String.Concat(this.ChmOutput.ItemSpec, "\\html"));
            Directory.CreateDirectory(String.Concat(this.ChmOutput.ItemSpec, "\\html", "\\images"));

            DirectoryInfo sourceDirectory = new DirectoryInfo("..\\tools\\DocxConverter\\local");

            foreach (FileInfo file in sourceDirectory.GetFiles())
            {
                file.CopyTo(String.Concat(Directory.CreateDirectory(String.Concat(ChmOutput.ItemSpec, "\\local")).FullName, "\\", file.Name), true);
            }
        }

        /// <summary>
        /// Set the default config files if it is ncessary.
        /// </summary>
        private void PrepareConfigFiles()
        {
            if (this.HtmlConfigurationFile == null)
            {
                this.HtmlConfigurationFile = new TaskItem("..\\tools\\DocxConverter\\config\\ppdocConfig.xml");
            }

            if (this.ChmConfigurationFile == null)
            {
                this.ChmConfigurationFile = new TaskItem("..\\tools\\DocxConverter\\config\\helpConfig.xml");
            }
        }

        /// <summary>
        /// Generate the necessary chm project files for compilation.
        /// </summary>
        /// <param name="xslFile">The xslt file use in the generation process.</param>
        /// <param name="fileName">The file name of the generated file.</param>
        /// <param name="configFile">The chm config file use to set the document properties.</param>
        private void GenerateChmProjectFile(string xslFile, string fileName, string configFile)
        {
            XslCompiledTransform tranform = new XslCompiledTransform();
            XsltArgumentList arguments = new XsltArgumentList();
            arguments.AddParam("configFile", String.Empty, Path.GetFullPath(configFile));

            StreamWriter indexFile = new StreamWriter(String.Concat(this.ChmOutput.ItemSpec, fileName));
            tranform.Load(String.Concat("..\\tools\\DocxConverter\\xsl\\", xslFile), XsltSettings.TrustedXslt, new XmlUrlResolver());
            tranform.Transform(this.GetMasterTocIndex(), arguments, indexFile);
        }

        /// <summary>
        /// Get all topics in the docxs registered in the MasterToc.
        /// </summary>
        /// <returns>XmlDocument with all topics.</returns>
        private XmlDocument GetMasterTocIndex()
        {
            Collection<string> fileList = this.GetFileListFromDocument(this.MasterToc.ItemSpec);
            MasterToc masterToc = new MasterToc(fileList);
            masterToc.Rebuild();
            XmlDocument xmlDoc = new XmlDocument();
            StringReader reader = new StringReader(masterToc.SerializeToXml());
            xmlDoc.Load(reader);

            return xmlDoc;
        }

        /// <summary>
        /// Desearalize the MasterToc file.
        /// </summary>
        /// <param name="masterTocPath">The path when find the MasterToc file.</param>
        /// <returns>The docx files colletion.</returns>
        private Collection<string> GetFileListFromDocument(string masterTocPath)
        {
            Collection<string> fileList = new Collection<string>();
            XmlDocument files = new XmlDocument();
            files.Load(masterTocPath);
            XmlNodeList nodes = files.SelectNodes("descendant-or-self::file");
            foreach (XmlNode node in nodes)
            {
                fileList.Add(node.InnerXml);
            }

            return fileList;
        }
    }
}
