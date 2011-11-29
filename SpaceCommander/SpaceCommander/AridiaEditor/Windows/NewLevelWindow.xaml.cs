using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Xna.Framework;

namespace AridiaEditor.Windows
{
    /// <summary>
    /// Interaktionslogik für NewLevelWindow.xaml
    /// </summary>
    public partial class NewLevelWindow : Window
    {
        public string Filename { get; set; }
        public bool CreateAxis { get; set; }
        public bool CreatePlane { get; set; }
        public bool CreateCamera { get; set; }

        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraTarget { get; set; }

        public NewLevelWindow()
        {
            InitializeComponent();
        }

        #region Events
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            Filename = fileNameTextBox.Text;
            CreateAxis = createAxisCheckBox.IsChecked.Value;
            CreatePlane = createPlaneCheckBox.IsChecked.Value;
            CreateCamera = createCameraCheckBox.IsChecked.Value;
            

            if (fileNameTextBox.Text != string.Empty && positionX.Text != string.Empty
                && positionY.Text != string.Empty && positionZ.Text != string.Empty
                && targetX.Text != string.Empty && targetY.Text != string.Empty 
                && targetZ.Text != string.Empty)
            {
                CameraPosition = new Vector3(float.Parse(positionX.Text), float.Parse(positionY.Text), float.Parse(positionZ.Text));
                CameraTarget = new Vector3(float.Parse(targetX.Text), float.Parse(targetY.Text), float.Parse(targetZ.Text));
                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a file name");
            }
        }
        #endregion

        private void createCameraCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;

            if (box.IsChecked == false)
            {
                if (optionsGrid != null)
                {
                    createAxisCheckBox.IsChecked = false;
                    createPlaneCheckBox.IsChecked = false;
                    optionsGrid.Visibility = Visibility.Hidden;
                    positionGrid.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                if (optionsGrid != null)
                {
                    optionsGrid.Visibility = Visibility.Visible;
                    positionGrid.Visibility = Visibility.Visible;
                    createAxisCheckBox.IsChecked = true;
                }
            }
        }

    }
}
