namespace XFENewsApplication.Views;

/// <summary>
/// …Ë÷√“≥√Ê
/// </summary>
public sealed partial class SettingPage : Page
{
    public static SettingPage? Current { get; set; }
    public SettingPageViewModel ViewModel { get; set; } = new();
    public SettingPage()
    {
        Current = this;
        this.InitializeComponent();
        ViewModel.DialogService.RegisterDialog(orderFarmingRatioSettingContentDialog);
        ViewModel.DialogService.RegisterDialog(orderFarmingCountSettingContentDialog);
        ViewModel.DialogService.RegisterDialog(orderFarmingStartValueSettingContentDialog);
        ViewModel.DialogService.RegisterDialog(cleanCacheContentDialog);
        ViewModel.SettingService.AddComboBox(appThemeComboBox, ProfileHelper.GetEnumProfileSaveFunc<ElementTheme>(), ProfileHelper.GetEnumProfileLoadFuncForComboBox());
        ViewModel.SettingService.Initialize();
        ViewModel.SettingService.RegisterEvents();
    }
}
