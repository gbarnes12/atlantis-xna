﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameApplicationTools.Interfaces;
using GameApplicationTools.Misc;

namespace GameApplicationTools.Actors.Cameras
{
    public class ChaseCamera : Camera
    {
        Vector3 _target;
        Vector3 _position;

        Vector3 rotation;
        Vector3 translation;

        private IDrawableActor actor;
        private Vector3 relativePosition; //relative position to target


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


        public ChaseCamera(String ID, Vector3 relativePosition, IDrawableActor actor)
            : base(ID, actor.Position + relativePosition, actor.Position)
        {
            this.actor = actor;
            this.relativePosition = relativePosition;

            View = Matrix.CreateLookAt(Position, Target, Up);
            Projection = Utils.CreateProjectionMatrix();

        }

        public override void Update(GameTime gameTime)
        {
            Position = new Vector3(0,0,actor.Position.Z) + relativePosition;

            Up = Vector3.Up;

            Target = new Vector3(0,0,actor.Position.Z);

            // Recalculate View and Projection using the new Position, Target, and Up
            View = Matrix.CreateLookAt(Position, Target, Up);
            Projection = Utils.CreateProjectionMatrix();

            base.Update(gameTime);
        }


    }
}