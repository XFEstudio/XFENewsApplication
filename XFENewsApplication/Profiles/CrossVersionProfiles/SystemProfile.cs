using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.WinUIHelper.Utilities;
using XFEExtension.NetCore.WinUIHelper.Utilities.Helper;

namespace XFENewsApplication.Profiles.CrossVersionProfiles;

public partial class SystemProfile : XFEProfile
{
    public SystemProfile() => ProfilePath = $@"{AppPathHelper.LocalProfile}\{nameof(SystemProfile)}";
    /// <summary>
    /// 主题颜色
    /// </summary>
    [ProfileProperty]
    private ElementTheme theme = ElementTheme.Default;
    /// <summary>
    /// 侧边栏距离
    /// </summary>
    [ProfileProperty]
    private double sideBarWidth = 450;
    /// <summary>
    /// 是否自动启动
    /// </summary>
    [ProfileProperty]
    private bool autoStart = false;
    /// <summary>
    /// 是否显示预览图
    /// </summary>
    [ProfileProperty]
    private bool showPreviewImage = false;

    static partial void SetThemeProperty(ref ElementTheme value) => AppThemeHelper.ChangeTheme(value);
}
