using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace GameEditor
{
    public static class GameEditorConfig
    {
        static GameEditorConfig()
        {
            Path_Base_Projects_Dir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Path_Base_Projects_Dir += @"\Documents\DungeonExplorer";

            if (!Directory.Exists(Path_Base_Projects_Dir))
                Directory.CreateDirectory(Path_Base_Projects_Dir);
        }

        public static readonly string Path_Base_Projects_Dir = "";

        

        public static class DungeonCreation
        {
            public static string ProjectName { get; set; } = "";
            public static string ProjectDir { get; set; } = "";
            public static int Level_X { get; set; } = 0;
            public static int Level_Y { get; set; } = 0;
        }
    }
}
