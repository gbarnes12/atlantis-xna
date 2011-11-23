#region File Description
//-----------------------------------------------------------------------------
// MainWindow.xaml.cs
//
// Copyright 2011, Nick Gravelyn.
// Licensed under the terms of the Ms-PL: http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Xna.Framework;

using GameApplicationTools.Actors.Primitives;
using GameApplicationTools;
using GameApplicationTools.Actors.Cameras;
using Microsoft.Xna.Framework.Content;
using AridiaEditor.ContentDevice;
using Microsoft.Xna.Framework.Graphics;
using AridiaEditor.Properties;
using System.Reflection;
using System.Windows.Controls;
using AridiaEditor.Windows;
using GameApplicationTools.Resources;

namespace AridiaEditor
{
    public partial class MainWindow : Window
    {
        private Axis axis;
        private Camera camera;
        
        private Stopwatch watch = new Stopwatch();

        ContentBuilder contentBuilder;
        ServiceContainer ServiceContainer;

        // The color applied to the cube in the second viewport
        Color cubeColor = Color.Red;

        public MainWindow()
        {
            // check if our content path settings are empty
            // if yes than don't proceed and prompt the user
            // to enter a content path!
            if (!(Settings.Default.ContentPath == string.Empty))
            {
                InitializeComponent();
                ServiceContainer = new ServiceContainer();
                contentBuilder = new ContentBuilder();
            }
            else
            {
                bool result = false;
                while (!result)
                {
                    var dialog = new System.Windows.Forms.FolderBrowserDialog();
                    dialog.SelectedPath = Assembly.GetExecutingAssembly().Location;
                    dialog.Description = "Please select content directory for your specific files!";
                    
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        result = true;
                        Settings.Default.ContentPath = dialog.SelectedPath;
                        Settings.Default.Save();
                    }
                }
               
            }
        }

        /// <summary>
        /// Invoked after either control has created its graphics device.
        /// </summary>
        private void loadContent(object sender, GraphicsDeviceEventArgs e)
        {
            // Because this same event is hooked for both controls, we check if the Stopwatch
            // is running to avoid loading our content twice.
            if (!watch.IsRunning)
            {
                GameApplication.Instance.SetGraphicsDevice(e.GraphicsDevice);
                
                ServiceContainer.AddService<IGraphicsDeviceService>(GraphicsDeviceService.AddRef(new IntPtr(), 100, 100));
                ResourceManager.Instance.Content = new ContentManager(ServiceContainer, contentBuilder.OutputDirectory);
                ResourceManager.Instance.Content.Unload();

                contentBuilder.Clear();
                contentBuilder.Add(Settings.Default.ContentPath + "\\" + GameApplication.Instance.EffectPath + "DefaultEffect.fx",  "DefaultEffect", null, "EffectProcessor");

                // Build this new model data.
                string buildError = contentBuilder.Build();

                if (string.IsNullOrEmpty(buildError))
                {
                    ResourceManager.Instance.AddResourceEditor(new Resource() {
                            Name = "DefaultEffect",
                            Path = GameApplication.Instance.EffectPath,
                            Type = ResourceType.Effect
                        });

                }
                else
                {
                    // If the build failed, display an error message.
                    MessageBox.Show(buildError, "Error");
                }

                camera = new Camera("camera", new Vector3(0, 2, 3), Vector3.Zero);
                camera.LoadContent();
                WorldManager.Instance.AddActor(camera);
                CameraManager.Instance.CurrentCamera = "camera";

                axis = new Axis("axis", Vector3.Zero, 1f);
                axis.LoadContent();
                WorldManager.Instance.AddActor(axis);

                // Start the watch now that we're going to be starting our draw loop
                watch.Start();
            }
        }

        #region XNA Events
        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void xnaControl_RenderXna(object sender, GraphicsDeviceEventArgs e)
        {
            GameApplication.Instance.Update(new GameTime(new TimeSpan(watch.ElapsedMilliseconds), new TimeSpan(watch.ElapsedTicks)));

            e.GraphicsDevice.Clear(Color.White);

            GameApplication.Instance.Render(new GameTime(new TimeSpan(watch.ElapsedMilliseconds), new TimeSpan(watch.ElapsedTicks)));
        }

        // Invoked when the mouse moves over the second viewport
        private void xnaControl_MouseMove(object sender, HwndMouseEventArgs e)
        {
            // If the left or right buttons are down, we adjust the yaw and pitch of the cube
            /*if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed ||
                e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                yaw += (float)(e.Position.X - e.PreviousPosition.X) * .01f;
                pitch += (float)(e.Position.Y - e.PreviousPosition.Y) * .01f;
            }*/
        }

        // We use the left mouse button to do exclusive capture of the mouse so we can drag and drag
        // to rotate the cube without ever leaving the control
        private void xnaControl_HwndLButtonDown(object sender, HwndMouseEventArgs e)
        {
            xnaControl.CaptureMouse();
        }

        private void xnaControl_HwndLButtonUp(object sender, HwndMouseEventArgs e)
        {
            xnaControl.ReleaseMouseCapture();
        }
        #endregion

        #region EditorEvents
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.Owner = this;
            settings.Show();
        }
        #endregion
    }
}
