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
    using GameApplicationTools.Resources.Shader;

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
        public Material Material { get; set; }
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

            Material = new Material();
        }

        /// <summary>
        /// Caches the basic effect and creates a mesh tag so
        /// we can recreate it later on!
        /// </summary>
        private void generateTags()
        {
            foreach(ModelMesh mesh in Model.Meshes)
                foreach(ModelMeshPart part in mesh.MeshParts)
                    if (part.Effect is BasicEffect)
                    {
                        BasicEffect effect = (BasicEffect)part.Effect;
                        MeshTag tag = new MeshTag(effect.DiffuseColor, effect.Texture, 
                            effect.SpecularPower);
                        part.Tag = tag;
                    }
        }

        /// <summary>
        /// Will cache the current effects thus we can restore 
        /// those later on!
        /// </summary>
        public void CacheEffects()
        {
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                    ((MeshTag)part.Tag).CachedEffect = part.Effect;
        }

        /// <summary>
        /// Restores the cached effects again
        /// to the model mesh parts.
        /// </summary>
        public void RestoreCachedEffects()
        {
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                    part.Effect = ((MeshTag)part.Tag).CachedEffect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="CopyEffect"></param>
        public void SetModelEffect(Effect effect, bool CopyEffect)
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Effect toSet = effect;

                    // Copy the effect if necessary
                    if (CopyEffect)
                        toSet = effect.Clone();

                    MeshTag tag = ((MeshTag)part.Tag);

                    // if this ModelMeshPart has a texture, set it to the effect
                    if (tag.Texture != null)
                    {
                        setEffectParameter(toSet, "DiffuseTexture", tag.Texture);
                        setEffectParameter(toSet, "TextureEnabled", true);
                    }
                    else
                    {
                        setEffectParameter(toSet, "TextureEnabled", false);
                    }

                    setEffectParameter(toSet, "DiffuseColor", tag.Color);
                    setEffectParameter(toSet, "SpecularPower", tag.SpecularPower);

                    part.Effect = toSet;
                }
            }
        }

        /// <summary>
        /// Will set a parameter on the effect file!
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="paramName"></param>
        /// <param name="val"></param>
        private void setEffectParameter(Effect effect, string paramName, object val)
        {
            if (effect.Parameters[paramName] == null)
                return;

            if (val is Vector3)
                effect.Parameters[paramName].SetValue((Vector3)val);
            else if(val is bool)
                effect.Parameters[paramName].SetValue((bool)val);
            else if (val is Matrix)
                effect.Parameters[paramName].SetValue((Matrix)val);
            else if (val is Texture2D)
                effect.Parameters[paramName].SetValue((Texture2D)val);
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
                generateTags();
                CalculateBoundingSphere();
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
                           foreach (ModelMeshPart part in mesh.MeshParts)
                           {
                               Effect effect = part.Effect;

                               if (effect is BasicEffect)
                               {
                                   ((BasicEffect)effect).World = transforms[mesh.ParentBone.Index] * AbsoluteTransform;
                                   ((BasicEffect)effect).View = camera.View;
                                   ((BasicEffect)effect).Projection = camera.Projection;
                                   ((BasicEffect)effect).EnableDefaultLighting();
                               }
                               else
                               {
                                   setEffectParameter(effect, "World", transforms[mesh.ParentBone.Index] * AbsoluteTransform);
                                   setEffectParameter(effect, "View", camera.View);
                                   setEffectParameter(effect, "Projection", camera.Projection);
                                   setEffectParameter(effect, "CameraPosition", camera.Position);

                                   Material.SetEffectParameters(effect);
                               }

                           }

                           mesh.Draw();
                       }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
