namespace Crom.Controls
{
   partial class CenterButtonToDockFill
   {
      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent ()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager (typeof (CenterButtonToDockFill));
         this._fillImage = new System.Windows.Forms.PictureBox ();
         ((System.ComponentModel.ISupportInitialize)(this._fillImage)).BeginInit ();
         this.SuspendLayout ();
         // 
         // _fillImage
         // 
         this._fillImage.Image = ((System.Drawing.Image)(resources.GetObject ("_fillImage.Image")));
         this._fillImage.Location = new System.Drawing.Point (9, 9);
         this._fillImage.Name = "_fillImage";
         this._fillImage.Size = new System.Drawing.Size (24, 24);
         this._fillImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
         this._fillImage.TabIndex = 0;
         this._fillImage.TabStop = false;
         // 
         // CenterButtonToDockFill
         // 
         this.BackColor = System.Drawing.Color.FromArgb (((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
         this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject ("$this.BackgroundImage")));
         this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
         this.ClientSize = new System.Drawing.Size (42, 42);
         this.Controls.Add (this._fillImage);
         ((System.ComponentModel.ISupportInitialize)(this._fillImage)).EndInit ();
         this.ResumeLayout (false);
         this.PerformLayout ();

      }

      #endregion

      private System.Windows.Forms.PictureBox _fillImage;
   }
}