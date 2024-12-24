using System.ComponentModel;

namespace Easy.Net.Starter.Resources
{
    public enum EmbeddedRessource
    {
        [Description("appsettings.json")]
        Appsettings = 0,

        [Description("database.zip")]
        Database = 1,

        [Description("systemdll.zip")]
        SystemDll = 2,
    }
}
