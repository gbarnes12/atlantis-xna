using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AwesomiumSharp;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;
using GameApplicationTools;

namespace TestEnvironment.UserInterface
{
    public class UIManagerEditor : DrawableGameComponent
    {
        public int thisWidth;
        public int thisHeight;

        protected Effect webEffect;

        public WebView webView;
        public Texture2D webRender;

        protected int[] webData;

        public bool TransparentBackground = false;

        protected SpriteBatch spriteBatch
        {
            get { return (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch)); }
        }

        public string URL;

        public UIManagerEditor(Game game, string baseUrl)
            : base(game)
        {
            URL = baseUrl;

            DrawOrder = int.MaxValue;
        }

        protected override void LoadContent()
        {
            WebCore.Config config = new WebCore.Config();
            config.enableJavascript = true;
            config.enablePlugins = true;
            WebCore.Initialize(config);

            thisWidth = GameApplication.Instance.GetGraphics().PresentationParameters.BackBufferWidth;
            thisHeight = GameApplication.Instance.GetGraphics().PresentationParameters.BackBufferHeight;

            webView = WebCore.CreateWebview(thisWidth, thisHeight);

            webRender = new Texture2D(GraphicsDevice, thisWidth, thisHeight, false, SurfaceFormat.Color);
            webData = new int[thisWidth * thisHeight];

            webEffect = Game.Content.Load<Effect>("Editor\\WebEffect");

            ReLoad();
        }

        public virtual void LoadFile(string file)
        {
            LoadURL(string.Format("file:///{0}\\{1}", Directory.GetCurrentDirectory(), file).Replace("\\", "/"));
        }

        public virtual void LoadURL(string url)
        {
            URL = url;
            webView.LoadURL(url);

            webView.SetTransparent(TransparentBackground);

            webView.Focus();
        }

        public virtual void ReLoad()
        {
            if (URL.Contains("http://") || URL.Contains("file:///"))
                LoadURL(URL);
            else
                LoadFile(URL);
        }

        public virtual void CreateObject(string name)
        {
            webView.CreateObject(name);
        }
        public virtual void CreateObject(string name, string method, WebView.JSCallback callback)
        {
            CreateObject(name);

            webView.SetObjectCallback(name, method, callback);
        }

        public virtual void PushData(string name, string method, params JSValue[] args)
        {
            webView.CallJavascriptFunction(name, method, args);
        }

        public void LeftButtonDown()
        {
            webView.InjectMouseDown(MouseButton.Left);
        }

        public void LeftButtonUp()
        {
            webView.InjectMouseUp(MouseButton.Left);
        }

        public void MouseMoved(int X, int Y)
        {
            webView.InjectMouseMove(X, Y);
        }

        public void ScrollWheel(int delta)
        {
            webView.InjectMouseWheel(delta);
        }

        public void KeyPressed(Keys key)
        {
            WebKeyboardEvent keyEvent = new WebKeyboardEvent();
            keyEvent.type = WebKeyType.Char;
            keyEvent.text = new ushort[] { (ushort)key, 0, 0, 0 };
            webView.InjectKeyboardEvent(keyEvent);
        }

        public override void Update(GameTime gameTime)
        {
            WebCore.Update();

            if (webView.IsDirty())
            {
                Marshal.Copy(webView.Render().GetBuffer(), webData, 0, webData.Length);
                webRender.SetData(webData);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (webRender != null)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                webEffect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(webRender, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();

                Game.GraphicsDevice.Textures[0] = null;
            }
        }
        protected void SaveTarget()
        {
            FileStream s = new FileStream("UI.jpg", FileMode.Create);
            webRender.SaveAsJpeg(s, webRender.Width, webRender.Height);
            s.Close();
        }
    }
}
