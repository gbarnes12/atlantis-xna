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

    /// <summary>
    /// This is the basic camera which is mainly just 
    /// a static camera which looks to an target. 
    /// It has no movement attached to it and won't rotate or 
    /// move in any instance as long as you don't attach any controller
    /// to it!
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class Camera
    {
        #region Public
        /// <summary>
        /// The unique id of this camera
        /// which we can use to get it from 
        /// the camera manager.
        /// </summary>
        public String ID { get; set; }

        /// <summary>
        /// We can attach a controller to the camera
        /// which can handle some sort of special movement
        /// operation or other things which might be necessary
        /// maybe for later implementation of cutscenes or anything else.
        /// </summary>
        public IController Controller { get; set; }
        #endregion

        #region [Virtual]
        /// <summary>
        /// The position of the camera within the world.
        /// </summary>
        public virtual Vector3 Position { get; set; }

        /// <summary>
        /// The bounding box to check if the camera 
        /// collides with something we could use 
        /// a bounding sphere, too. 
        /// </summary>
        public virtual BoundingBox BoundingBox
        { 
            get { return new BoundingBox(Position - Vector3.One, Position + Vector3.One); } 
        }

        /// <summary>
        /// The View Matrix!
        /// </summary>
        public virtual Matrix View { get; set; }

        /// <summary>
        /// The Projection Matrix
        /// </summary>
        public virtual Matrix Projection { get; set; }

        /// <summary>
        /// The target to which the camera points.
        /// </summary>
        public virtual Vector3 Target { get; set; }

        /// <summary>
        /// The up vector
        /// </summary>
        public virtual Vector3 Up { get; set; }

        /// <summary>
        /// The current rotation of the camera
        /// </summary>
        public virtual Quaternion Rotation { get; set; }

        /// <summary>
        /// The field of view
        /// </summary>
        public virtual float FieldOfView { get; set; }

        /// <summary>
        /// AspectRatio.
        /// </summary>
        public virtual float AspectRatio { get; set; }

        /// <summary>
        /// The nearest point to view of the camera
        /// </summary>
        public virtual float NearPlane { get; set; }

        /// <summary>
        /// The farthest point to view.
        /// </summary>
        public virtual float FarPlane { get; set; }

        /// <summary>
        /// The bounding frustum which can be used
        /// to check if objects are within the field of view.
        /// </summary>
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

            this.NearPlane = GameApplication.Instance.NearPlane;
            this.FarPlane = GameApplication.Instance.FarPlane;
            this.AspectRatio = (float)GameApplication.Instance.GetGraphics().Viewport.Width /
                            (float)GameApplication.Instance.GetGraphics().Viewport.Height;
            this.FieldOfView = MathHelper.PiOver4;

            // create the view matrix
            View = Matrix.CreateLookAt(this.Position, this.Target, this.Up);

            // create projection matrix
            Projection = Utils.CreateProjectionMatrix();

            Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));

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
            Vector3 farSource = new Vector3(mousePosition, 1f);

            Vector3 nearPoint = viewport.Unproject(nearSource,
                Projection, View, Matrix.Identity);

            Vector3 farPoint = viewport.Unproject(farSource,
                Projection, View, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        /// <summary>
        /// The load content method. This just does nothing!
        /// </summary>
        public virtual void LoadContent() { }

        /// <summary>
        /// The update method. Basically just updates 
        /// the view and projection matrix and last but not least
        /// our bounding frustum to check if the objects are within
        /// the view of our camera. 
        /// </summary>
        /// <param name="gameTime">The elapsed game time - <see cref="GameTime"/></param>
        public virtual void Update(GameTime gameTime)
        {
            if (Updateable)
            {
                if (Controller != null)
                    Controller.UpdateCamera(this, gameTime);

                Projection = Utils.CreateProjectionMatrix(FieldOfView,
                            AspectRatio,
                            NearPlane, FarPlane); // update the projection matrix

                View = Matrix.CreateLookAt(this.Position, this.Target, this.Up);

                Frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            }
        }
    }
}
