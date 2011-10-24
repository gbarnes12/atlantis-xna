using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xen.Ex.Graphics.Content;

namespace Atlantis
{
    public class Actor : IDraw, IContentOwner
    {
        private ModelInstance model;

        private Matrix worldMatrix;
        private Matrix scaleMatrix;
        private Matrix rotationMatrix;

        private String filename;

        private Vector3 position;
        private float scale;

       
        public Vector3 Position
        {
            get
            {
                return this.position;
            }
        }

        public float Scale
        {
            get
            {
                return this.scale;
            }
        }

      /// <summary>
      /// create a new actor
      /// </summary>
      /// <param name="content"></param>
      /// <param name="modelfile">model file to be loaded</param>
      /// <param name="position">position</param>
      /// <param name="scale_factor">scale</param>
        public Actor(ContentRegister content,String modelfile, Vector3 position, float scale_factor)
        {
            this.scale = scale_factor;
            this.position = position;

            Matrix.CreateTranslation(ref position, out this.worldMatrix);
            Matrix.CreateScale(scale_factor,out this.scaleMatrix);

            this.filename = modelfile;

            this.model = new ModelInstance();

            content.Add(this);
        }

        public void LoadContent(ContentState state)
        {
            this.model.ModelData = state.Load<ModelData>(filename);
        }

        public void Draw(DrawState state)
        {
            using (state.WorldMatrix.Push(ref worldMatrix))
            {
                using(state.WorldMatrix.Push(ref scaleMatrix))
                {
                    if (model.CullTest(state))
                    {
                        model.Draw(state);
                    }
                }
            }
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }
    }
}
