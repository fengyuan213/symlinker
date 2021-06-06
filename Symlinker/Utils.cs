using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SymbolicLinkSupport;

namespace Symlinker
{
    public static class Utils
    {
        public static string GetFileName(this string path)
        {
            if (path.Contains("/") || path.Contains("\\")) return Path.GetFileName(path);

            return path;
        }

        public static string GetRelativePath(this string absolutePath, string pathRelativeTo)
        {
            var tmp = string.Empty;
            pathRelativeTo += "/";
            if (pathRelativeTo.LastIndexOf('\\') ==
                pathRelativeTo.Length - 1 ||
                pathRelativeTo.LastIndexOf('/') ==
                pathRelativeTo.Length - 1)
            {
                tmp = pathRelativeTo.Substring(0,
                    pathRelativeTo.Length -1);
            }
            else
            {
                tmp = pathRelativeTo;
            };

            var result = absolutePath.Remove(0, tmp.Length +1);

            return result;
        }


        public enum DataType
        {
            file,
            folder
        }
        public static void CreateMissingDirectory(this string path,DataType dataType)
        {
            if ((dataType == DataType.file))
            {
                if (!File.Exists(path))
                {
                    path= path.Replace('\\', '/');
          
                    if (path.LastIndexOf('/')==path.Length-1)
                    {
                        path= path.Substring(0, path.Length - 1);
                    }

                    var folderToCreate = path.Substring(0, path.LastIndexOf('/'));
                    Directory.CreateDirectory(folderToCreate);
                }
            }
            else
            {

                Directory.CreateDirectory(path);
                Directory.Delete(path,true);
                
            
            }
        }
        public static void CreateSymlink(this SymlinkFolderData symlinkFolderData)
        {
            try
            {
                foreach (var targetFolder in symlinkFolderData.TargetFolders)
                {
                    if (Directory.Exists(targetFolder)||File.Exists(targetFolder))
                    {
                        var file = new DirectoryInfo(targetFolder);
                        if (!file.IsSymbolicLink() || !file.IsSymbolicLinkValid())
                        {
                            try
                            {
                                File.Delete(targetFolder);
                                Directory.Delete(targetFolder, true);
                            }
                            catch (Exception e)
                            {
                            }
                            
                            CreateSymlinkInternal();
                            return;
                        }
                    }
                    else
                    {
                        CreateSymlinkInternal();
                        return;
                    }


                    void CreateSymlinkInternal()
                    {
                        var target = "";
                        target = !Path.IsPathRooted(targetFolder)
                            ? Path.Combine(Program.CurrentDir, targetFolder)
                            : targetFolder;
                        target.CreateMissingDirectory(DataType.folder);
                        symlinkFolderData.DirectoryInfo.CreateSymbolicLink(target, false);
                        
                        Console.WriteLine(symlinkFolderData.DirectoryInfo.FullName + "--->" + target);
                    }
                }
            }
            catch (Exception e)
            {
                foreach (DictionaryEntry dictionaryEntry in e.Data)
                {
                    Console.WriteLine("Key:"+dictionaryEntry.Key+"Value:"+dictionaryEntry.Value);
                }
                Console.WriteLine(e);
            }
        }

        public static void CreateSymlink(this SymlinkFileData symlinkFileData)
        {
            try
            {
                foreach (var targetFileName in symlinkFileData.TargetFileNames)
                {
                    if (File.Exists(targetFileName.AbsolutePath)|| Directory.Exists(targetFileName.AbsolutePath))
                    {
                        var file = new FileInfo(targetFileName.AbsolutePath);
                        if (!file.IsSymbolicLink() || !file.IsSymbolicLinkValid())
                        {
                            try
                            {
                                File.Delete(targetFileName.AbsolutePath);
                                Directory.Delete(targetFileName.AbsolutePath, true);
                            }
                            catch (Exception e)
                            {
                            }
                            
                            CreateSymlinkInternal();
                            return;
                        }
                    }
                    else
                    {
                        CreateSymlinkInternal();
                        return;
                    }


                    void CreateSymlinkInternal()
                    {
                        var target = targetFileName.AbsolutePath;
                        /*
                        target = !Path.IsPathRooted(targetFileName.)
                            ? Path.Combine(Program.CurrentDir, targetFileName)
                            : targetFileName;
                            */


                        target.CreateMissingDirectory(DataType.file);
                        symlinkFileData.FileInfo.CreateSymbolicLink(target, false);

                      
                        Console.WriteLine(symlinkFileData.FileInfo.FullName + "--->" + target);
                    }
                }
            }
            catch (Exception e)
            {
                foreach (DictionaryEntry dictionaryEntry in e.Data)
                {
                    Console.WriteLine("Key:"+dictionaryEntry.Key+"Value:"+dictionaryEntry.Value);
                }
                Console.WriteLine(e);
            }
        }


        public static FileInfo GetFileInfo(string fileName)
        {
            return new(fileName);
        }

        public static DirectoryInfo GetDirectoryInfo(string fileName)
        {
            return new(fileName);
        }

        public static void GetFilesAndDirectories(string directory, out DirectoryInfo[] dirs, out FileInfo[] files,
            bool recursiveMode = false)
        {
            if (recursiveMode)
            {
                var dirInfo = new DirectoryInfo(directory);
                if (!dirInfo.Exists) throw new IOException("插件文件夹不存在");
                var allFiles = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
                directory = directory.TrimEnd();
                var tmp = string.Empty;
                if (directory.LastIndexOf('\\') == directory.Length - 1 ||
                    directory.LastIndexOf('/') == directory.Length - 1)
                    tmp = directory.Substring(0, directory.Length - 1); else tmp=directory;

                
                var allFilesRelative = new List<string>();
                var allFileInfos = new List<FileInfo>();
                foreach (var file in allFiles)
                {
                    allFileInfos.Add(new FileInfo(file));
                    var temp = file.Remove(0, tmp.Length + 1);

                    allFilesRelative.Add(temp);
                }

                files = allFileInfos.ToArray();
                dirs = dirInfo.GetDirectories();
            }
            else
            {
                var dirInfo = new DirectoryInfo(directory);
                if (!dirInfo.Exists) throw new IOException("插件文件夹不存在");
                dirs = dirInfo.GetDirectories();
                files = dirInfo.GetFiles();
            }
        }

        public static void GetFilesAndDirectories(IEnumerable<string> directories, out List<FileInfo> fileInfos,
            out List<DirectoryInfo> dirInfos, bool recursiveMode = false)
        {
            fileInfos = new List<FileInfo>();
            dirInfos = new List<DirectoryInfo>();

            foreach (var directory in directories)
            {
                var dirsData = new DirectoryInfo[] { };
                var filesData = new FileInfo[] { };
                GetFilesAndDirectories(directory, out dirsData, out filesData, recursiveMode);
                fileInfos.AddRange(filesData);
                dirInfos.AddRange(dirsData);
            }
        }
    }
}