using GameLib.Files.SaveFiles;
using GameLib.Logging;
using GameLib.Ressources;
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
    internal static class FilesManager
    {
        public static class FilesManagerConfig
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
            private static string _worldName = "";

            private static string _pathDirLevels = "";
            private static string _pathDirRessources = "";
            private static string _pathDirResDoors = "";
            private static string _pathDirResEntities = "";
            private static string _pathDirResGrounds = "";
            private static string _pathDirResPlayer = "";
            private static string _pathDirResProps = "";
            private static string _pathDirResWalls = "";

            private static string _pathFileWorld = "";

            internal static void Init(string dirMain, string worldName)
            {
                _dirMain = dirMain;
                _worldName = worldName;

                _pathFileWorld = $@"{_dirMain}\{_worldName}.{GameLibConfig.Global.Ext_WorldBaseFile}";

                InitSubDirectories();
            }

            internal static void Init(string worldFile)
            {
                string[] pathArray = worldFile.Split('\\');

                string dirMain = "";

                for (int i = 0; i < pathArray.Length - 1; ++i)
                {
                    dirMain += $@"{pathArray[i]}\";
                }

                _dirMain = dirMain.TrimEnd('\\');

                InitSubDirectories();

                _pathFileWorld = worldFile;
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

            public static string WorldName { get { return _worldName; } }
            public static string PathDirMain { get { return _dirMain; } }
            public static string PathDirLevels { get { return _pathDirLevels; } }
            public static string PathDirRessources { get { return _pathDirRessources; } }
            public static string PathDirResDoors { get { return _pathDirResDoors; } }
            public static string PathDirResEntities { get { return _pathDirResEntities; } }
            public static string PathDirResGrounds { get { return _pathDirResGrounds; } }
            public static string PathDirResPlayer { get { return _pathDirResPlayer; } }
            public static string PathDirResProps { get { return _pathDirResProps; } }
            public static string PathDirResWalls { get { return _pathDirResWalls; } }

            public static string PathFileWorld { get { return _pathFileWorld; } }

            #endregion

        }

        public static WorldSave LoadWorldSave(string path)
        {
            FilesManagerConfig.Init(path);

            string content = String.Empty;

            IOResult ior = IOHelper.ReadToEnd(path, out content);

            if (ior != IOResult.OK)
                throw new Exception("Failed to read .world file");

            WorldSave? worldSave = Newtonsoft.Json.JsonConvert.DeserializeObject<WorldSave>(content);

            if (worldSave == null)
                throw new Exception("Failed to deserialize .world file content");

            return worldSave;
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

        public static IOResult CopyTileTextureInProject(string sourceFileName, TileType type, out string destFileName)
        {
            destFileName = string.Empty;

            FileInfo origin = new FileInfo(sourceFileName);
            
            if (!origin.Exists)
                return IOResult.DONT_EXISTS;

            destFileName = string.Empty;

            switch (type)
            {
                case TileType.Door:
                    {
                        destFileName = FilesManagerConfig.PathDirResDoors;
                        break;
                    }
                case TileType.Ground:
                    {
                        destFileName = FilesManagerConfig.PathDirResGrounds;
                        break;
                    }
                case TileType.Wall:
                    {
                        destFileName = FilesManagerConfig.PathDirResWalls;
                        break;
                    }

            }

            DirectoryInfo destDir = new DirectoryInfo(destFileName);

            bool exists = destDir.EnumerateFiles().Where(x => x.Name == origin.Name).Count() > 0;

            if (exists)
                return IOResult.ALREADY_EXISTS;

            destFileName += "\\";
            destFileName += sourceFileName.Split('\\').Last();

            IOResult ior = IOHelper.CopyFile(sourceFileName, destFileName);

            return ior;
        }

        public static IOResult CopyPropTextureInProject(string sourceFileName, out string destFileName)
        {
            destFileName = string.Empty;

            FileInfo origin = new FileInfo(sourceFileName);

            if (!origin.Exists)
                return IOResult.DONT_EXISTS;

            destFileName = FilesManagerConfig.PathDirResProps;

            DirectoryInfo destDir = new DirectoryInfo(destFileName);

            bool exists = destDir.EnumerateFiles().Where(x => x.Name == origin.Name).Count() > 0;

            if (exists)
                return IOResult.ALREADY_EXISTS;

            destFileName += "\\";
            destFileName += sourceFileName.Split('\\').Last();

            IOResult ior = IOHelper.CopyFile(sourceFileName, destFileName);

            return ior;
        }

        public static IOResult CopyPlayerTextureImageInProject(string sourceFileName, out string destFilename)
        {
            FileInfo origin = new FileInfo(sourceFileName);
            destFilename = FilesManagerConfig.PathDirResPlayer;

            if (!origin.Exists)
                return IOResult.DONT_EXISTS;

            DirectoryInfo destDir = new DirectoryInfo(destFilename);

            bool exists = destDir.EnumerateFiles().Where(x => x.Name == origin.Name).Count() > 0;

            if (exists)
                return IOResult.ALREADY_EXISTS;

            destFilename += "\\";
            destFilename += sourceFileName.Split('\\').Last();

            IOResult ior = IOHelper.CopyFile(sourceFileName, destFilename);

            return ior;
        }

        public static IOResult CreatePlayerTextureJsonFileInProject(string destFileName, string json)
        {
            FileInfo file = new(destFileName);

            if (file.Exists)
                return IOResult.ALREADY_EXISTS;

            IOResult ior = IOHelper.CreateFile(destFileName);

            if (ior != IOResult.OK)
                return ior;

            ior = IOHelper.WriteToFile(destFileName, json, false, Encoding.UTF8);

            return ior;
        }

        public static IOResult LoadPlayerTexture(string jsonPath, out PlayerTextureSave? save)
        {
            save = null;

            FileInfo file = new(jsonPath);

            if (!file.Exists)
                return IOResult.DONT_EXISTS;

            string content = "";

            IOResult ior = IOHelper.ReadToEnd(jsonPath, out content);

            if (ior != IOResult.OK)
                return ior;

            save = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerTextureSave?>(content);

            if (save == null)
                return IOResult.ERROR;

            return IOResult.OK;
        }

    }
}
