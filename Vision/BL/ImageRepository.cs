using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision.BL
{
    public static class ImageRepository
    {
        private const string IMAGE_DIR = "VisionImages";

        private static string RepositoryPath { get; set; }

        static ImageRepository()
        {
            string rootPath = Path.GetDirectoryName(Application.ExecutablePath);
            RepositoryPath = Path.Combine(rootPath, IMAGE_DIR);

            if (!Directory.Exists(RepositoryPath))
            {
                Directory.CreateDirectory(RepositoryPath);
            }
        }

        public static Image Get(string id)
        {
            var filename = Path.Combine(RepositoryPath, id + ".bmp");
            var image = Image.FromFile(filename);
            return image;
        }

        public static string Add(Image image)
        {
            var id = Guid.NewGuid().ToString();
            var filename = Path.Combine(RepositoryPath, id + ".bmp");
            image.Save(filename, ImageFormat.Bmp);
            return id;
        }
    }
}
