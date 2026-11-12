using GameLib.Files.SaveFiles;
using GameLib.Logging;
using GameLib.World;
using GameLib.World.Tiles;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace GameLib.Files
{
    public static class FilesManager
    {
        public static class Config
        {
            private const string DIR_LEVELS_NAME = "Levels";
            private const string DIR_RESSOURCES_NAME = "Ressources";
            private const string DIR_RESSOURCES_DOORS_NAME = "Doors";
            private const string DIR_RESSOURCES_ENTITIES_NAME = "Entities";
            private const string DIR_RESSOURCES_GROUNDS_NAME = "Grounds";
            private const string DIR_RESSOURCES_PLAYER_NAME = "Player";
            private const string DIR_RESSOURCES_PROPS_NAME = "Props";
            private const string DIR_RESSOURCES_WALLS_NAME = "Walls";

            private static string _dirMain = "";
            private static string _dungeonName = "";

            private static string _pathDirLevels = "";
            private static string _pathDirRessources = "";
            private static string _pathDirResDoors = "";
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

                _pathFileDungeon = $@"{_dirMain}\{_dungeonName}.{GameLibConfig.Global.Ext_DungeonBaseFile}";

                InitSubDirectories();
            }

            internal static void Init(string dungeonFile)
            {
                string[] pathArray = dungeonFile.Split('\\');

                string dirMain = "";

                for(int i = 0; i < pathArray.Length - 1; ++i)
                {
                    dirMain += $@"{pathArray[i]}\";
                }

                _dirMain = dirMain.TrimEnd('\\');

                InitSubDirectories();

                _pathFileDungeon = dungeonFile;
            }

            private static void InitSubDirectories()
            {
                _pathDirLevels = $@"{_dirMain}\{DIR_LEVELS_NAME}";
                _pathDirRessources = $@"{_dirMain}\{DIR_RESSOURCES_NAME}";
                _pathDirResDoors = $@"{_dirMain}\{DIR_RESSOURCES_NAME}\{DIR_RESSOURCES_DOORS_NAME}";
                _pathDirResEntities = $@"{_dirMain}\{DIR_RESSOURCES_NAME}\{DIR_RESSOURCES_ENTITIES_NAME}";
                _pathDirResGrounds = $@"{_dirMain}\{DIR_RESSOURCES_NAME}\{DIR_RESSOURCES_GROUNDS_NAME}";
                _pathDirResPlayer = $@"{_dirMain}\{DIR_RESSOURCES_NAME}\{DIR_RESSOURCES_PLAYER_NAME}";
                _pathDirResProps = $@"{_dirMain}\{DIR_RESSOURCES_NAME}\{DIR_RESSOURCES_PROPS_NAME}";
                _pathDirResWalls = $@"{_dirMain}\{DIR_RESSOURCES_NAME}\{DIR_RESSOURCES_WALLS_NAME}";
            }

            #region GET_SET

            public static string DungeonName { get { return _dungeonName; } }
            public static string PathDirMain { get { return _dirMain; } }
            public static string PathDirLevels { get { return _pathDirLevels; } }
            public static string PathDirRessources { get { return _pathDirRessources; } }
            public static string PathDirResDoors { get { return _pathDirResDoors; } }
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
            _ = IOHelper.CreateDirectory(Config.PathDirResDoors);
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
                RessourcesDoors = [],
                RessourcesEntities = [],
                RessourcesGrounds = [],
                RessourcesPlayer = [],
                RessourcesProps = [],
                RessourcesWalls = []
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
            Config.Init(path);

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

        public static void SaveProject()
        {
            DungeonSave dungeonSave = LoadDungeonSave(Config.PathFileDungeon);

            dungeonSave.LevelCount = Dungeon.Levels.Count;

            dungeonSave.RessourcesDoors = Dungeon.TexturesDoorsFiles.ToArray();
            dungeonSave.RessourcesEntities = Dungeon.TexturesEntitiesFiles.ToArray();
            dungeonSave.RessourcesGrounds = Dungeon.TexturesGroundFiles.ToArray();
            dungeonSave.RessourcesPlayer = Dungeon.TexturesPlayerFiles.ToArray();
            dungeonSave.RessourcesProps = Dungeon.TexturesPropsFiles.ToArray();
            dungeonSave.RessourcesWalls = Dungeon.TexturesWallsFiles.ToArray();

            string dungeonSaveJson = Newtonsoft.Json.JsonConvert.SerializeObject(dungeonSave, Newtonsoft.Json.Formatting.Indented);

            _ = IOHelper.WriteToFile(Config.PathFileDungeon, dungeonSaveJson, false, Encoding.UTF8);

            foreach(DungeonLevel level in Dungeon.Levels)
            {
                LevelSave levelSave = new LevelSave(level);

                string levelSaveJson = Newtonsoft.Json.JsonConvert.SerializeObject(levelSave, Newtonsoft.Json.Formatting.Indented);

                _ = IOHelper.WriteToFile(dungeonSave.PathFirstLevel, levelSaveJson, false, Encoding.UTF8);
            }
        }


        public static IOResult CopyTileTextureInProject(string sourceFileName, TileType type)
        {
            FileInfo origin = new FileInfo(sourceFileName);

            if (!origin.Exists)
                return IOResult.DONT_EXISTS;

            string destFilename = string.Empty;

            switch (type)
            {
                case TileType.Door:
                    {
                        destFilename = Config.PathDirResDoors;
                        break;
                    }
                case TileType.Ground:
                    {
                        destFilename = Config.PathDirResGrounds;
                        break;
                    }
                case TileType.Wall:
                    {
                        destFilename = Config.PathDirResWalls;
                        break;
                    }
                
            }

            DirectoryInfo destDir = new DirectoryInfo(destFilename);

            bool exists = destDir.EnumerateFiles().Where(x => x.Name == origin.Name).Count() > 0;

            if (exists)
                return IOResult.ALREADY_EXISTS;

            destFilename += "\\";
            destFilename += sourceFileName.Split('\\').Last();

            IOResult ior = IOHelper.CopyFile(sourceFileName, destFilename);

            if (ior == IOResult.OK)
                Dungeon.AddTileTextureFile(destFilename, type);

            return ior;
        }

    }
}
