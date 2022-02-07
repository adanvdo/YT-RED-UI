using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YT_RED.Controls
{
    public partial class PropertyGrid : DevExpress.XtraEditors.XtraUserControl
    {
		public string GridName { get { return this.pgcPropertyGrid.Name; } set { this.pgcPropertyGrid.Name = value; } }

		public int GridTabIndex { get { return this.pgcPropertyGrid.TabIndex; } set { this.pgcPropertyGrid.TabIndex = value; } }

		public object SelectedObject { get { return this.pgcPropertyGrid.SelectedObject; } set { this.pgcPropertyGrid.SelectedObject = value; } }
		public PropertyGrid()
		{
			InitializeComponent();
		}
	}
}
