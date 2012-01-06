namespace GameApplicationTools.Actors.Properties
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework.Graphics;
    using EffectPropertyControllers;
    using Microsoft.Xna.Framework;


    public class EffectProperty : ActorProperty
    {
        public Effect Effect { get; set; }

        public String Name { get; set; }

        public EffectPropertyController Controller { get; set; }

        public EffectProperty(String name)
        {
            this.Name = name;
            Controller = new EffectPropertyController();
        }

        public void Update(Effect effect, Matrix AbsoluteTransform)
        {
            Controller.Update(effect, AbsoluteTransform, Effect);
        }
    }
}
