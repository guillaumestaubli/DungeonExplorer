using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLib.Core;
using GameLib.Entities;
using GameLib.Files;
using GameLib.Files.SaveFiles;
using GameLib.Logging;
using GameLib.Ressources;
using GameLib.Ressources.Textures;
using GameLib.World.Props;
using GameLib.World.Tiles;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace GameLib.World
{
    public static class World
    {
        #region ATTTRIBUTES

        private static List<Level> _levels = new List<Level>();
        private static int _currentLevelIndex = 0;
        private static Player _player = new();

        private static List<string> _texturesDoorsFiles = [];
        private static List<string> _texturesEntitiesFiles = [];
        private static List<string> _texturesGroundFiles = [];
        private static List<string> _texturesPlayerFiles = [];
        private static List<string> _texturesPropsFiles = [];
        private static List<string> _texturesWallsFiles = [];

        #endregion

        #region GET_SET

        public static List<Level> Levels { get { return _levels; } }
        public static Level CurrentLevel { get { return _levels[CurrentLevelIndex]; } }
        public static int CurrentLevelIndex { get { return _currentLevelIndex; } }
        public static Player Player { get { return _player; } }

        public static List<string> TexturesDoorsFiles { get { return _texturesDoorsFiles; } set { _texturesDoorsFiles = value; } }
        public static List<string> TexturesEntitiesFiles { get { return _texturesEntitiesFiles; } set { _texturesEntitiesFiles = value; } }
        public static List<string> TexturesGroundFiles { get { return _texturesGroundFiles; } set { _texturesGroundFiles = value; } }
        public static List<string> TexturesPlayerFiles { get { return _texturesPlayerFiles; } set { _texturesPlayerFiles = value; } }
        public static List<string> TexturesPropsFiles { get { return _texturesPropsFiles; } set { _texturesPropsFiles = value; } }
        public static List<string> TexturesWallsFiles { get { return _texturesWallsFiles; } set { _texturesWallsFiles = value; } }

        #endregion

        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="gridSize"></param>
        /// <param name="tileRenderSize"></param>
        /// <returns></returns>
        public static bool CreateNewWorld(Vector2i gridSize, Vector2f tileRenderSize)
        {
            bool outSuccess = true;

            Level firstLevel = new Level(0, gridSize, tileRenderSize, "level_0");

            _levels.Add(firstLevel);

            return outSuccess;
        }

        /// <summary>
        /// Load a project from a file (.world)
        /// </summary>
        /// <param name="path">path of the .world file</param>
        public static void LoadFromFile(string path)
        {
            _levels.Clear();

            WorldSave worldSave = FilesManager.LoadWorldSave(path);

            // Load textures
            _texturesDoorsFiles = worldSave.RessourcesDoors.ToList();
            _texturesEntitiesFiles = worldSave.RessourcesEntities.ToList();
            _texturesGroundFiles = worldSave.RessourcesGrounds.ToList();
            _texturesPlayerFiles = worldSave.RessourcesPlayer.ToList();
            _texturesPropsFiles = worldSave.RessourcesProps.ToList();
            _texturesWallsFiles = worldSave.RessourcesWalls.ToList();

            RessourcesManager.Init();

            // load levels
            foreach (string levelPath in worldSave.Levels)
            {
                LevelSave levelSave = FilesManager.LoadLevelSave(levelPath);

                Level level = new(levelSave);

                _levels.Add(level);
            }

            LevelSave firstLevelSave = FilesManager.LoadLevelSave(worldSave.PathFirstLevel);

            Level? firstLevel = _levels.Where(x => x.ID.ToString() == firstLevelSave.ID).FirstOrDefault();

            if (firstLevel == null)
                _currentLevelIndex = 0;
            else
            {
                for (int i = 0; i < _levels.Count; ++i)
                {
                    if (_levels[i].ID == firstLevel.ID)
                    {
                        _currentLevelIndex = i;
                        break;
                    }
                }
            }

            DEV_CreatePlayer();

            return;
        }

        private static void DEV_CreatePlayer()
        {
            Vector2i gridIndex = new(5, 5);

            Vector2f position = new()
            {
                X = gridIndex.X * _levels[_currentLevelIndex].TileRenderSize.X,
                Y = gridIndex.Y * _levels[_currentLevelIndex].TileRenderSize.Y
            };

            Vector2f size = _levels[_currentLevelIndex].TileRenderSize;

            Dictionary<Entities.Action, PlayerTexture> textures = new();

            foreach (PlayerTexture texture in RessourcesManager.PlayerTextures)
            {
                textures.Add(texture.Action, texture);
            }

            _player = new(position, size, textures);

        }

        #region GAME_EDITOR

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

        public static void AddPropTextureFile(string file)
        {
            _texturesPropsFiles.Add(file);
        }

        public static void AddPlayerTextureFile(string imgFile, string jsonFile)
        {
            _texturesPlayerFiles.Add(imgFile);
            _texturesPlayerFiles.Add(jsonFile);
        }


        public static void RemoveTileTextureFile(TileTexture texture, TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Ground:
                    {
                        _texturesGroundFiles = _texturesGroundFiles.Where(x => x != texture.Path).ToList();
                        break;
                    }
                case TileType.Wall:
                    {
                        _texturesWallsFiles = _texturesWallsFiles.Where(x => x != texture.Path).ToList();
                        break;
                    }
                case TileType.Door:
                    {
                        _texturesDoorsFiles = _texturesDoorsFiles.Where(x => x != texture.Path).ToList();
                        break;
                    }
            }
        }

        public static void RemovePropTextureFile(PropTexture texture)
        {
            _texturesPropsFiles = _texturesPropsFiles.Where(x => x != texture.Path).ToList();
        }

        public static void RemovePlayerTextureFile(PlayerTexture texture)
        {
            string imgFile = texture.Path;
            string jsonFile = texture.Path.Split('.').First() + ".json";

            _texturesPlayerFiles = _texturesPlayerFiles.Where(x => x != imgFile).ToList();
            _texturesPlayerFiles = _texturesPlayerFiles.Where(x => x != jsonFile).ToList();
        }


        /// <summary>
        /// Convert screen/world coordinates to grid index for the current level
        /// </summary>
        /// <param name="screenCoords"></param>
        /// <returns></returns>
        public static Vector2i ConvertWorldCoordsToGridIndex(Vector2f screenCoords)
        {
            int x = (int)(screenCoords.X / _levels[_currentLevelIndex].TileRenderSize.X);
            int y = (int)(screenCoords.Y / _levels[_currentLevelIndex].TileRenderSize.Y);

            return new(x, y);
        }

        /// <summary>
        /// Get a tile from the current level from grid coordinates
        /// </summary>
        /// <param name="gridIndex"></param>
        /// <returns></returns>
        public static Tile GetTile(Vector2i gridIndex)
        {
            return _levels[_currentLevelIndex].GetTile(gridIndex);
        }

        /// <summary>
        /// Get a tile from the current level from screen coordinates
        /// </summary>
        /// <param name="screenCoords"></param>
        /// <returns></returns>
        public static Tile GetTile(Vector2f screenCoords)
        {
            Vector2i index = ConvertWorldCoordsToGridIndex(screenCoords);

            return GetTile(index);
        }

        /// <summary>
        /// Update the specified tile in the current level
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="gridIndex"></param>
        public static void UpdateTile(Tile tile, Vector2i gridIndex)
        {
            Level level = _levels[_currentLevelIndex];

            level.UpdateTile(tile, gridIndex);

            _levels[_currentLevelIndex] = level;
        }

        /// <summary>
        /// Add a Prop in the current level
        /// </summary>
        /// <param name="prop"></param>
        public static void AddProp(Prop prop)
        {
            Level level = _levels[_currentLevelIndex];

            level.Props.Add(prop);

            _levels[_currentLevelIndex] = level;
        }

        public static Prop? GetProp(Vector2i gridIndex)
        {
            foreach (Prop prop in _levels[_currentLevelIndex].Props)
            {
                if (prop.GridIndex == gridIndex)
                    return prop;
            }

            return null;
        }

        public static List<Prop> GetProps(Vector2i gridIndex)
        {
            List<Prop> props = [];

            foreach (Prop prop in _levels[_currentLevelIndex].Props)
            {
                if (prop.GridIndex == gridIndex)
                    props.Add(prop);
            }

            return props;
        }

        #endregion

        #region RENDERING

        /// <summary>
        /// Update the drawables of the game needed to draw the frame on the screen
        /// </summary>
        /// <param name="elapsedMs"></param>
        public static void RenderFrame(long elapsedMs)
        {
            _player.UpdateDrawables(elapsedMs);
        }

        #endregion


        /// <summary>
        /// Handle the player movements based on the keyboard inputs
        /// </summary>
        public static void MovePlayer()
        {
            bool northPressed = Keyboard.IsKeyPressed(GameLibConfig.Controls.KeyMoveNorth);
            bool southPressed = Keyboard.IsKeyPressed(GameLibConfig.Controls.KeyMoveSouth);
            bool westPressed = Keyboard.IsKeyPressed(GameLibConfig.Controls.KeyMoveWest);
            bool eastPressed = Keyboard.IsKeyPressed(GameLibConfig.Controls.KeyMoveEast);

            bool sprintPressed = Keyboard.IsKeyPressed(GameLibConfig.Controls.KeyMoveSprint);

            if (!northPressed && !southPressed && !westPressed && !eastPressed)
            {
                if (_player.Action != Entities.Action.Idle)
                    _player.SetAction(Entities.Action.Idle);

                return;
            }

            float delta = GameLibConfig.Controls.MoveOffsetPerFrame;

            if (sprintPressed)
                delta *= GameLibConfig.Controls.SprintRatio;

            Vector2f offset = new(0.0f, 0.0f);

            if (northPressed)
                offset.Y -= delta;

            if (southPressed)
                offset.Y += delta;

            if (westPressed)
                offset.X -= delta;

            if (eastPressed)
                offset.X += delta;

            offset = CollisionHelper.DetectAndCorrectPlayerCollisionsWhenMoving(offset);

            Direction direction = GetGlobalPlayerDirection(offset);

            _player.Move(offset, direction);

            if (sprintPressed)
                _player.SetAction(Entities.Action.Run);
            else
                _player.SetAction(Entities.Action.Walk);
        }

        /// <summary>
        /// Get the global direction depending on the movement
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static Direction GetGlobalPlayerDirection(Vector2f offset)
        {
            Direction direction = Direction.None;

            if (offset.Y >= 0) // => South
            {
                if (offset.X >= 0) // => South East
                {
                    if (Math.Abs(offset.Y) >= Math.Abs(offset.X))
                    {
                        direction = Direction.South;
                    }
                    else
                    {
                        direction = Direction.East;
                    }
                }
                else // => South West
                {
                    if (Math.Abs(offset.Y) >= Math.Abs(offset.X))
                    {
                        direction = Direction.South;
                    }
                    else
                    {
                        direction = Direction.West;
                    }
                }
            }
            else // => North
            {
                if (offset.X >= 0) // => North East
                {
                    if (Math.Abs(offset.Y) >= Math.Abs(offset.X))
                    {
                        direction = Direction.North;
                    }
                    else
                    {
                        direction = Direction.East;
                    }
                }
                else // => North West
                {
                    if (Math.Abs(offset.Y) >= Math.Abs(offset.X))
                    {
                        direction = Direction.North;
                    }
                    else
                    {
                        direction = Direction.West;
                    }
                }
            }

            return direction;
        }
    }
}
