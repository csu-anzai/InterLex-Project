namespace Interlex.Crawler.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Interlex.Crawler.Common;

    /// <summary>
    /// Name with specific rules which represnts valid name in the database
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class NameModel
    {
        private static IReadOnlyCollection<char> allowedChars = "qwertyuiopasdfghjklzxcvbnm1234567890@_-".ToArray();

        /// <summary>
        /// Creates new name with the spcified value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws when the value has invalid charachters</exception>
        public static NameModel Create(String value)
        {
            value = value.ToLower();
            if (IsValid(value) == false)
            {
                throw new ArgumentException($"Cannot create name with value {value}.");
            }

            return new NameModel(value);
        }

        /// <summary>
        /// Unifies the provided value and creates the name with the unified value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws when the value has invalid charachters</exception>
        public static NameModel UnifyAndCreate(String value)
        {
            var unified = Unify(value);

            return Create(unified);
        }

        /// <summary>
        /// Unifies the last <see cref="Uri.Segments"/> of the url and creates name
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static NameModel UnifyAndCreateFromUrlLastPart(String url)
        {
            var lastPart = new Uri(url).Segments.Last();
            lastPart = Path.GetFileNameWithoutExtension(lastPart);

            return UnifyAndCreate(lastPart);
        }

        private NameModel(String value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Value
        /// </summary>
        public String Value { get; }

        private static bool IsValid(String name)
        {
            var isValid = String.IsNullOrEmpty(name) == false;
            isValid = isValid && name.Except(allowedChars).IsEmpty();
            isValid = isValid && name.Length < 240;

            return isValid;
        }

        private static String Unify(String name)
        {
            var unified = System.Net.WebUtility.HtmlDecode(name);

            unified = unified
                .SafeReplaceAll(new[] { "@", "]", "[", "#", "amp;", "=", "|", "?", "\"", ":", ">", "<", "*", "\\", "/", "‘", "%", "~", "!", "&", ".", ",", "’", "(", ")", "'" }, String.Empty)
                .Replace(" ", "_")
                .Replace(" ", "_")
                .Replace("–", "-");

            if (unified.Length >= 240)
            {
                unified = unified.ToHexMD5();
            }

            return unified;
        }
    }
}
