using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib.Ressources
{
    public class TextureBase
    {
        private string _name;
        private string _path;

        private Texture _texture;
        private Image _image;

        #region GET_SET

        public string Name { get { return _name; } }
        public string Path { get { return _path; } }

        public Texture Texture { get { return _texture; } }
        public Image Image { get { return _image; } }

        #endregion

        public TextureBase(string path)
        {
            _path = path;

            // get the name of the file (ex: C:\Textures\file1.png => file1)
            _name = path.Split('\\').Last().Split('.').First();

            _image = new(path);

            _texture = new(_image)
            {
                Smooth = true,
                Repeated = true
            };
        }
    }
}
