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
    using GameApplicationTools.Actors.Properties;
    using GameApplicationTools.Resources.Shader;

    using AridiaEditor.Windows.CreateWindows;
    using GameApplicationTools.Actors.Advanced;
    using Microsoft.Windows.Controls.Ribbon;
    using AridiaEditor.Databases;
    using System.CodeDom.Compiler;
    using AridiaEditor.Databases.Data;
    using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

    public partial class MainWindow : RibbonWindow
    {
        public static List<Error> errors;
        public static TextBlock outputTextBlock;
        public static SceneGraphManager sceneGraph;
        public static EditorStatus EditorStatus { get; set; }
        public static EditMode EditMode { get; set; }
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

            EditMode = EditMode.STANDARD;

            // set the index.hml content file!
            webBrowser.Source = new Uri(Environment.CurrentDirectory + "\\Content\\Instructions\\index.html");
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

                resourceContent.Activate();

                errors = new List<Error>();
                outputTextBlock = output;
                EditorStatus = EditorStatus.STARTING;
                EditMode = AridiaEditor.EditMode.STANDARD;
                errorDataGrid.ItemsSource = errors;
                Output.AddToOutput("WELCOME TO ARIDIA WORLD EDITOR ------------");

                GameApplication.Instance.SetGraphicsDevice(e.GraphicsDevice);
                MouseDevice.Instance.ResetMouseAfterUpdate = false;
                ServiceContainer.AddService<IGraphicsDeviceService>(GraphicsDeviceService.AddRef(new IntPtr(), 100, 100));
                ResourceManager.Instance.Content = new ContentManager(ServiceContainer, contentBuilder.OutputDirectory);
                ResourceManager.Instance.Content.Unload();
                


                sceneGraph = new SceneGraphManager();
                sceneGraph.CullingActive = true;
                sceneGraph.LightingActive = false; //deactivate lighting on beginning!

                e.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

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
                Database.Instance.LoadData();

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
                sceneGraph.Lighting.LoadContent();
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
                if (camera.ID != "editor.Camera")
                {
                    TreeViewItem cameraItem = new TreeViewItem();
                    cameraItem.Header = camera.ID;

                    #region CreateMenu
                    ContextMenu menu = new ContextMenu();
                    MenuItem setToCurrentCamera = new MenuItem();
                    setToCurrentCamera.Name = "SetToCurrentCameraMenuItem";
                    setToCurrentCamera.Header = "Set as current camera";
                    menu.Items.Add(setToCurrentCamera);
                    #endregion
                    cameraItem.ContextMenu = menu;
                    cameraNode.Items.Add(cameraItem);
                }
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
                itemx.Header = actorx.ID;
                item.Items.Add(itemx);
            }

            return item;
        }
        #endregion

        #region XNA Events
        private void ControlEditMode()
        {
            if(SelectedObject != null)
            {
                if(SelectedObject is Actor)
                {
                    Actor obj = SelectedObject as Actor;
                    if (EditMode == EditMode.MOVE)
                    {
                
                        Vector3 inputModifier = new Vector3(
                        (KeyboardDevice.Instance.IsKeyDown(Keys.Left) ? -1 : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.Right) ? 1 : 0),
                        (KeyboardDevice.Instance.IsKeyDown(Keys.Down) ? -1 : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.Up) ? 1 : 0),
                        (KeyboardDevice.Instance.IsKeyDown(Keys.PageDown) ? -1 : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.PageUp) ? 1 : 0));

                        obj.Position += inputModifier * 0.05f;
                    }
                    else if (EditMode == EditMode.SCALE)
                    {
                        Vector3 inputModifier = new Vector3(
                        (KeyboardDevice.Instance.IsKeyDown(Keys.Left) ? -1 : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.Right) ? 1 : 0),
                        (KeyboardDevice.Instance.IsKeyDown(Keys.Down) ? -1 : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.Up) ? 1 : 0),
                        (KeyboardDevice.Instance.IsKeyDown(Keys.PageDown) ? -1 : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.PageUp) ? 1 : 0));

                        obj.Scale += inputModifier * 0.05f;
                    }
                    else if (EditMode == EditMode.ROTATE)
                    {
                        float yaw = (KeyboardDevice.Instance.IsKeyDown(Keys.Left) ? -1f : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.Right) ? 1f : 0);
                        float pitch = (KeyboardDevice.Instance.IsKeyDown(Keys.Down) ? -1f : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.Up) ? 1f : 0);
                        float roll = (KeyboardDevice.Instance.IsKeyDown(Keys.PageDown) ? -1f : 0) + (KeyboardDevice.Instance.IsKeyDown(Keys.PageUp) ? 1f : 0);

                        obj.Rotation *= Quaternion.CreateFromYawPitchRoll(yaw * 0.05f,  pitch * 0.05f, roll * 0.05f);
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void xnaControl_RenderXna(object sender, GraphicsDeviceEventArgs e)
        {
            if (watch != null)
            {
                sceneGraph.Update(new GameTime(new TimeSpan(watch.ElapsedMilliseconds), new TimeSpan(watch.ElapsedTicks)));

                // Control the modes for the object rotation, scaling and moving
                ControlEditMode();

                KeyboardDevice.Instance.Update();
                MouseDevice.Instance.ResetMouseAfterUpdate = false;
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
                        if (actor.Properties.ContainsKey(ActorPropertyType.PICKABLE))
                        {
                            Camera cam = CameraManager.Instance.GetCurrentCamera();
                            float x = (float)e.Position.X;
                            float y = (float)e.Position.Y;
                            //new Vector2(x, y)
                            if (cam.GetMouseRay(new Vector2(x, y)).Intersects(Utils.TransformBoundingSphere(actor.BoundingSphere, actor.AbsoluteTransform)) != null)
                            {
                                Output.AddToOutput("Object : " + actor.ID + " has been picked!");

                                if (actor is MeshObject)
                                {
                                    propertyGrid.SelectedObject = (MeshObject)actor;
                                    shaderPropertyGrid.Enabled = true;
                                    shaderPropertyGrid.SelectedObject = ((MeshObject)actor).Material;
                                    
                                }
                                else
                                {
                                    propertyGrid.SelectedObject = actor;
                                }

                                SelectedObject = actor;

                                ((PickableProperty)actor.Properties[ActorPropertyType.PICKABLE]).IsPicked = true;
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
            if (SelectedObject != null)
            {
                // set actor to isn't picked.
                if (SelectedObject is Actor)
                {
                    if (((Actor)SelectedObject).Properties.ContainsKey(ActorPropertyType.PICKABLE))
                        ((PickableProperty)((Actor)SelectedObject).Properties[ActorPropertyType.PICKABLE]).IsPicked = true;
                }

                propertyGrid.SelectedObject = null;
                shaderPropertyGrid.SelectedObject = null;
                SelectedObject = null;
            }

            if (CameraManager.Instance.CurrentCamera != null)
            {
                EditorCamera cam = CameraManager.Instance.GetCurrentCamera() as EditorCamera;
                if (cam != null)
                {
                    cam.Active = true;
                    MouseDevice.Instance.ResetMouseAfterUpdate = true;
                    NativeMethods.ShowCursor(false);
                    
                }
            }
        }

        private void xnaControl_HwndRButtonUp(object sender, HwndMouseEventArgs e)
        {
            if (CameraManager.Instance.CurrentCamera != null)
            {
                EditorCamera cam = CameraManager.Instance.GetCurrentCamera() as EditorCamera;
                if (cam != null)
                {
                    cam.Active = false;
                    MouseDevice.Instance.ResetMouseAfterUpdate = false;
                    NativeMethods.ShowCursor(true);
                }
            }
        }
        #endregion

        #region EditorEvents
        private void CreateSkySphereMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Level != null)
            {
                CreateSkySphereWindow createSkySphereWindow = new CreateSkySphereWindow();
                createSkySphereWindow.Owner = this;

                if (createSkySphereWindow.ShowDialog().Value)
                {

                
                    SkySphere sphere = new SkySphere(createSkySphereWindow.ID, createSkySphereWindow.Texture, 1f);
                    sphere.Position = createSkySphereWindow.Position;
                    sphere.Scale = createSkySphereWindow.Scale;
                    sphere.LoadContent();
                    sceneGraph.RootNode.Children.Add(sphere);
                    LoadWorldView();
                }
            }
            else
            {
                MessageBox.Show("Please create or load a level first before you add any actor");
            }
        }

        private void CreateMeshObjectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Level != null)
            {
                CreateMeshObjectWindow createMeshObjectWindow = new CreateMeshObjectWindow();
                createMeshObjectWindow.Owner = this;

                if (createMeshObjectWindow.ShowDialog().Value)
                {
                    MeshObject obj = new MeshObject(createMeshObjectWindow.ID, createMeshObjectWindow.Model, 1f);
                    obj.Position = createMeshObjectWindow.Position;
                    obj.Scale = createMeshObjectWindow.Scale;
                    obj.LoadContent();
                    
                    obj.SetModelEffect(ResourceManager.Instance.GetResource<Effect>(Database.Instance.Shaders[createMeshObjectWindow.Shader].File), true);
                    Shader shader = Database.Instance.Shaders[createMeshObjectWindow.Shader];

                    if (shader.Type == Databases.Data.MaterialType.None ||
                        shader.Type == Databases.Data.MaterialType.Engine)
                    {
                        // because we are using an in engine shader we don't need to compile anything in
                        // here and can use the shader straight away and create a new instance of its material!

                        string typeName = shader.Content;
                        Type type = Type.GetType(typeName);
                        if (type != null)
                        {
                            Material mat = (Material)Activator.CreateInstance(type);
                            obj.Material = mat;
                        }
                    }
                    else if (shader.Type == Databases.Data.MaterialType.Compile)
                    {
                        // compile our shader code !
                        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

                        CompilerParameters cp = new CompilerParameters();
                        cp.ReferencedAssemblies.Add("system.dll");
                        cp.ReferencedAssemblies.Add("system.drawing.dll");
                        cp.ReferencedAssemblies.Add("C:\\Program Files (x86)\\Microsoft XNA\\XNA Game Studio\\v4.0\\References\\Windows\\x86\\Microsoft.Xna.Framework.Graphics.dll");
                        cp.ReferencedAssemblies.Add("C:\\Program Files (x86)\\Microsoft XNA\\XNA Game Studio\\v4.0\\References\\Windows\\x86\\Microsoft.Xna.Framework.dll");

                        FileInfo fi = new FileInfo(Assembly.GetEntryAssembly().Location);

                        String path = System.IO.Path.GetDirectoryName(
                            System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                        cp.ReferencedAssemblies.Add(fi.DirectoryName + "\\GameApplication.dll");

                        cp.CompilerOptions = "/t:library";
                        cp.GenerateInMemory = true;

                        CompilerResults cr =
                        provider.CompileAssemblyFromSource(cp, shader.Content);

                        if (cr.Errors.Count > 0)
                        {
                            foreach (CompilerError error in cr.Errors)
                            {
                                Error err = new Error();
                                err.Name = "MESHOBJECT::SHADER_COMPILE1001";
                                err.Type = ErrorType.FATAL;
                                err.Description = new Exception(error.ErrorText).ToString();
                                Output.AddToError(err);
                            }
                        }
                        else
                        {
                            Assembly assembly = cr.CompiledAssembly;
                            try
                            {
                                object o = assembly.CreateInstance(shader.Namespace);
                                if (o != null)
                                    obj.Material = (Material)o;
                            }
                            catch (Exception exception)
                            {
                                Error err = new Error();
                                err.Name = "MESHOBJECT::MATERIAL_CREATE_INSTANCE1001";
                                err.Type = ErrorType.FATAL;
                                err.Description = exception.ToString();
                                Output.AddToError(err);
                            }
                        }
                    }

                    sceneGraph.RootNode.Children.Add(obj);
                    LoadWorldView();
                }
            }
            else
            {
                MessageBox.Show("Please create or load a level first before you add any actor");
            }
        }

        private void EditModeMoveButton_Click(object sender, RoutedEventArgs e)
        {
            EditMode = EditMode.MOVE;
        }

        private void EditModeRotateButton_Click(object sender, RoutedEventArgs e)
        {
            EditMode = EditMode.ROTATE;
        }

        private void EditModeScaleButton_Click(object sender, RoutedEventArgs e)
        {
            EditMode = EditMode.SCALE;
        }

        private void EditModeStandardButton_Click(object sender, RoutedEventArgs e)
        {
            EditMode = EditMode.STANDARD;
        }

        private void TextureBrowserItem_Click(object sender, RoutedEventArgs e)
        {
            if (EditorStatus == EditorStatus.IDLE)
            {
                TextureBrowserWindow textureBrowser = new TextureBrowserWindow();
                textureBrowser.Owner = this;
                textureBrowser.Show();
            }
        }

        private void WorldTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = e.NewValue as TreeViewItem;
            TreeViewItem parent = item.Parent as TreeViewItem;

            if (parent != null)
            {
                if (WorldManager.Instance.GetActors().ContainsKey(item.Header.ToString()))
                {
                    if (parent.Header.ToString() == "Actors" || parent.Header.ToString() == WorldManager.Instance.GetActor(item.Header.ToString()).Parent.ID)
                    {
                        SelectedObject = WorldManager.Instance.GetActor(item.Header.ToString());

                        if (SelectedObject as Actor is MeshObject)
                        {
                            propertyGrid.SelectedObject = SelectedObject as MeshObject;
                            shaderPropertyGrid.Enabled = true;
                            shaderPropertyGrid.SelectedObject = ((MeshObject)SelectedObject).Material;
                        }
                        else
                        {
                            shaderPropertyGrid.Enabled = false;
                            propertyGrid.SelectedObject = SelectedObject as Actor;
                        }
                    }
                }
                else if (parent.Header.ToString() == "Cameras")
                {
                    SelectedObject = CameraManager.Instance.GetCamera(item.Header.ToString());
                    propertyGrid.SelectedObject = SelectedObject as Camera;
                }
            }
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

                            EditorCamera editorCamera = new EditorCamera("editor.Camera", newLevelWindow.CameraPosition, newLevelWindow.CameraTarget);
                            editorCamera.LoadContent();
                            CameraManager.Instance.CurrentCamera = "editor.Camera";

                            if (newLevelWindow.CreateAxis)
                            {
                                Axis axis = new Axis("default.Axis", 1f);
                                axis.LoadContent();
                                sceneGraph.RootNode.Children.Add(axis);
                            }

                            if (newLevelWindow.CreatePlane)
                            {
                                GameApplicationTools.Actors.Primitives.Plane plane = new GameApplicationTools.Actors.Primitives.Plane("defaultPlane", "kachel2_bump");
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

        private void CreateBoxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Level != null)
            {
                CreateBoxWindow createBoxWindow = new CreateBoxWindow();
                createBoxWindow.Owner = this;

                if (createBoxWindow.ShowDialog().Value)
                {
                    Box box = new Box(createBoxWindow.ID, createBoxWindow.Texture, createBoxWindow.Normalmap, 1f);
                    box.Position = createBoxWindow.Position;
                    box.Scale = createBoxWindow.Scale;
                    box.LoadContent();
                    sceneGraph.RootNode.Children.Add(box);
                    LoadWorldView();
                }
            }
            else
            {
                MessageBox.Show("Please create or load a level first before you add any actor");
            }
        }

        private void CreatePlaneMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Level != null)
            {
                CreatePlaneWindow createPlaneWindow = new CreatePlaneWindow();
                createPlaneWindow.Owner = this;

                if (createPlaneWindow.ShowDialog().Value)
                {
                    GameApplicationTools.Actors.Primitives.Plane plane = new GameApplicationTools.Actors.Primitives.Plane(createPlaneWindow.ID, createPlaneWindow.Texture);
                    plane.Position = createPlaneWindow.Position;
                    plane.Scale = createPlaneWindow.Scale;
                    plane.LoadContent();
                    sceneGraph.RootNode.Children.Add(plane);
                    LoadWorldView();
                }
            }
            else
            {
                MessageBox.Show("Please create or load a level first before you add any actor");
            }
        }

        private void CreateTriangleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Level != null)
            {
                CreateTriangleWindow createTriangleWindow = new CreateTriangleWindow();
                createTriangleWindow.Owner = this;

                if (createTriangleWindow.ShowDialog().Value)
                {
                    Triangle triangle = new Triangle(createTriangleWindow.ID);
                    triangle.Position = createTriangleWindow.Position;
                    triangle.Scale = createTriangleWindow.Scale;
                    triangle.LoadContent();
                    sceneGraph.RootNode.Children.Add(triangle);
                    LoadWorldView();
                }
            }
            else
            {
                MessageBox.Show("Please create or load a level first before you add any actor");
            }
        }
        #endregion  

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Up || e.Key == System.Windows.Input.Key.Down
                || e.Key == System.Windows.Input.Key.Left || e.Key == System.Windows.Input.Key.Right)
            {
                e.Handled = true;
            }
        }

        private void shaderPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            string test = e.ChangedItem.PropertyDescriptor.Category;

        }
 
    }

    public enum EditorStatus
    {
        LOADING,
        IDLE,
        STARTING
    }

    public enum EditMode
    {
        STANDARD,
        MOVE,
        ROTATE,
        SCALE
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
