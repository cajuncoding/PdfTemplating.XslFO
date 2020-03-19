namespace Fonet.Cli.Parser
{
    using System;

    /// <summary>
    /// Thrown if an unknown option is encountered or a parameter option is missing an argument.
    /// </summary>
    internal class CommandLineException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the CommandLineException class.
        /// </summary>
        /// <param name="msg">A message that describes the unexpected condition.</param>
        public CommandLineException(string msg) : base(msg)
        {
        }
    }
}