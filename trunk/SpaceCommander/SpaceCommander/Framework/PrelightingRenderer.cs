using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using GameApplicationTools.Actors.Advanced;
using GameApplicationTools.Actors.Cameras;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GameApplicationTools.Actors;

namespace GameApplicationTools
{
    public class PrelightingRenderer
    {
        // Normal, depth, and light map render targets
        RenderTarget2D depthTarg;
        RenderTarget2D normalTarg;
        RenderTarget2D lightTarg;

        // Depth/normal effect and light mapping effect
        Effect depthNormalEffect;
        Effect lightingEffect;

        // Point light (sphere) mesh
        Model lightMesh;

        // List of models, lights, and the camera, currently only support for MeshObjects !
        public List<MeshObject> Models { get; set; }
        public List<PointLight> Lights { get; set; }
      //  public Camera Camera { get; set; }

        GraphicsDevice graphicsDevice;
        int viewWidth = 0, viewHeight = 0;

        SceneGraphManager sceneGraph;

        public PrelightingRenderer(GraphicsDevice GraphicsDevice, SceneGraphManager sceneGraph)
        {
            //get sceneGraph
            this.sceneGraph = sceneGraph;

            viewWidth = GraphicsDevice.Viewport.Width;
            viewHeight = GraphicsDevice.Viewport.Height;

            // Create the three render targets
            depthTarg = new RenderTarget2D(GraphicsDevice, viewWidth,
                viewHeight, false, SurfaceFormat.Single, DepthFormat.Depth24);

            normalTarg = new RenderTarget2D(GraphicsDevice, viewWidth,
                viewHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);

            lightTarg = new RenderTarget2D(GraphicsDevice, viewWidth,
                viewHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);

            this.graphicsDevice = GraphicsDevice;

            Lights = new List<PointLight>();
        }

        public void LoadContent()
        {
            // Load effects
            depthNormalEffect = ResourceManager.Instance.GetResource<Effect>("PPDepthNormal");
            lightingEffect = ResourceManager.Instance.GetResource<Effect>("PPLight");

            // Set effect parameters to light mapping effect
            lightingEffect.Parameters["viewportWidth"].SetValue(viewWidth);
            lightingEffect.Parameters["viewportHeight"].SetValue(viewHeight);

            // Load point light mesh and set light mapping effect to it
            lightMesh = ResourceManager.Instance.GetResource<Model>("PPLightMesh");
            lightMesh.Meshes[0].MeshParts[0].Effect = lightingEffect;
        }

        public void Draw(Actor node)
        {
            drawDepthNormalMap(node);
            drawLightMap();
            prepareMainPass(node);
        }

        void drawDepthNormalMap(Actor rootNode)
        {
            // Set the render targets to 'slots' 1 and 2
            graphicsDevice.SetRenderTargets(normalTarg, depthTarg);

            // Clear the render target to 1 (infinite depth)
            graphicsDevice.Clear(Color.White);

            //render recursive
            drawDepthNormalMapRecursive(rootNode);

            // Un-set the render targets
            graphicsDevice.SetRenderTargets(null);
        }

        void drawDepthNormalMapRecursive(Actor actor)
        {
            if (actor.GetType() == typeof(MeshObject))
            {
                ((MeshObject)actor).CacheEffects();
                ((MeshObject)actor).SetModelEffect(depthNormalEffect, false);

                ((MeshObject)actor).Render(this.sceneGraph);
                ((MeshObject)actor).RestoreCachedEffects();
            }

            foreach (Actor node in actor.Children)
                drawDepthNormalMapRecursive(node);
        }

        void drawLightMap()
        {
            // Set the depth and normal map info to the effect
            lightingEffect.Parameters["DepthTexture"].SetValue(depthTarg);
            lightingEffect.Parameters["NormalTexture"].SetValue(normalTarg);

            // Calculate the view * projection matrix
            Matrix viewProjection = CameraManager.Instance.GetCurrentCamera().View * CameraManager.Instance.GetCurrentCamera().Projection;

            // Set the inverse of the view * projection matrix to the effect
            Matrix invViewProjection = Matrix.Invert(viewProjection);
            lightingEffect.Parameters["InvViewProjection"].SetValue(
                invViewProjection);

            // Set the render target to the graphics device
            graphicsDevice.SetRenderTarget(lightTarg);

            // Clear the render target to black (no light)
            graphicsDevice.Clear(Color.Black);

            // Set render states to additive (lights will add their influences)
            graphicsDevice.BlendState = BlendState.Additive;
            graphicsDevice.DepthStencilState = DepthStencilState.None;

            if (Lights.Count > 0)
            {
                foreach (PointLight light in Lights)
                {
                    // Set the light's parameters to the effect
                    light.SetEffectParameters(lightingEffect);

                    // Calculate the world * view * projection matrix and set it to 
                    // the effect
                    Matrix wvp = (Matrix.CreateScale(light.Attenuation)
                        * Matrix.CreateTranslation(light.Position)) * viewProjection;

                    lightingEffect.Parameters["WorldViewProjection"].SetValue(wvp);

                    // Determine the distance between the light and camera
                    float dist = Vector3.Distance(CameraManager.Instance.GetCurrentCamera().Position,
                        light.Position);

                    // If the camera is inside the light-sphere, invert the cull mode
                    // to draw the inside of the sphere instead of the outside
                    if (dist < light.Attenuation)
                        graphicsDevice.RasterizerState =
                            RasterizerState.CullClockwise;

                    // Draw the point-light-sphere
                    lightMesh.Meshes[0].Draw();

                    // Revert the cull mode
                    graphicsDevice.RasterizerState =
                        RasterizerState.CullCounterClockwise;
                }
            }

            // Revert the blending and depth render states
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Un-set the render target
            graphicsDevice.SetRenderTarget(null);
        }

        void prepareMainPass(Actor rootNode)
        {
            prepareMainPassRecursive(rootNode);
        }

        void prepareMainPassRecursive(Actor node)
        {
            if (node.GetType() == typeof(MeshObject))
            {
                foreach (ModelMesh mesh in ((MeshObject)node).Model.Meshes)
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        // Set the light map and viewport parameters to each model's effect
                        if (part.Effect.Parameters["LightTexture"] != null)
                            part.Effect.Parameters["LightTexture"].SetValue(lightTarg);

                        if (part.Effect.Parameters["viewportWidth"] != null)
                            part.Effect.Parameters["viewportWidth"].SetValue(viewWidth);

                        if (part.Effect.Parameters["viewportHeight"] != null)
                            part.Effect.Parameters["viewportHeight"].SetValue(viewHeight);
                    }
            }

            foreach (Actor child_node in node.Children)
                this.prepareMainPassRecursive(child_node);
        }
    }
}
