using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLib.Files;
using GameLib.Files.SaveFiles;
using SFML.System;

namespace GameLib.World
{
    public static class Dungeon
    {
        #region ATTTRIBUTES

        private static List<DungeonLevel> _levels = new List<DungeonLevel>();
        private static int _currentLevelIndex = 0;

        #endregion

        #region GET_SET

        public static List<DungeonLevel> Levels { get { return _levels; } }
        public static DungeonLevel CurrentLevel { get { return _levels[CurrentLevelIndex]; } }
        public static int CurrentLevelIndex { get { return _currentLevelIndex; } }

        #endregion

        public static bool CreateNewDungeon(Vector2i gridSize, Vector2f tileRenderSize)
        {
            bool outSuccess = true;

            DungeonLevel firstLevel = new DungeonLevel(0, gridSize, tileRenderSize, "level_0");

            _levels.Add(firstLevel);

            return outSuccess;
        }

        public static void LoadFromFile(string path)
        {
            DungeonSave dungeonSave = FilesManager.LoadDungeonSave(path);

            foreach(string levelPath in dungeonSave.Levels)
            {
                LevelSave levelSave = FilesManager.LoadLevelSave(levelPath);

                DungeonLevel level = new(levelSave);

                _levels.Add(level);
            }

            DungeonLevel firstLevel = _levels.OrderBy(x => x.Index).First();

            for(int i = 0; i < _levels.Count; ++i)
            {
                if (_levels[i].ID == firstLevel.ID)
                {
                    _currentLevelIndex = i;
                    break;
                }
            }


            return;
        }
    }
}
