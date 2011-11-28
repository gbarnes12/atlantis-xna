namespace GameApplicationTools.Actors.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using Misc;
    using Actors.Cameras;
    using Interfaces;
    using GameApplicationTools.Actors.Primitives;

    /// <summary>
    /// Represents a basic model in our world
    /// can be used for furniture and other kind 
    /// of object which don't need any special treatment. 
    /// 
    /// @todo: Implement normal mapping on this object!
    /// 
    /// Author: Dominik Finkbeiner
    /// Version: 1.0
    /// </summary>
    public class MeshObject : Actor
    {
        #region Private
        private String _fileName;
        private Model model;
        private BoundingSphere modelSphere;
        private Sphere sphere;
        #endregion

        public MeshObject(String ID, String modelFile, float scale)
            : base(ID, null)
        {          
            _fileName = modelFile;
            this.Scale = new Vector3(scale, scale, scale);
            sphere = new Sphere(ID + "_sphere",scale);
            this.Children.Add(sphere);
        }

        public MeshObject(String ID, String modelFile, float scale, float angle)
            : base(ID, null)
        {
            _fileName = modelFile;
            this.Scale = new Vector3(scale, scale, scale);
            sphere = new Sphere(ID + "_sphere", scale);
            this.Children.Add(sphere);
        }

        /// <summary>
        /// Calculates the bounding sphere which we get from
        /// our model meshes!
        /// </summary>
        private void CalculateBoundingSphere()
        {
            //Calculate the bounding sphere for the entire model

            modelSphere = new BoundingSphere();

            foreach (ModelMesh mesh in model.Meshes)
            {
                modelSphere = Microsoft.Xna.Framework.BoundingSphere.CreateMerged(
                                    modelSphere,
                                    model.Meshes[0].BoundingSphere);
            }
        }

        /// <summary>
        /// Returns an active bounding sphere for this object!
        /// </summary>
        /// <returns>BoundingSphere which gets computed after every update</returns>
        public override BoundingSphere GetBoundingSphere()
        {
            return modelSphere;
        }

        /// <summary>
        /// The method which loads our necessary content from
        /// the resource manager.
        /// </summary>
        public override void LoadContent()
        {
            sphere.LoadContent();

            if (_fileName != "")
                model = ResourceManager.Instance.GetResource<Model>(_fileName);
        }


        /// <summary>
        /// Finally render the model to the screen with some basic effect
        /// </summary>
        /// <param name="sceneGraph">The scene graph responsible for this actor - <see cref="SceneGraphManager"/></param>
        public override void Render(SceneGraphManager sceneGraph)
        {
            if (CameraManager.Instance.GetCurrentCamera() != null)
            {
                if (model != null)
                {
                    Camera camera = CameraManager.Instance.GetCurrentCamera();
                    // Copy the model hierarchy transforms
                       Matrix[] transforms = new Matrix[model.Bones.Count];
                       model.CopyAbsoluteBoneTransformsTo(transforms);
 
                       // Render each mesh in the model
                       foreach (ModelMesh mesh in model.Meshes)
                       {
                           foreach (Effect effect in mesh.Effects)
                           {
                               BasicEffect basicEffect = effect as BasicEffect;
 
                               if (basicEffect == null)
                               {
                                       throw new NotSupportedException("there is no basic effect");
                               }
 
                               //Set the matrices
                               basicEffect.World = Matrix.CreateScale(Scale) * transforms[mesh.ParentBone.Index] *
                                                       AbsoluteTransform;
                               basicEffect.View = camera.View;
                               basicEffect.Projection = camera.Projection;
                               
                               basicEffect.EnableDefaultLighting();
                           }
 
                           mesh.Draw();
                       }
                    
                }
            }
        }

       
    }
}
