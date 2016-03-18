using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTool.Common
{
    public static class FileTool
    {
        public static List<string> ListFiles(string dirPath, bool subDir = true, bool showHiddenFile = true)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            IEnumerable<FileInfo> fileList = subDir ? dir.GetFiles("*.*", SearchOption.AllDirectories)
                                                    : dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);

            return showHiddenFile ? fileList.Select(f => f.Name).OrderBy(f=>f).ToList()
                                  : fileList.Where(f => !f.FullName.Contains("\\.") && (f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden).Select(f => f.Name).OrderBy(f=>f).ToList();
        }
    }
}
