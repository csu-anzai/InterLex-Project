namespace Interlex.Crawler.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;
    using Interlex.Crawler.Model;

    /// <summary>
    /// Common extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the href attribute of the node. If the attribute is missing empty string is returned.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static String Href(this HtmlNode a)
        {
            return a.GetAttributeValue("href", String.Empty);
        }

        /// <summary>
        /// Replaces the <paramref name="oldValue"/> with the new value if <paramref name="str"/> starts with it.
        /// If <paramref name="oldValue"/> is null or empty the original string is returned
        /// </summary>
        /// <param name="str">Original string</param>
        /// <param name="oldValue">Value to replace</param>
        /// <param name="newValue">Value to replace with</param>
        /// <returns></returns>
        public static String SafeReplaceAtStart(this String str, String oldValue, String newValue)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str), $"Can't be null or empty in the context of {nameof(Extensions)}.{nameof(SafeReplaceAtStart)} method");

            if (str == String.Empty)
                return String.Empty;

            if (String.IsNullOrEmpty(oldValue))
                return str;

            var startIndex = str.IndexOf(oldValue);
            if (startIndex == 0)
            {
                var result = $"{newValue}{str.Substring(oldValue.Length)}";

                return result;
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// Splits the string into exactly two pieces using the provided separator.
        /// NOTE: If more or less then two pieces are the result of the split operation, exception is thrown.
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Throws when the str has more or less then two pieces after the split.</exception>
        public static (String f, String s) TupleSplit(this String str, String separator)
        {
            var split = str.Split(separator);
            if (split.Length == 2)
            {
                return (split[0], split[1]);
            }
            // for cases like 'value='
            else if (split.Length == 1 && $"{split[0]}{separator}" == str)
            {
                return (split[0], null);
            }
            else
            {
                throw new ArgumentException("The string is splited in more then two parts. In cases like this use the build in .Split method.");
            }
        }

        /// <summary>
        /// Returns html decoded string from the current using <see cref="System.Net.WebUtility.HtmlDecode(string)"/>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String HtmlDecode(this String str)
        {
            return System.Net.WebUtility.HtmlDecode(str);
        }

        /// <summary>
        /// Returns map for the query arguments for the provided uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Dictionary<String, String> GetQueryFragmentsMap(this Uri uri)
        {
            var absUri = uri;
            if (absUri.IsAbsoluteUri == false)
            {
                // NOTE: this is accepted to be used only for http based uris (even relative)
                // create absolute uri so we can use the parsed query string
                absUri = new Uri(new Uri("https://stackoverflow.com"), uri.OriginalString);
            }

            var query = absUri.Query.SafeReplaceAtStart("?", String.Empty);

            var map = (from pair in query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                       let nameValue = pair.TupleSplit("=")
                       select nameValue)
                       .DistinctBy(x => x.f)
                       .ToDictionary(x => x.f, x => x.s);

            return map;
        }

        /// <summary>
        /// Distincts element in sequnce by given property of the <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Source type</typeparam>
        /// <typeparam name="TDistinctKey">Type of property to distinct by.</typeparam>
        /// <param name="sequance">Source sequence</param>
        /// <param name="distinctBySelector">Key selector for distinction.</param>
        /// <returns>Sequence of the same type distincted by the given property of the source type.</returns>
        /// <exception cref="System.ArgumentNullException">Throws when <paramref name="sequance"/> or <paramref name="distinctBySelector"/> is null.</exception>
        public static IEnumerable<T> DistinctBy<T, TDistinctKey>(this IEnumerable<T> sequance, Func<T, TDistinctKey> distinctBySelector)
        {
            var distinctors = new HashSet<TDistinctKey>();
            foreach (var item in sequance)
            {
                if (distinctors.Add(distinctBySelector(item)))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Determs if the current string starts with any of the 'start with variants' by using the default string comparison
        /// </summary>
        /// <param name="str">original string</param>
        /// <param name="startWithVariants">starts with variatns</param>
        /// <returns></returns>
        public static bool StartsWithAny(this String str, params String[] startWithVariants)
        {
            return startWithVariants.Any(x => str.StartsWith(x));
        }

        /// <summary>
        /// Determs if the collection contains the currenct object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool IsIn<T>(this T @object, IEnumerable<T> items)
        {
            return items.Contains(@object);
        }

        /// <summary>
        /// Determs if the collection contains the currenct object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool IsIn<T>(this T @object, params T[] items)
        {
            return items.Contains(@object);
        }


        /// <summary>
        /// Returns unicode escape sequence.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String EncodeNonAsciiCharacters(this String str)
        {
            // https://stackoverflow.com/questions/1615559/convert-a-unicode-string-to-an-escaped-ascii-string

            var sb = new StringBuilder();
            foreach (char c in str)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Replaces all occurences of the old values with the provided new value
        /// <para>NOTE: If the str is empty, empty string is returned
        /// If the oldvalues is empty, the original string is returned.</para>
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="oldValues">Values to replace</param>
        /// <param name="newValue">Value to replace with</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Throws when <paramref name="str"/> is null</exception>
        public static String SafeReplaceAll(this String str, String[] oldValues, String newValue)
        {
            if (str == null)
                throw new ArgumentNullException($"{nameof(str)}", $"Can't be null in the context of {nameof(SafeReplaceAll)} extension method");

            if (String.IsNullOrEmpty(str))
                return str;

            if (oldValues.IsEmpty())
                return str;

            var result = oldValues.Aggregate(seed: str, func: (state, toReplace) => state.SafeReplace(replace: toReplace, newValue: newValue));

            return result;
        }

        /// <summary>
        /// Replace value from the string with new one.
        /// <para>
        /// NOTE: If <param name="str"/> is Empty string the result will be the original string
        /// </para>
        /// <para>
        /// If <param name="replace"/> is Empty string the result will be the original string
        /// </para>>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="newValue"></param>
        /// <param name="replace"></param>
        /// <returns>The original string if replace is empty, or the result of the replacment</returns>
        /// <exception cref="System.ArgumentNullException">Throws when <paramref name="str"/>, <paramref name="replace"/> or <paramref name="newValue"/>  is null</exception>
        public static String SafeReplace(this String str, String replace, String newValue)
        {
            if (str == null) { throw new ArgumentNullException($"{str} can't be null when using {nameof(SafeReplace)}"); }
            if (replace == null) { throw new ArgumentNullException($"{replace} can't be null when using {nameof(SafeReplace)}"); }
            if (newValue == null) { throw new ArgumentNullException($"{newValue} can't be null when using {nameof(SafeReplace)}"); }

            if (str == String.Empty || replace == String.Empty)
            {
                return str;
            }

            return str.Replace(replace, newValue);
        }

        /// <summary>
        /// Replace value from the string with new one.
        /// <para>
        /// NOTE: If <param name="str"/> is Empty string the result will be the original string
        /// </para>
        /// <para>
        /// If <param name="oldValue"/> is Empty string the result will be the original string
        /// </para>>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static String SafeReplace(this String str, String oldValue, String newValue, StringComparison comparison)
        {
            if (str == null)
            {
                throw new ArgumentNullException($"The input string can't be null in the context of {nameof(SafeReplace)}");
            }

            if (oldValue == null)
            {
                throw new ArgumentNullException($"The old value to be replaced can't be null in the context of {nameof(SafeReplace)}");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException($"The new value to replace with can't be null in the context of {nameof(SafeReplace)}");
            }

            if (str == String.Empty || oldValue == String.Empty)
            {
                return str;
            }

            var options = RegexOptions.None;
            if (comparison == StringComparison.CurrentCultureIgnoreCase
                || comparison == StringComparison.InvariantCultureIgnoreCase
                || comparison == StringComparison.OrdinalIgnoreCase)
            {
                options = RegexOptions.IgnoreCase;
            }

            var result = Regex.Replace(input: str, pattern: oldValue, replacement: newValue, options: options);

            return result;
        }

        /// <summary>
        /// Returns true if the squence has no elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> sequence)
        {
            return !sequence.Any();
        }

        /// <summary>
        /// Returns the hex representation of the md5 hash for the current string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String ToHexMD5(this String str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(paramName: str, message: $"Can't be null in the context of {nameof(ToHexMD5)} method.");
            }


            var hash = str.ToMD5Hash();
            var bitRep = BitConverter.ToString(hash).Replace("-", String.Empty);

            return bitRep;
        }

        /// <summary>
        /// Returns the md5 hash bytes for the current string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToMD5Hash(this String str)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                var hash = md5.ComputeHash(bytes);

                return hash;
            }
        }
    }
}
