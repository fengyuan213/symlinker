using System;
using System.Collections;
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
        public static string CurrentDir { get; set; } = Directory.GetCurrentDirectory();

        private static void ProcessingData(ref List<SymlinkFolderData> folderDatas, ref List<SymlinkFileData> fileDatas)
        {
            foreach (var customFolderConfig in Config.CurrentConfig.CustomFolderConfigs)
            {
                var dirInfo = Utils.GetDirectoryInfo(customFolderConfig.SourceFolderName);
                var folderData = new SymlinkFolderData(dirInfo, customFolderConfig.SymlinkTargetFolder);
                folderDatas.Add(folderData);
            }

            foreach (var customFileConfig in Config.CurrentConfig.CustomFileConfigs)
            {
                var dirInfo = Utils.GetFileInfo(customFileConfig.SourceFileName);
                var targetNames = customFileConfig.SymlinkTargetName.Select(targetname =>
                {
             
                    var result = new TargetNames(targetname,CurrentDir);
                    return result;
                });
                var parentRoot = string.Empty;
                if (Path.IsPathRooted(customFileConfig.SourceFileName))
                {
                    parentRoot = customFileConfig.ParentRootForIdentifyExpiredLink;
                }
                else
                {
                    parentRoot = CurrentDir;
                }
                var fileData = new SymlinkFileData(dirInfo, targetNames,parentRoot);
                fileDatas.Add(fileData);
            }

            foreach (var autoSearchConfig in Config.CurrentConfig.AutoSearchConfigs)
                if (autoSearchConfig.Mode == FilterMode.Recursive)
                {
                    Utils.GetFilesAndDirectories(autoSearchConfig.FolderToBeSearched, out var directoryInfos,
                        out var fileInfos, true);
                  
        
                    var filteredFileRelativePaths = new List<string>();
                    var filteredFolderRelativePaths = new List<string>();
                    
                    if (autoSearchConfig.TargetFilters != null && autoSearchConfig.TargetFilters.Any())
                    {
                   
                        //Read Config
                        foreach (var targetFilter in autoSearchConfig.TargetFilters)
                        {
                            if (string.IsNullOrWhiteSpace(targetFilter.Path)) continue;
                            if (!Path.IsPathRooted(targetFilter.Path))
                                targetFilter.Path = Path.Combine(autoSearchConfig.FolderToBeSearched, targetFilter.Path);
                            if (targetFilter.Type != 0)
                            {
                                switch (targetFilter.Type)
                                {
                                    case 1:
                                        filteredFileRelativePaths.Add(targetFilter.Path.GetRelativePath(autoSearchConfig.FolderToBeSearched));
                                        break;
                                    case 2:
                                        filteredFolderRelativePaths.Add(targetFilter.Path.GetRelativePath(autoSearchConfig.FolderToBeSearched));
                                        break;
                                }
                            }
                            else
                            {
                                if (File.Exists(targetFilter.Path))
                                    filteredFileRelativePaths.Add(targetFilter.Path.GetRelativePath(autoSearchConfig.FolderToBeSearched));

                                if (Directory.Exists(targetFilter.Path))
                                    filteredFolderRelativePaths.Add(targetFilter.Path.GetRelativePath(autoSearchConfig.FolderToBeSearched));
                            }
                        }


                        foreach (var fileInfo in fileInfos)
                        {
                            if ( MatchFilterFolderRule(fileInfo.FullName, filteredFileRelativePaths) || MatchFilterFolderRule(fileInfo.FullName, filteredFolderRelativePaths))
                            {
                                continue;
                               
                            }
                            var targetNames = autoSearchConfig.SymlinkTargetFolders
                                .Select( targetFolder => new TargetNames(  Path.Combine(targetFolder,fileInfo.FullName.GetRelativePath(autoSearchConfig.FolderToBeSearched)),CurrentDir) ).ToList();
                            fileDatas.Add(new SymlinkFileData(fileInfo, targetNames,autoSearchConfig.FolderToBeSearched));
                        }

                    }
                }
                else
                {
                    Utils.GetFilesAndDirectories(autoSearchConfig.FolderToBeSearched, out var directoryInfos,
                        out var fileInfos);
                    var fileData = new List<SymlinkFileData>();
                    var folderData = new List<SymlinkFolderData>();
                    var filteredFileNames = new List<string>();
                    var filteredFolderNames = new List<string>();
                    if (autoSearchConfig.TargetFilters != null)
                    {
                        if (autoSearchConfig.TargetFilters.Any())
                            foreach (var targetFilter in autoSearchConfig.TargetFilters)
                            {
                                if (string.IsNullOrWhiteSpace(targetFilter.Path)) continue;
                                if (!Path.IsPathRooted(targetFilter.Path))
                                    targetFilter.Path = Path.Combine(CurrentDir, targetFilter.Path);
                                if (targetFilter.Type != 0)
                                {
                                    switch (targetFilter.Type)
                                    {
                                        case 1:
                                            filteredFileNames.Add(targetFilter.Path.GetFileName());
                                            break;
                                        case 2:
                                            filteredFolderNames.Add(targetFilter.Path.GetFileName());
                                            break;
                                    }
                                }
                                else
                                {
                                    if (File.Exists(targetFilter.Path))
                                        filteredFileNames.Add(targetFilter.Path.GetFileName());

                                    if (Directory.Exists(targetFilter.Path))
                                        filteredFolderNames.Add(targetFilter.Path.GetFileName());
                                }
                            }


                        foreach (var fileInfo in fileInfos)
                        {
                            if (filteredFileNames.Contains(fileInfo.Name)) continue;
                            /*var targetFolders = autoSearchConfig.SymlinkTargetFolders
                                .Select(targetFolder => Path.Combine(targetFolder, fileInfo.Name)).ToList();*/
                            var targetNames = autoSearchConfig.SymlinkTargetFolders
                                .Select( targetFolder => new TargetNames(  Path.Combine(targetFolder,fileInfo.FullName.GetRelativePath(autoSearchConfig.FolderToBeSearched)),CurrentDir) ).ToList();
                            fileDatas.Add(new SymlinkFileData(fileInfo, targetNames,autoSearchConfig.FolderToBeSearched));
                            fileData.Add(new SymlinkFileData(fileInfo, targetNames,autoSearchConfig.FolderToBeSearched));
                        }

                        foreach (var directoryInfo in directoryInfos)
                        {
                            if (filteredFolderNames.Contains(directoryInfo.Name)) continue;
                            var targetFolders = autoSearchConfig.SymlinkTargetFolders
                                .Select(targetFolder => Path.Combine(targetFolder, directoryInfo.Name)).ToList();
                            folderData.Add(new SymlinkFolderData(directoryInfo, targetFolders));
                        }

                        fileDatas.AddRange(fileData);
                        folderDatas.AddRange(folderData);
                    }
                }
        }

        /// <summary>
        ///  Match the is the filter folder rule apply to the specified path or folder
        /// </summary>
        /// <param name="absolutePath"> Absolute path of file or folder to be matched </param>
        /// <param name="filteredRelativePaths"></param>
        /// <returns></returns>
        public static bool MatchFilterFolderRule(string absolutePath,IEnumerable<string> filteredRelativePaths)
        {
            foreach (var filteredFolderName in filteredRelativePaths)
            {
                if (absolutePath. Contains(filteredFolderName))
                {


                    return true;

                }
            }

            return false;
        }

        
        private static void GetExpiredSymlink(ref List<SymlinkFolderData> folderDatas,
            ref List<SymlinkFileData> fileDatas, ref List<FileInfo> fileLinksToClean,
            ref List<DirectoryInfo> folderLinksToClean)
        {
            var fileDataNames = fileDatas.Select(data => data.FileInfo.FullName.GetRelativePath(data.ParentSourceRoot));
            var directoryDataNames = folderDatas.Select(data => data.DirectoryInfo.Name);

            
            foreach (var symlinkTargetFolders in GetAutoSearchConfigSymlinkTargetFolders())
            {
                Utils.GetFilesAndDirectories(symlinkTargetFolders, out var directoryInfos, out var fileInfos,true);

                var validFileInfos = new List<FileInfo>();
                foreach (var fileInfo in fileInfos)
                    if (fileInfo.Exists)
                    {
                        foreach (var fileDataName in fileDataNames)
                        {
                            if (fileInfo.FullName.GetRelativePath(CurrentDir).Contains(fileDataName))
                            {
                               validFileInfos.Add(fileInfo);
                            }
                        }
                       
                    }

                var unVerifiedFileInfos= fileInfos.Except(validFileInfos);
                foreach (var unVerifiedFileInfo in unVerifiedFileInfos)
                {
                    if (unVerifiedFileInfo.IsSymbolicLink())
                        fileLinksToClean.Add(unVerifiedFileInfo);
                }
               
                foreach (var directoryInfo in directoryInfos)
                    if (directoryInfo.Exists && !directoryDataNames.Contains(directoryInfo.Name))
                        if (directoryInfo.IsSymbolicLink())
                            folderLinksToClean.Add(directoryInfo);
            }
        }

        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(CurrentDir);
            Console.WriteLine("口袋之都服务器内部使用!");
            InitializeConfig();

            var folderDatas = new List<SymlinkFolderData>();
            var fileDatas = new List<SymlinkFileData>();
            Console.WriteLine("正在处理数据");
            ProcessingData(ref folderDatas, ref fileDatas);
            Console.WriteLine("数据处理完成");
            foreach (var fileData in fileDatas) fileData.CreateSymlink();
            foreach (var folderData in folderDatas) folderData.CreateSymlink();

            Console.WriteLine("创建完成，清理杂鱼!");

            var fileLinksToClean = new List<FileInfo>();
            var folderLinksToClean = new List<DirectoryInfo>();
            GetExpiredSymlink(ref folderDatas, ref fileDatas, ref fileLinksToClean, ref folderLinksToClean);
            Console.WriteLine("杂鱼统计完成，总计:{0}.即将清理!", fileLinksToClean.Count + folderLinksToClean.Count);
            foreach (var fileInfo in fileLinksToClean)
            {
                Console.WriteLine("带清理的文件Link:{0}",fileInfo);
            }
            foreach (var fileInfo in fileLinksToClean)
            {
                Console.WriteLine("带清理的文件夹Link:{0}",folderLinksToClean);
            }
            CleanSymlinks(ref fileLinksToClean, ref folderLinksToClean);
            Console.WriteLine("已经设置完成。可以开启服务器啦! 按任意键退出");
            Console.ReadLine();
        }

        private static void CleanSymlinks(ref List<FileInfo> fileLinksToClean,
            ref List<DirectoryInfo> folderLinksToClean)
        {
            foreach (var fileInfo in fileLinksToClean)
                try
                {
                    File.Delete(fileInfo.FullName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            foreach (var directoryInfo in folderLinksToClean)
                try
                {
                    Directory.Delete(directoryInfo.FullName, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
        }

        private static IEnumerable<string> GetAutoSearchConfigSymlinkTargetFolders()
        {
            var symlinkTargetFolderss =
                Config.CurrentConfig.AutoSearchConfigs.Select(config => config.SymlinkTargetFolders);
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
                    //Config.CurrentConfig = Config.GetDefaultConfig();
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