using Nop.Data.Mapping;
using Nop.Plugin.Misc.TTBOLTContentSuite.Domain;

namespace Nop.Plugin.Misc.TTBOLTContentSuite.Data;
public class NameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new() {
        { typeof(TTBlogPost), "BlogPost" }
    };

    public Dictionary<(Type, string), string> ColumnName => new();
}
