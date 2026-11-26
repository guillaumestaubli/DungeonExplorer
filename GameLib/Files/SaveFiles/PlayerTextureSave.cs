using GameLib.Entities;
using GameLib.Ressources;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Files.SaveFiles
{
    public class PlayerTextureSave
    {
        public string File { get; set; } = "";
        public Entities.Action Action { get; set; } = Entities.Action.None;
        public int Duration { get; set; } = 1000;
        public int TileSize_X { get; set; } = 0;
        public int TileSize_Y { get; set; } = 0;
        public PlayerTextureLineSave[] Lines { get; set; } = Array.Empty<PlayerTextureLineSave>();

        public PlayerTextureSave()
        {

        }

        public PlayerTextureSave(PlayerTexture texture)
        {
            this.File = texture.Path;
            this.Action = texture.Action;
            this.Duration = texture.AnimationDurationMs;
            this.TileSize_X = texture.TileSize.X;
            this.TileSize_Y = texture.TileSize.Y;

            this.Lines = new PlayerTextureLineSave[texture.Lines.Length];

            for (int i = 0; i < texture.Lines.Length; ++i)
            {
                this.Lines[i] = new PlayerTextureLineSave(texture.Lines[i]);
            }
        }
    }

    public class PlayerTextureLineSave
    {
        public int LineIndex { get; set; } = 0;
        public Direction Direction { get; set; } = Direction.None;
        public int TexturesCount { get; set; } = 0;

        public PlayerTextureLineSave()
        {

        }

        public PlayerTextureLineSave(PlayerTextureLine line)
        {
            this.LineIndex = line.LineIndex;
            this.Direction = line.Direction;
            this.TexturesCount = line.TexturesCount;
        }
    }
}
