using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLib.Files;
using GameLib.Files.SaveFiles;
using GameLib.World.Tiles;
using SFML.System;

namespace GameLib.World
{
    public static class Dungeon
    {
        #region ATTTRIBUTES

        private static List<DungeonLevel> _levels = new List<DungeonLevel>();
        private static int _currentLevelIndex = 0;

        private static List<string> _texturesDoorsFiles = [];
        private static List<string> _texturesEntitiesFiles = [];
        private static List<string> _texturesGroundFiles = [];
        private static List<string> _texturesPlayerFiles = [];
        private static List<string> _texturesPropsFiles = [];
        private static List<string> _texturesWallsFiles = [];


        #endregion

        #region GET_SET

        public static List<DungeonLevel> Levels { get { return _levels; } }
        public static DungeonLevel CurrentLevel { get { return _levels[CurrentLevelIndex]; } }
        public static int CurrentLevelIndex { get { return _currentLevelIndex; } }

        public static List<string> TexturesDoorsFiles { get { return _texturesDoorsFiles; } set { _texturesDoorsFiles = value; } }
        public static List<string> TexturesEntitiesFiles { get { return _texturesEntitiesFiles; } set { _texturesEntitiesFiles = value; } }
        public static List<string> TexturesGroundFiles { get { return _texturesGroundFiles; } set { _texturesGroundFiles = value; } }
        public static List<string> TexturesPlayerFiles { get { return _texturesPlayerFiles; } set { _texturesPlayerFiles = value; } }
        public static List<string> TexturesPropsFiles { get { return _texturesPropsFiles; } set { _texturesPropsFiles = value; } }
        public static List<string> TexturesWallsFiles { get { return _texturesWallsFiles; } set { _texturesWallsFiles = value; } }

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

            foreach (string levelPath in dungeonSave.Levels)
            {
                LevelSave levelSave = FilesManager.LoadLevelSave(levelPath);

                DungeonLevel level = new(levelSave);

                _levels.Add(level);
            }

            _texturesDoorsFiles = dungeonSave.RessourcesDoors.ToList();
            _texturesEntitiesFiles = dungeonSave.RessourcesEntities.ToList();
            _texturesGroundFiles = dungeonSave.RessourcesGrounds.ToList();
            _texturesPlayerFiles = dungeonSave.RessourcesPlayer.ToList();
            _texturesPropsFiles = dungeonSave.RessourcesProps.ToList();
            _texturesWallsFiles = dungeonSave.RessourcesWalls.ToList();

            DungeonLevel firstLevel = _levels.OrderBy(x => x.Index).First();

            for (int i = 0; i < _levels.Count; ++i)
            {
                if (_levels[i].ID == firstLevel.ID)
                {
                    _currentLevelIndex = i;
                    break;
                }
            }


            return;
        }

        public static Tile GetTile(Vector2f screenCoords)
        {
            throw new NotImplementedException();
        }

        public static Tile GetTile(Vector2i gridIndex)
        {
            throw new NotImplementedException();
        }

        public static void AddTileTextureFile(string file, TileType type)
        {
            switch (type)
            {
                case TileType.Ground:
                    {
                        _texturesGroundFiles.Add(file);
                        break;
                    }
                case TileType.Wall:
                    {
                        _texturesWallsFiles.Add(file);
                        break;
                    }
                case TileType.Door:
                    {
                        _texturesDoorsFiles.Add(file);
                        break;
                    }
            }
        }
    }
}
