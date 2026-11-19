using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib.Ressources
{
    public class PropTexture : TextureBase
    {
        private Vector2f[] _pointsPixelPerfectBorder;

        public Vector2f[] PointsPixelPerfectBorder { get { return _pointsPixelPerfectBorder; } }

        public PropTexture(string path) : base(path)
        {
            _pointsPixelPerfectBorder = RessourcesHelper.GeneratePixelPerfectborderPoints(this.Image).ToArray();
        }

        /// <summary>
        /// Generate an array of points for a specified location on the screen
        /// and a scale factor defined by the ratio between the original and render size of the prop image
        /// </summary>
        /// <param name="renderPosition">Position of the Prop on the screen</param>
        /// <param name="scale">Scale factor of the prop when rendering</param>
        /// <returns></returns>
        public Vector2f[] GetGlobalPointsPixelPerfectBorder(Vector2f renderPosition, Vector2f scale)
        {
            Vector2f[] outArray = new Vector2f[_pointsPixelPerfectBorder.Length];

            for (int i = 0; i < _pointsPixelPerfectBorder.Length; ++i)
            {
                outArray[i] = new()
                {
                    X = (_pointsPixelPerfectBorder[i].X * scale.X) + renderPosition.X,
                    Y = (_pointsPixelPerfectBorder[i].Y * scale.Y) + renderPosition.Y
                };
            }

            return outArray;
        }
    }
}
