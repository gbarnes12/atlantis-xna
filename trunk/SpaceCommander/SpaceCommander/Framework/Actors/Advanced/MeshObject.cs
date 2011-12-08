namespace GameApplicationTools.Actors.Advanced
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
    using GameApplicationTools.Actors.Properties;

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
        #region Public
        public Model Model { get; set; }
        #endregion

        #region Private
        private String _modelFileName;
        private BoundingSphere modelSphere;
        private Sphere sphere;
        #endregion

        public MeshObject(String ID, String modelFile, float scale)
            : base(ID, null)
        {
            _modelFileName = modelFile;
            this.Scale = new Vector3(scale, scale, scale);
            sphere = new Sphere(ID + "_sphere",scale);
            this.Children.Add(sphere);
        }

        /// <summary>
        /// Calculates the bounding sphere which we get from
        /// our model meshes!
        /// </summary>
        private void CalculateBoundingSphere()
        {
            //Calculate the bounding sphere for the entire model

            //Calculate the bounding sphere for the entire model
            Matrix[] modelTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            modelSphere = new BoundingSphere(Vector3.Zero, 0);

            foreach (ModelMesh mesh in Model.Meshes)
            {
                BoundingSphere transformed = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index]);

                modelSphere = BoundingSphere.CreateMerged(modelSphere, transformed);
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

            if (_modelFileName != "")
                Model = ResourceManager.Instance.GetResource<Model>(_modelFileName);

            if (this.Properties.ContainsKey(ActorPropertyType.EFFECT))
            {
                Effect effect = ((EffectProperty)Properties[ActorPropertyType.EFFECT]).Effect;
                if (effect != null)
                {
                    foreach (ModelMesh mesh in Model.Meshes)
                        foreach (ModelMeshPart part in mesh.MeshParts)
                            part.Effect = effect;
                }
            }

            if(Model != null)
                CalculateBoundingSphere();
        }


        /// <summary>
        /// Finally render the model to the screen with some basic effect
        /// </summary>
        /// <param name="sceneGraph">The scene graph responsible for this actor - <see cref="SceneGraphManager"/></param>
        public override void Render(SceneGraphManager sceneGraph)
        {
            if (CameraManager.Instance.GetCurrentCamera() != null)
            {
                if (Model != null)
                {
                    Camera camera = CameraManager.Instance.GetCurrentCamera();
                    // Copy the model hierarchy transforms
                       Matrix[] transforms = new Matrix[Model.Bones.Count];
                       Model.CopyAbsoluteBoneTransformsTo(transforms);
 
                       // Render each mesh in the model
                       foreach (ModelMesh mesh in Model.Meshes)
                       {
                           foreach (Effect effect in mesh.Effects)
                           {
                               if (!this.Properties.ContainsKey(ActorPropertyType.EFFECT))
                               {
                                   BasicEffect basicEffect = effect as BasicEffect;

                                   //Set the matrices
                                   basicEffect.World = transforms[mesh.ParentBone.Index] *
                                                           AbsoluteTransform;
                                   basicEffect.View = camera.View;
                                   basicEffect.Projection = camera.Projection;

                                   basicEffect.EnableDefaultLighting();
                               }
                               else
                               {
                                   effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] *
                                                           AbsoluteTransform);
                                   effect.Parameters["View"].SetValue(camera.View);
                                   effect.Parameters["Projection"].SetValue(camera.Projection);
                               }
                           }
 
                           mesh.Draw();
                       }
                    
                }
            }
        }

       
    }
}
