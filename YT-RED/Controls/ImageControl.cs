using DevExpress.XtraEditors;
using System;
using System.Drawing;

namespace YTR.Controls
{
    public partial class ImageControl : DevExpress.XtraEditors.XtraUserControl
    {
        public ImageControl(XtraForm parent) : this(parent, null) { }
        public ImageControl(XtraForm parent, Image image)
        {
            this.Parent = parent;
            if(image == null)
                throw new ArgumentNullException(nameof(image));
            InitializeComponent();
            this.BackgroundImage = image;
            this.Width = image.Width + 10;
            this.Height = image.Height + 10;

        }
    }
}
