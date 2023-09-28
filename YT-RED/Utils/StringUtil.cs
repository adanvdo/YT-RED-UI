using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTR.Utils
{
	public class StringUtility
	{
		public static string PrepareString(string str)
		{
			return str.Replace("'", "''");
		}

		public static int countDigits(string s)
		{
			// Passed a string, returns the number of digits in that string
			// Example: countDigits("123abc805") returns 6

			int digits = 0;
			for (int i = 0; i < s.Length; i++)
			{
				if (Char.IsDigit(s, i))
				{
					digits += 1;
				}
			}
			return digits;
		}

		public static string getDigit(string s, int index)
		{
			// Passed a string, returns the number the nth digit from that string
			// Example: getDigit("123abc805", 3) returns "8", since 8 is the 3rd (zero-relative) digit in string s

			int digits = 0;
			string returnValue = "";
			for (int i = 0; i < s.Length; i++)
			{
				if (Char.IsDigit(s, i))
				{
					if (digits.Equals(index))
					{
						returnValue = s.Substring(i, 1);
					}
					digits += 1;
				}
			}
			return returnValue;
		}

		public static int getDigitInt(string s, int index)
		{
			// Passed a string, returns the number the nth digit from that string
			// Example: getDigit("123abc805", 3) returns "8", since 8 is the 3rd (zero-relative) digit in string s

			return Int32.Parse(StringUtility.getDigit(s, index));
		}

		public static string GetCmdLineValue(string[] strs, string targetStr)
		{
			if (strs != null)
			{
				foreach (string str in strs)
				{
					if (str != null
						&& str.Length > targetStr.Length
						&& str.Substring(0, targetStr.Length).Equals(targetStr))
					{
						return str.Substring(targetStr.Length);
					}
				}

			}

			return null;
		}

		public static bool IsStringInStringArray(string[] strs, string targetStr)
		{
			if (strs != null)
			{
				foreach (string str in strs)
				{
					if (targetStr.Equals(str))
					{
						return true;
					}
				}

			}
			return false;
		}
	}
}
