using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameApplicationTools.Interfaces;
using GameApplicationTools.Misc;

namespace GameApplicationTools.Actors.Cameras
{
    public class ThirdPersonCamera : Camera
    {
        Vector3 _target;
        Vector3 _position;

        Vector3 rotation;
        Vector3 translation;

        public Matrix Rotation { get; set; }

        public override Vector3 Target
        {
            get { return _target; }
            set
            {
                _target = value;
                View = Matrix.CreateLookAt(Position, _target, Vector3.Up);
            }
        }

        public override Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                View = Matrix.CreateLookAt(_position, Target, Vector3.Up);
            }
        }


        private IDrawableActor actor;
        private Vector3 relativePosition; //relative position to target
        private Matrix shipRotationMatrix = Matrix.Identity;

        public ThirdPersonCamera(String ID,Vector3 relativePosition, IDrawableActor actor)
            :base(ID,actor.Position+relativePosition,actor.Position)
        {
            this.actor = actor;
            this.relativePosition = relativePosition;

            View = Matrix.CreateLookAt(Position, Target, Up);
            Projection = Utils.CreateProjectionMatrix();
        }

        public override void Update(GameTime gameTime)
        {
            shipRotationMatrix = Matrix.Lerp(shipRotationMatrix, actor.RotationMatrix, 0.1f);

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
