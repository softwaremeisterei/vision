namespace Crom.Controls
{
   partial class DockContainer
   {
      /// <summary> 
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent ()
      {
         this.components = new System.ComponentModel.Container ();
         this._mouseCheckTimer = new System.Windows.Forms.Timer (this.components);
         this.SuspendLayout ();
         // 
         // _temporizatorTestCursor
         // 
         this._mouseCheckTimer.Enabled = true;
         // 
         // DockContainer
         // 
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
         this.BackColor = System.Drawing.SystemColors.Window;
         this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.Name = "DockContainer";
         this.Size = new System.Drawing.Size (577, 485);

         this.ResumeLayout (false);

      }

      #endregion

      private System.Windows.Forms.Timer _mouseCheckTimer;
   }
}
