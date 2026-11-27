using GameLib.Files;
using GameLib.Files.SaveFiles;
using GameLib.Logging;
using GameLib.World;
using GameLib.World.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib.Ressources
{
    public static class RessourcesManager
    {
        private static TextureBase _textureNotFound = new(GameLibConfig.Textures.Path_Img_TextureNotFound);
        private static TileTexture _tileTextureNotFound = new(GameLibConfig.Textures.Path_Img_TextureNotFound);
        private static PropTexture _propTextureNotFound = new(GameLibConfig.Textures.Path_Img_TextureNotFound);
        private static PlayerTexture _playerTextureNotFound;

        private static List<TileTexture> _groundTextures = [];
        private static List<TileTexture> _wallTextures = [];
        private static List<TileTexture> _doorTextures = [];

        private static List<PropTexture> _propTextures = [];

        private static List<PlayerTexture> _playerTextures = [];

        #region GET_SET

        public static TextureBase TextureNotFound { get { return _textureNotFound; } }
        public static TileTexture TileTextureNotFound { get { return _tileTextureNotFound; } }
        public static PropTexture PropTextureNotFound { get { return _propTextureNotFound; } }
        public static PlayerTexture PlayerTextureNotFound { get { return _playerTextureNotFound; } }

        public static List<TileTexture> GroundTextures { get { return _groundTextures; } }
        public static List<TileTexture> WallTextures { get { return _wallTextures; } }
        public static List<TileTexture> DoorTextures { get { return _doorTextures; } }

        public static List<PropTexture> PropTextures { get { return _propTextures; } }

        public static List<PlayerTexture> PlayerTextures { get { return _playerTextures; } }

        #endregion

        static RessourcesManager()
        {
            _playerTextureNotFound = CreatePlayerTextureNotFound();
        }

        private static PlayerTexture CreatePlayerTextureNotFound()
        {
            string path = GameLibConfig.Textures.Path_Img_TextureNotFound;

            Entities.Action action = Entities.Action.None;

            PlayerTextureLine[] lines = [
                new(0, Entities.Direction.North, 1),
                new(0, Entities.Direction.South, 1),
                new(0, Entities.Direction.West, 1),
                new(0, Entities.Direction.East, 1)
                ];

            int duration = 1000;

            return new(path, action, lines, duration);
        }

        public static void Init()
        {
            ClearTextures();
            LoadTextures();
        }

        private static void ClearTextures()
        {
            _groundTextures.Clear();
            _wallTextures.Clear();
            _doorTextures.Clear();
            _propTextures.Clear();
            _playerTextures.Clear();
        }

        private static void LoadTextures()
        {
            foreach (string path in World.World.TexturesGroundFiles)
                _groundTextures.Add(new TileTexture(path));

            foreach (string path in World.World.TexturesWallsFiles)
                _wallTextures.Add(new TileTexture(path));

            foreach (string path in World.World.TexturesDoorsFiles)
                _doorTextures.Add(new TileTexture(path));

            foreach (string path in World.World.TexturesPropsFiles)
                _propTextures.Add(new PropTexture(path));

            List<string> jsonPlayerTexFiles = World.World.TexturesPlayerFiles.Where(x => x.Split('.').Last() == "json").ToList();

            foreach (string path in jsonPlayerTexFiles)
            {
                PlayerTextureSave? save = null;

                IOResult ior = FilesManager.LoadPlayerTexture(path, out save);

                if (ior != IOResult.OK || save == null)
                {
                    Logger.Error($"{nameof(RessourcesManager)}.{nameof(LoadTextures)}", $"Failed to load player texture with IOResult = {ior}, save == null : {save == null}, path : {path}", null);
                    continue;
                }

                _playerTextures.Add(new(save));
            }

        }

        public static TileTexture GetTileTexture(string name, TileType type)
        {
            TileTexture? texture = null;

            switch (type)
            {
                case TileType.Ground:
                    {
                        texture = _groundTextures.FirstOrDefault(x => x.Name == name);
                        break;
                    }
                case TileType.Wall:
                    {
                        texture = _wallTextures.FirstOrDefault(x => x.Name == name);
                        break;
                    }
                case TileType.Door:
                    {
                        texture = _doorTextures.FirstOrDefault(x => x.Name == name);
                        break;
                    }
            }

            if (texture == null)
            {
                texture = _tileTextureNotFound;
                Logger.Error($"{nameof(RessourcesManager)}.{nameof(GetTileTexture)}", $"Texture with name \"{name}\" and type \"{type}\" was not found", null);
            }

            return texture;
        }

        /// <summary>
        /// Get all the textures for the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>TileTexture[0] if no textures were found for the specified type</returns>
        public static TileTexture[] GetTileTextures(TileType type)
        {
            switch (type)
            {
                case TileType.Ground:
                    return GroundTextures.ToArray();
                case TileType.Wall:
                    return WallTextures.ToArray();
                case TileType.Door:
                    return DoorTextures.ToArray();
                default:
                    return [];
            }
        }

        public static void ApplyTextureToAllTiles(TileTexture texture, TileType type)
        {
            Level level = World.World.CurrentLevel;

            for (int x = 0; x < level.GridSize.X; ++x)
            {
                for (int y = 0; y < level.GridSize.Y; ++y)
                {
                    level.Tiles[x, y].TileTexture = texture;
                    level.Tiles[x, y].Type = type;
                }
            }

            World.World.Levels[World.World.CurrentLevelIndex] = level;
        }

        public static PropTexture GetPropTexture(string name)
        {
            PropTexture? texture = _propTextures.Where(x => x.Name == name).FirstOrDefault();

            if (texture == null)
                return _propTextureNotFound;

            return texture;
        }

        public static PlayerTexture GetPlayerTexture(string name)
        {
            PlayerTexture? texture = _playerTextures.Where(x => x.Name == name).FirstOrDefault();

            if (texture == null)
                return _playerTextureNotFound;

            return texture;


        }
    }
}
