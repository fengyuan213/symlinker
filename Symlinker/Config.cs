using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Symlinker
{
    public class FileConfig
    {
        public string SourceFileName { get; set; }
        public IEnumerable<string> SymlinkTargetName { get; set; }
    }

    /// <summary>
    ///     Symlink folder only doesn't look content inside it.
    /// </summary>
    public class FolderConfig
    {
        public string SourceFolderName { get; set; }
        public IEnumerable<string> SymlinkTargetFolder { get; set; }
    }

    /// <summary>
    ///     Symlink folders and files inside the FolderToBeSearched
    /// </summary>
    public class AutoSearchConfig
    {
        public string FolderToBeSearched { get; set; }
        public IEnumerable<string> SymlinkTargetFolders { get; set; }
    }

    public class Config
    {
        [JsonIgnore] public static Config CurrentConfig { get; set; } = new Config();

        /// <summary>
        ///     Create Directories Symlink
        /// </summary>
        public IEnumerable<FolderConfig> CustomFolderConfigs { get; set; } = new List<FolderConfig>();

        public IEnumerable<FileConfig> CustomFileConfigs { get; set; } = new List<FileConfig>();
        public IEnumerable<AutoSearchConfig> AutoSearchConfigs { get; set; } = new List<AutoSearchConfig>();

        public static Config GetDefaultConfig()
        {
            var config = new Config();
            var plugin = new AutoSearchConfig
            {
                FolderToBeSearched = Path.Combine(Program.CurrentDir, "共享插件"),
                SymlinkTargetFolders = new[]
                    {Path.Combine(Program.CurrentDir, "[主服01]TestFlight Server 0528-0.0.1/plugins")}
            };
            var pluginConfig = new AutoSearchConfig
            {
                FolderToBeSearched = Path.Combine(Program.CurrentDir, "共享插件配置"),
                SymlinkTargetFolders = new[]
                    {Path.Combine(Program.CurrentDir, "[主服01]TestFlight Server 0528-0.0.1/plugins")}
            };
            var mod = new AutoSearchConfig
            {
                FolderToBeSearched = Path.Combine(Program.CurrentDir, "共享模组"),
                SymlinkTargetFolders = new[]
                    {Path.Combine(Program.CurrentDir, "[主服01]TestFlight Server 0528-0.0.1/mods")}
            };

            var bukkitYml = new FileConfig
            {
                SourceFileName = Path.Combine(Program.CurrentDir, "bukkit.yml"),
                SymlinkTargetName = new[]
                    {Path.Combine(Program.CurrentDir, "[主服01]TestFlight Server 0528-0.0.1/bukkit.yml")}
            };
            var spigotYml = new FileConfig
            {
                SourceFileName = Path.Combine(Program.CurrentDir, "spigot.yml"),
                SymlinkTargetName = new[]
                    {Path.Combine(Program.CurrentDir, "[主服01]TestFlight Server 0528-0.0.1/spigot.yml")}
            };

            config.AutoSearchConfigs = config.AutoSearchConfigs.Append(plugin);
            config.AutoSearchConfigs = config.AutoSearchConfigs.Append(mod);
            config.AutoSearchConfigs = config.AutoSearchConfigs.Append(pluginConfig);
            config.CustomFileConfigs = config.CustomFileConfigs.Append(bukkitYml);
            config.CustomFileConfigs = config.CustomFileConfigs.Append(spigotYml);
            return config;
        }
    }
}