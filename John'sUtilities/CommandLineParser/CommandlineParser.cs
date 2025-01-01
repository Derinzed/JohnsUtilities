using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpUtilities.CommandLineParser
{
    public class CommandlineParser
    {
		#region Private Variables
		private System.Collections.ArrayList mOptions = null;
		#endregion

		#region Constructors
		public CommandlineParser(object classForAutoAttributes)
		{
			Type type = classForAutoAttributes.GetType();
			System.Reflection.MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			for (int i = 0; i < members.Length; i++)
			{
				//Console.WriteLine("checking {0}", members[i].Name);
				CommandLineOptionAttribute attrib = Attribute.GetCustomAttribute(members[i], typeof(CommandLineOptionAttribute)) as CommandLineOptionAttribute;
				if (attrib == null)
					continue;
				//Console.WriteLine("see attribute for {0}", attrib.Name);
				Option option = new Option(attrib, members[i], classForAutoAttributes);
				// See if any aliases are required.  We can only do this after
				// a switch has been registered and the framework doesn't make
				// any guarantees about the order of attributes, so we have to
				// walk the collection a second time.
				if (option != null && attrib.Aliases != null && attrib.Aliases.Length > 0)
				{
					string[] aliases = attrib.Aliases.Split(',');
					foreach (string alieas in aliases)
						option.AddAlias(alieas);
				}

				// Assuming we have a option (that may or may not have
				// aliases), add it to the collection of switches.
				if (option != null)
				{
					if (mOptions == null)
						mOptions = new System.Collections.ArrayList();
					mOptions.Add(option);
				}
			}
		}
		#endregion

		#region Public Methods
		public List<string> Parse(string[] arguments)
		{
			List<string> extra = new List<string>();
			//Console.WriteLine("Number of arguments : {0}", arguments.Length);
			Regex r = new Regex(@"(?<flag>--|-)(?<name>[^:\+-=]+)", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase, new TimeSpan(0,1,0));

			foreach (string argument in arguments)
			{
				Match m = r.Match(argument);
				if (m.Success)
				{
					string name = m.Groups["name"].Value;
					//Console.WriteLine("Processing argument : {0}", argument);
					if (HandleOptions(name, argument))
						continue;
				}
				extra.Add(argument);
			}
            foreach (Option rec in mOptions)
            {
                if (rec.Required && rec.SetCount == 0)
                    throw new MissingRequiredOptionException(String.Format("Missing required option {0}.", rec.Name));
            }
            return extra;
		}

		public string[] Aliases(string optionName )
        {
			string[] result = null;
			if ( mOptions != null )
            {
				foreach( Option option in mOptions)
                {
					if (string.Compare(option.Name, optionName, true) == 0)
                    {
						result = option.Aliases;
						break;
                    }

				}
			}
			return result;
        }

		public string GetOptionsAsString()
		{
			StringBuilder result = new StringBuilder();
			result.Append("Options:\n");
			if (mOptions != null)
			{
				for (int i = 0; i < mOptions.Count; i++)
				{
					result.Append(mOptions[i].ToString());
					result.Append("\n");
				}

			}
			return result.ToString();
		}
		#endregion

		#region Private Utility Functions
		private bool HandleOptions(string name, string argument)
		{
			if (mOptions == null)
				return false;
			Option opt = null;
			for (int i = 0; i < mOptions.Count; i++)
			{
				Option option = (Option)mOptions[i];
				if (string.Compare(option.Name, name, true) == 0)
				{
					opt = option;
					break;
				}
				if (option.Aliases != null && option.Aliases.Contains(name, StringComparer.OrdinalIgnoreCase))
				{
					opt = option;
					break;
				}
			}

			if (opt == null)
				return false;

			//			Console.WriteLine("pattern = {0}", record.Pattern);
			Regex r = new Regex(opt.Pattern,
				RegexOptions.ExplicitCapture
				| RegexOptions.IgnoreCase, new TimeSpan(0, 1, 0));
			MatchCollection m = r.Matches(argument);
			if (m == null)
				return false;

			for (int i = 0; i < m.Count; i++)
			{
				string value = null;
				if (m[i].Groups != null && m[i].Groups["value"] != null)
					value = m[i].Groups["value"].Value;

				if (opt.Type == typeof(bool))
				{
					bool state = true;
					// The value string may indicate what value we want.
					if (m[i].Groups != null && m[i].Groups["value"] != null)
					{
						switch (value)
						{
							case "+":
								state = true;
								break;
							case "-":
								state = false;
								break;
							case "":
								if (opt.Value != null)
									state = !(bool)opt.Value;
								break;
							default: break;
						}
					}
					opt.Value = state;
					break;
				}
				else
				{
					try
					{
						opt.Value = value;
					}
					catch (FormatException)
					{
						throw new InvalidValueForOptionException(String.Format("Invalid value ({0}) for option {1}", value, opt.Name));
					}
					catch (OverflowException)
					{
						throw new OverflowException(String.Format("Value specified for {0} ({3}) is out of range; expected a numeric value between {1} and {2}", opt.Name, opt.MinValue, opt.MaxValue, value));
					}
				}
			}
			return true;
		}
		#endregion
	}
}
