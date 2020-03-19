namespace Fonet.Cli.Parser
{
    using System;
    using System.Collections;

    /// <summary>
    /// A simple set of classes that parses command line arguments.
    /// </summary>
    internal sealed class CommandLineParser
    {
        /// <summary>
        /// Maps a switch string to an instance of the Option class.
        /// </summary>
        private Hashtable options = new Hashtable();

        /// <summary>
        /// The command line options.
        /// </summary>
        private string[] args;

        /// <summary>
        /// Any supplementary command line arguments.
        /// </summary>
        private string[] remainder;

        /// <summary>
        /// Registers a parameterless command line switch <code>switchName</code>.
        /// If the option already exists, the old version will be replaced by the
        /// new version.
        /// </summary>
        /// <param name="switchName">A command line switch excluding any hyphen.</param>
        /// <returns>A newly created instance of Option.</returns>
        public Option AddOption(string switchName)
        {
            Option opt = new ParameterlessOption(switchName);
            this.options[switchName] = opt;

            return opt;
        }

        /// <summary>
        /// Registers a command line switch <code>switchName</code> that expects 
        /// a parameter.  If the option already exists, the old version will be
        /// replaced by the new version.
        /// </summary>
        /// <param name="switchName">A command line switch excluding any hyphen.</param>
        /// <returns>A newly created instance of Option.</returns>
        public Option AddParameterOption(string switchName)
        {
            Option opt = new ParameterOption(switchName);
            this.options[switchName] = opt;

            return opt;
        }

        /// <summary>
        /// Parses the command line objects.
        /// </summary>
        /// <param name="args">The argument array passed to Main().</param>
        public void Parse(string[] args)
        {
            this.args = args;

            // Exit immediately if nothing to do
            if (args == null)
            {
                return;
            }

            int index = 0;
            while (index < args.Length && args[index][0] == '-')
            {
                string arg = args[index++];
                if (arg[0] == '-')
                {
                    // We have a command line switch
                    string switchName = arg.Substring(1);

                    Option option = this.GetOptionFromSwitch(switchName);
                    if (option == null)
                    {
                        throw new CommandLineException("Option '" + switchName + "' is unregistered");
                    }

                    option.IsProvided = true;

                    if (option.ExpectsArgument)
                    {
                        if (!this.HasMoreArgs(index))
                        {
                            throw new CommandLineException("Missing argument for " + switchName);
                        }

                        string argument = args[index++];
                        if (argument[0] == '-')
                        {
                            // Missing argument  - this is the beginning of another option
                            throw new CommandLineException("Missing argument for " + switchName);
                        }

                        option.Argument = argument;
                    }
                }
            }

            if (index < args.Length)
            {
                this.remainder = new string[args.Length - index];
                Array.Copy(args, index, this.remainder, 0, this.remainder.Length);
            }
            else
            {
                this.remainder = new string[0];
            }
        }

        /// <summary>
        /// Retrieves any remaining arguments after the command line switches 
        /// have been parsed.  
        /// </summary>
        /// <returns>a non-null array containing the remaining arguments.</returns>
        public string[] GetRemainder()
        {
            return this.remainder;
        }

        /// <summary>
        /// Returns true if there are still more arguments to process.
        /// </summary>
        /// <param name="index">The current index.</param>
        /// <returns>True if more arguments are to be processed.</returns>
        private bool HasMoreArgs(int index)
        {
            return index < this.args.Length;
        }

        /// <summary>
        /// Returns the instance of Option that has been configured for the passed switch.
        /// </summary>
        /// <param name="switchName">The switch whose Option instance we should return.</param>
        /// <returns>An instance of Option.</returns>
        private Option GetOptionFromSwitch(string switchName)
        {
            return (Option)this.options[switchName];
        }
    }
}