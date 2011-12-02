﻿namespace AridiaEditor.Windows
{
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
    using GameApplicationTools;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Interaktionslogik für CreatePlaneWindow.xaml
    /// </summary>
    public partial class CreatePlaneWindow : Window
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public String ID { get; set; }
        public String Texture { get; set; }

        public CreatePlaneWindow()
        {
            InitializeComponent();

            foreach (string texture in ResourceManager.Instance.GetResourcesOfType<Texture2D>().Keys)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = texture;
                textures.Items.Add(item);
            }
        }

        #region Events
        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            if (idTextBox.Text != string.Empty && positionX.Text != string.Empty
                && positionY.Text != string.Empty && positionZ.Text != string.Empty
                && scaleX.Text != string.Empty && scaleY.Text != string.Empty
                && scaleZ.Text != string.Empty && textures.SelectedItem != null)
            {
                if (!WorldManager.Instance.GetActors().ContainsKey(idTextBox.Text))
                {
                    Position = new Vector3(float.Parse(positionX.Text), float.Parse(positionY.Text), float.Parse(positionZ.Text));
                    Scale = new Vector3(float.Parse(scaleX.Text), float.Parse(scaleY.Text), float.Parse(scaleZ.Text));
                    Texture = ((ComboBoxItem)textures.SelectedItem).Content.ToString();
                    ID = idTextBox.Text.ToString();
                    DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("The id you have chosen is already taken.");
                }
            }
            else
            {
                MessageBox.Show("Please provide all information");
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
