using System.Windows.Forms;

namespace Docking.Controls
{
   /// <summary>
   /// Dock preview form
   /// </summary>
   internal partial class DockPreview : Form
   {
      #region Instance.

      /// <summary>
      /// Default constructor
      /// </summary>
      public DockPreview ()
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
   }
}