using System;
using System.Collections.Generic;
using System.Text;

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

            DungeonLevel firstLevel = new DungeonLevel(gridSize, tileRenderSize);

            _levels.Add(firstLevel);

            return outSuccess;
        }

        public static bool LoadFromFile(string path)
        {
            return false;
        }
    }
}
