using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameApplicationTools.Resources.PostProcessors
{
    public class BloomProcessor : PostProcessor
    {
        public BloomProcessor(string ID, string FileName)
            : base(ID, FileName)
        {
        }

        public override void Update(Microsoft.Xna.Framework.Graphics.Texture2D result)
        {
            Effect.Parameters["ColorMap"].SetValue(result);

            base.Update(result);
        }
    }
}
