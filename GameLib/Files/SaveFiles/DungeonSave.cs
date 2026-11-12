using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Files.SaveFiles
{
    public class DungeonSave
    {
        public string PathLevels { get; set; } = "Levels";
        public string PathFirstLevel { get; set; } = "Levels\\level_0.json";
        public int LevelCount { get; set; } = 1;
        public string[] Levels { get; set; } = [];
        public string PathRessources { get; set; } = "Ressources";
        public string[] RessourcesDoors { get; set; } = [];
        public string[] RessourcesEntities { get; set; } = [];
        public string[] RessourcesGrounds { get; set; } = [];
        public string[] RessourcesPlayer { get; set; } = [];
        public string[] RessourcesProps { get; set; } = [];
        public string[] RessourcesWalls { get; set; } = [];

        public DungeonSave()
        {

        }
    }
}
