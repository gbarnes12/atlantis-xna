using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameApplicationTools.Actors.Advanced
{
    public class PointLight : Actor
    {
        public Color Color { get; set; }
        public float Attenuation { get; set; }

        public PointLight(String ID,String gameViewID,Vector3 Position, Color Color, float Attenuation)
            :base(ID,gameViewID)
        {
            this.Position = Position;
            this.Color = Color;
            this.Attenuation = Attenuation;
        }

        public void SetEffectParameters(Effect effect)
        {
            effect.Parameters["LightPosition"].SetValue(Position);
            effect.Parameters["LightAttenuation"].SetValue(Attenuation);
            effect.Parameters["LightColor"].SetValue(Color.ToVector3());
        }
    }
}
