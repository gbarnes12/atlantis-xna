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
    /// This follows a character not only based on its position 
    /// but also on its rotation. It can be used to have some sort 
    /// of full third person camera instead of the normal chase camera.
    /// 
    /// Author: Dominik Finkbeiner
    /// Version: 1.0
    /// </summary>
    public class ThirdPersonCamera : Camera
    {
        #region Private
        Vector3 _target;
        Vector3 _position;

        Vector3 rotation;
        Vector3 translation;

        private Actor actor;
        private Vector3 relativePosition; //relative position to target
        private Matrix shipRotationMatrix = Matrix.Identity;
        #endregion

        public ThirdPersonCamera(String ID,Vector3 relativePosition, Actor actor)
            :base(ID,actor.Position+relativePosition,actor.Position)
        {
            this.actor = actor;
            this.relativePosition = relativePosition;

            View = Matrix.CreateLookAt(Position, Target, Up);
            Projection = Utils.CreateProjectionMatrix();
        }

        /// <summary>
        /// The update method. It will update the camera's position and rotation regarding 
        /// an actor's position and rotation.
        /// </summary>
        /// <param name="gameTime">The elapsed game time - <see cref="GameTime"/></param>
        public override void Update(GameTime gameTime)
        {
            shipRotationMatrix = Matrix.Lerp(shipRotationMatrix, Matrix.CreateFromQuaternion(actor.Rotation), 0.1f);

            Position = Vector3.Transform(relativePosition,shipRotationMatrix)+actor.Position;

            Up = Vector3.Transform(Vector3.Up, shipRotationMatrix);

            Target = actor.Position;

            // Recalculate View and Projection using the new Position, Target, and Up
            View = Matrix.CreateLookAt(Position, Target, Up);
            Projection = Utils.CreateProjectionMatrix();

            base.Update(gameTime);
        }
    }
}
