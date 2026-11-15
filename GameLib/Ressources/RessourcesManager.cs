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
        private static TextureBase _textureNotFound = new TextureBase(GameLibConfig.Textures.Path_Img_TextureNotFound);
        private static TileTexture _tileTextureNotFound = new TileTexture(GameLibConfig.Textures.Path_Img_TextureNotFound);

        private static List<TileTexture> _groundTextures = [];
        private static List<TileTexture> _wallTextures = [];
        private static List<TileTexture> _doorTextures = [];

        #region GET_SET

        public static TextureBase TextureNotFound { get { return _textureNotFound; } }
        public static TileTexture TileTextureNotFound { get { return _tileTextureNotFound; } }

        public static List<TileTexture> GroundTextures { get { return _groundTextures; } }
        public static List<TileTexture> WallTextures { get { return _wallTextures; } }
        public static List<TileTexture> DoorTextures { get { return _doorTextures; } }

        #endregion

        public static void Init()
        {
            _groundTextures.Clear();
            _wallTextures.Clear();
            _doorTextures.Clear();

            foreach (string path in Dungeon.TexturesGroundFiles)
                _groundTextures.Add(new TileTexture(path));

            foreach (string path in Dungeon.TexturesWallsFiles)
                _wallTextures.Add(new TileTexture(path));

            foreach (string path in Dungeon.TexturesDoorsFiles)
                _doorTextures.Add(new TileTexture(path));
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

        public static void ApplyTextureToAllTiles(TileTexture texture, TileType type)
        {
            DungeonLevel level = Dungeon.CurrentLevel;

            for (int x = 0; x < level.GridSize.X; ++x)
            {
                for (int y = 0; y < level.GridSize.Y; ++y)
                {
                    level.Tiles[x, y].TileTexture = texture;
                    level.Tiles[x, y].Type = type;
                }
            }

            Dungeon.Levels[Dungeon.CurrentLevelIndex] = level;
        }
    }
}
