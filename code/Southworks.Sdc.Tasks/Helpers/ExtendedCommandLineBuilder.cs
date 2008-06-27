namespace Southworks.Sdc.Tasks.Helpers
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System;

    /// <summary>
    /// This class is a smarter implementation of the CommandLine builder, is
    /// used by the tasks that interact with the command line utilities.
    /// </summary>
    internal class ExtendedCommandLineBuilder : CommandLineBuilder
    {
        /// <summary>
        /// Appends the switch if it is not null.
        /// </summary>
        /// <param name="switchName">The switch to be appended.</param>
        /// <param name="parameters">The parameters to be appended to the switch.</param>
        public void AppendSwitchesIfNotNull(string switchName, ITaskItem[] parameters)
        {
            this.AppendSwitchesIfNotNull(switchName, parameters, " ");
        }

        /// <summary>
        /// Appends the switch if it is not null.
        /// </summary>
        /// <param name="switchName">The switch to be appended.</param>
        /// <param name="parameters">The parameters to be appended to the switch.</param>
        public void AppendSwitchesIfNotNull(string switchName, string[] parameters)
        {
            this.AppendSwitchesIfNotNull(switchName, parameters, " ");
        }

        /// <summary>
        /// Appends the switch if it is not null specifing which is the delimiter character.
        /// </summary>
        /// <param name="switchName">The switch to be appended.</param>
        /// <param name="parameters">The parameters to be appended to the switch.</param>
        /// <param name="delimeter">The delimiter to be appended between switches.</param>
        public void AppendSwitchesIfNotNull(string switchName, ITaskItem[] parameters, string delimeter)
        {
            this.AppendSwitchIfNotNull(switchName, parameters, delimeter + switchName);
        }

        /// <summary>
        /// Appends the switch if it is not null specifing which is the delimiter character.
        /// </summary>
        /// <param name="switchName">The switch to be appended.</param>
        /// <param name="parameters">The parameters to be appended to the switch.</param>
        /// <param name="delimeter">The delimiter to be appended between switches.</param>
        public void AppendSwitchesIfNotNull(string switchName, string[] parameters, string delimeter)
        {
            this.AppendSwitchIfNotNull(switchName, parameters, delimeter + switchName);
        }

        /// <summary>
        /// Appends the switch if it is condition is true.
        /// </summary>
        /// <param name="switchName">The switch to be appended.</param>
        /// <param name="switchValue">The switch condition.</param>
        public void AppendSwitchIfTrue(string switchName, bool switchValue)
        {
            if (switchValue)
            {
                this.AppendSwitch(switchName);
            }
        }
    }
}