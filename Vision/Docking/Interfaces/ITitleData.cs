using System.Drawing;

namespace Crom.Controls
{
   /// <summary>
   /// Data from tool window title
   /// </summary>
   internal interface ITitleData
   {
      /// <summary>
      /// Title of the tool window
      /// </summary>
      /// <returns>title of the tool window</returns>
      string Title ();

      /// <summary>
      /// Icon of the tool window
      /// </summary>
      Icon Icon
      {
         get ;
      }
   }
}
