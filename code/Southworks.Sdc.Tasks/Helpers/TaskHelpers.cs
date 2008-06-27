namespace Southworks.Sdc.Tasks.Helpers
{
    using Microsoft.Build.Utilities;
    using System;
    using System.IO;

    /// <summary>
    /// This class contains the logic to perform checks in order to be used 
    /// with a task that interacts with the FileSystem.
    /// </summary>
    internal static class TaskHelpers
    {
        /// <summary>
        /// Checks that the file path is sanitized and follows
        /// all the conventions estabilished by the operating system.
        /// </summary>
        /// <param name="fileName">The input file name to validate.</param>
        /// <param name="log">The current log instance.</param>
        /// <returns>A value indicating whether the action was executed succesfuly.</returns>
        internal static bool CheckFilePath(string fileName, TaskLoggingHelper log)
        {
            bool flag = true;
            string directoryName = string.Empty;
            try
            {
                directoryName = Path.GetDirectoryName(fileName);
            }
            catch (ArgumentException exception)
            {
                directoryName = exception.Message;
                flag = false;
            }
            catch (PathTooLongException exception2)
            {
                directoryName = exception2.Message;
                flag = false;
            }

            if (!flag)
            {
                log.LogErrorFromResources("InvalidPathChars", new object[] { directoryName });
            }

            return flag;
        }

        /// <summary>
        /// Executes a directory creation procedure and safely handles the exceptions 
        /// that might pop up.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        /// <param name="log">The current instance of the log.</param>
        /// <returns>A value indicating whether the action was executed succesfuly.</returns>
        internal static bool SafeCreateDirectory(string path, TaskLoggingHelper log)
        {
            bool flag = true;
            string message = string.Empty;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (UnauthorizedAccessException exception)
                {
                    message = exception.Message;
                    flag = false;
                }
                catch (ArgumentNullException exception2)
                {
                    message = exception2.Message;
                    flag = false;
                }
                catch (ArgumentException exception3)
                {
                    message = exception3.Message;
                    flag = false;
                }
                catch (PathTooLongException exception4)
                {
                    message = exception4.Message;
                    flag = false;
                }
                catch (DirectoryNotFoundException exception5)
                {
                    message = exception5.Message;
                    flag = false;
                }
                catch (IOException exception6)
                {
                    message = exception6.Message;
                    flag = false;
                }
                catch (NotSupportedException exception7)
                {
                    message = exception7.Message;
                    flag = false;
                }
            }

            if (!flag)
            {
                log.LogErrorFromResources("DirectoryCreationError", new object[] { message });
            }

            return flag;
        }
    }
}
