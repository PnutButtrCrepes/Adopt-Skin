﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoptSkin
{
    class ModConfig
    {
        /// <summary>Whether or not to allow horses being ridden to fit through any area that the player can normally walk through</summary>
        public bool OneTileHorse { get; set; } = true;

        /// <summary>Whether or not the player can walk through pets on the farm</summary>
        public bool WalkThroughPets { get; set; } = false;

        public int StrayAdoptionPrice { get; set; } = 1000;

        public string HorseWhistleKey { get; set; } = "R";



        /// <summary>Determines whether wild adoptable horses can spawn in the map after the player obtains a stable</summary>
        public bool WildHorseSpawn { get; set; } = true;

        /// <summary>Determines whether stray pets will appear at Marnie's after the player obtains a pet</summary>
        public bool StraySpawn { get; set; } = true;




        /// <summary>The percentage chance that a WildHorse will spawn. 25% by default.</summary>
        public int WildHorseChancePercentage { get; set; } = 25;

        /// <summary>The percentage chance that a Stray will spawn. 60% by default.</summary>
        public int StrayChancePercentage { get; set; } = 60;




        /// <summary>Whether or not to tell the player when and where a WildHorse has spawned on the map. Will be FALSE by default</summary>
        public bool NotifyHorseSpawn { get; set; } = true;

        /// <summary>Whether or not to tell the player when a Stray has spawned at Marnie's. Will be FALSE by default</summary>
        public bool NotifyStraySpawn { get; set; } = true;




        /// <summary>Whether or not to allow debugging commands for Adopt & Skin. Will be FALSE by default</summary>
        public bool DebuggingMode { get; set; } = true;
    }
}
