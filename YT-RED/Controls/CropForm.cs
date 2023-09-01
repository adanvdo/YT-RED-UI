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
    public partial class CropForm : DevExpress.XtraEditors.XtraForm
    {
        private Image image;

        public CropForm()
        {
            InitializeComponent();
        }

        public CropForm(Image image)
        {
            InitializeComponent();
            this.image = image;
            this.pePictureEdit.Image = this.image;
        }

        private void pePictureEdit_MouseDown(object sender, MouseEventArgs e)
        {

        }
    }
}