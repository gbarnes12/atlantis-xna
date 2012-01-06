using System;
namespace GameApplicationTools.Actors.Properties.EffectPropertyControllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using GameApplicationTools.Actors.Cameras;

    /// <summary>
    /// Basic effect property controller. 
    /// This controller is used to update the specific 
    /// parameters in the shader.
    /// </summary>
    public class EffectPropertyController
    {
        /// <summary>
        /// Can be overwriten to add your own logic.
        /// Will update world, view and projection matrix
        /// parameters by default.
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="AbsoluteTransform"></param>
        public virtual void Update(Effect effect, Matrix AbsoluteTransform, Effect PropertyEffect)
        {
            Camera camera = CameraManager.Instance.GetCurrentCamera();

            effect.Parameters["World"].SetValue(AbsoluteTransform);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
        }
    }
}
