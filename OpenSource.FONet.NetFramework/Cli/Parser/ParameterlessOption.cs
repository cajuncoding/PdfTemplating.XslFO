namespace Fonet.Cli.Parser
{
    /// <summary>
    /// An option that does not expect a parameter.
    /// </summary>
    internal class ParameterlessOption : Option
    {
        /// <summary>
        /// Initializes a new instance of the ParameterlessOption class.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        public ParameterlessOption(string optionName)
            : base(optionName)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the option expects an argument.
        /// </summary>
        public override bool ExpectsArgument
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the option's argument.
        /// </summary>
        public override string Argument
        {
            get { throw new CommandLineException("This option cannot have an argument"); }
            set { throw new CommandLineException("This option cannot have an argument"); }
        }
    }
}
