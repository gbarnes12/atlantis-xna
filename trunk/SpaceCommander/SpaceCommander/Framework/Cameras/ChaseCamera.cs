namespace GameApplicationTools.Actors.Cameras
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework;

    using Interfaces;
    using Misc;

    /// <summary>
    /// This camera class is used to chase a target but won't take 
    /// any rotation parameters because it is just a static camera 
    /// which follows it target. It is used in the game codenamed 
    /// SpaceCommander and represents the implementation of the 
    /// StarFox Camera. 
    /// 
    /// Author: Dominik Finkbeiner
    /// Version: 1.0
    /// </summary>
    public class ChaseCamera : Camera
    {
        #region Private
        Vector3 _target;
        Vector3 _position;

        Vector3 rotation;
        Vector3 translation;

        private Actor actor;
        private Vector3 relativePosition; //relative position to target
        #endregion

        public ChaseCamera(String ID, Vector3 relativePosition, Actor actor)
            : base(ID, actor.Position + relativePosition, actor.Position)
        {
            this.actor = actor;
            this.relativePosition = relativePosition;

            View = Matrix.CreateLookAt(Position, Target, Up);
            Projection = Utils.CreateProjectionMatrix();

        }

        /// <summary>
        /// The update method. Updates the position and the target of the camera
        /// regarding the position of an actor. Won't rotate around the actor
        /// or anything else. 
        /// </summary>
        /// <param name="gameTime">The elapsed game time - <see cref="GameTime"/></param>
        public override void Update(GameTime gameTime)
        {
            Position = new Vector3(0,0,actor.Position.Z) + relativePosition;

            Up = Vector3.Up;

            Target = new Vector3(0,0,actor.Position.Z);

            // Recalculate View and Projection using the new Position, Target, and Up
            base.Update(gameTime);
        }


    }
}
