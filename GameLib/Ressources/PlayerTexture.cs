using GameLib.Entities;
using GameLib.Files.SaveFiles;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GameLib.Ressources
{
    public class PlayerTexture : TextureBase
    {
        private Entities.Action _action;
        private PlayerTextureLine[] _textureLines;
        private Vector2i _tileSize;

        #region GET_SET

        public Entities.Action Action { get { return _action; } }
        public PlayerTextureLine[] Lines { get { return _textureLines; } }
        public Vector2i TileSize { get { return _tileSize; } }

        #endregion

        /// <summary>
        /// Construct the texture from a file
        /// </summary>
        /// <param name="path">source file of the texture (.png)</param>
        /// <param name="action">action represented by the texture</param>
        /// <param name="lines">description of the lines of the texture</param>
        public PlayerTexture(string path, Entities.Action action, PlayerTextureLine[] lines) : base(path)
        {
            _action = action;
            _textureLines = lines;

            int nbCol = _textureLines.OrderBy(x => x.TexturesCount).Last().TexturesCount;
            int nbLines = _textureLines.Length;

            _tileSize = new()
            {
                X = (int)this.Image.Size.X / nbCol,
                Y = (int)this.Image.Size.Y / nbLines
            };
        }

        public PlayerTexture(PlayerTextureSave save) : base(save.File)
        {
            _action = save.Action;
            _tileSize = new(save.TileSize_X, save.TileSize_Y);

            _textureLines = new PlayerTextureLine[save.Lines.Length];

            for (int i = 0; i < save.Lines.Length; ++i)
            {
                _textureLines[i] = new PlayerTextureLine(save.Lines[i]);
            }
        }
    }

    /// <summary>
    /// Describe the lines of the spritesheet
    /// </summary>
    public struct PlayerTextureLine
    {
        public int LineIndex { get; }
        public Direction Direction { get; }
        public int TexturesCount { get; }

        public PlayerTextureLine(int lineIndex, Direction direction, int texturesCount)
        {
            LineIndex = lineIndex;
            Direction = direction;
            TexturesCount = texturesCount;
        }

        public PlayerTextureLine(PlayerTextureLineSave save)
        {
            LineIndex = save.LineIndex;
            Direction = save.Direction;
            TexturesCount = save.TexturesCount;
        }
    }
}
