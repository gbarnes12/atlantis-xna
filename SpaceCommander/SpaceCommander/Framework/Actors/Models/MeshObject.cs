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

    public class MeshObject : Actor
    {
        #region Private
        private String _fileName;
        private Model model;
        private BoundingSphere modelSphere;
        #endregion

        public MeshObject(String ID, String modelFile, float scale, Vector3 position)
            : base(ID, null)
        {
            
            _fileName = modelFile;
            this.Scale = new Vector3(scale, scale, scale);
        }
        public MeshObject(String ID, String modelFile, float scale, Vector3 position,float angle)
            : base(ID, null)
        {
            _fileName = modelFile;
            this.Scale = new Vector3(scale, scale, scale);

        }

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

        public override BoundingSphere GetBoundingSphere()
        {
            return modelSphere;
        }

        public override void LoadContent()
        {
            if (_fileName != "")
                model = ResourceManager.Instance.GetResource<Model>(_fileName);
        }


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
                               basicEffect.World = transforms[mesh.ParentBone.Index] *
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
