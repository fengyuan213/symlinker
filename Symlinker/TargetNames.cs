using System.IO;

namespace Symlinker
{
    public class TargetNames
    {

        public TargetNames(string relativePath, string parentRootPath)
        {
            RelativePath = relativePath;
            ParentRootPath = parentRootPath;
        }

        public string RelativePath { get; set; }
        public string ParentRootPath { get; set; }

        public string AbsolutePath
        {
            get
            {
                return Path.Combine(ParentRootPath, RelativePath);
            }
        }

 
    }
}