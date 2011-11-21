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
using Primitives3D;

namespace AridiaEditor
{
    public partial class MainWindow : Window
    {
        //private Axis axis;

        // We use a Stopwatch to track our total time for cube animation
        private Stopwatch watch = new Stopwatch();

        // A yaw and pitch applied to the second viewport based on input
        private float yaw = 0f;
        private float pitch = 0f;

        // The color applied to the cube in the second viewport
        Color cubeColor = Color.Red;

        public MainWindow()
        {
            InitializeComponent();
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
                // Create our 3D cube object

                // Start the watch now that we're going to be starting our draw loop
                watch.Start();
            }
        }


        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void xnaControl_RenderXna(object sender, GraphicsDeviceEventArgs e)
        {
            e.GraphicsDevice.Clear(Color.CornflowerBlue);

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
    }
}
