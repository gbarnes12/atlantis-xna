namespace GameApplicationTools.Actors.Cameras
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Events;
    using UI;
    using Input;
    using Misc;

    public class FPSCamera : Camera
    {
        Vector3 _target;
        Vector3 _position;

        Vector3 rotation;
        Vector3 translation;

        public Matrix Rotation { get; set; }

        public override Vector3 Target
        {
            get { return _target; }
            set {
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



        public FPSCamera(String ID, Vector3 Position, Vector3 Target)
            : base(ID, Position, Target)
        {
            View = Matrix.CreateLookAt(Position, Target, Vector3.Up);
        }

        // This adds to rotation and translation to change the camera view
        public void RotateTranslate(Vector3 Rotation, Vector3 Translation)
        {
            translation += Translation;
            rotation += Rotation;
        }

       

        public override void Update(GameTime gameTime)
        {
            if (KeyboardDevice.Instance != null && MouseDevice.Instance != null)
            {
                Vector3 inputModifier = new Vector3(
                (KeyboardDevice.Instance.IsKeyDown(Keys.A) ? -1 : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.D) ? 1 : 0),
                (KeyboardDevice.Instance.IsKeyDown(Keys.Q) ? -1 : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.E) ? 1 : 0),
                (KeyboardDevice.Instance.IsKeyDown(Keys.W) ? -1 : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.S) ? 1 : 0)
                );

                RotateTranslate(new Vector3(MouseDevice.Instance.Delta.Y * -.002f, MouseDevice.Instance.Delta.X * -.002f, 0), inputModifier * .05f);

                Rotation = Utils.Vector3ToMatrix(rotation);

                translation = Vector3.Transform(translation, Rotation);
                Position += translation;

                // Reset translation
                translation = Vector3.Zero;

                // Calculate the new target
                Target = Vector3.Add(Position, Rotation.Forward);
            }


            // Calculate the direction from the position to the target, and normalize
            Vector3 newForward = Target - Position;
            newForward.Normalize();

            // Set the rotation matrix's forward to this vector
            Matrix rotationMatrixCopy = this.Rotation;
            rotationMatrixCopy.Forward = newForward;

            // Save a copy of "Up" (0, 1, 0)
            Vector3 referenceVector = Vector3.Up;

            // On the slim chance that the camera is pointed perfectly parallel with
            // the Y Axis, we cannot use cross product with a parallel axis, so we
            // change the reference vector to the forward axis (Z).
            if (rotationMatrixCopy.Forward.Y == referenceVector.Y
                || rotationMatrixCopy.Forward.Y == -referenceVector.Y)
                referenceVector = Vector3.Backward;

            // Calculate the other parts of the rotation matrix
            rotationMatrixCopy.Right = Vector3.Cross(this.Rotation.Forward,
                referenceVector);
            rotationMatrixCopy.Up = Vector3.Cross(this.Rotation.Right,
                this.Rotation.Forward);

            this.Rotation = rotationMatrixCopy;

            // Use the rotation matrix to find the new up
            Up = Rotation.Up;

            base.Update(gameTime);
        }

    }
}
