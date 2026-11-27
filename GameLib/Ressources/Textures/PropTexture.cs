using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib.Ressources.Textures
{
    public class PropTexture : TextureBase
    {
        private Vector2f[] _pointsPixelPerfectBorder;

        public Vector2f[] PointsPixelPerfectBorder { get { return _pointsPixelPerfectBorder; } }

        public PropTexture(string path) : base(path)
        {
            _pointsPixelPerfectBorder = PixelPerfectBorderHelper.GeneratePixelPerfectBorderPoints(this.Image).ToArray();
        }
    }
}
