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

#if DEBUG
           // sphere = new Sphere(ID + "_sphere",scale);
            //this.Children.Add(sphere);
#endif
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

        public override BoundingBox GetBoundingBox()
        {
            Matrix[] modelTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            // Create variables to hold min and max xyz values for the model. Initialise them to extremes
            Vector3 modelMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 modelMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (ModelMesh mesh in Model.Meshes)
            {
                //Create variables to hold min and max xyz values for the mesh. Initialise them to extremes
                Vector3 meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                Vector3 meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                // There may be multiple parts in a mesh (different materials etc.) so loop through each
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    // The stride is how big, in bytes, one vertex is in the vertex buffer
                    // We have to use this as we do not know the make up of the vertex
                    int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                    byte[] vertexData = new byte[stride * part.NumVertices];
                    part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, 1); // fixed 13/4/11

                    // Find minimum and maximum xyz values for this mesh part
                    // We know the position will always be the first 3 float values of the vertex data
                    Vector3 vertPosition = new Vector3();
                    for (int ndx = 0; ndx < vertexData.Length; ndx += stride)
                    {
                        vertPosition.X = BitConverter.ToSingle(vertexData, ndx);
                        vertPosition.Y = BitConverter.ToSingle(vertexData, ndx + sizeof(float));
                        vertPosition.Z = BitConverter.ToSingle(vertexData, ndx + sizeof(float) * 2);

                        // update our running values from this vertex
                        meshMin = Vector3.Min(meshMin, vertPosition);
                        meshMax = Vector3.Max(meshMax, vertPosition);
                    }
                }
                

                // transform by mesh bone transforms
                meshMin = Vector3.Transform(meshMin, modelTransforms[mesh.ParentBone.Index]);
                meshMax = Vector3.Transform(meshMax, modelTransforms[mesh.ParentBone.Index]);

                // Expand model extents by the ones from this mesh
                modelMin = Vector3.Min(modelMin, meshMin);
                modelMax = Vector3.Max(modelMax, meshMax);
            }


            // Create and return the model bounding box
            return new BoundingBox(modelMin - Position *  Scale, modelMax + Position * Scale); 
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
#if DEBUG
           // sphere.LoadContent();
#endif
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

                       Vector3 up = Up;
                       up.Normalize();

                       Vector3 forward = Forward;
                       forward.Normalize();

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
