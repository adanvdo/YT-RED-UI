using System;
using System.ComponentModel;

namespace YT_RED
{
	public static class EnumExtensions
	{
		public static string GetCustomDescription(object objEnum)
		{
			var fi = objEnum.GetType().GetField(objEnum.ToString());
			var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			var adjusted = (attributes.Length > 0) ? attributes[0].Description.Replace("_","") : objEnum.ToString().Replace("_", "");
			if (adjusted.EndsWith("p")) adjusted = adjusted.Replace("p", "");
			return adjusted;
		}

		public static string ToFriendlyString(this Enum value, bool lowerCase = false)
		{
			return lowerCase ? GetCustomDescription(value).ToLower() : GetCustomDescription(value);
		}

		/// <summary>
		/// Gets an enum value based on the value's description attribute.
		/// </summary>
		/// <typeparam name="T">An enum type</typeparam>
		/// <param name="description">The value of the description attribute</param>
		/// <returns>The value of the enum that matches the description else null.</returns>
		public static T? GetFromDescription<T>(string description) where T : struct, IConvertible
		{
			var type = typeof(T);

			if (!type.IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}

			foreach (var enumValue in Enum.GetValues(type))
			{
				var fi = type.GetField(enumValue.ToString());
				var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (attributes.Length == 0)
					continue;
				if (String.Equals(attributes[0].Description, description))
					return (T)enumValue;
			}

			return null;
		}
	}
}
