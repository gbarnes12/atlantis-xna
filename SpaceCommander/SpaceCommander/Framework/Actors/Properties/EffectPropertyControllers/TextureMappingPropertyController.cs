namespace GameApplicationTools.Actors.Properties.EffectPropertyControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class TextureMappingPropertyController : EffectPropertyController
    {
        public override void Update(Effect effect, Matrix AbsoluteTransform, Effect PropertyEffect)
        {
            effect.Parameters["CameraPosition"].SetValue(PropertyEffect.Parameters["CameraPosition"].GetValueVector3());
            effect.Parameters["LightDirection"].SetValue(PropertyEffect.Parameters["LightDirection"].GetValueVector3());

            effect.Parameters["AmbientIntensity"].SetValue(PropertyEffect.Parameters["DiffuseIntensity"].GetValueSingle());
            effect.Parameters["DiffuseIntensity"].SetValue(PropertyEffect.Parameters["DiffuseIntensity"].GetValueSingle());

            effect.Parameters["SpecularColorActive"].SetValue(PropertyEffect.Parameters["SpecularColorActive"].GetValueBoolean());

            effect.Parameters["DiffuseColor"].SetValue(PropertyEffect.Parameters["DiffuseColor"].GetValueVector4());
            effect.Parameters["AmbientColor"].SetValue(PropertyEffect.Parameters["AmbientColor"].GetValueVector4());
            effect.Parameters["SpecularColor"].SetValue(PropertyEffect.Parameters["SpecularColor"].GetValueVector4());

            base.Update(effect, AbsoluteTransform, PropertyEffect);
        }
    }
}
