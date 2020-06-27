using System.IO;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace HTMLParser.Processing.Processors
{
    public class ImageProcessor
    {
       public void ResizeImage(string filePath, string outputDirectory)
        {
            System.IO.Directory.CreateDirectory(outputDirectory+"\\"+"resized");

            //Configuration.Default.ImageFormatsManager.SetEncoder(PngFormat.Instance, new PngEncoder()
            //{
            //    CompressionLevel = PngCompressionLevel.BestCompression
            //});

            using (Image image = Image.Load(outputDirectory+"\\"+filePath))
            {
                // Add options for different file sizes, larger files need bigger reductions.
                // TODO: Setup different size options

               
                    image.Mutate(x => x
                     .Resize(300, 0));



                string fileName = Path.GetFileNameWithoutExtension(filePath);
                image.Save(outputDirectory+"\\resized\\"+fileName+".jpg"); // Automatic encoder selected based on extension.
            }
        }
    }
}
