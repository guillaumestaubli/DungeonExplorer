using System;
using System.Collections.Generic;
using System.Text;

namespace GameEditor.Windows.WorldEditor
{
    public class DisplaySettings
    {
        [DisplaySettingDisplayName("Show Tile Border")]
        public bool ShowTileBorder { get; set; } = false;


        [DisplaySettingDisplayName("Show Tile Index")]
        public bool ShowTileIndex { get; set; } = true;


        [DisplaySettingDisplayName("Show Tile Type")]
        public bool ShowTileType { get; set; } = false;


        [DisplaySettingDisplayName("Show Tile Texture")]
        public bool ShowTileTexture { get; set; } = true;


        [DisplaySettingDisplayName("Show Wall Hitbox")]
        public bool ShowWallHitbox { get; set; } = true;




        [DisplaySettingDisplayName("Show Props")]
        public bool ShowProps { get; set; } = true;


        [DisplaySettingDisplayName("Show Props Rectangle Hitbox")]
        public bool ShowPropsRectangleHitbox { get; set; } = true;


        [DisplaySettingDisplayName("Show Props Pixel Perfect Hitbox")]
        public bool ShowPropsPixelPerfectHitbox { get; set; } = true;




        [DisplaySettingDisplayName("Show Entities")]
        public bool ShowEntities { get; set; } = true;


        [DisplaySettingDisplayName("Show Entities Rectangle Hitbox")]
        public bool ShowEntitiesRectangleHitbox { get; set; } = true;


        [DisplaySettingDisplayName("Show Entities Pixel Perfect Hitbox")]
        public bool ShowEntitiesPixelPerfectHitbox { get; set; } = true;


        [DisplaySettingDisplayName("Show Entities Paths")]
        public bool ShowEntitiesPaths { get; set; } = false;




        [DisplaySettingDisplayName("Show Player")]
        public bool ShowPlayer { get; set; } = true;


        [DisplaySettingDisplayName("Show Player Rectangle Hitbox")]
        public bool ShowPlayerRectangleHitbox { get; set; } = true;


        [DisplaySettingDisplayName("Show Player Pixel Perfect Hitbox")]
        public bool ShowPlayerPixelPerfectHitbox { get; set; } = true;


        [DisplaySettingDisplayName("Show Player Spawn Point")]
        public bool ShowPlayerSpawnPoint { get; set; } = false;
    }





    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class DisplaySettingDisplayName : Attribute
    {
        public string DisplayName { get; }

        public DisplaySettingDisplayName(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
