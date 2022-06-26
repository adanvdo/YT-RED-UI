﻿using DevExpress.XtraEditors;
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