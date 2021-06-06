using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Symlinker
{
    public class FileConfig
    {
        public string SourceFileName { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public string ParentRootForIdentifyExpiredLink { get; set; }
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

    public enum FilterMode
    {
        Regular,
        Recursive
    }

    /// <summary>
    ///     Symlink folders and files inside the FolderToBeSearched
    /// </summary>
    public class AutoSearchConfig
    {
        public FilterMode Mode { get; set; } = FilterMode.Regular;
        public string FolderToBeSearched { get; set; }
        public IEnumerable<string> SymlinkTargetFolders { get; set; }

        public IEnumerable<TargetFilter> TargetFilters { get; set; } = new TargetFilter[] { };
    }

    public class Config
    {
        [JsonIgnore] public static Config CurrentConfig { get; set; } = new();

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
                {
                    "[主服01]主服/plugins",
                    "[主服02]地皮开放世界服/plugins",
                    "[主服03]剧情旷野世界服/plugins"
                },
                TargetFilters = new List<TargetFilter>()
            };
            var mod = new AutoSearchConfig
            {
                FolderToBeSearched = Path.Combine(Program.CurrentDir, "共享模组"),
                SymlinkTargetFolders = new[]
                {
                    "[主服01]主服/mods",
                    "[主服02]地皮开放世界服/mods",
                    "[主服03]剧情旷野世界服/mods"
                },
                TargetFilters = new List<TargetFilter>()
            };
            var sharedFile = new AutoSearchConfig
            {
                FolderToBeSearched = Path.Combine(Program.CurrentDir, "共享文件"),
                SymlinkTargetFolders = new[]
                {
                    "[主服01]主服",
                    "[主服02]地皮开放世界服",
                    "[主服03]剧情旷野世界服"
                },
                TargetFilters = new List<TargetFilter>()
            };

            var pluginConfig = new AutoSearchConfig
            {
                Mode = FilterMode.Recursive,
                FolderToBeSearched = Path.Combine(Program.CurrentDir, "共享插件配置"),
                SymlinkTargetFolders = new[]
                {
                    "[主服01]主服/plugins",
                    "[主服02]地皮开放世界服/plugins",
                    "[主服03]剧情旷野世界服/plugins"
                },
                TargetFilters = new List<TargetFilter>
                {
                    new() {Path = @"Essentials/userdata", Type = 2},
                    new() {Path = @"Essentials/warps", Type = 2},
                    new() {Path = @"Essentials/usermap.csv", Type = 1},
                    new() {Path = @"InvUnload\playerdata", Type = 2},
                    new() {Path = @"Multiverse-Core\worlds.yml", Type = 1},
                    new() {Path = @"CrazyCrates\data.yml", Type = 1},
                    new() {Path = @"CrazyCrates\Locations.yml", Type = 1},
                    new() {Path = @"PixelmonEssentials\data", Type = 2},
                    new() {Path = @"PlugMan\resourcemaps.yml", Type = 1},
                    new() {Path = @"PokeBackup\Backups.yml", Type = 1},
                    new() {Path = @"PokeBackup\BackupData", Type = 2},
                    new() {Path = @"React\cache", Type = 2},
                    new() {Path = @"React\worlds", Type = 2},
                    new() {Path = @"SuperVanish\data.yml", Type = 1},
                    new() {Path = @"WarpSystem\Backups", Type = 2},
                    new() {Path = @"WarpSystem\Memory", Type = 2},
                    new() {Path = @"WorldEdit\sessions", Type = 2},
                    new() {Path = @"HolographicDisplays\database.yml", Type = 1},
                    new() {Path = @"LegendLog\LegendaryCapture.log", Type = 1},
                    new() {Path = @"LegendLog\LegendaryReplace.log", Type = 1},
                    new() {Path = @"LegendLog\LegendarySpawn.log", Type = 1},
                    new() {Path = @"LagAssist\data.yml", Type =1},
                    new() {Path = @"ProtocolLib\lastupdate", Type = 1},
                    new() {Path = @"OpGuard\guard.log", Type = 1},
                    new() {Path = @"Plan\ServerInfoFile.yml", Type = 1},
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                }
            };
            /*new() {Path = "共享插件配置/Essentials/userdata", Type = 2},
            new() {Path = "共享插件配置/Essentials/warps", Type = 2},
            new() {Path = "共享插件配置/Essentials/usermap.csv", Type = 1},
            new() {Path = @"共享插件配置\InvUnload\playerdata", Type = 2},
            new() {Path = @"共享插件配置\Multiverse-Core\worlds.yml", Type = 1},
            new() {Path = @"共享插件配置\CrazyCrates\data.yml", Type = 1},
            new() {Path = @"共享插件配置\CrazyCrates\Locations.yml", Type = 1},
            new() {Path = @"共享插件配置\PixelmonEssentials\data", Type = 2},
            new() {Path = @"共享插件配置\PlugMan\resourcemaps.yml", Type = 1},
            new() {Path = @"共享插件配置\PokeBackup\Backups.yml", Type = 1},
            new() {Path = @"共享插件配置\PokeBackup\BackupData", Type = 2},
            new() {Path = @"共享插件配置\React\cache", Type = 2},
            new() {Path = @"共享插件配置\React\worlds", Type = 2},
            new() {Path = @"共享插件配置\SuperVanish\data.yml", Type = 1},
            new() {Path = @"共享插件配置\WarpSystem\Backups", Type = 2},
            new() {Path = @"共享插件配置\WarpSystem\Memory", Type = 2},
            new() {Path = @"共享插件配置\WorldEdit\sessions", Type = 2}*/
            config.AutoSearchConfigs = config.AutoSearchConfigs.Append(plugin);
            config.AutoSearchConfigs = config.AutoSearchConfigs.Append(mod);
            config.AutoSearchConfigs = config.AutoSearchConfigs.Append(pluginConfig);
            config.AutoSearchConfigs = config.AutoSearchConfigs.Append(sharedFile);

            return config;
        }
    }
}