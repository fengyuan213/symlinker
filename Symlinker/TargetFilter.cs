using System.Collections.Generic;
using System.IO;

namespace Symlinker
{
    public class TargetFilter
    {
        /// <summary>
        ///     true for whitelist mode,default false blacklist mode
        /// </summary>
        /// <summary>
        ///     Blacklisted
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        ///     1 File, 2 Folder
        /// </summary>
        public int Type { get; set; } = 0;

    }
}