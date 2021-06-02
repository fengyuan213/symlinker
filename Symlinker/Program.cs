using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SymbolicLinkSupport;

namespace Symlinker
{
    internal class Program
    {
        //    public static string CurrentDir { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public static string CurrentDir { get; set; } = @"C:\Users\fengy\Desktop\ServerBase";

        public static void Main(string[] args)
        {
            Console.WriteLine("口袋之都服务器内部使用!");
            InitializeConfig();

            var folderDatas = new List<SymlinkFolderData>();
            var fileDatas = new List<SymlinkFileData>();
            Console.WriteLine("正在处理数据");
            foreach (var customFolderConfig in Config.CurrentConfig.CustomFolderConfigs)
            {
                var dirInfo = Utils.GetDirectoryInfo(customFolderConfig.SourceFolderName);
                var folderData = new SymlinkFolderData(dirInfo, customFolderConfig.SymlinkTargetFolder);
                folderDatas.Add(folderData);
            }

            foreach (var customFileConfig in Config.CurrentConfig.CustomFileConfigs)
            {
                var dirInfo = Utils.GetFileInfo(customFileConfig.SourceFileName);
                var fileData = new SymlinkFileData(dirInfo, customFileConfig.SymlinkTargetName);
                fileDatas.Add(fileData);
            }

            foreach (var autoSearchConfig in Config.CurrentConfig.AutoSearchConfigs)
            {
                Utils.GetFilesAndDirectories(autoSearchConfig.FolderToBeSearched, out var directoryInfos,
                    out var fileInfos);
                var fileData = new List<SymlinkFileData>();
                var folderData = new List<SymlinkFolderData>();

                foreach (var fileInfo in fileInfos)
                {
                    var targetFolders = autoSearchConfig.SymlinkTargetFolders
                        .Select(targetFolder => Path.Combine(targetFolder, fileInfo.Name)).ToList();
                    fileData.Add(new SymlinkFileData(fileInfo, targetFolders));
                }

                foreach (var directoryInfo in directoryInfos)
                {
                    var targetFolders = autoSearchConfig.SymlinkTargetFolders
                        .Select(targetFolder => Path.Combine(targetFolder, directoryInfo.Name)).ToList();
                    folderData.Add(new SymlinkFolderData(directoryInfo, targetFolders));
                }

                fileDatas.AddRange(fileData);
                folderDatas.AddRange(folderData);
            }

            Console.WriteLine("数据处理完成");
            foreach (var fileData in fileDatas) fileData.CreateSymlink();
            foreach (var folderData in folderDatas) folderData.CreateSymlink();

            Console.WriteLine("创建完成，清理杂鱼!");
            var fileDataNames = fileDatas.Select(data=> data.FileInfo.Name);
            var directoryDataNames = folderDatas.Select(data => data.DirectoryInfo.Name);

            var fileLinksToClean = new List<FileInfo>();
            var folderLinksToClean = new List<DirectoryInfo>();
            
            foreach (var symlinkTargetFolders in GetAutoSearchConfigSymlinkTargetFolders())
            {
                Utils.GetFilesAndDirectories(symlinkTargetFolders,out var directoryInfos,out var fileInfos);
               
                foreach (var fileInfo in fileInfos)
                {
                    
                    if (fileInfo.Exists && !fileDataNames.Contains(fileInfo.Name) )
                    {
                        if (fileInfo.IsSymbolicLink())
                        {
                            fileLinksToClean.Add(fileInfo);
                        }
                    }
                
                    
                }

                foreach (var directoryInfo in directoryInfos)      
                {
                    if (directoryInfo.Exists && !directoryDataNames.Contains(directoryInfo.Name) )
                    {
                        if (directoryInfo.IsSymbolicLink())
                        {
                            folderLinksToClean.Add(directoryInfo);
                        }
                    }
                }
               
            }
            Console.WriteLine("杂鱼统计完成，总计:{0}.即将清理!",fileLinksToClean.Count+folderLinksToClean.Count);
            foreach (var fileInfo in fileLinksToClean)
            {
                try
                {
                    File.Delete(fileInfo.FullName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                        
                }
                   
            }
            foreach (var directoryInfo in folderLinksToClean)
            {
                try
                {
                    Directory.Delete(directoryInfo.FullName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                        
                }
                   
            }
        }

        private static IEnumerable<string> GetAutoSearchConfigSymlinkTargetFolders()
        {
            var symlinkTargetFolderss = Config.CurrentConfig.AutoSearchConfigs.Select(config => config.SymlinkTargetFolders);
            var targetFolders = symlinkTargetFolderss.SelectMany(symlinkTargetFolders => symlinkTargetFolders).ToList();
         
            return targetFolders.Distinct();
        }

     
        public static void InitializeConfig()
        {
            var configFile = Path.Combine(CurrentDir, "symlinker.json");
            try
            {
                if (File.Exists(configFile))
                {
                    Config.CurrentConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));
                }
                else
                {
                    File.WriteAllText(configFile, JsonConvert.SerializeObject(Config.GetDefaultConfig()));
                    Config.CurrentConfig = Config.GetDefaultConfig();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("无法初始化配置");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}