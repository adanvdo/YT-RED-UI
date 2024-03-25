using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace YTR.Controls
{
    public partial class CropForm : DevExpress.XtraEditors.XtraForm
    {
        private Image image;
        private Size useSize;
        private Point mouseDownPoint = Point.Empty;
        private Rectangle area = Rectangle.Empty;
        private int[] crops = null;
        public int[] Crops { get { return crops; } }

        public CropForm()
        {
            InitializeComponent();
        }

        public CropForm(Image image, Size useSize)
        {
            InitializeComponent();
            this.image = image;
            this.useSize = useSize;
            this.pePictureEdit.Image = this.image;
        }

        private void resetArea()
        {
            area = Rectangle.Empty;
            pePictureEdit.Refresh();
        }

        bool checkBounds(int x, int y)
        {
            var viewInfo = pePictureEdit.GetViewInfo() as PictureEditViewInfo;
            var visibleBounds = viewInfo.PictureScreenBounds;
            bool result = x >= visibleBounds.X && x <= (visibleBounds.X + visibleBounds.Width) && y >= visibleBounds.Y && y <= (visibleBounds.Y + visibleBounds.Height);
            return result;
        }

        private void clear()
        {
            var old = pePictureEdit.Image;
            pePictureEdit.Image = null;
            if (old != null) old.Dispose();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            clear();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            clear();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void pePictureEdit_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !checkBounds(e.Location.X, e.Location.Y)) return;
            mouseDownPoint = e.Location;
            resetArea();
            PictureEdit edit = sender as PictureEdit;
            edit.Refresh();
        }

        private void pePictureEdit_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouseMovePoint = e.Location;
            if (checkBounds(mouseMovePoint.X, mouseMovePoint.Y))
            {
                pePictureEdit.Cursor = Cursors.Cross;
                if (mouseDownPoint.IsEmpty) return;
                if (e.Button != MouseButtons.Left) return;

                area = new Rectangle(mouseDownPoint.X, mouseDownPoint.Y, mouseMovePoint.X - mouseDownPoint.X, mouseMovePoint.Y - mouseDownPoint.Y);
                PictureEdit edit = sender as PictureEdit;
                edit.Refresh();
            }
            else
            {
                pePictureEdit.Cursor = Cursors.Default;
            }
        }

        private void pePictureEdit_MouseUp(object sender, MouseEventArgs e)
        {            
            if(area == Rectangle.Empty || area.Width < 50 || area.Height < 50)
            {
                resetArea();
                this.crops = null;
                btnOk.Enabled = false;
                return;
            }
            btnOk.Enabled = true;
            PictureEdit edit = sender as PictureEdit;
            var viewInfo = edit.GetViewInfo() as PictureEditViewInfo;
            var visibleBounds = viewInfo.PictureScreenBounds;
            Point start = convertPoint(edit, area.Location);
            Point end = convertPoint(edit, new Point(area.Right, area.Bottom));

            Rectangle selectedImageRectangle = new Rectangle(start.X, start.Y, end.X - start.X, end.Y - start.Y);
            if (selectedImageRectangle.Size.Width <= 0 || selectedImageRectangle.Size.Height <= 0) return;

            this.crops = calculateCrops(this.useSize, visibleBounds, start, end);
        }

        private int[] calculateCrops(Size imageSize, RectangleF visibleBounds, Point start, Point end)
        {
            var ratio = imageSize.Width / visibleBounds.Width;
            Point adjustStart = new Point((int)(start.X * ratio), (int)(start.Y * ratio));
            Point adjustEnd = new Point((int)(end.X * ratio), (int)(end.Y * ratio));

            var top = adjustStart.Y;
            var bottom = imageSize.Height - adjustEnd.Y;
            var left = adjustStart.X;
            var right = imageSize.Width - adjustEnd.X;

            return new int[] { top, bottom, left, right };
        }

        private void pePictureEdit_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Red), area);
        }

        private Point convertPoint(PictureEdit edit, Point p)
        {
            Point result = Point.Empty;
            if (edit.Image != null)
            {
                PictureEditViewInfo vi = edit.GetViewInfo() as PictureEditViewInfo;

                int scrollX = (edit.Controls[1] as DevExpress.XtraEditors.HScrollBar).Value;
                int scrollY = (edit.Controls[0] as DevExpress.XtraEditors.VScrollBar).Value;

                int x, y;
                if (edit.Controls[1].Visible == true)
                    x = (int)(p.X + scrollX - vi.PictureScreenBounds.X);
                else
                    x = (int)(p.X - vi.PictureScreenBounds.X);
                if (edit.Controls[0].Visible == true)
                    y = (int)(p.Y + scrollY - vi.PictureScreenBounds.Y);
                else
                    y = (int)(p.Y - vi.PictureScreenBounds.Y);
                result = new Point(x, y);
            }
            return result;
        }
    }
}