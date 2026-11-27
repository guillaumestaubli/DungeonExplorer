using GameLib.Files;
using GameLib.Files.SaveFiles;
using GameLib.World;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using static GameLib.Files.FilesManager;

namespace GameLib
{
    public static class ProjectManager
    {
        public static string CreateNewProject(string worldName, string dirMain, Vector2i gridSize, Vector2f tileRenderSize)
        {
            FilesManagerConfig.Init(dirMain, worldName);

            _ = IOHelper.CreateDirectory(FilesManagerConfig.PathDirMain);
            _ = IOHelper.CreateDirectory(FilesManagerConfig.PathDirLevels);
            _ = IOHelper.CreateDirectory(FilesManagerConfig.PathDirRessources);
            _ = IOHelper.CreateDirectory(FilesManagerConfig.PathDirResDoors);
            _ = IOHelper.CreateDirectory(FilesManagerConfig.PathDirResEntities);
            _ = IOHelper.CreateDirectory(FilesManagerConfig.PathDirResGrounds);
            _ = IOHelper.CreateDirectory(FilesManagerConfig.PathDirResPlayer);
            _ = IOHelper.CreateDirectory(FilesManagerConfig.PathDirResProps);
            _ = IOHelper.CreateDirectory(FilesManagerConfig.PathDirResWalls);

            _ = IOHelper.CreateFile(FilesManagerConfig.PathFileWorld);

            World.World.CreateNewWorld(gridSize, tileRenderSize);

            WorldSave worldSave = new()
            {
                PathLevels = FilesManagerConfig.PathDirLevels,
                PathFirstLevel = FilesManagerConfig.PathDirLevels + "\\" + "level_0.json",
                LevelCount = 1,
                Levels = [FilesManagerConfig.PathDirLevels + "\\" + "level_0.json"],
                PathRessources = FilesManagerConfig.PathDirRessources,
                RessourcesDoors = [],
                RessourcesEntities = [],
                RessourcesGrounds = [],
                RessourcesPlayer = [],
                RessourcesProps = [],
                RessourcesWalls = []
            };

            string worldSaveJson = Newtonsoft.Json.JsonConvert.SerializeObject(worldSave, Newtonsoft.Json.Formatting.Indented);

            _ = IOHelper.WriteToFile(FilesManagerConfig.PathFileWorld, worldSaveJson, false, Encoding.UTF8);

            LevelSave levelSave = new(World.World.CurrentLevel);

            string levelSaveJson = Newtonsoft.Json.JsonConvert.SerializeObject(levelSave, Newtonsoft.Json.Formatting.Indented);

            _ = IOHelper.CreateFile(worldSave.PathFirstLevel);
            _ = IOHelper.WriteToFile(worldSave.PathFirstLevel, levelSaveJson, false, Encoding.UTF8);

            return FilesManagerConfig.PathFileWorld;
        }

        public static void SaveProject()
        {
            WorldSave worldSave = LoadWorldSave(FilesManagerConfig.PathFileWorld);

            worldSave.LevelCount = World.World.Levels.Count;

            worldSave.RessourcesDoors = World.World.TexturesDoorsFiles.ToArray();
            worldSave.RessourcesEntities = World.World.TexturesEntitiesFiles.ToArray();
            worldSave.RessourcesGrounds = World.World.TexturesGroundFiles.ToArray();
            worldSave.RessourcesPlayer = World.World.TexturesPlayerFiles.ToArray();
            worldSave.RessourcesProps = World.World.TexturesPropsFiles.ToArray();
            worldSave.RessourcesWalls = World.World.TexturesWallsFiles.ToArray();

            string worldSaveJson = Newtonsoft.Json.JsonConvert.SerializeObject(worldSave, Newtonsoft.Json.Formatting.Indented);

            _ = IOHelper.WriteToFile(FilesManagerConfig.PathFileWorld, worldSaveJson, false, Encoding.UTF8);

            foreach (Level level in World.World.Levels)
            {
                LevelSave levelSave = new LevelSave(level);

                string levelSaveJson = Newtonsoft.Json.JsonConvert.SerializeObject(levelSave, Newtonsoft.Json.Formatting.Indented);

                _ = IOHelper.WriteToFile(worldSave.PathFirstLevel, levelSaveJson, false, Encoding.UTF8);
            }
        }
    }
}
