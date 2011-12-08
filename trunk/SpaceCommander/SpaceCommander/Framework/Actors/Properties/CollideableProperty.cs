using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameApplicationTools.Actors.Properties
{
    public class CollideableProperty : ActorProperty
    {
        public bool Collided { get; set; }

        public CollideableProperty()
        {
            Collided = false;
        }
    }
}
