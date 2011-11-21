using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameApplicationTools.Interfaces
{
    public interface ICollideable
    {
        /// <summary>
        /// boundingsphere for handling collision
        /// </summary>
        BoundingSphere Sphere{get;set;}
    }
}
