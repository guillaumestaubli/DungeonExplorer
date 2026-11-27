using GameEditor.Logging;
using GameLib;
using GameLib.Entities;
using GameLib.Files;
using GameLib.Ressources;
using GameLib.World;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameEditor.Windows.Texture.AddTexture
{
    /// <summary>
    /// Logique d'interaction pour WindowAddPlayerTexture.xaml
    /// </summary>
    public partial class WindowAddPlayerTexture : Window
    {
        private string? _file = null;

        public WindowAddPlayerTexture()
        {
            InitializeComponent();

            this.Loaded += WindowAddPlayerTexture_Loaded;
        }

        private void WindowAddPlayerTexture_Loaded(object sender, RoutedEventArgs e)
        {
            string[] actions = Enum.GetNames<GameLib.Entities.Action>();

            for (int i = 0; i < actions.Length; ++i)
            {
                ComboBoxItem item = new()
                {
                    Name = $"Item_{actions[i]}",
                    Content = actions[i],
                    Tag = i
                };

                this.ComboBox_Action.Items.Add(item);
            }

        }

        private void Btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new()
            {
                InitialDirectory = GameEditorConfig.Path_Base_Projects_Dir,
                Filter = $"Image file (*.png) | *.png",
                FilterIndex = 1,
                RestoreDirectory = true,
                Multiselect = false
            };

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            _file = null;

            if (result == System.Windows.Forms.DialogResult.OK)
                _file = dialog.FileName;

            this.TxtBox_TextureFile.Text = _file ?? "";

            SetTexturePreview(_file);
        }

        private void SetTexturePreview(string? file)
        {
            if (file == null)
            {
                this.Img_TexturePreview.Source = null;
                return;
            }

            BitmapImage bitmapImage = new();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(file);

            if (this.Img_TexturePreview.RenderSize.Width < this.Img_TexturePreview.RenderSize.Height)
                bitmapImage.DecodePixelWidth = (int)this.Img_TexturePreview.RenderSize.Width;
            else
                bitmapImage.DecodePixelHeight = (int)this.Img_TexturePreview.RenderSize.Height;

            bitmapImage.EndInit();

            this.Img_TexturePreview.Source = bitmapImage;
        }

        private void TxtBox_Validation_Number_Integer(object? sender, TextCompositionEventArgs e)
        {
            if (sender == null || sender is not TextBox)
            {
                e.Handled = false;
                return;
            }

            string fullInput = ((TextBox)sender).Text + e.Text;

            if (fullInput.Length == 0)
                fullInput = "0";

            bool success = int.TryParse(fullInput, CultureInfo.InvariantCulture, out _);

            e.Handled = !success;

            // e.Handled = false; => write new char
            // e.Handled = true;  => don't write new char
        }

        private void Btn_Validate_Click(object sender, RoutedEventArgs e)
        {
            string? file = null;
            GameLib.Entities.Action? action = null;
            int duration = -1;
            PlayerTextureLine[] lines = Array.Empty<PlayerTextureLine>();

            if (!GetTextureFile(out file))
                return;

            if (!GetAction(out action))
                return;

            if (!GetDuration(out duration))
                return;

            if (!GetTextureLines(out lines))
                return;

            if (file == null || action == null || lines.Length == 0)
                return;

            bool success = RessourcesManager.AddPlayerTextureInProject(file, (GameLib.Entities.Action)action, lines, duration);

            if (!success)
            {
                string message = "An error happened when copying and creating texture in project";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            this.Close();
        }

        private bool GetTextureFile(out string? file)
        {
            file = null;

            if (_file == null)
            {
                string text = "No texture file selected (.png)";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                file = string.Empty;
                return false;
            }

            file = _file;
            return true;
        }

        private bool GetAction(out GameLib.Entities.Action? action)
        {
            action = null;

            ComboBoxItem? itemAction = (ComboBoxItem?)this.ComboBox_Action.SelectedItem;

            if (itemAction == null)
            {
                string text = "No Action selected";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                return false;
            }

            int actionIndex = 0;

            if (!int.TryParse(itemAction.Tag.ToString(), out actionIndex))
            {
                string text = "Failed to fetch selected action";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                return false;
            }

            action = (GameLib.Entities.Action)actionIndex;
            return true;
        }

        private bool GetTextureLines(out PlayerTextureLine[] lines)
        {
            int texCount_0 = 0,
                texCount_1 = 0,
                texCount_2 = 0,
                texCount_3 = 0;

            Direction
                dir_0 = Direction.None,
                dir_1 = Direction.None,
                dir_2 = Direction.None,
                dir_3 = Direction.None;

            #region TEX_COUNT

            if (!int.TryParse(this.TxtBox_TexCount_0.Text, out texCount_0))
            {
                string text = "Failed to parse textures count for line index 0";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                lines = [];
                return false;
            }

            if (!int.TryParse(this.TxtBox_TexCount_1.Text, out texCount_1))
            {
                string text = "Failed to parse textures count for line index 1";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                lines = [];
                return false;
            }

            if (!int.TryParse(this.TxtBox_TexCount_2.Text, out texCount_2))
            {
                string text = "Failed to parse textures count for line index 2";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                lines = [];
                return false;
            }

            if (!int.TryParse(this.TxtBox_TexCount_3.Text, out texCount_3))
            {
                string text = "Failed to parse textures count for line index 3";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                lines = [];
                return false;
            }

            #endregion

            #region DIR

            if (!GetDirFromComboBox(this.ComboBox_Direction_0, out dir_0))
            {
                string text = "Failed to parse direction for line index 0";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                lines = [];
                return false;
            }

            if (!GetDirFromComboBox(this.ComboBox_Direction_1, out dir_1))
            {
                string text = "Failed to parse direction for line index 1";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                lines = [];
                return false;
            }

            if (!GetDirFromComboBox(this.ComboBox_Direction_2, out dir_2))
            {
                string text = "Failed to parse direction for line index 2";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                lines = [];
                return false;
            }

            if (!GetDirFromComboBox(this.ComboBox_Direction_3, out dir_3))
            {
                string text = "Failed to parse direction for line index 3";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                lines = [];
                return false;
            }

            #endregion

            lines = [
                new(0, dir_0, texCount_0),
                new(1, dir_1, texCount_1),
                new(2, dir_2, texCount_2),
                new(3, dir_3, texCount_3),
                ];

            return true;
        }

        private bool GetDirFromComboBox(ComboBox comboBox, out Direction dir)
        {
            dir = Direction.None;

            ComboBoxItem? item = (ComboBoxItem)comboBox.SelectedItem;

            if (item == null)
                return false;

            int tag = 0;

            if (!int.TryParse(item.Tag.ToString(), out tag))
                return false;

            dir = (Direction)tag;
            return true;
        }

        private bool GetDuration(out int duration)
        {
            duration = -1;

            string input = this.TxtBox_AnimationDuration.Text;

            if (input.Length == 0)
                input = "xxx";

            if (!int.TryParse(input, out duration))
            {
                string text = "Failed to parse duration";
                string caption = "DungeonExplorer - GameEditor - AddPlayerTexture";

                _ = MessageBox.Show(text, caption, MessageBoxButton.OK);

                return false;
            }

            return true;
        }
    }
}
