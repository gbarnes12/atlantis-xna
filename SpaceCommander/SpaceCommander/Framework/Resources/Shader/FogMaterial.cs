namespace GameApplicationTools.Resources.Shader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// You can determine how to draw the fog on
    /// the specific object!
    /// </summary>
    public class FogMaterial : Material
    {
        public Vector3 AmbientColor { get; set; }
        public Vector3 LightDirection { get; set; }
        public Vector3 LightColor { get; set; }
        public Vector3 SpecularColor { get; set; }
        public Vector3 FogColor { get; set; }

        public float SpecularPower { get; set; }
        public float FogStart { get; set; }
        public float FogEnd { get; set; }

        public FogMaterial()
        {
            AmbientColor = new Vector3(.1f, .1f, .1f);
            LightDirection = new Vector3(1, 1, 1);
            LightColor = new Vector3(.9f, .9f, .9f);
            SpecularColor = new Vector3(1, 1, 1);
            FogColor = new Vector3(1, 1, 1);
            
            SpecularPower = 32f;
            FogStart = 1000f;
            FogEnd = GameApplication.Instance.FarPlane;
        }

        public override void SetEffectParameters(Effect effect)
        {
            if (effect.Parameters["AmbientColor"] != null)
                effect.Parameters["AmbientColor"].SetValue(AmbientColor);

            if (effect.Parameters["LightDirection"] != null)
                effect.Parameters["LightDirection"].SetValue(LightDirection);

            if (effect.Parameters["LightColor"] != null)
                effect.Parameters["LightColor"].SetValue(LightColor);

            if (effect.Parameters["SpecularColor"] != null)
                effect.Parameters["SpecularColor"].SetValue(SpecularColor);

            if (effect.Parameters["SpecularPower"] != null)
                effect.Parameters["SpecularPower"].SetValue(SpecularPower);

            if (effect.Parameters["FogColor"] != null)
                effect.Parameters["FogColor"].SetValue(FogColor);

            if (effect.Parameters["FogStart"] != null)
                effect.Parameters["FogStart"].SetValue(FogStart);

            if (effect.Parameters["FogEnd"] != null)
                effect.Parameters["FogEnd"].SetValue(FogEnd);
        }
    }
}
