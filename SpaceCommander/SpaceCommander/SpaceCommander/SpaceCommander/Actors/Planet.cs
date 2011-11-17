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
    using Resources.Shader;
    using GameApplicationTools;

    /// <summary>
    /// 
    /// </summary>
    public class Planet : Actor, IDrawableActor
    {
        #region Public
        /// <summary>
        /// The Position of this Actor 
        /// in the World. 
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Sets the current angle of this 
        /// Actor.
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Sets the scale of our model
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// The world matrix of the inheriting actor
        /// that we need to set the current scale, position
        /// and rotation.
        /// </summary>
        public Matrix WorldMatrix { get; set; }

        /// <summary>
        /// Determines whether the 
        /// actor gets drawn or not
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Determines whether the 
        /// actor gets updated or not
        /// </summary>
        public bool IsUpdateable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Quaternion Rotation { get; set; }

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
        #endregion

        public Planet(String ID, Vector3 Position, float Scale)
            : base(ID, null)
        {
            AmbientColor = Color.Black;
            AmbientIntensity = .02f;
            CloudSpeed = .0000025f;
            CloudHeight = .0001f;
            CloudShadowIntensity = .2f;

            Rotation = new Quaternion(0, 0, 0, 1);
            this.Position = Position;
            this.Scale = Scale;
            this.Angle = 0f;
            this.LightPosition = new Vector3(60, 60, 60);
            this.IsVisible = true;
        }

        public Planet(String ID, String GameViewID ,Vector3 Position, float Scale)
            : base(ID, GameViewID)
        {
            AmbientColor = Color.Black;
            AmbientIntensity = .02f;
            CloudSpeed = .0025f;
            CloudHeight = .0001f;
            CloudShadowIntensity = .2f;

            Rotation = new Quaternion(0, 0, 0, 1);
            this.Position = Position;
            this.Scale = Scale;
            this.Angle = 0f;
            this.LightPosition = new Vector3(60, 60, 60);
            this.IsVisible = true;
        }

        /// <summary>
        /// The body of a load content method which
        /// allows us to load some basic stuff in here.
        /// </summary>
        /// <param name="content"></param>
        public virtual void LoadContent(ContentManager content)
        {
            // load our model business in here
            Model = content.Load<Model>("Models\\planet");
            Effect = new PlanetEffect(content.Load<Effect>("Effects\\PlanetEarthEffect"));

            // load the corresponding textures
            // that are needed to create a planet!
            ColorMap = content.Load<Texture2D>("Textures\\PlanetEarth\\Earth_Diffuse");
            BumpMap = content.Load<Texture2D>("Textures\\PlanetEarth\\Earth_NormalMap");
            GlowMap = content.Load<Texture2D>("Textures\\PlanetEarth\\Earth_Night");
            ReflectionMap = content.Load<Texture2D>("Textures\\PlanetEarth\\Earth_ReflectionMask");
            CloudMap = content.Load<Texture2D>("Textures\\PlanetEarth\\Earth_Cloud");
            WaterMap = content.Load<Texture2D>("Textures\\PlanetEarth\\WaterRipples");
        }

        /// <summary>
        /// The Update method. This will
        /// take care of updating our world matrix
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            
        }

        /// <summary>
        /// The render method. Renders the 
        /// vertices with the help of a vertex and index buffer
        /// onto the screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Render(GameTime gameTime)
        {
            Camera camera = WorldManager.Instance.GetActor("camera") as Camera;

            WorldMatrix = Utils.CreateWorldMatrix(Position, Matrix.CreateFromQuaternion(Rotation), new Vector3(Scale));
            Matrix wvp = WorldMatrix * camera.View * camera.Projection;

            Effect.World= WorldMatrix;
            Effect.WVP = wvp;

            Effect.Time = (float)gameTime.TotalGameTime.TotalSeconds * 3;

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
        }
    }
}
