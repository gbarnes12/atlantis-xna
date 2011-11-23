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
using AridiaEditor.Properties;

namespace AridiaEditor.Windows
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            contentPathTextBox.Text = Settings.Default.ContentPath;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ContentPath = contentPathTextBox.Text;
            Settings.Default.Save();
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
