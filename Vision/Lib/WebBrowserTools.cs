using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision.Lib
{
    public class WebBrowserTools
    {
        public static Bitmap GenerateScreenshot(WebBrowser wb, int width = -1, int height = -1)
        {
            while (wb.ReadyState != WebBrowserReadyState.Complete) { Application.DoEvents(); }

            // Set the size of the WebBrowser control
            wb.Width = width;
            wb.Height = height;

            if (width == -1)
            {
                // Take Screenshot of the web pages full width
                wb.Width = wb.Document.Body.ScrollRectangle.Width;
            }

            if (height == -1)
            {
                // Take Screenshot of the web pages full height
                wb.Height = wb.Document.Body.ScrollRectangle.Height;
            }

            // Get a Bitmap representation of the webpage as it's rendered in the WebBrowser control
            var bitmap = new Bitmap(wb.Width, wb.Height);
            wb.DrawToBitmap(bitmap, new Rectangle(0, 0, wb.Width, wb.Height));
            wb.Dispose();

            return bitmap;
        }
    }
}
