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
            _pointsPixelPerfectBorder = GeneratePixelPerfectborderPoints(this.Image).ToArray();
        }

        /// <summary>
        /// Generate a list of points representing the border of the prop
        /// </summary>
        /// <param name="img">Image representing the prop</param>
        /// <returns></returns>
        private List<Vector2f> GeneratePixelPerfectborderPoints(Image img)
        {
            List<Vector2u> pointsBorder = [];

            List<Vector2u> topPoints = GetTopPoints(img).OrderBy(x => x.X).Distinct().ToList();
            List<Vector2u> rightPoints = GetRightPoints(img).OrderBy(x => x.Y).Distinct().ToList();
            List<Vector2u> bottomPoints = GetBottomPoints(img).OrderByDescending(x => x.X).Distinct().ToList();
            List<Vector2u> leftPoints = GetLeftPoints(img).OrderByDescending(x => x.Y).Distinct().ToList();

            pointsBorder.AddRange(topPoints);
            pointsBorder.AddRange(rightPoints);
            pointsBorder.AddRange(bottomPoints);
            pointsBorder.AddRange(leftPoints);

            pointsBorder = pointsBorder.Distinct().ToList();

            List<Vector2f> outPoints = [];

            foreach (Vector2u point in pointsBorder)
            {
                outPoints.Add((Vector2f)point);
            }

            return outPoints;
        }

        /// <summary>
        /// Check if a pixel is transparent using the alpha component of the image
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns>True if pixel.A < 255</returns>
        private bool IsTransparent(Color pixel)
        {
            return (int)pixel.A < 255;
        }

        /// <summary>
        /// Get the border points from the top of the image
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private List<Vector2u> GetTopPoints(Image img)
        {
            List<Vector2u> pointsBorder = [];

            Vector2u coords = new(0, 0);

            // check each column from top to bottom
            for (coords.X = 0; coords.X < img.Size.X; ++coords.X)
            {
                for (coords.Y = 0; coords.Y < img.Size.Y; ++coords.Y)
                {
                    Color pixel = img.GetPixel(coords);

                    // means the top pixel of the column is the border
                    if (coords.Y == 0 && !IsTransparent(pixel))
                    {
                        pointsBorder.Add(coords);
                        break;
                    }

                    // means no border is present in this column (all the pixels are transparent)
                    if (coords.Y == img.Size.Y - 1 && IsTransparent(pixel))
                        break;

                    // get the adjacent pixel
                    Vector2u nextCoords = (Vector2u)((Vector2i)coords + new Vector2i(0, 1));
                    Color nextPixel = img.GetPixel(nextCoords);

                    // add the adjacent point to the list if this is the border
                    if (!IsTransparent(nextPixel))
                    {
                        pointsBorder.Add(nextCoords);
                        break;
                    }

                }
            }

            return pointsBorder;
        }

        /// <summary>
        /// Get the border points from the bottom of the image
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private List<Vector2u> GetBottomPoints(Image img)
        {
            List<Vector2u> pointsBorder = [];

            Vector2u coords = new(0, 0);

            // check each column from bottom to top
            for (coords.X = 0; coords.X < img.Size.X; ++coords.X)
            {
                for (coords.Y = img.Size.Y - 1; coords.Y >= 0; --coords.Y)
                {
                    Color pixel = img.GetPixel(coords);

                    // means the bottom pixel of the column is the border
                    if (coords.Y == img.Size.Y - 1 && !IsTransparent(pixel))
                    {
                        pointsBorder.Add(coords);
                        break;
                    }

                    // means no border is present in this column (all the pixels are transparent)
                    if (coords.Y == 0 && IsTransparent(pixel))
                        break;

                    // get the adjacent pixel
                    Vector2u nextCoords = (Vector2u)((Vector2i)coords + new Vector2i(0, -1));
                    Color nextPixel = img.GetPixel(nextCoords);

                    // add the adjacent point to the list if this is the border
                    if (!IsTransparent(nextPixel))
                    {
                        pointsBorder.Add(nextCoords);
                        break;
                    }
                }
            }

            return pointsBorder;
        }

        /// <summary>
        /// Get the border points from the left of the image
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private List<Vector2u> GetLeftPoints(Image img)
        {
            List<Vector2u> pointsBorder = [];

            Vector2u coords = new(0, 0);

            // check each line from left to right
            for (coords.Y = 0; coords.Y < img.Size.Y; ++coords.Y)
            {
                for (coords.X = 0; coords.X < img.Size.X; ++coords.X)
                {
                    Color pixel = img.GetPixel(coords);

                    // means the left pixel of the line is the border
                    if (coords.X == 0 && !IsTransparent(pixel))
                    {
                        pointsBorder.Add(coords);
                        break;
                    }

                    // means no border is present in this column (all the pixels are transparent)
                    if (coords.X == img.Size.X - 1 && IsTransparent(pixel))
                        break;

                    // get the adjacent pixel
                    Vector2u nextCoords = (Vector2u)((Vector2i)coords + new Vector2i(1, 0));
                    Color nextPixel = img.GetPixel(nextCoords);

                    // add the adjacent point to the list if this is the border
                    if (!IsTransparent(nextPixel))
                    {
                        pointsBorder.Add(nextCoords);
                        break;
                    }
                }
            }

            return pointsBorder;
        }

        /// <summary>
        /// Get the border points from the right of the image
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private List<Vector2u> GetRightPoints(Image img)
        {
            List<Vector2u> pointsBorder = [];

            Vector2u coords = new(0, 0);

            // check each line from right to left
            for (coords.Y = 0; coords.Y < img.Size.Y; ++coords.Y)
            {
                for (coords.X = img.Size.X - 1; coords.X >= 0; --coords.X)
                {
                    Color pixel = img.GetPixel(coords);

                    // means the right pixel of the line is the border
                    if (coords.X == img.Size.X - 1 && !IsTransparent(pixel))
                    {
                        pointsBorder.Add(coords);
                        break;
                    }

                    // means no border is present in this column (all the pixels are transparent)
                    if (coords.X == 0 && IsTransparent(pixel))
                        break;

                    // get the adjacent pixel
                    Vector2u nextCoords = (Vector2u)((Vector2i)coords + new Vector2i(-1, 0));
                    Color nextPixel = img.GetPixel(nextCoords);

                    // add the adjacent point to the list if this is the border
                    if (!IsTransparent(nextPixel))
                    {
                        pointsBorder.Add(nextCoords);
                        break;
                    }
                }
            }

            return pointsBorder;
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
