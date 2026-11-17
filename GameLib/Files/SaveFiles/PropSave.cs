using GameLib.World.Props;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Files.SaveFiles
{
    public class PropSave
    {
        public string ID { get; set; } = "";
        public int GridIndex_X { get; set; } = 0;
        public int GridIndex_Y { get; set; } = 0;
        public float RenderSize_X {  get; set; } = 0;
        public float RenderSize_Y {  get; set; } = 0;
        public float RenderPosition_X { get; set; } = 0;
        public float RenderPosition_Y { get; set; } = 0;
        public string TextureName { get; set; } = "";

        public PropSave()
        {

        }

        public PropSave(Prop prop)
        {
            ID = prop.Id.ToString();
            GridIndex_X = prop.GridIndex.X;
            GridIndex_Y = prop.GridIndex.Y;
            RenderSize_X = prop.RenderSize.X;
            RenderSize_Y = prop.RenderSize.Y;
            RenderPosition_X = prop.RenderPosition.X;
            RenderPosition_Y = prop.RenderPosition.Y;
            TextureName = prop.Texture.Name;
        }
    }
}
