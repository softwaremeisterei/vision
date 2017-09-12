namespace Crom.Controls
{
   /// <summary>
   /// Utility for enums
   /// </summary>
   internal class EnumUtility
   {
      /// <summary>
      /// Check if current value contains the checked value
      /// </summary>
      /// <param name="currentValue">current value</param>
      /// <param name="checkedValue">checked value</param>
      /// <returns>true if the current value contains checked value</returns>
      public static bool Contains (object currentValue, object checkedValue)
      {
         return ((int)currentValue & (int)checkedValue) == (int)checkedValue;
      }
   }
}
