namespace GameApplicationTools.Misc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using System.IO;

    /// <summary>
    /// This class is supposed to load files from a 
    /// stream which is needed by our editor since
    /// we cannot use any content pipeline in there.
    /// 
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public static class StreamLoader
    {

        public static Stream LoadFileFromStream(string Name, string Path)
        {
           return File.OpenRead(Path + Name);
        }

        public static byte[] GetBytesFromFile(string Name, string Path)
        {
            // this method is limited to 2^32 byte files (4.2 GB)

            FileStream fs = File.OpenRead(Path + Name);
            try
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                return bytes;
            }
            catch(Exception e)
            {
                    fs.Close();
                    throw new Exception(e.Message);
            }

        }
    }
}
