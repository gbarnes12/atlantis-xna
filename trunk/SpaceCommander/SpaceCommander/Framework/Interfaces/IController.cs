namespace GameApplicationTools.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework;

    using Actors.Cameras;

    /// <summary>
    /// The interface for some sort of controller in the 
    /// game application. This is used to hook some function
    /// into the update pipeline of an actor or a camera.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// This is used if the controller is responsible 
        /// for an actor node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="gameTime"></param>
        void UpdateSceneNode(IActorNode node, GameTime gameTime);

        /// <summary>
        /// This is used if the controller is responsible 
        /// for a camera.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="gameTime"></param>
        void UpdateCamera(Camera camera, GameTime gameTime);

        /// <summary>
        /// This is used if the controller is responsible 
        /// for an UINode.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="gameTime"></param>
        void UpdateUINode();
    }
}
