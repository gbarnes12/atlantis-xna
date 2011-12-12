namespace GameApplicationTools.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;

    public interface IGameLogic
    {
        void Update(GameTime gameTime, SceneGraphManager sceneGraph);
    }
}
