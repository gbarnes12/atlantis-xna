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
    using GameApplicationTools.Actors.Properties.EffectPropertyControllers;

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

            // create properties
            PickableProperty pickableProperty = new PickableProperty();
            Properties.Add(ActorPropertyType.PICKABLE, pickableProperty);

            EffectProperty effProp = new EffectProperty("TextureMappingEffect");
            effProp.Effect = ResourceManager.Instance.GetResource<Effect>("TextureMappingEffect");
            effProp.Controller = new TextureMappingPropertyController();
            Properties.Add(ActorPropertyType.EFFECT, effProp);
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

            if (Model != null)
            {
                #region TextureMappingCustomEffect
                

                if (this.Properties.ContainsKey(ActorPropertyType.EFFECT))
                {
                    Effect effect = ((EffectProperty)Properties[ActorPropertyType.EFFECT]).Effect;
                    if (effect != null)
                    {
                        foreach (ModelMesh mesh in Model.Meshes)
                        {
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                if (((EffectProperty)Properties[ActorPropertyType.EFFECT]).Name == "TextureMappingEffect")
                                    effect.Parameters["DiffuseTexture"].SetValue(((BasicEffect)part.Effect).Texture);


                                part.Effect = effect.Clone();
                            }
                        }
                    }
                }
                #endregion

                CalculateBoundingSphere();
            }
        }

        public override void PreRender()
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GameApplication.Instance.GetGraphics().RasterizerState = rs;

            base.PreRender();
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
                                   ((EffectProperty)Properties[ActorPropertyType.EFFECT]).Update(effect, 
                                                                                    transforms[mesh.ParentBone.Index] * AbsoluteTransform);
                                   
                               }
                           }
 
                           mesh.Draw();
                       }
                }
            }
        }

        public override void PostRender()
        {
            RasterizerState rs = new RasterizerState();
            rs = null;
            rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            GameApplication.Instance.GetGraphics().RasterizerState = rs;


            base.PostRender();
        }

       
    }
}
