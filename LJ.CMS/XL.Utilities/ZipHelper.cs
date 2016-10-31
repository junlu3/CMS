using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;

namespace XL.Utilities
{
    public class ZipHelper
    {
        public static void CompressFiles(string targetFile, List<string> files)
        {
            using (ZipFile zip = ZipFile.Create(targetFile))
            {
                zip.BeginUpdate();
                foreach (var file in files)
                {
                    zip.Add(file, System.IO.Path.GetFileName(file));
                }
                zip.CommitUpdate();
            }
        }
    }
}
