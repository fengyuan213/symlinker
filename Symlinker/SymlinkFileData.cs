using System.Collections.Generic;
using System.IO;

namespace Symlinker
{
    public class SymlinkFileData
    {
        public SymlinkFileData(FileInfo fileInfo, IEnumerable<string> targetFileNames)
        {
            FileInfo = fileInfo;
            TargetFileNames = targetFileNames;
        }

        public FileInfo FileInfo { get; set; }
        public IEnumerable<string> TargetFileNames { get; set; }
    }
}