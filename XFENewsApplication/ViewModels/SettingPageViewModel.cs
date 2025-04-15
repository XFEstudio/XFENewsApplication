using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Diagnostics;
using XFEExtension.NetCore.FileExtension;
using XFEExtension.NetCore.WinUIHelper.Interface.Services;
using XFEExtension.NetCore.WinUIHelper.Utilities;
using XFENewsApplication.Core.Utilities.Helpers;
using XFENewsApplication.Profiles.CrossVersionProfiles;

namespace XFENewsApplication.ViewModels;

public partial class SettingPageViewModel : ViewModelBase
{
    [ObservableProperty]
    bool isAutoStartEnable = SystemProfile.AutoStart;
    [ObservableProperty]
    string appCacheDirectory = AppPathHelper.AppCache;
    [ObservableProperty]
    string appCacheSize = FileHelper.GetDirectorySize(new(AppPathHelper.AppCache)).FileSize();
    [ObservableProperty]
    string appDataDirectory = AppPathHelper.AppLocalData;
    [ObservableProperty]
    string appDataSize = FileHelper.GetDirectorySize(new(AppPathHelper.AppLocalData)).FileSize();
    public ISettingService SettingService { get; set; } = ServiceManager.GetService<ISettingService>();
    public IDialogService DialogService { get; set; } = ServiceManager.GetService<IDialogService>();

    partial void OnIsAutoStartEnableChanged(bool value)
    {
        SystemProfile.AutoStart = value;
        SetAutoStart(value);
    }

    private static void SetAutoStart(bool enable) => SetAutoStart(enable, "News");

    private static void SetAutoStart(bool enable, string appName, string exePath = "")
    {
        string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        using var key = Registry.CurrentUser.OpenSubKey(runKey, true);
        if (enable)
        {
            if (exePath == string.Empty)
                exePath = Environment.ProcessPath ?? string.Empty;

            key?.SetValue(appName, $"\"{exePath}\"");
        }
        else
        {
            if (key?.GetValue(appName) != null)
            {
                key.DeleteValue(appName);
            }
        }
    }

    [RelayCommand]
    static void OpenPath(string originalPath) => Helper.OpenPath(originalPath);

    [RelayCommand]
    async Task ClearCache()
    {
        if (await DialogService.ShowDialog("cleanCacheContentDialog") == ContentDialogResult.Primary)
        {
            Directory.Delete(AppPathHelper.AppCache, true);
            AppCacheSize = FileHelper.GetDirectorySize(new(AppPathHelper.AppCache)).FileSize();
        }
    }
}
