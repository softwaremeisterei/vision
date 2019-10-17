using Docking.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision.Forms
{
    public class ImageViewerToolWindow : ToolWindow
    {
        private System.Windows.Forms.PictureBox pictureBox1;
        private Image _originalImage;

        public ImageViewerToolWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(579, 463);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // ImageViewerToolWindow
            // 
            this.ClientSize = new System.Drawing.Size(579, 463);
            this.Controls.Add(this.pictureBox1);
            this.Name = "ImageViewerToolWindow";
            this.Text = "Image Viewer";
            this.ResizeEnd += new System.EventHandler(this.ImageViewerToolWindow_ResizeEnd);
            this.Resize += new System.EventHandler(this.ImageViewerToolWindow_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

            this.ToolWindowDockChanged += ImageViewerToolWindow_ToolWindowDockChanged;
        }

        private void ImageViewerToolWindow_ToolWindowDockChanged(object sender, EventArgs e)
        {
            FitImage();
        }

        public void SetImage(Image image)
        {
            if (pictureBox1 == null)
            {
                throw new InvalidOperationException("ImageViewerToolWindow must be created before calling SetImage");
            }

            _originalImage = image;
            FitImage();
        }

        private void ImageViewerToolWindow_Resize(object sender, EventArgs e)
        {
        }

        private void FitImage()
        {
            if (_originalImage == null)
            {
                return;
            }

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }

            var rect = new Rectangle();

            rect.Width = pictureBox1.Width;
            rect.Height = _originalImage.Height * pictureBox1.Width / _originalImage.Width;


            if (rect.Height > pictureBox1.Height)
            {
                rect.Width = _originalImage.Width * pictureBox1.Height / _originalImage.Height;
                rect.Height = pictureBox1.Height;
            }

            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            var resizedImage = GetResizedImage(_originalImage, rect);
            pictureBox1.Image = resizedImage;
        }

        public static Image GetResizedImage(Image img, Rectangle rect)
        {
            using (var b = new Bitmap(rect.Width, rect.Height))
            {
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(img, 0, 0, rect.Width, rect.Height);
                    g.Dispose();

                    return (Image)b.Clone();
                }
            }
        }

        private void ImageViewerToolWindow_ResizeEnd(object sender, EventArgs e)
        {
            FitImage();
        }
    }
}
