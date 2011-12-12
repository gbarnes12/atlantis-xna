namespace SpaceCommander.GameViews.Gameplay.GameLogics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;

    using GameApplicationTools.Interfaces;
    using GameApplicationTools.Actors;
    using GameApplicationTools;
    using GameApplicationTools.Actors.Properties;
    using GameApplicationTools.UI;

    public class CollisionGameLogic : IGameLogic
    {
        public void Update(Microsoft.Xna.Framework.GameTime gameTime, SceneGraphManager sceneGraph)
        {
            //check collision
            foreach (Actor actor in sceneGraph.RootNode.Children)
            {
                if (actor.Properties.ContainsKey(ActorPropertyType.COLLIDEABLE) && actor.Visible)
                {

                    BoundingSphere transformedSphere = actor.BoundingSphere.Transform(actor.AbsoluteTransform);
                    BoundingSphere transformedLaserSphere = WorldManager.Instance.GetActor("testlaser").BoundingSphere.Transform(WorldManager.Instance.GetActor("testlaser").AbsoluteTransform);


                    if (WorldManager.Instance.GetActor("testlaser").Visible)
                    {
                        if (transformedSphere.Intersects(transformedLaserSphere))
                        {
                            WorldManager.Instance.GetActor("testlaser").Visible = false;
                            GameConsole.Instance.WriteLine("Collision!");

                            actor.Visible = false;
                            actor.Updateable = false;
                        }
                    }
                }
            }
        }
    }
}
