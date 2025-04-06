using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.WinUIHelper.Utilities;
using XFEExtension.NetCore.WinUIHelper.Utilities.Helper;

namespace XFENewsApplication.Profiles.CrossVersionProfiles;

public partial class SystemProfile : XFEProfile
{
    public SystemProfile() => ProfilePath = $@"{AppPathHelper.LocalProfile}\{nameof(SystemProfile)}";

    [ProfileProperty]
    private ElementTheme theme;

    static partial void SetThemeProperty(ref ElementTheme value) => AppThemeHelper.ChangeTheme(value);
}
