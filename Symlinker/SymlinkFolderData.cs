using System.Collections.Generic;
using System.IO;

namespace Symlinker
{
    public class SymlinkFolderData
    {
        public SymlinkFolderData(DirectoryInfo directoryInfo, IEnumerable<string> targetFolders)
        {
            DirectoryInfo = directoryInfo;
            TargetFolders = targetFolders;
        }

        public DirectoryInfo DirectoryInfo { get; set; }

        public IEnumerable<string> TargetFolders { get; set; }
    }
}