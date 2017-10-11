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
    public class ImageRepository
    {
        private const string IMAGE_DIR = "VisionImages";

        private static string RepositoryPath { get; set; }

        public ImageRepository(string projectRootDir)
        {
            RepositoryPath = Path.Combine(projectRootDir, IMAGE_DIR);

            if (!Directory.Exists(RepositoryPath))
            {
                Directory.CreateDirectory(RepositoryPath);
            }
        }

        public Image Get(string id)
        {
            var filename = Path.Combine(RepositoryPath, id + ".bmp");
            var image = Image.FromFile(filename);
            return image;
        }

        public string Add(Image image)
        {
            var id = Guid.NewGuid().ToString();
            var filename = Path.Combine(RepositoryPath, id + ".bmp");
            image.Save(filename, ImageFormat.Bmp);
            return id;
        }
    }
}
