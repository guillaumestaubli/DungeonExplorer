using GameEditor.Windows.WorldEditor;
using GameLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

            this.Loaded += WindowLauncher_Loaded;
        }

        private void WindowLauncher_Loaded(object sender, RoutedEventArgs e)
        {
            _filename = null;

            DirectoryInfo directoryInfo = new DirectoryInfo(GameEditorConfig.Path_Base_Projects_Dir);

            foreach(DirectoryInfo subDir in directoryInfo.GetDirectories())
            {
                foreach(FileInfo file in subDir.GetFiles())
                {
                    if(file.Name.Split('.').Last() == "world")
                    {
                        _filename = file.FullName;
                        break;
                    }
                }

                if (_filename != null)
                    break;
            }

            if(_filename != null)
            {
                this.TxtBox_FilePath.Text = _filename;
                this.Btn_LoadWorld.IsEnabled = true;
            }
        }

        private void Btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                InitialDirectory = GameEditorConfig.Path_Base_Projects_Dir,
                Filter = $"World file (*.{GameLib.GameLibConfig.Global.Ext_WorldBaseFile}) | *.{GameLib.GameLibConfig.Global.Ext_WorldBaseFile}",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _filename = dialog.FileName;
                this.TxtBox_FilePath.Text = _filename;
                this.Btn_LoadWorld.IsEnabled = true;
            }
            else
            {
                _filename = null;
                this.TxtBox_FilePath.Text = "";
                this.Btn_LoadWorld.IsEnabled = false;
            }

        }

        private void Btn_LoadWorld_Click(object sender, RoutedEventArgs e)
        {
            if (_filename == null)
                throw new Exception();

            WindowWorldEditor windowWorldEditor = new(_filename);

            windowWorldEditor.Show();

            this.Close();
        }

        private void Btn_CreateNewWorld_Click(object sender, RoutedEventArgs e)
        {
            WindowCreateWorld window = new();

            bool? result = window.ShowDialog();

            if (result == null || result == false)
            {
                return;
            }

            string path = ProjectManager.CreateNewProject(
                worldName: GameEditorConfig.WorldCreation.ProjectName,
                dirMain: GameEditorConfig.WorldCreation.ProjectDir,
                gridSize: new(GameEditorConfig.WorldCreation.Level_X, GameEditorConfig.WorldCreation.Level_Y),
                tileRenderSize: new(64.0f, 64.0f)
                );


            WindowWorldEditor windowWorldEditor = new(path);

            windowWorldEditor.Show();

            this.Close();
        }
    }
}
