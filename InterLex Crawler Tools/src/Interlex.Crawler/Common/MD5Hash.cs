namespace Interlex.Crawler.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Provides md5 hasing capabilities
    /// </summary>
    public static class MD5Hash
    {
        /// <summary>
        /// Returns md5 hex representation of the provided byte array
        /// </summary>
        /// <param name="data">Byte array to calc hash for</param>
        /// <returns></returns>
        public static string GetMd5Hash(byte[] data)
        {
            var sb = new StringBuilder();
            using (var md5Hash = MD5.Create())
            {
                var hash = md5Hash.ComputeHash(data);

                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
            }

            return sb.ToString();
        }
    }
}
