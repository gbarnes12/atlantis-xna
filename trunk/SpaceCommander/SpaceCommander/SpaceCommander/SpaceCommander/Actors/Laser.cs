using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApplicationTools.Actors;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameApplicationTools.Actors.Primitives;
using GameApplicationTools;
using GameApplicationTools.Actors.Cameras;

namespace SpaceCommander.Actors
{
    class Laser : Actor
    {
        #region Private
        private String _fileName;
        private Model model;
        private BoundingSphere modelSphere;
        private Sphere sphere;
        private bool fired = false;

        private float maxRange = 2000; // max range of 2km

        #endregion

        public Laser(String ID,String modelFile,float scale)
            :base(ID,null)
        {
            this._fileName = modelFile;
            this.Scale = new Vector3(scale);

            sphere = new Sphere(ID + "_sphere", 0.1f*scale);
            this.Children.Add(sphere);

            Updateable = true;
        }

        public override void LoadContent()
        {
            sphere.LoadContent();

            if (_fileName != "")
                model = ResourceManager.Instance.GetResource<Model>(_fileName);

            if (model != null)
                CalculateBoundingSphere();
        }

        public void fire(Vector3 shipPosition)
        {
           this.Position = shipPosition;
            fired = true;
        }

        public override void Update(SceneGraphManager sceneGraph)
        {
            if(fired)
                this.Position -= 150* Vector3.UnitZ;

            base.Update(sceneGraph);
        }

        public BoundingSphere getBoundingSphere()
        {
            return this.modelSphere;
        }

        private void CalculateBoundingSphere()
        {
            //Calculate the bounding sphere for the entire model
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            modelSphere = new BoundingSphere(Vector3.Zero, 0);

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere transformed = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index]);

                modelSphere = BoundingSphere.CreateMerged(modelSphere, transformed);
            }
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
                    GameApplication.Instance.GetGraphics().BlendState = BlendState.AlphaBlend;

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
                            basicEffect.Alpha = 0.5f;
                            basicEffect.DiffuseColor = Color.Blue.ToVector3();
                            basicEffect.EmissiveColor = Color.Blue.ToVector3();

                            basicEffect.EnableDefaultLighting();
                        }

                        mesh.Draw();
                    }
                }
            }
        }
    }
}
