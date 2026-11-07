using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Logique d'interaction pour WindowLauncher.xaml
    /// </summary>
    public partial class WindowLauncher : Window
    {
        private string? _filename = null;

        public WindowLauncher()
        {
            InitializeComponent();
        }

        private void Btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                InitialDirectory = GameEditorConfig.Path_Base_Projects_Dir,
                Filter = $"Dungeon file (*.{GameLib.GameLibConfig.Global.Ext_DungeonBaseFile}) | *.{GameLib.GameLibConfig.Global.Ext_DungeonBaseFile}",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _filename = dialog.FileName;
                this.TxtBox_FilePath.Text = _filename;
                this.Btn_LoadDungeon.IsEnabled = true;
            }
            else
            {
                _filename = null;
                this.TxtBox_FilePath.Text = "";
                this.Btn_LoadDungeon.IsEnabled = false;
            }

        }

        private void Btn_LoadDungeon_Click(object sender, RoutedEventArgs e)
        {
            if (_filename == null)
                throw new Exception();

            WindowWorldEditor windowWorldEditor = new(_filename);

            windowWorldEditor.Show();

            this.Close();
        }

        private void Btn_CreateNewDungeon_Click(object sender, RoutedEventArgs e)
        {
            WindowCreateDungeon window = new();

            bool? result = window.ShowDialog();

            if (result == null || result == false)
            {
                return;
            }

            string path = GameLib.Files.FilesManager.CreateNewProject(
                dungeonName: GameEditorConfig.DungeonCreation.ProjectName,
                dirMain: GameEditorConfig.DungeonCreation.ProjectDir,
                gridSize: new(GameEditorConfig.DungeonCreation.Level_X, GameEditorConfig.DungeonCreation.Level_Y),
                tileRenderSize: new(64.0f, 64.0f)
                );


            WindowWorldEditor windowWorldEditor = new(path);

            windowWorldEditor.Show();

            this.Close();
        }
    }
}
