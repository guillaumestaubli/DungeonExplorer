using GameLib.Files.SaveFiles;
using GameLib.Logging;
using GameLib.World;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GameLib.Files
{
    public static class FilesManager
    {
        public static class Config
        {
            public const string DIR_LEVELS_NAME = "Levels";
            public const string DIR_RESSOURCES_NAME = "Ressources";
            public const string DIR_RESSOURCES_ENTITIES_NAME = "Entities";
            public const string DIR_RESSOURCES_GROUNDS_NAME = "Grounds";
            public const string DIR_RESSOURCES_PLAYER_NAME = "Player";
            public const string DIR_RESSOURCES_PROPS_NAME = "Props";
            public const string DIR_RESSOURCES_WALLS_NAME = "Walls";

            private static string _dirMain = "";
            private static string _dungeonName = "";

            private static string _pathDirLevels = "";
            private static string _pathDirRessources = "";
            private static string _pathDirResEntities = "";
            private static string _pathDirResGrounds = "";
            private static string _pathDirResPlayer = "";
            private static string _pathDirResProps = "";
            private static string _pathDirResWalls = "";

            private static string _pathFileDungeon = "";

            internal static void Init(string dirMain, string dungeonName)
            {
                _dirMain = dirMain;
                _dungeonName = dungeonName;

                _pathDirLevels = $@"{_dirMain}\{DIR_LEVELS_NAME}";
                _pathDirRessources = $@"{_dirMain}\{DIR_RESSOURCES_NAME}";
                _pathDirResEntities = $@"{_dirMain}\{DIR_RESSOURCES_NAME}\{DIR_RESSOURCES_ENTITIES_NAME}";
                _pathDirResGrounds = $@"{_dirMain}\{DIR_RESSOURCES_NAME}\{DIR_RESSOURCES_GROUNDS_NAME}";
                _pathDirResPlayer = $@"{_dirMain}\{DIR_RESSOURCES_NAME}\{DIR_RESSOURCES_PLAYER_NAME}";
                _pathDirResProps = $@"{_dirMain}\{DIR_RESSOURCES_NAME}\{DIR_RESSOURCES_PROPS_NAME}";
                _pathDirResWalls = $@"{_dirMain}\{DIR_RESSOURCES_NAME}\{DIR_RESSOURCES_WALLS_NAME}";

                _pathFileDungeon = $@"{_dirMain}\{_dungeonName}.{GameLibConfig.Global.Ext_DungeonBaseFile}";
            }

            #region GET_SET

            public static string DungeonName { get { return _dungeonName; } }
            public static string PathDirMain { get { return _dirMain; } }
            public static string PathDirLevels { get { return _pathDirLevels; } }
            public static string PathDirRessources { get { return _pathDirRessources; } }
            public static string PathDirResEntities { get { return _pathDirResEntities; } }
            public static string PathDirResGrounds { get { return _pathDirResGrounds; } }
            public static string PathDirResPlayer { get { return _pathDirResPlayer; } }
            public static string PathDirResProps { get { return _pathDirResProps; } }
            public static string PathDirResWalls { get { return _pathDirResWalls; } }

            public static string PathFileDungeon { get { return _pathFileDungeon; } }

            #endregion

        }

        public static string CreateNewProject(string dungeonName, string dirMain, Vector2i gridSize, Vector2f tileRenderSize)
        {
            Config.Init(dirMain, dungeonName);

            _ = IOHelper.CreateDirectory(Config.PathDirMain);
            _ = IOHelper.CreateDirectory(Config.PathDirLevels);
            _ = IOHelper.CreateDirectory(Config.PathDirRessources);
            _ = IOHelper.CreateDirectory(Config.PathDirResEntities);
            _ = IOHelper.CreateDirectory(Config.PathDirResGrounds);
            _ = IOHelper.CreateDirectory(Config.PathDirResPlayer);
            _ = IOHelper.CreateDirectory(Config.PathDirResProps);
            _ = IOHelper.CreateDirectory(Config.PathDirResWalls);

            _ = IOHelper.CreateFile(Config.PathFileDungeon);

            Dungeon.CreateNewDungeon(gridSize, tileRenderSize);

            DungeonSave dungeonSave = new()
            {
                PathLevels = Config.PathDirLevels,
                PathFirstLevel = Config.PathDirLevels + "\\" + "level_0.json",
                LevelCount = 1,
                Levels = [Config.PathDirLevels + "\\" + "level_0.json"],
                PathRessources = Config.PathDirRessources,
                RessourcesEntities = ["#"],
                RessourcesGrounds = ["#"],
                RessourcesPlayer = ["#"],
                RessourcesProps = ["#"],
                RessourcesWalls = ["#"]
            };

            string dungeonSaveJson = Newtonsoft.Json.JsonConvert.SerializeObject(dungeonSave, Newtonsoft.Json.Formatting.Indented);

            IOHelper.WriteToFile(Config.PathFileDungeon, dungeonSaveJson, false, Encoding.UTF8);

            LevelSave levelSave = new(Dungeon.CurrentLevel);

            string levelSaveJson = Newtonsoft.Json.JsonConvert.SerializeObject(levelSave, Newtonsoft.Json.Formatting.Indented);

            IOHelper.CreateFile(dungeonSave.PathFirstLevel);
            IOHelper.WriteToFile(dungeonSave.PathFirstLevel, levelSaveJson, false, Encoding.UTF8);

            return Config.PathFileDungeon;
        }

        public static DungeonSave LoadDungeonSave(string path)
        {
            string content = String.Empty;

            IOResult ior = IOHelper.ReadToEnd(path, out content);

            if (ior != IOResult.OK)
                throw new Exception("Failed to read .dungeon file");

            DungeonSave? dungeonSave = Newtonsoft.Json.JsonConvert.DeserializeObject<DungeonSave>(content);

            if (dungeonSave == null)
                throw new Exception("Failed to deserialize .dungeon file content");

            return dungeonSave;
        }

        public static LevelSave LoadLevelSave(string path)
        {
            string content = String.Empty;

            IOResult ior = IOHelper.ReadToEnd(path, out content);

            if (ior != IOResult.OK)
                throw new Exception("Failed to read level file");

            LevelSave? levelSave = Newtonsoft.Json.JsonConvert.DeserializeObject<LevelSave>(content);

            if (levelSave == null)
                throw new Exception("Failed to deserialize level file");

            return levelSave;
        }
    }
}
