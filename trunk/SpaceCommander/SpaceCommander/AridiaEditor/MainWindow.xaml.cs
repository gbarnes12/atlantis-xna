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
using GameApplicationTools.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

namespace AridiaEditor
{
    public partial class MainWindow : Window
    {
        public static List<Error> errors;
        public static TextBlock outputTextBlock;
        


        Stopwatch watch = new Stopwatch();
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
                ResourceBuilder.Instance.ContentBuilder = contentBuilder;

                errors = new List<Error>();
                outputTextBlock = output;

                errorDataGrid.ItemsSource = errors;
                Output.AddToOutput("WELCOME TO ARIDIA WORLD EDITOR ------------");
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

                if (File.Exists(Settings.Default.LayoutFile))
                    dockManager.RestoreLayout(Settings.Default.LayoutFile);

                // after we initialized everything we need start loading the content
                // in a new thread!
                StartContentBuilding();

                // Start the watch now that we're going to be starting our draw loop
                watch.Start();
            }
        }

        #region ContentBuildingThread
        private void StartContentBuilding()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            #region Events
            worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                progressStatusBarItem.Dispatcher.Invoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(
                    delegate()
                    {
                        progressStatusBarItem.Content = "Building content...";
                    }
                ));

                #region Resource Builder Events
                ResourceBuilder.Instance.OnPercentChanged += new EventHandler<OnPercentChangedEventArgs>(delegate(object o, OnPercentChangedEventArgs OnPercentChangedEventArgs)
                {
                    worker.ReportProgress(OnPercentChangedEventArgs.Percent);
                });


                ResourceBuilder.Instance.OnBuildFailed +=  new EventHandler<EventArgs>(delegate(object onBuildFailed, EventArgs onBuildFailedArgs)
                {
                    worker.CancelAsync();
                });
                #endregion

                ResourceBuilder.Instance.BuildContent();
            };

            worker.ProgressChanged += delegate(object s, ProgressChangedEventArgs args)
            {
                int percentage = args.ProgressPercentage;

                progressBar.Value = percentage;
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                progressStatusBarItem.Content = "Ready";
                progressBar.Value = 0;
                Output.AddToOutput("Building of content files completed...");

                try
                {
                    Camera camera = new Camera("camera", new Vector3(0, 2, 3), Vector3.Zero);
                    camera.LoadContent();
                    WorldManager.Instance.AddActor(camera);
                    CameraManager.Instance.CurrentCamera = "camera";

                    Axis axis = new Axis("axis", Vector3.Zero, 1f);
                    axis.LoadContent();
                    WorldManager.Instance.AddActor(axis);
                }
                catch (System.Exception ex)
                {
                    Output.AddToError(new Error()
                    {
                        Name = "ACTOR_INITIALISING_FAILED",
                        Description = ex.Message,
                        Type = ErrorType.FATAL
                    });
                }
                
            };
            #endregion

            worker.RunWorkerAsync();
        }
        #endregion

        #region XNA Events
        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void xnaControl_RenderXna(object sender, GraphicsDeviceEventArgs e)
        {
            GameApplication.Instance.Update(new GameTime(new TimeSpan(watch.ElapsedMilliseconds), new TimeSpan(watch.ElapsedTicks)));
           // KeyboardDevice.Instance.Update();
            //MouseDevice.Instance.Update();

            if (CameraManager.Instance.CurrentCamera != null)
                CameraPosition.Content = "Camera: " + CameraManager.Instance.GetCurrentCamera().Position;

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

        private void SaveLayoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            dockManager.SaveLayout(Settings.Default.LayoutFile);
        }

        private void ExitAppMenuItem_Click(object sender, RoutedEventArgs e)
        {
            dockManager.SaveLayout(Settings.Default.LayoutFile);
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            dockManager.SaveLayout(Settings.Default.LayoutFile);
        }
        #endregion
    }
}
