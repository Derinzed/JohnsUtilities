using System;

namespace CSharpUtilities.CommandLineParser
{
	/// <summary>Implements a basic command-line option by taking the
	/// switching name and the associated description.</summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
	public class CommandLineOptionAttribute : System.Attribute
	{
		#region Public Properties
		/// <summary>Accessor for retrieving the switch-name for an associated
		/// property.</summary>
		public string Name { get; set; }

		/// <summary>Accessor for retrieving the description for a switch of
		/// an associated property.</summary>
		public string Description { get; set; }
		public string Aliases { get; set; }

		public bool Required { get; set; }

        public object MaxValue { get; set; }

        public object MinValue { get; set; }

        #endregion

        #region Constructors
        /// <summary>Attribute constructor.</summary>
        public CommandLineOptionAttribute() { /* Needed to grant public access */ }
		#endregion
	}
}
