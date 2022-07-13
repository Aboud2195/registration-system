using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace RegistrationSystem.Api.Helpers
{
    public static class AbstractionsUtilsExtensions
    {
        public static int Hash(params object[] args)
        {
            if (args == null)
            {
                return 0;
            }

            int num = 42;

            unchecked
            {
                foreach (var item in args)
                {
                    GetNextHashCode(item, ref num);
                }
            }

            return num;
        }

        private static void GetNextHashCode(object item, ref int num)
        {
            if (item is not null)
            {
                if (item.GetType().IsArray)
                {
                    foreach (var subItem in (IEnumerable)item)
                    {
                        num = (num * 37) + Hash(subItem);
                    }
                }
                else
                {
                    num = (num * 37) + item.GetHashCode();
                }
            }
        }

        public static bool IsNumericType(this object value)
        {
            return value is sbyte
                    or byte
                    or short
                    or ushort
                    or int
                    or uint
                    or long
                    or ulong
                    or float
                    or double
                    or decimal;
        }

        public static T Get<T>(this SerializationInfo info, string key)
        {
            return (T)(info?.GetValue(key, typeof(T)) ?? throw new NullReferenceException($"{key} is not found in serialization info."));
        }

        public static T GetOrDefault<T>(this SerializationInfo info, string key, T defaultValue)
            where T : notnull
        {
            return (T)((info?.GetValue(key, typeof(T)) ?? defaultValue) ?? defaultValue);
        }
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool IgnoreCaseEquals(this string value1, string value2)
        {
            return value1.Equals(value2, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Removes all accents from the input string.
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <returns>Text without accents.</returns>
        public static string RemoveAccents(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            text = text.Normalize(NormalizationForm.FormD);
            char[] chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Turn a string into a slug by removing all accents,
        /// special characters, additional spaces, substituting
        /// spaces with hyphens & making it lower-case.
        /// </summary>
        /// <param name="phrase">The string to turn into a slug.</param>
        /// <returns>Slugified phrase.</returns>
        public static string Slugify(this string phrase)
        {
            // Remove all accents and make the string lower case.
            string output = phrase.RemoveAccents();

            // Remove all special characters from the string.
            output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", string.Empty);

            // Remove all additional spaces in favour of just one.
            output = Regex.Replace(output, @"\s+", " ").Trim();

            // Replace all spaces with the hyphen.
            output = Regex.Replace(output, @"\s", "-");

            output = string.Join(string.Empty, output.Select(c => char.IsUpper(c) ? "-" + char.ToLowerInvariant(c) : c.ToString()));

            output = output.Trim('-').Trim().ToLowerInvariant();

            // Return the slug.
            return output;
        }
    }
}
