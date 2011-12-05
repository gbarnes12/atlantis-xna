using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameApplicationTools.Actors.Properties
{
    public class PickableProperty : ActorProperty
    {
        public bool IsPicked { get; set; }

        public PickableProperty()
        {
            IsPicked = false;
        }
    }
}
