using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Vision.BL.Lib
{
    public class KeyHelper
    {
        public static bool IsControlPressed()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }

        public static bool IsShiftPressed()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Shift;
        }

        public static bool IsAltPressed()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Alt;
        }

    }
}
