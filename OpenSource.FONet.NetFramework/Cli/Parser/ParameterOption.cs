namespace Fonet.Cli.Parser
{
    /// <summary>
    /// An option that expects a parameter.
    /// </summary>
    internal class ParameterOption : Option
    {
        /// <summary>
        /// The parameter provided.
        /// </summary>
        private string argument;

        /// <summary>
        /// Initializes a new instance of the ParameterOption class.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        public ParameterOption(string optionName)
            : base(optionName)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the option expects an argument.
        /// </summary>
        public override bool ExpectsArgument
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the option's argument.
        /// </summary>
        public override string Argument
        {
            get { return this.argument; }
            set { this.argument = value; }
        }
    }
}
