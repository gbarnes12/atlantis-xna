namespace SpaceCommander.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GameApplicationTools.Resources;
    using GameApplicationTools;

    public static class DefaultScript
    {
        /// <summary>
        /// Gets called once the game is created
        /// and the resource manager is ready!
        /// </summary>
        /// <returns>float</returns>
        public static void LoadContents()
        {
            // load all the resources we have for now
            // this will be dynamically depending on the scene we load
            // later on!
            List<Resource> resources = new List<Resource>()
            {
                // AUDIOS!
                new Resource() {
                    Name = "Sleep Away",
                    Path = GameApplication.Instance.AudioPath,
                    Type = ResourceType.Song
                },
                // EFFECT FOLDER
                new Resource() {
                    Name = "DefaultEffect",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                new Resource() {
                    Name = "PlanetEarthEffect",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                new Resource() {
                    Name = "TextureMappingEffect",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                new Resource() {
                    Name = "BlurPostProcessor",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                new Resource() {
                    Name = "SSAO",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                 new Resource() {
                    Name = "PPModel",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                // FONTS
                new Resource() {
                    Name = "Arial",
                    Path = GameApplication.Instance.FontPath,
                    Type = ResourceType.SpriteFont
                },
                new Resource() {
                    Name = "ConsoleFont",
                    Path = GameApplication.Instance.FontPath,
                    Type = ResourceType.SpriteFont
                },
                // MODELS
                new Resource() {
                    Name = "asteroid",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "p1_wedge",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "spaceship",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "raumschiff7",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "planet",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "sphere",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "skysphere",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                 new Resource() {
                    Name = "laser",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                 new Resource() {
                    Name = "teapot",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                // TEXTURES
                new Resource() {
                    Name = "space",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "level1_skymap",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "crate",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "noise",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                // EARTH TEXTURES!
                new Resource() {
                    Name = "Earth_Atmos",
                    Path = GameApplication.Instance.TexturePath + "PlanetEarth\\",
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "Earth_Atmosx",
                    Path = GameApplication.Instance.TexturePath + "PlanetEarth\\",
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "Earth_Cloud",
                    Path = GameApplication.Instance.TexturePath + "PlanetEarth\\",
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "Earth_Diffuse",
                    Path = GameApplication.Instance.TexturePath + "PlanetEarth\\",
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "Earth_Night",
                    Path = GameApplication.Instance.TexturePath + "PlanetEarth\\",
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "Earth_NormalMap",
                    Path = GameApplication.Instance.TexturePath + "PlanetEarth\\",
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "Earth_ReflectionMask",
                    Path = GameApplication.Instance.TexturePath + "PlanetEarth\\",
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "WaterRipples",
                    Path = GameApplication.Instance.TexturePath + "PlanetEarth\\",
                    Type = ResourceType.Texture2D
                },
                // UI
                new Resource() {
                    Name = "startnewgame_button",
                    Path = GameApplication.Instance.UIPath + "Buttons\\",
                    Type = ResourceType.Texture2D
                },

                  new Resource() {
                    Name = "crosshair_far",
                    Path = GameApplication.Instance.UIPath + "CrossHair\\",
                    Type = ResourceType.Texture2D
                },

                 new Resource() {
                    Name = "crosshair_near",
                    Path = GameApplication.Instance.UIPath + "CrossHair\\",
                    Type = ResourceType.Texture2D
                },
            };

            ResourceManager.Instance.LoadResources(resources);
        }
    }
}
