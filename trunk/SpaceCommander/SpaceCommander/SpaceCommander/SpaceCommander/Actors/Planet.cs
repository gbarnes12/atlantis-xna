namespace SpaceCommander.Actors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    using GameApplicationTools.Resources;
    using GameApplicationTools.Actors.Cameras;
    using GameApplicationTools.Actors;
    using GameApplicationTools.Interfaces;
    using GameApplicationTools;
    using GameApplicationTools.Misc;

    using Resources.Shader;
    using GameApplicationTools.Actors.Primitives;

    /// <summary>
    /// 
    /// </summary>
    public class Planet : Actor
    {
        #region Public
        /// <summary>
        /// 
        /// </summary>
        public Vector3 LightPosition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Color AmbientColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float AmbientIntensity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float CloudSpeed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float CloudHeight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float CloudShadowIntensity { get; set; }
        #endregion

        #region Private
        /// <summary>
        /// 
        /// </summary>
        private Model Model { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private PlanetEffect Effect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Texture2D ColorMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Texture2D BumpMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Texture2D GlowMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Texture2D ReflectionMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Texture2D CloudMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Texture2D WaterMap { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Texture2D AtmosphereMap { get; set; }

        Sphere sphere;

        #endregion

        public Planet(String ID, float Scale)
            : base(ID, null)
        {
            AmbientColor = Color.Black;
            AmbientIntensity = .02f;
            CloudSpeed = .00025f;
            CloudHeight = .0001f;
            CloudShadowIntensity = .2f;

            Rotation = new Quaternion(0, 0, 0, 1);
            this.Scale = new Vector3(Scale, Scale, Scale);
            this.LightPosition = new Vector3(60, 60, 60);

            sphere = new Sphere(ID + "_sphere", Scale);
            this.Children.Add(sphere);
        }

        public Planet(String ID, String GameViewID, float Scale)
            : base(ID, GameViewID)
        {
            AmbientColor = Color.Black;
            AmbientIntensity = .02f;
            CloudSpeed = .00025f;
            CloudHeight = .0001f;
            CloudShadowIntensity = .2f;

            Rotation = new Quaternion(0, 0, 0, 1);
            this.Scale = new Vector3(Scale, Scale, Scale);
            this.LightPosition = new Vector3(60, 60, 60);

            sphere = new Sphere(ID + "_sphere", Scale);
            this.Children.Add(sphere);
        }

        /// <summary>
        /// The body of a load content method which
        /// allows us to load some basic stuff in here.
        /// </summary>
        /// <param name="content"></param>
        public override void LoadContent()
        {

            sphere.LoadContent();
            // load our model business in here
            Model = ResourceManager.Instance.GetResource<Model>("planet");
            Effect = new PlanetEffect(ResourceManager.Instance.GetResource<Effect>("PlanetEarthEffect"));

            // load the corresponding textures
            // that are needed to create a planet!
            ColorMap = ResourceManager.Instance.GetResource<Texture2D>("Earth_Diffuse");
            BumpMap = ResourceManager.Instance.GetResource<Texture2D>("Earth_NormalMap");
            GlowMap = ResourceManager.Instance.GetResource<Texture2D>("Earth_Night");
            ReflectionMap = ResourceManager.Instance.GetResource<Texture2D>("Earth_ReflectionMask");
            CloudMap = ResourceManager.Instance.GetResource<Texture2D>("Earth_Cloud");
            WaterMap = ResourceManager.Instance.GetResource<Texture2D>("WaterRipples");
            AtmosphereMap = ResourceManager.Instance.GetResource<Texture2D>("Earth_Atmosx");
        }

        public override void PreRender()
        {
            GameApplication.Instance.GetGraphics().SamplerStates[0] = new SamplerState()
            {
                Filter = TextureFilter.Linear,

                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap
            };

            base.PreRender();
        }

        /// <summary>
        /// The render method. Renders the 
        /// vertices with the help of a vertex and index buffer
        /// onto the screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Render(SceneGraphManager sceneGraph)
        {
            Camera camera = CameraManager.Instance.GetCurrentCamera();

           // Matrix world =  Utils.CreateWorldMatrix(Position, Matrix.CreateFromQuaternion(Rotation), Scale);
           

            Effect.World = AbsoluteTransform;
            Effect.WVP = AbsoluteTransform * camera.View * camera.Projection;

            Effect.Time = (float)sceneGraph.GameTime.TotalGameTime.TotalSeconds * 3;

            Effect.LightDirection = LightPosition - Position;

            Effect.AmbientColor = AmbientColor.ToVector4();
            Effect.AmbientIntensity= AmbientIntensity;
            Effect.ColorMap = ColorMap;
            Effect.BumpMap = BumpMap;
            Effect.GlowMap = GlowMap;
            Effect.ReflectionMap = ReflectionMap;
            Effect.CloudMap = CloudMap;
            Effect.WaveMap = WaterMap;
            Effect.AtmosMap = AtmosphereMap;
            Effect.CloudSpeed = CloudSpeed;
            Effect.CloudHeight = CloudHeight;
            Effect.CloudShadowIntensity = CloudShadowIntensity;

            Effect.CameraPosition = camera.Position;

       

            for (int pass = 0; pass < Effect.CurrentTechnique.Passes.Count; pass++)
            {
                for (int msh = 0; msh < Model.Meshes.Count; msh++)
                {
                    ModelMesh mesh = Model.Meshes[msh];
                    for (int prt = 0; prt < mesh.MeshParts.Count; prt++)
                        mesh.MeshParts[prt].Effect = Effect;
                    mesh.Draw();
                }
            }

            GameApplication.Instance.GetGraphics().BlendState = new Microsoft.Xna.Framework.Graphics.BlendState()
            {
                AlphaSourceBlend = Blend.SourceAlpha
            };

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            GameApplication.Instance.GetGraphics().RasterizerState = rs;

            base.Render(sceneGraph);
        }
    }
}
