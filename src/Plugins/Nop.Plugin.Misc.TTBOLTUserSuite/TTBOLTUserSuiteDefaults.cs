namespace Nop.Plugin.Misc.TTBOLTUserSuite;

public static class TTBOLTUserSuiteDefaults
{
    public const int DefaultMaximumPictureSizeBytes = 5 * 1024 * 1024;

    public const string ProfilePictureRouteName = "Plugin.Misc.TTBOLTUserSuite.ProfilePicture";

    public const int ProfilePictureMenuTab = 9100;

    public const string ProfilePictureMenuClass = "customer-profile-picture";

    public static Dictionary<string, string> GetLocalizationResources()
    {
        return new Dictionary<string, string>
        {
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture"] = "عکس پروفایل",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Description"] = "تصویری برای حساب کاربری خود انتخاب کنید.",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Select"] = "انتخاب تصویر",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.DropHint"] = "تصویر را اینجا رها کنید یا برای انتخاب کلیک کنید",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Upload"] = "ذخیره تصویر",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Remove"] = "حذف تصویر",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Rules"] = "فرمت‌های JPEG، PNG، GIF و WebP تا حجم ۵ مگابایت مجاز هستند.",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.InvalidFormat"] = "فرمت تصویر مجاز نیست.",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.FileTooLarge"] = "حجم تصویر بیشتر از حد مجاز است.",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Required"] = "ابتدا یک تصویر انتخاب کنید.",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Saved"] = "عکس پروفایل ذخیره شد.",
            ["Plugins.Misc.TTBOLTUserSuite.ProfilePicture.Removed"] = "عکس پروفایل حذف شد."
        };
    }
}
