using System;
using System.Drawing;

namespace Docking.Controls
{
   /// <summary>
   /// Auto-hide event args
   /// </summary>
   public class AutoHideEventArgs : EventArgs
   {
      #region Fields.

      private DockMode _selection = DockMode.None;

      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Create a new instance of <see cref="ContextMenuEventArg"/>
      /// </summary>
      /// <param name="selection">dock mode of the selected panel</param>
      public AutoHideEventArgs (DockMode selection)
      {
         _selection = selection;
      }

      #endregion Instance.

      #region Public section.

      /// <summary>
      /// Getter for the dock mode of the panel for which the auto-hide state was toggled.
      /// </summary>
      public DockMode Selection
      {
         get { return _selection; }
      }

      #endregion Public section.
   }
}
