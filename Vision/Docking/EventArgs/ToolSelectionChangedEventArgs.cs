using System;
using System.Drawing;
using System.Windows.Forms;

namespace Docking.Controls
{
   /// <summary>
   /// Tool selection changed event args
   /// </summary>
   public class ToolSelectionChangedEventArgs : EventArgs
   {
      #region Fields.

      private ToolWindow          _selection        = null;

      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Create a new instance of <see cref="ContextMenuEventArg"/>
      /// </summary>
      /// <param name="selection">selected tool window</param>
      public ToolSelectionChangedEventArgs (ToolWindow selection)
      {
         _selection     = selection;
      }

      #endregion Instance.

      #region Public section.

      /// <summary>
      /// Getter for the tool window which was selected when context menu
      /// is requested.
      /// </summary>
      public ToolWindow Selection
      {
         get { return _selection; }
      }

      #endregion Public section.
   }
}
