namespace SpaceCommander.Resources.Shader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using GameApplicationTools.Interfaces;

    /// <summary>
    /// 
    /// </summary>
    public class PlanetEffect : Effect, IEffectMatrices, IResource
    {
        #region Private
        // the effect parameters we can
        // use within the shader files.
        EffectParameter world;
        EffectParameter view;
        EffectParameter projection;
        EffectParameter time;
        EffectParameter lightDirection;
        EffectParameter ambientColor;
        EffectParameter ambientIntensity;
        EffectParameter colorMap;
        EffectParameter bumpMap;
        EffectParameter glowMap;
        EffectParameter reflectionMap;
        EffectParameter cloudMap;
        EffectParameter waveMap;
        EffectParameter atmosMap;
        EffectParameter cloudSpeed;
        EffectParameter cloudHeight;
        EffectParameter cloudShadowIntensity;
        EffectParameter cameraPosition;
        #endregion

        #region Pulbic
        /// <summary>
        /// If you want to assign the World matrix use this
        /// </summary>
        public Matrix World
        {
            get { return world.GetValueMatrix(); }
            set { world.SetValue(value); }
        }

        public Matrix Projection
        {
            get { return projection.GetValueMatrix(); }
            set { projection.SetValue(value); }
        }

        public Matrix View
        {
            get { return view.GetValueMatrix(); }
            set { view.SetValue(value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public float Time
        {
            get { return time.GetValueSingle(); }
            set { time.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 LightDirection
        {
            get { return lightDirection.GetValueVector3(); }
            set { lightDirection.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector4 AmbientColor
        {
            get { return ambientColor.GetValueVector4(); }
            set { ambientColor.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float AmbientIntensity
        {
            get { return ambientIntensity.GetValueSingle(); }
            set { ambientIntensity.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Texture2D ColorMap
        {
            get { return colorMap.GetValueTexture2D(); }
            set { colorMap.SetValue(value); }
        }

        /// <summary>
        /// The texture you want to use. 
        /// </summary>
        public Texture2D BumpMap
        {
            get { return bumpMap.GetValueTexture2D(); }
            set { bumpMap.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Texture2D GlowMap
        {
            get { return glowMap.GetValueTexture2D(); }
            set { glowMap.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Texture2D ReflectionMap
        {
            get { return reflectionMap.GetValueTexture2D(); }
            set { reflectionMap.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Texture2D CloudMap
        {
            get { return cloudMap.GetValueTexture2D(); }
            set { cloudMap.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Texture2D WaveMap
        {
            get { return waveMap.GetValueTexture2D(); }
            set { waveMap.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Texture2D AtmosMap
        {
            get { return atmosMap.GetValueTexture2D(); }
            set { atmosMap.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float CloudSpeed
        {
            get { return cloudSpeed.GetValueSingle(); }
            set { cloudSpeed.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float CloudHeight
        {
            get { return cloudHeight.GetValueSingle(); }
            set { cloudHeight.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float CloudShadowIntensity
        {
            get { return cloudShadowIntensity.GetValueSingle(); }
            set { cloudShadowIntensity.SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 CameraPosition
        {
            get { return cameraPosition.GetValueVector3(); }
            set { cameraPosition.SetValue(value); }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the TextureMappingEffect
        /// 
        /// Example: new TextureMappingEffect(Content.Load<![CDATA[<Effect>]]>("filename"));
        /// </summary>
        /// <param name="effect">The effect file which we need to load through the content pipeline</param>
        public PlanetEffect(Effect effect)
            : base(effect)
        {
            // just assign standard values to the
            // corresponding parameters.
            world = Parameters["World"];
            view = Parameters["View"];
            projection = Parameters["Projection"];
            time = Parameters["time"];
            lightDirection = Parameters["LightDirection"];
            ambientColor = Parameters["AmbientColor"];
            ambientIntensity = Parameters["AmbientIntensity"];
            colorMap = Parameters["ColorMap"];
            bumpMap = Parameters["BumpMap"];
            glowMap = Parameters["GlowMap"];
            reflectionMap = Parameters["ReflectionMap"];
            cloudMap = Parameters["CloudMap"];
            waveMap = Parameters["WaveMap"];
            atmosMap = Parameters["AtmosMap"];
            cloudSpeed = Parameters["cloudSpeed"];
            cloudHeight = Parameters["cloudHeight"];
            cloudShadowIntensity = Parameters["cloudShadowIntensity"];
            cameraPosition = Parameters["CameraPosition"];
        }

        /// <summary>
        /// Here you could perform necessary loading of some stuff 
        /// you need to load but currently it doesn't do anything.
        /// </summary>
        /// <param name="content">An instance of the ContentManager we can use</param>
        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content) {}

        
    }
}
