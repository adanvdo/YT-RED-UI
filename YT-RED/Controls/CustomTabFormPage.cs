using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YT_RED.Controls
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
