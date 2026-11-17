using GameEditor.Logging;
using GameLib.Files;
using GameLib.World.Tiles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameEditor.Windows
{
    /// <summary>
    /// Logique d'interaction pour WindowAddPropTexture.xaml
    /// </summary>
    public partial class WindowAddPropTexture : Window
    {
        private List<string> _files = [];

        public WindowAddPropTexture()
        {
            InitializeComponent();
        }

        private void Btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                InitialDirectory = GameEditorConfig.Path_Base_Projects_Dir,
                Filter = $"Image file (*.png) | *.png",
                FilterIndex = 1,
                RestoreDirectory = true,
                Multiselect = true
            };

            DialogResult result = dialog.ShowDialog();

            _files.Clear();

            if (result == System.Windows.Forms.DialogResult.OK)
                _files.AddRange(dialog.FileNames);

            UpdateListBoxFiles();
        }

        private void ListBox_Files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem? item = (ListBoxItem?)ListBox_Files.SelectedItem;

            if (item == null)
            {
                SetTexturePreview(null);
                return;
            }

            int index = 0;

            if (!int.TryParse(item.Tag.ToString(), out index))
            {
                SetTexturePreview(null);
                return;
            }

            string filename = _files[index];

            SetTexturePreview(filename);
        }

        private void Btn_Validate_Click(object sender, RoutedEventArgs e)
        {
            if (_files.Count <= 0)
                return;

            List<string> errorFiles = [];

            foreach (string file in _files)
            {
                IOResult ior = FilesManager.CopyPropTextureInProject(file);

                if (ior != IOResult.OK)
                {
                    errorFiles.Add(file);

                    Logger.Error($"{nameof(WindowAddPropTexture)}.{nameof(Btn_Validate_Click)}", $"An error occured during the copy of the file : {file}", null);
                }
            }

            if (errorFiles.Count > 0)
            {
                string text = "Errors occured for the file : \n";

                foreach (string file in errorFiles)
                {
                    text += $"> {file}\n";
                }

                string caption = "GameEditor - WindwoAddPropTexture";

                _ = System.Windows.MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            GameLib.Files.FilesManager.SaveProject();
            GameLib.Ressources.RessourcesManager.Init();

            this.Close();
        }

        private void UpdateListBoxFiles()
        {
            this.ListBox_Files.Items.Clear();

            for (int i = 0; i < _files.Count; ++i)
            {
                ListBoxItem item = new()
                {
                    Content = _files[i],
                    Name = $"item_{i}",
                    Tag = i
                };

                this.ListBox_Files.Items.Add(item);
            }
        }

        private void SetTexturePreview(string? filename)
        {
            if (filename == null)
            {
                Img_File.Source = null;
                return;
            }

            BitmapImage bitmapImage = new();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(filename);

            if (Img_File.RenderSize.Width < Img_File.RenderSize.Height)
                bitmapImage.DecodePixelWidth = (int)Img_File.RenderSize.Width;
            else
                bitmapImage.DecodePixelHeight = (int)Img_File.RenderSize.Height;

            bitmapImage.EndInit();

            Img_File.Source = bitmapImage;
        }
    }
}
