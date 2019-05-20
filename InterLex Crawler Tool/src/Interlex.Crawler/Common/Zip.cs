namespace Interlex.Crawler.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using ICSharpCode.SharpZipLib.Zip;
    using Interlex.Crawler.Model;

    /// <summary>
    /// Provides ziping capabilities
    /// </summary>
    internal static class Zip
    {
        /// <summary>
        /// Returns zip representation of the provided documents content (check: <see cref="DocumentModel.Raw"/>)
        /// </summary>
        /// <param name="documentGroup">Document group with documents to be zipped</param>
        /// <returns></returns>
        public static byte[] DocumentGroup(DocumentGroupModel documentGroup)
        {
            using (var outputMemStream = new MemoryStream())
            using (var s = new ZipOutputStream(outputMemStream))
            {
                s.SetLevel(9); // 0-9, 9 being the highest compression
                byte[] buffer = new byte[4096];
                foreach (var doc in documentGroup.Documents)
                {
                    var entry = new ZipEntry(Path.GetFileName(doc.Name));

                    // entry.DateTime = DateTime.Now;
                    s.PutNextEntry(entry);
                    using (var ms = new MemoryStream(doc.Raw, 0, doc.Raw.Length))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = ms.Read(buffer, 0, buffer.Length);
                            s.Write(buffer, 0, sourceBytes);
                        }
                        while (sourceBytes > 0);
                    }
                }

                s.Finish();

                return outputMemStream.ToArray();
            }
        }
    }
}
