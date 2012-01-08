namespace GameApplicationTools.Resources.Shader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The basic material class which can be used
    /// to create other materials.
    /// </summary>
    public class Material
    {
        public virtual void SetEffectParameters(Effect effect)
        {
        }
    }
}
