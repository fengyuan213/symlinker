using System.Collections.Generic;
using System.IO;

namespace Symlinker
{
    public class SymlinkFileData
    {
        public SymlinkFileData(FileInfo fileInfo, IEnumerable<TargetNames> targetFileNames,string parentSourceRoot)
        {
            FileInfo = fileInfo;
            TargetFileNames = targetFileNames;
            ParentSourceRoot = parentSourceRoot;
        }

        public string ParentSourceRoot { get; set; }
        public FileInfo FileInfo { get; set; }
        public IEnumerable<TargetNames> TargetFileNames { get; set; }
   
    }
}