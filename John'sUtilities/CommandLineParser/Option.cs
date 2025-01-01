using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CSharpUtilities.CommandLineParser
{
	/// <summary>
	/// The SwitchRecord is stored within the parser's collection of registered
	/// switches.  This class is private to the outside world.
	/// </summary>
	public class Option
	{
		#region Private Variables
		private object mObject = null;
		private Type mOptionType = null;
		private MemberInfo mMember = null;
		private string mName = "";
		private string mDescription = "";
		private System.Collections.ArrayList mAliases = null;
		private HashSet<string> mEnumerationValues;
		private string mPattern = "";
		private int mSetCount;
		private bool mRequired;
		private object mMinValue;
		private object mMaxValue;
		#endregion

		#region Constructors
		public Option(CommandLineOptionAttribute attribute, MemberInfo memberInfo, object classForAutoAttributes)
		{
			mObject = classForAutoAttributes;
			mMember = memberInfo;
			mName = attribute.Name;
			mDescription = attribute.Description;
			mRequired = attribute.Required;
            mMinValue = attribute.MinValue;
            mMaxValue = attribute.MaxValue;
            mSetCount = 0;

			switch (memberInfo.MemberType)
			{
				case MemberTypes.Field:
					FieldInfo fieldInfo = (FieldInfo)memberInfo;
					if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
						throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
							"Illegal field for this attribute; field must be writeable");

					mOptionType = fieldInfo.FieldType;
					break;
				case MemberTypes.Property:
					PropertyInfo propInfo = (PropertyInfo)memberInfo;

					if (!propInfo.CanWrite)
						throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
							"Illegal property for this attribute; property type must be writable");

					mOptionType = propInfo.PropertyType;
					break;
				case MemberTypes.Method:
					MethodInfo method = (MethodInfo)memberInfo;
					ParameterInfo[] parameters = method.GetParameters();

					if (parameters.Length != 1)
						throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
							"Illegal method for this attribute; the method must accept exactly one parameter");

					if (parameters[0].IsOut)
						throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
							"Illegal method for this attribute; the parameter of the method must not be an out parameter");

					if (parameters[0].ParameterType.IsArray || parameters[0].ParameterType.GetInterface("System.Collections.ICollection") != null || parameters[0].ParameterType.GetInterface("System.Collections.Generic.ICollection") != null)
						throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
							"Illegal method for this attribute; the parameter of the method must be a non-array and non-collection type");

					mOptionType = parameters[0].ParameterType;
					break;
				default:
					throw new AttributeException(typeof(CommandLineOptionAttribute), memberInfo,
						"Illegal member for this attribute; member must be a property, method (accepting one parameter) or a field");
			}
			BuildPattern();
			if (mOptionType.IsEnum)
			{
				mEnumerationValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (FieldInfo field in mOptionType.GetFields())
				{
					if (field.IsLiteral)
					{
						if (mEnumerationValues.Contains(field.Name))
						{
							throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
								"This enumeration is not allowed as a command line option since it contains fields that differ only by case");
						}
						mEnumerationValues.Add(field.Name);
					}
				}
			}

            // Make sure MinValue and MaxValue is not specified for any non-numerical type.
            if (mMinValue != null || mMaxValue != null)
            {
                if (!IsNumericalType)
                {
                    throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                        "MinValue and MaxValue must not be specified for a non-numerical type");
                }
                else if (mMinValue != null && !mMinValue.GetType().IsAssignableFrom(mOptionType))
                {
                    throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                        "Illegal value for MinValue, not the same type as option");
                }
                else if (mMaxValue != null && !mMaxValue.GetType().IsAssignableFrom(mOptionType))
                {
                    throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                        "Illegal value for MaxValue, not the same type as option");
                }
            }

            // Some special checks for numerical types
            if (IsNumericalType)
            {
                // Assign the default MinValue if it was not set and this is a numerical type
                if (mMinValue == null)
                {
                    mMinValue = mOptionType.GetField("MinValue", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                }

                // Assign the defaul MaxValue if it was not set and this is a numerical type
                if (mMaxValue == null)
                {
                    mMaxValue = mOptionType.GetField("MaxValue", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                }

                // Check that MinValue <= MaxValue
                if (((IComparable)mMinValue).CompareTo(mMaxValue) > 0)
                {
                    throw new AttributeException(typeof(CommandLineOptionAttribute), mMember,
                        "MinValue must not be greater than MaxValue");
                }
            }
        }
		#endregion

		#region Public Properties
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}

		public string Description
		{
			get { return mDescription; }
			set { mDescription = value; }
		}

		public System.Type Type
		{
			get { return mOptionType; }
		}

		public string[] Aliases
		{
			get { return (mAliases != null) ? (string[])mAliases.ToArray(typeof(string)) : null; }
		}

		public string Pattern
		{
			get { return mPattern; }
		}

		public int SetCount
		{
			get { return mSetCount; }
			set { mSetCount = value; }
		}

		public bool Required
		{
			get { return mRequired; }
			set { mRequired = value; }
		}
		public object MinValue
		{
			get { return mMinValue; }
		}

		public object MaxValue
		{
			get { return mMaxValue; }
		}

		public object Value
		{
			get
			{
				object o = null;
				switch (mMember.MemberType)
				{
                    case MemberTypes.Field:
						FieldInfo fieldInfo = (FieldInfo)mMember;
                        fieldInfo.GetValue(mObject);
						break;
                    case MemberTypes.Property:
						PropertyInfo propInfo = (PropertyInfo)mMember;
						if (propInfo.CanRead)
						{
							o = propInfo.GetValue(mObject);
						}
						break;
					//case MemberTypes.Method:
					//	SetMethodValue(value);
					//	break;
					default:
						throw new ArgumentException(
							String.Format(
							"Internal error; unimplemented member type {0} found in Option.Value", mMember.MemberType.ToString()));
				}
				return o;
			}
			set
			{
				++mSetCount;
				switch (mMember.MemberType)
				{
					case MemberTypes.Field:
						SetFieldValue(value);
						break;
					case MemberTypes.Property:
						SetPropertyValue(value);
						break;
					case MemberTypes.Method:
						SetMethodValue(value);
						break;
					default:
						throw new ArgumentException(
							String.Format(
							"Internal error; unimplemented member type {0} found in Option.Value", mMember.MemberType.ToString()));
				}
			}
		}
		public bool IsNumericalType
		{
			get
			{
				return mOptionType == typeof(byte) ||
					mOptionType == typeof(short) ||
					mOptionType == typeof(int) ||
					mOptionType == typeof(long) ||
					mOptionType == typeof(ushort) ||
					mOptionType == typeof(uint) ||
					mOptionType == typeof(ulong) ||
					mOptionType == typeof(decimal) ||
					mOptionType == typeof(float) ||
					mOptionType == typeof(double);
			}
		}
		#endregion

		#region Public Methods
		public void AddAlias(string alias)
		{
			if (mAliases == null)
				mAliases = new System.Collections.ArrayList();
			mAliases.Add(alias);

			BuildPattern();
		}

		public override string ToString()
		{
			StringBuilder names = new StringBuilder();
			if (!mRequired)
				names.Append("[");
			names.Append($"-{Name}");
			if (mAliases != null)
			{
				foreach (string alias in mAliases)
				{
					names.Append(",");
					names.Append($"-{alias}");
				}
			}
			if (!mRequired)
				names.Append("]");
			String result = String.Format("{0,4} {1,-25} {2,-40}", " ", names.ToString(), Description);
			return result;
		}
		#endregion

		#region Private Utility Functions
		private void BuildPattern()
		{
			string matchString = Name;

			if (Aliases != null && Aliases.Length > 0)
				foreach (string s in Aliases)
					matchString += "|" + s;

			string strPatternStart = @"(\s|^)(?<match>(-{1,2}|/)(";
			string strPatternEnd;  // To be defined below.

			// The common suffix ensures that the switches are followed by
			// a white-space OR the end of the string.  This will stop
			// switches such as /help matching /helpme
			//
			string strCommonSuffix = @"(?=(\s|$))";

			if (Type == typeof(bool))
				strPatternEnd = @")(?<value>(\+|-){0,1}))";
			else
				strPatternEnd = @")(?<sep>[:=]))(?<value>.*)";

			//else if (Type == typeof(string) || Type.IsEnum)
			//	//strPatternEnd = @")(?<sep>[:=]))((?:"")(?<value>.+)(?:"")|(?<value>\S+))";
			//	strPatternEnd = @")(?<sep>[:=]))(?<value>.*)";
			//else if (Type == typeof(int))
			//	strPatternEnd = @")(?<sep>[:=]))((?<value>(-|\+)[0-9]+)|(?<value>[0-9]+))";
			//else if (Type.IsEnum)
			//{
			//	string[] enumNames = mEnumerationValues.K;
			//	string e_str = enumNames[0];
			//	for (int e = 1; e < enumNames.Length; e++)
			//		e_str += "|" + enumNames[e];
			//	strPatternEnd = @")(?<sep>[:=]))(?<value>" + e_str + @")";
			//}
			//else
			//	throw new System.ArgumentException();

			// Set the internal regular expression pattern.
			mPattern = strPatternStart + matchString + strPatternEnd + strCommonSuffix;
		}

		private void SetFieldValue(object value)
		{
			FieldInfo field = (FieldInfo)mMember;
			field.SetValue(mObject, GetCheckedValueOnConvert(value));
		}

		private void SetPropertyValue(object value)
		{
			Console.WriteLine("setting value of {0} value = {1}", mName, value.ToString());
			PropertyInfo property = (PropertyInfo)mMember;
			property.SetValue(mObject, GetCheckedValueOnConvert(value), null);
		}

		private void SetMethodValue(object value)
		{
			MethodInfo method = (MethodInfo)mMember;
			method.Invoke(mObject, new object[] { GetCheckedValueOnConvert(value) });
		}

		private object GetCheckedValueOnConvert(object value)
		{
			try
			{
				value = ConvertValueTypeForSetOperation(value);

			}
			catch (TargetInvocationException tie)
			{
				if (tie.InnerException is FormatException)
					throw tie.InnerException;
				else
					throw;
			}

            if (IsNumericalType)
            {
                // All numerical types are comparable and MinValue and MaxValue are both automatically set
                // in the constructor for any numerical type supported by this class.
                IComparable comparable = (IComparable)value;
				if (comparable.CompareTo(mMinValue) < 0 || comparable.CompareTo(mMaxValue) > 0)
					throw new OverflowException(String.Format("Value({0}) was either too large or too small for this option type", value));
			}
			return value;

		}
		private object ConvertValueTypeForSetOperation(object value)
		{
			//Console.WriteLine("converting value of {0}, {1}; value = {2}", mName, mOptionType.Name, value);
			Type type = mOptionType;
			if (value.GetType() == typeof(string))
			{
				string stringValue = (string)value;
				if (type.Equals(typeof(string)))
				{
					return value;
				}
				else if (type.IsEnum)
				{
					if (!mEnumerationValues.Contains(stringValue))
					{
						throw new InvalidEnumerationValueException(String.Format("Invalid enumeration value {0}", stringValue));
					}
					return Enum.Parse(type, stringValue, true);
				}
				else // We have a numerical type
				{
					return type.InvokeMember("Parse", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { value });
				}
			}
			else
			{
				return value;
			}

		}
		#endregion
	}

}
