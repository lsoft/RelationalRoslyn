using Community.VisualStudio.Toolkit;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ReRoExtension
{
    internal partial class OptionsProvider
    {
        [ComVisible(true)]
        public class GeneralOptions : BaseOptionPage<General>
        {
        }
    }

    public class General : BaseOptionModel<General>
    {
        [Category("Viewer")]
        [DisplayName("SQL query font size")]
        [Description("A font size for SQL query text box.")]
        [DefaultValue(12)]
        public int SqlQueryFontSize{ get; set; } = 12;
    }
}
