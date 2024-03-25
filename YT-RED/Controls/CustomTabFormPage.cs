using System.ComponentModel;

namespace YTR.Controls
{
    public class CustomTabFormPage : DevExpress.XtraBars.TabFormPage
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        public bool IsLocked { get; set; }

        public CustomTabFormPage()
        {
            IsLocked = false;
        }
    }
}
