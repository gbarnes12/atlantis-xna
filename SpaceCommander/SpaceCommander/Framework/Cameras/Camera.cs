namespace GameApplicationTools.Actors.Cameras
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Actors;
    using Interfaces;
    using Misc;
    using Microsoft.Xna.Framework.Graphics;

    public class Camera
    {
        #region Public
        public String ID { get; set; }
        #endregion

        #region [Virtual]
        public virtual Vector3 Position { get; set; }

        public virtual BoundingBox BoundingBox
        { get { return new BoundingBox(Position - Vector3.One, Position + Vector3.One); } }

        public virtual Matrix View { get; set; }

        public virtual Matrix Projection { get; set; }

        public virtual Vector3 Target { get; set; }

        public virtual Vector3 Up { get; set; }

        public virtual Quaternion Rotation { get; set; }

        public virtual float FieldOfView { get; set; }

        public virtual float AspectRatio { get; set; }

        public virtual float NearPlane { get; set; }

        public virtual float FarPlane { get; set; }

        public BoundingFrustum Frustum { get; set; }

        /// <summary>
        /// Determines whether the 
        /// actor gets updated or not
        /// </summary>
        public virtual bool Updateable { get; set; }
        #endregion

        public Camera(String ID, Vector3 Position, Vector3 Target)
        {
            this.ID = ID;
            this.Position = Position;
            this.Target = Target;
            this.Up = Vector3.Up;
            Updateable = true;

            // create the view matrix
            View = Matrix.CreateLookAt(this.Position, this.Target, 
                                        this.Up);

            // create projection matrix
            Projection = Utils.CreateProjectionMatrix();

            // add the camera to the camera manager
            CameraManager.Instance.AddCamera(this.ID, this);
        }


        /// <summary>
        /// Returns the ray on which we just need to 
        /// check if it collides with any bounding sphere 
        /// in the scene!
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns>Ray with the near point and the far point</returns>
        public virtual Ray GetMouseRay(Vector2 mousePosition)
        {
            Viewport viewport = GameApplication.Instance.GetGraphics().Viewport;

            //GameApplication.Instance.NearPlane
            //GameApplication.Instance.FarPlane
            Vector3 nearSource = new Vector3(mousePosition, 0f);
            Vector3 farSource = new Vector3(mousePosition, -1000f);

            Vector3 nearPoint = viewport.Unproject(nearSource,
                Projection, View, Matrix.Identity);

            Vector3 farPoint = viewport.Unproject(farSource,
                Projection, View, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        

        public virtual void LoadContent() { }

        public virtual void Update(GameTime gameTime)
        {
            //View = Matrix.CreateLookAt(this.Position, this.Target, this.Up);

            if (Updateable)
            {
                Projection = Utils.CreateProjectionMatrix();

                Matrix matrix = Matrix.CreateFromQuaternion(Rotation);
                matrix.Translation = Position;

                View = Matrix.Invert(matrix);

                Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            }
        }
    }
}
