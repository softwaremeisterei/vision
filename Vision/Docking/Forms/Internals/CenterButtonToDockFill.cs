using System.Windows.Forms;
using System.Drawing;

namespace Crom.Controls
{
   /// <summary>
   /// Center button for guiding dock fill
   /// </summary>
   internal partial class CenterButtonToDockFill : DockButton
   {
      #region Instance.

      /// <summary>
      /// Default constructor
      /// </summary>
      public CenterButtonToDockFill ()
      {
         InitializeComponent ();
      }

      #endregion Instance.

      #region Public section.

      /// <summary>
      /// Show fill preview button inside the form
      /// </summary>
      public bool ShowFillPreview
      {
         get { return _fillImage.Visible; }
         set { _fillImage.Visible = value; }
      }

      /// <summary>
      /// Checks if the given mouse location is contained in the fill preview button
      /// </summary>
      /// <param name="location">location</param>
      /// <returns>true if the fill preview button is visible and contains mouse location</returns>
      public bool Contains (Point location)
      {
         if (ShowFillPreview == false)
         {
            return false;
         }

         Rectangle bounds = RectangleToScreen(_fillImage.Bounds);
         return bounds.Contains(location);
      }

      #endregion Public section.
   }
}