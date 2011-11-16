namespace GameApplicationTools.Actors.Cameras
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Actors;
    using Interfaces;

    public class Camera : Actor, IUpdateableActor
    {
        #region [Virtual]
        public virtual Vector3 Position { get; set; }

        public virtual BoundingBox BoundingBox
        { get { return new BoundingBox(Position - Vector3.One, Position + Vector3.One); } }

        public virtual Matrix View { get; set; }

        public virtual Matrix Projection { get; set; }

        public virtual Vector3 Target { get; set; }

        public virtual Vector3 Up { get; set; }

        /// <summary>
        /// Determines whether the 
        /// actor gets updated or not
        /// </summary>
        public virtual bool IsUpdateable { get; set; }
        #endregion

        public Camera(String ID, Vector3 Position, Vector3 Target)
            : base(ID, null)
        {
            this.Position = Position;
            this.Target = Target;
            this.Up = Vector3.Up;
            IsUpdateable = true;
            // create the view matrix
            View = Matrix.CreateLookAt(this.Position, this.Target, 
                                        this.Up);

            // create projection matrix
            Projection = Utils.CreateProjectionMatrix();

        }

        public Camera(String ID, String GameViewID, Vector3 Position, Vector3 Target)
            : base(ID, GameViewID)
        {
            this.Position = Position;
            this.Target = Target;
            this.Up = Vector3.Up;
            IsUpdateable = true;
            // create the view matrix
            View = Matrix.CreateLookAt(this.Position, this.Target,
                                        this.Up);

            // create projection matrix
            Projection = Utils.CreateProjectionMatrix();

        }


        public virtual void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content){}

        public virtual void Update(GameTime gameTime) { }
    }
}
