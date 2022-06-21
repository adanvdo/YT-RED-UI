using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraVerticalGrid;

namespace YT_RED.Controls
{
    public partial class PropertyGrid : DevExpress.XtraEditors.XtraUserControl
    {
		public string GridName { get { return this.pgcPropertyGrid.Name; } set { this.pgcPropertyGrid.Name = value; } }

		public int GridTabIndex { get { return this.pgcPropertyGrid.TabIndex; } set { this.pgcPropertyGrid.TabIndex = value; } }

		public object SelectedObject { get { return this.pgcPropertyGrid.SelectedObject; } set { this.pgcPropertyGrid.SelectedObject = value; } }

		public PropertyGridControl Grid { get { return this.pgcPropertyGrid; } set { this.pgcPropertyGrid = value; } }

		public PropertyGrid()
		{
			InitializeComponent();
			
		}

        private void pgcPropertyGrid_CustomPropertyDescriptors(object sender, DevExpress.XtraVerticalGrid.Events.CustomPropertyDescriptorsEventArgs e)
        {
			if(e.Properties.Find("Version", false) != null)
            {
				this.pgcPropertyGrid.OptionsBehavior.PropertySort = DevExpress.XtraVerticalGrid.PropertySort.NoSort;
				e.Properties = e.Properties.Sort(new string[] { "Version", "Build", "GitHub", "Contact" });
            } 
			if(e.Properties.Find("UseTitleAsFileName", false) != null)
            {
				this.pgcPropertyGrid.OptionsBehavior.PropertySort = DevExpress.XtraVerticalGrid.PropertySort.NoSort;
				e.Properties = e.Properties.Sort(new string[] { "UseTitleAsFileName", "AudioDownloadPath", "VideoDownloadPath", "AutomaticallyOpenDownloadLocation", "EnableDownloadHistory", "HistoryAge", "ErrorLogPath" });
            }
        }
    }
}
