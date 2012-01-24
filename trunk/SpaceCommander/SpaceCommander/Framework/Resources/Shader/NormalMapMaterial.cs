namespace GameApplicationTools.Resources.Shader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using System.ComponentModel;
    using GameApplicationTools.Structures.Editor;

    public class NormalMapMaterial : LightingMaterial
    {
#if !XBOX360
        [EditorAttribute(typeof(EditorTextureList), typeof(System.Drawing.Design.UITypeEditor))]
#endif
        public Texture2D NormalMap { get; set; }

        public NormalMapMaterial()
        {
        }

        public override void SetEffectParameters(Effect effect)
        {
            base.SetEffectParameters(effect);

            if (effect.Parameters["NormalTexture"] != null)
                effect.Parameters["NormalTexture"].SetValue(NormalMap);
        }

    }
}
