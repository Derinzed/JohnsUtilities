using System;
using System.Reflection;

namespace CSharpUtilities
{
	public class AttributeException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AttributeException"/> class.
		/// </summary>
		public AttributeException()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AttributeException"/> class.
		/// </summary>
		/// <param name="attributeType">Type of the attribute on which the error is present.</param>
		/// <param name="objectType">Type of the object implementing the attribute on which the error occured.</param>
		/// <param name="message">The error message.</param>
		public AttributeException(Type attributeType, Type objectType, string message)
			: this(String.Format("In attribute {0} defined on {1}; {2}", attributeType.Name, objectType.FullName, message))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AttributeException"/> class.
		/// </summary>
		/// <param name="attributeType">Type of the attribute on which the error is present.</param>
		/// <param name="member">The member assigned the attribute with the error.</param>
		/// <param name="message">The error message.</param>
		public AttributeException(Type attributeType, MemberInfo member, string message)
			: this(String.Format("In attribute {0} defined on member \"{1}\" of {2}; {3}", attributeType.Name, member.Name, member.DeclaringType.FullName, message))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AttributeException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public AttributeException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AttributeException"/> class.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="innerException">The inner exception.</param>
		public AttributeException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	public abstract class ParseException : Exception
	{


		/// <summary>
		/// Initializes a new instance of the <see cref="ParseException"/> class.
		/// </summary>
		protected ParseException()
			: base("Error in commandline")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParseException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		protected ParseException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParseException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		protected ParseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

	}

	public class InvalidEnumerationValueException : ParseException
	{
		public InvalidEnumerationValueException()
			: base()
		{
		}

		public InvalidEnumerationValueException(string message)
			: base(message)
		{
		}

		public InvalidEnumerationValueException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	public class MissingRequiredOptionException : ParseException
	{
		public MissingRequiredOptionException()
			: base()
		{
		}

		public MissingRequiredOptionException(string message)
			: base(message)
		{
		}

		public MissingRequiredOptionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	public class InvalidValueForOptionException : ParseException
	{
		public InvalidValueForOptionException()
			: base()
		{
		}

		public InvalidValueForOptionException(string message)
			: base(message)
		{
		}

		public InvalidValueForOptionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
