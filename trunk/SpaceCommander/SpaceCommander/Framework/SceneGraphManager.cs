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

    public class SceneGraphManager
    {
        // Fields
        #region Private
        Actor rootNode;
        GraphicsDevice device;
        GameTime gameTime;
        int nodesCulled;
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
        }

        public void SetGameTime(GameTime gameTime)
        {
            this.gameTime = gameTime;
        }

        // Methods
        void CalculateTransformsRecursive(Actor node)
        {
            node.AbsoluteTransform = Matrix.CreateScale(node.Scale)* Matrix.CreateTranslation(node.Offset) *
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

            if(CameraManager.Instance.CurrentCamera != null)
                CameraManager.Instance.GetCurrentCamera().Update(time);

            UpdateRecursive(rootNode);
            CalculateTransformsRecursive(rootNode);
        }

        public void Render()
        {
            nodesCulled = 0;
            rootNode.AbsoluteTransform = Matrix.Identity;

            DrawRecursive(rootNode);
        }
    }
}
