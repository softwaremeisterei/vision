using System.Windows.Forms;

namespace Docking.Controls
{
   /// <summary>
   /// Forma suport pentru docare in centru
   /// </summary>
   internal partial class DockButton : Form
   {
      #region Instance.

      /// <summary>
      /// Default constructor
      /// </summary>
      public DockButton ()
      {
         InitializeComponent ();
         TabStop = false;
      }

      #endregion Instance.

      #region Protected section.

      /// <summary>
      /// Hide form from Alt-Tab list
      /// </summary>
      protected override CreateParams CreateParams
      {
         get
         {
            CreateParams cp = base.CreateParams;
            // turn on WS_EX_TOOLWINDOW style bit
            cp.ExStyle |= 0x80;
            return cp;
         }
      }

      #endregion Protected section.

      #region Private section.

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent ()
      {
         this.SuspendLayout ();
         // 
         // DockButton
         // 
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
         this.BackColor = System.Drawing.Color.FromArgb (((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
         this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
         this.ClientSize = new System.Drawing.Size (86, 86);
         this.ControlBox = false;
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "DockButton";
         this.ShowIcon = false;
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
         this.TopMost = true;
         this.TransparencyKey = System.Drawing.Color.FromArgb (((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
         this.ResumeLayout (false);
      }

      #endregion Private section.
   }
}