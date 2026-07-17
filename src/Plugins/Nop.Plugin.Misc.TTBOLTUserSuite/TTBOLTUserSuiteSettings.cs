using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.TTBOLTUserSuite;

public class TTBOLTUserSuiteSettings : ISettings
{
    public bool Enabled { get; set; }

    public int MaximumPictureSizeBytes { get; set; }
}
