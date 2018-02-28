namespace Fonet.Cli.Parser
{
    /// <summary>
    /// A wrapper for a command line switch.
    /// </summary>
    internal abstract class Option
    {
        /// <summary>
        /// The name of the option.
        /// </summary>
        private string name;

        /// <summary>
        /// Indictates if the option has been provided or not.
        /// </summary>
        private bool provided;

        /// <summary>
        /// Initializes a new instance of the Option class.
        /// </summary>
        /// <param name="optionName">The name of the Option.</param>
        public Option(string optionName)
        {
            this.name = optionName;
        }

        /// <summary>
        /// Gets or sets a value indicating whether an option has been provided or not.
        /// </summary>
        public bool IsProvided
        {
            get { return this.provided; }
            set { this.provided = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the option expects an argument.
        /// </summary>
        public abstract bool ExpectsArgument { get; }

        /// <summary>
        /// Gets or sets the option's argument.
        /// </summary>
        public abstract string Argument { get; set; }
    }
}