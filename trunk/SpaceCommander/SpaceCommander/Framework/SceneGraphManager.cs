namespace GameApplicationTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Actors;
    using Actors.Cameras;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using GameApplicationTools.Actors.Advanced;

    public class SceneGraphManager
    {
        // Fields
        #region Private
        Actor rootNode;
        GraphicsDevice device;
        GameTime gameTime;
        int nodesCulled;

        PrelightingRenderer lightRenderer;
        #endregion
        // Properties

        #region Public
        public Actor RootNode
        {
            get { return rootNode; }
            set { rootNode = value; }
        }

        public GameTime GameTime
        {
            get { return gameTime; }
        }


        public int NodesCulled
        {
            get { return nodesCulled; }
        }

        public bool CullingActive
        {
            get;
            set;
        }
        #endregion

        // Constructor
        public SceneGraphManager()
        {
            rootNode = new Actor("rootNode", null);
            CullingActive = true;

            lightRenderer = new PrelightingRenderer(GameApplication.Instance.GetGame().GraphicsDevice, GameApplication.Instance.GetGame().Content, this);

            lightRenderer.Lights = new List<PointLight>() 
            {
                new PointLight("pointLight01",null,new Vector3(-1000, 1000, 0), Color.Red * .85f, 2000),
                new PointLight("pointLight09",null,new Vector3(0, 0, 0), Color.Red * .85f, 2000),
                new PointLight("pointLight02",null,new Vector3(1000, 1000, 0), Color.Orange * .85f, 2000),
                new PointLight("pointLight03",null,new Vector3(0, 1000, 1000), Color.Yellow * .85f, 2000),
                new PointLight("pointLight04",null,new Vector3(0, 1000, -1000), Color.Green * .85f, 2000),
                new PointLight("pointLight05",null,new Vector3(1000, 1000, 1000), Color.Blue * .85f, 2000),
                new PointLight("pointLight06",null,new Vector3(-1000, 1000, 1000), Color.Indigo * .85f, 2000),
                new PointLight("pointLight07",null,new Vector3(1000, 1000, -1000), Color.Violet * .85f, 2000),
                new PointLight("pointLight08",null,new Vector3(-1000, 1000, -1000), Color.White * .85f, 2000)
            };
        }

        public void SetGameTime(GameTime gameTime)
        {
            this.gameTime = gameTime;
        }

        // Methods
        void CalculateTransformsRecursive(Actor node)
        {
            node.AbsoluteTransform = Matrix.CreateScale(node.Scale) * Matrix.CreateTranslation(node.Offset) *
                Matrix.CreateFromQuaternion(node.Rotation) *
                node.AbsoluteTransform *
                Matrix.CreateTranslation(node.Position);

            //Update children recursively
            foreach (Actor childNode in node.Children)
            {
                childNode.AbsoluteTransform = node.AbsoluteTransform;
                CalculateTransformsRecursive(childNode);
            }
        }

        void UpdateRecursive(Actor node)
        {
            if (node.Updateable)
            {   //Update node
                node.Update(this);
            }

            //Update children recursively
            foreach (Actor childNode in node.Children)
            {
                UpdateRecursive(childNode);
            }
        }

        void DrawRecursive(Actor node)
        {
            //Draw
            if (node.Visible)
            {
                if (CameraManager.Instance.CurrentCamera != null)
                {
                    BoundingSphere transformedSphere = new BoundingSphere();

                    transformedSphere.Center = Vector3.Transform(node.BoundingSphere.Center,
                                                                 node.AbsoluteTransform);
                    transformedSphere.Radius = node.BoundingSphere.Radius;


                    if (CullingActive)
                    {
                        if (CameraManager.Instance.GetCurrentCamera().Frustum.Intersects(transformedSphere))
                        {
                            node.PreRender();
                            node.Render(this);
                            node.PostRender();
                        }
                        else
                        {
                            nodesCulled++;
                        }
                    }
                    else
                    {
                        node.PreRender();
                        node.Render(this);
                        node.PostRender();
                    }
                }
            }

            foreach (Actor childNode in node.Children)
            {
                DrawRecursive(childNode);
            }
        }

        void CalculateTransforms()
        {
            CalculateTransformsRecursive(rootNode);
        }

        public void Update(GameTime time)
        {
            gameTime = time;

            if (CameraManager.Instance.CurrentCamera != null)
                CameraManager.Instance.GetCurrentCamera().Update(time);

            UpdateRecursive(rootNode);
            CalculateTransformsRecursive(rootNode);
        }

        public void Render()
        {
            nodesCulled = 0;
            rootNode.AbsoluteTransform = Matrix.Identity;

            applyLighting(rootNode);

            GameApplication.Instance.GetGame().GraphicsDevice.Clear(Color.Black);

            DrawRecursive(rootNode);
        }

        private void applyLighting(Actor node)
        {
            lightRenderer.Draw(node);
          
        }
    }
}
