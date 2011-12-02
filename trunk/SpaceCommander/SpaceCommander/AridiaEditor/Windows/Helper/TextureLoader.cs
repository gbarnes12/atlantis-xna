namespace AridiaEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Media.Imaging;
    using System.IO;
    using AridiaEditor.Properties;
    using Microsoft.Xna.Framework.Graphics;
    using System.Windows.Controls;
    using System.Media;
    using GameApplicationTools;
    using System.Drawing;
    using System.Windows.Media;

    public static class TextureImageLoader
    {
        public static List<BitmapImage> LoadImages()
        {
            List<BitmapImage> images = new List<BitmapImage>();
            
            foreach (Texture2D texture in ResourceManager.Instance.GetResourcesOfType<Texture2D>().Values)
            {
                images.Add(TextureImageLoader.Texture2Image(texture));
            }

            return images;
        }

        public static BitmapImage Texture2Image(Texture2D texture)
        {
            try
            {
                if (texture == null)
                {
                    return null;
                }

                if (texture.IsDisposed)
                {
                    return null;
                }

                //Memory stream to store the bitmap data.
                MemoryStream ms = new MemoryStream();

                //Save the texture to the stream.
                texture.SaveAsJpeg(ms, texture.Width, texture.Height);

                ms.Seek(0, SeekOrigin.Begin);

                byte[] bytes = ms.ToArray();

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(bytes);
                bi.EndInit();

                //Close the stream, we nolonger need it.
                ms.Close();
                ms = null;

                return bi;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
