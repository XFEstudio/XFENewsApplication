using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using XFEExtension.NetCore.FileExtension;
using XFEExtension.NetCore.WinUIHelper.Interface.Services;
using XFEExtension.NetCore.WinUIHelper.Utilities;
using XFENewsApplication.Core.Utilities.Helpers;

namespace XFENewsApplication.ViewModels;

public partial class SettingPageViewModel : ViewModelBase
{
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


    [RelayCommand]
    static void OpenPath(string originalPath) => Process.Start("explorer.exe", originalPath);

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
