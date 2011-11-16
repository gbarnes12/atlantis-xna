namespace GameApplication.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Interfaces;

    /// <summary>
    /// This is a default effect with no effects
    /// applied to it. It just accepts three parameters:
    /// a World, View and Projection matrix. 
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    class DefaultEffect : Effect, IEffectMatrices, IResource
    {
        #region Private
        // the effect parameters we can
        // use within the shader files.
        EffectParameter world;
        EffectParameter projection;
        EffectParameter view;
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

        /// <summary>
        /// The view matrix
        /// </summary>
        public Matrix View
        {
            get { return view.GetValueMatrix(); }
            set { view.SetValue(value); }
        }

        /// <summary>
        /// The projection matrix
        /// </summary>
        public Matrix Projection
        {
            get { return projection.GetValueMatrix(); }
            set { projection.SetValue(value); }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the DefaultEffect
        /// 
        /// Example: new DefaultEffect(Content.Load<![CDATA[<Effect>]]>("filename"));
        /// </summary>
        /// <param name="effect">The effect file which we need to load through the content pipeline</param>
        public DefaultEffect(Effect effect)
            : base(effect)
        {
            // just assign standard values to the
            // corresponding parameters.
            world = Parameters["World"];
            projection = Parameters["Projection"];
            view = Parameters["View"];
        }

        /// <summary>
        /// Here you could perform necessary loading of some stuff 
        /// you need to load but currently it doesn't do anything.
        /// </summary>
        /// <param name="content">An instance of the ContentManager we can use</param>
        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content) {}
    }
}
