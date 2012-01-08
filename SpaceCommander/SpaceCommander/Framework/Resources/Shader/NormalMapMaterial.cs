namespace GameApplicationTools.Resources.Shader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;

    public class NormalMapMaterial : LightingMaterial
    {
        public Texture2D NormalMap { get; set; }

        public NormalMapMaterial(Texture2D NormalMap)
        {
            this.NormalMap = NormalMap;
        }

        public override void SetEffectParameters(Effect effect)
        {
            base.SetEffectParameters(effect);

            if (effect.Parameters["NormalTexture"] != null)
                effect.Parameters["NormalTexture"].SetValue(NormalMap);
        }

    }
}
