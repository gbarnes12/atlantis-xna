using System.Windows.Forms;
#region File Description
//-----------------------------------------------------------------------------
// MainWindow.xaml.cs
//
// Copyright 2011, Gavin Barnes.
// Licensed under the terms of the Ms-PL: http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
//-----------------------------------------------------------------------------
#endregion

namespace AridiaEditor
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Reflection;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Graphics;

    using AridiaEditor.Cameras;
    using AridiaEditor.Windows;
    using AridiaEditor.ContentDevice;
    using AridiaEditor.Properties;

    using GameApplicationTools.Structures;
    using GameApplicationTools.Actors.Primitives;
    using GameApplicationTools;
    using GameApplicationTools.Actors.Cameras;
    using GameApplicationTools.Actors;
    using GameApplicationTools.Interfaces;
    using GameApplicationTools.Misc;
    using GameApplicationTools.Resources;
    using GameApplicationTools.Input;

    public partial class MainWindow : Window
    {
        public static List<Error> errors;
        public static TextBlock outputTextBlock;
        public static SceneGraphManager sceneGraph;
        public static EditorStatus EditorStatus { get; set; }
        public static Level Level { get; set; }

        Stopwatch watch = new Stopwatch();
        ContentBuilder contentBuilder;
        ServiceContainer ServiceContainer;

        object SelectedObject = null;
        

        public MainWindow()
        {

            // WRITE ASSEMBLY FILE TO README!
            if (File.Exists("README.txt"))
            {
                TextFile file = new TextFile();
                file.WriteLine(@"README.txt", 7, "* BUILD:  " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(), true);
            }

            // check if our content path settings are empty
            // if yes than don't proceed and prompt the user
            // to enter a content path!
            if (!(Settings.Default.ContentPath == string.Empty))
            {
                InitializeComponent();
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

                        InitializeComponent();
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

                ServiceContainer = new ServiceContainer();
                contentBuilder = new ContentBuilder();
                ResourceBuilder.Instance.ContentBuilder = contentBuilder;

                errors = new List<Error>();
                outputTextBlock = output;
                EditorStatus = EditorStatus.STARTING;
                errorDataGrid.ItemsSource = errors;
                Output.AddToOutput("WELCOME TO ARIDIA WORLD EDITOR ------------");

                GameApplication.Instance.SetGraphicsDevice(e.GraphicsDevice);
                MouseDevice.Instance.ResetMouseAfterUpdate = false;
                ServiceContainer.AddService<IGraphicsDeviceService>(GraphicsDeviceService.AddRef(new IntPtr(), 100, 100));
                ResourceManager.Instance.Content = new ContentManager(ServiceContainer, contentBuilder.OutputDirectory);
                ResourceManager.Instance.Content.Unload();
                sceneGraph = new SceneGraphManager();

                var versionAttribute = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                AssemblyBuild.Content = "Build: (Alpha) " + versionAttribute;
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
                EditorStatus = EditorStatus.LOADING;

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
                EditorStatus = EditorStatus.IDLE;
                progressStatusBarItem.Content = "Ready";
                progressBar.Value = 0;
                Output.AddToOutput("Building of content files completed...");
            };
            #endregion

            worker.RunWorkerAsync();
        }
        #endregion

        #region EditorFunctions
        void LoadWorldView()
        {
            Dictionary<string, Camera> cameras = CameraManager.Instance.GetCameras();

            TreeViewItem worldNode = WorldTreeView.Items[0] as TreeViewItem;
            TreeViewItem actorNode = worldNode.Items[0] as TreeViewItem;
            TreeViewItem cameraNode = worldNode.Items[1] as TreeViewItem;

            actorNode.Items.Clear();
            cameraNode.Items.Clear();

            foreach(Camera camera in cameras.Values)
            {
                TreeViewItem cameraItem = new TreeViewItem();
                cameraItem.Header = camera.ID;
                cameraNode.Items.Add(cameraItem);
            }

            if (sceneGraph != null)
            {
                foreach (Actor actor in sceneGraph.RootNode.Children)
                {
                    TreeViewItem item = new TreeViewItem();
                    item = LoadActorsRecursive(actor, item);
                    item.Header = actor.ID;
                    actorNode.Items.Add(item);
                }
            }

            actorNode.IsExpanded = true;
            cameraNode.IsExpanded = true;
        }

        TreeViewItem LoadActorsRecursive(Actor actor, TreeViewItem item)
        {
            foreach (Actor actorx in actor.Children)
            {
                TreeViewItem itemx = new TreeViewItem();
                itemx = LoadActorsRecursive(actorx, itemx);
                itemx.Header = actor.ID;
                item.Items.Add(itemx);
            }

            return item;
        }
        #endregion

        #region XNA Events
        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void xnaControl_RenderXna(object sender, GraphicsDeviceEventArgs e)
        {
            if (watch != null)
            {
                sceneGraph.Update(new GameTime(new TimeSpan(watch.ElapsedMilliseconds), new TimeSpan(watch.ElapsedTicks)));

                KeyboardDevice.Instance.Update();
                MouseDevice.Instance.Update();

                if (CameraManager.Instance.CurrentCamera != null)
                    CameraPosition.Content = "Camera: " + CameraManager.Instance.GetCurrentCamera().Position;

                e.GraphicsDevice.Clear(Color.White);

                sceneGraph.Render();
            }
        }

        // Invoked when the mouse moves over the second viewport
        private void xnaControl_MouseMove(object sender, HwndMouseEventArgs e)
        {
            if (SelectedObject != null && SelectedObject is Actor)
            {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    Camera cam = CameraManager.Instance.GetCurrentCamera();
                    float x = (float)e.Position.X;
                    float y = (float)e.Position.Y;
                    if (cam.GetMouseRay(new Vector2(x, y)).Intersects(Utils.TransformBoundingSphere(((Actor)SelectedObject).BoundingSphere, ((Actor)SelectedObject).AbsoluteTransform)) != null)
                    {
                        ((Actor)SelectedObject).Position = new Vector3(((Actor)SelectedObject).Position.X, ((Actor)SelectedObject).Position.Y, ((Actor)SelectedObject).Position.Z + 0.2f * 0.2f);
                    }
                }
            }
        }

        // We use the left mouse button to do exclusive capture of the mouse so we can drag and drag
        // to rotate the cube without ever leaving the control
        private void xnaControl_HwndLButtonDown(object sender, HwndMouseEventArgs e)
        {
            //xnaControl.CaptureMouse();
            if (CameraManager.Instance.CurrentCamera != null)
            {
                if (SelectedObject == null)
                {
                    foreach (Actor actor in WorldManager.Instance.GetActors().Values)
                    {
                        if (actor is IPickable)
                        {
                            Camera cam = CameraManager.Instance.GetCurrentCamera();
                            float x = (float)e.Position.X;
                            float y = (float)e.Position.Y;
                            //new Vector2(x, y)
                            if (cam.GetMouseRay(new Vector2(x, y)).Intersects(Utils.TransformBoundingSphere(actor.BoundingSphere, actor.AbsoluteTransform)) != null)
                            {
                                Output.AddToOutput("Object : " + actor.ID + " has been picked!");
                                propertyGrid.SelectedObject = actor;
                                SelectedObject = actor;
                            }
                            else
                            {
                                Output.AddToOutput("ray didn't hit any object");
                            }
                        }
                    }
                }
            }
        }

        private void xnaControl_HwndLButtonUp(object sender, HwndMouseEventArgs e)
        {
           // xnaControl.ReleaseMouseCapture();
        }

        private void xnaControl_HwndRButtonDown(object sender, HwndMouseEventArgs e)
        {
            propertyGrid.SelectedObject = null;
            SelectedObject = null;
            EditorCamera cam = CameraManager.Instance.GetCurrentCamera() as EditorCamera;
            cam.Active = true;
            MouseDevice.Instance.ResetMouseAfterUpdate = true;
            NativeMethods.ShowCursor(false);
        }

        private void xnaControl_HwndRButtonUp(object sender, HwndMouseEventArgs e)
        {
            EditorCamera cam = CameraManager.Instance.GetCurrentCamera() as EditorCamera;
            cam.Active = false;
            MouseDevice.Instance.ResetMouseAfterUpdate = false;
            NativeMethods.ShowCursor(true);
        }
        #endregion

        #region EditorEvents
        private void WorldTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = e.NewValue as TreeViewItem;
            TreeViewItem parent = item.Parent as TreeViewItem;

            if (parent.SelectedItem.Header.ToString() == "Actors")
                propertyGrid.SelectedObject = SelectedObject as Actor;
            else if (parent.Header.ToString() == "Cameras")
                propertyGrid.SelectedObject = SelectedObject as Camera;
        }

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (EditorStatus == EditorStatus.IDLE)
            {
                NewLevelWindow newLevelWindow = new NewLevelWindow();
                newLevelWindow.Owner = this;

                if (newLevelWindow.ShowDialog().Value)
                {
                    if (newLevelWindow.Filename != "")
                    {
                        Level = new Level();
                        Level.Name = newLevelWindow.Filename;
                        LevelStatus.Content = "Level: " + newLevelWindow.Filename;

                        if (newLevelWindow.CreateCamera)
                        {
                            Camera camera = new Camera("default.Camera", newLevelWindow.CameraPosition, newLevelWindow.CameraTarget);
                            camera.LoadContent();
                            CameraManager.Instance.CurrentCamera = "default.Camera";

                            if (newLevelWindow.CreateAxis)
                            {
                                Axis axis = new Axis("default.Axis", 1f);
                                axis.LoadContent();
                                sceneGraph.RootNode.Children.Add(axis);
                            }

                            if (newLevelWindow.CreatePlane)
                            {
                                GameApplicationTools.Actors.Primitives.Plane plane = new GameApplicationTools.Actors.Primitives.Plane("defaultPlane", Vector3.Zero);
                                plane.LoadContent();
                                sceneGraph.RootNode.Children.Add(plane);
                            }

                            LoadWorldView();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please wait till the Editor is fully loaded");
            }
        }

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

    public enum EditorStatus
    {
        LOADING,
        IDLE,
        STARTING
    }

    /*
     * 
     * try
                {
                    EditorCamera camera = new EditorCamera("camera", new Vector3(0, 0, 4), Vector3.Zero);
                    camera.LoadContent();
                    CameraManager.Instance.CurrentCamera = "camera";

                    Axis axis = new Axis("axis", 1f);
                    axis.LoadContent();
                    sceneGraph.RootNode.Children.Add(axis);

                    Box box = new Box("box", 1f);
                    box.Position = new Vector3(0, 0, 0);
                    box.Rotation *= Quaternion.CreateFromYawPitchRoll(0, 2f, 0);
                    box.LoadContent();

                    Sphere sphere = new Sphere("sphere", 2f);
                    sphere.Offset = Vector3.Zero;
                    sphere.LoadContent();

                    box.Children.Add(sphere);
                    sceneGraph.RootNode.Children.Add(box);

                }
                catch (System.Exception ex)
                {
                    Output.AddToError(new Error()
                    {
                        Name = "ACTOR_INITIALISING_FAILED",
                        Description = ex.Message,
                        Type = ErrorType.FATAL
                    });
                }*/
}
