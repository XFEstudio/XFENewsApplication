using IWshRuntimeLibrary;

namespace XFENewsApplication.Core.Utilities.Helpers;

public static class FileHelper
{
    public static string[] RootPath { get; set; } = [@"C:\", @"D:\", @"E:\", @"F:\", @"G:\", @"H:\", @"I:\", @"J:\", @"K:\", @"L:\", @"M:\", @"N:\", @"O:\", @"P:\", @"Q:\", @"R:\", @"S:\", @"T:\", @"U:\", @"V:\", @"W:\", @"X:\", @"Y:\", @"Z:\",];
    public static long GetDirectorySize(DirectoryInfo directoryInfo)
    {
        long size = 0;
        FileInfo[] files = directoryInfo.GetFiles();
        foreach (FileInfo file in files)
        {
            size += file.Length;
        }
        DirectoryInfo[] directories = directoryInfo.GetDirectories();
        foreach (DirectoryInfo directory in directories)
        {
            size += GetDirectorySize(directory);
        }
        return size;
    }

    public static bool IsRootPath(string path) => RootPath.Any(rootPath => rootPath == path);

    public static void CreateShortCut(string targetPath, string originalPath, string argument, string description = "快捷方式")
    {
        var wshShortcut = (IWshShortcut)new WshShell().CreateShortcut(targetPath);
        wshShortcut.TargetPath = originalPath;
        wshShortcut.WorkingDirectory = Path.GetDirectoryName(originalPath);
        wshShortcut.WindowStyle = 1;
        wshShortcut.Description = description;
        wshShortcut.Arguments = argument;
        wshShortcut.Save();
    }
}
