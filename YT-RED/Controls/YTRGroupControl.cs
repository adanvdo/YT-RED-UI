using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace YT_RED.Controls
{
    public class YTRGroupControl : GroupControl
    {
        [Browsable(true)]
        public ControlGroups ControlGroup { get; set; } = ControlGroups.General;

        public YTRGroupControl() : base()
        {

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (ButtonsPanel != null && ButtonsPanel.Bounds.Contains(e.Location))
            {
                ButtonsPanel.Handler.OnMouseDown(e);
                return;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (ButtonsPanel != null && ButtonsPanel.Bounds.Contains(e.Location))
            {
                ButtonsPanel.Handler.OnMouseUp(e);
                return;
            }
            base.OnMouseUp(e);
        }
    }
}
