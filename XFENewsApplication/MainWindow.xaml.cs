﻿using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;
using XFEExtension.NetCore.WinUIHelper.Utilities.Helper;

namespace XFENewsApplication;

/// <summary>
/// 主窗体
/// </summary>
public sealed partial class MainWindow : Window
{
    public static UISettings UISettings { get; set; } = new UISettings();
    public MainWindow()
    {
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/Icons/EditorIcon.ico"));
        AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        UISettings.ColorValuesChanged += (_, _) => DispatcherQueue.TryEnqueue(() => AppThemeHelper.ChangeTheme(AppThemeHelper.Theme));
    }
}