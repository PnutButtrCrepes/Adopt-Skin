﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using xTile.Layers;
using xTile.Dimensions;
using xTile.Tiles;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Characters;
using StardewValley.Buildings;

namespace AdoptSkin.Framework
{
    class CreationHandler
    {
        /************************
        ** Fields
        *************************/

        /// <summary>Randomizer for logic within CreationHandler instances.</summary>
        private readonly Random Randomizer = new Random();

        /// <summary>Reference to Adopt & Skin's ModEntry. Used to access creature information and print information to the monitor when necessary.</summary>
        internal ModEntry Earth;

        /// <summary>Whether or not a potential pet is available for adoption at Marnie's currently</summary>
        internal bool CanAdopt;

        /// <summary>The pet available for adoption at Marnies for the current day.</summary>
        internal Stray StrayInfo = null;
        /// <summary>The wild horse available for adoption somewhere in the map. This variable is null if there is no current wild horse.</summary>
        internal WildHorse HorseInfo = null;






        /// <summary>The handler and creator for potential pets and wild horses to adopt</summary>
        /// <param name="modentry"></param>
        internal CreationHandler(ModEntry modentry)
        {
            Earth = modentry;
        }


        /// <summary>Resets variables that change each day</summary>
        internal void ProcessNewDay(object sender, DayStartedEventArgs e)
        {
            NewStray();
            if (ModEntry.Config.WildHorses)
                NewWildHorse();
        }


        /// <summary>Creates a new Stray at Marnie's. Used for new save files.</summary>
        internal void NewStray()
        {
            // If a Stray was around yesterday, remove it from the map
            if (StrayInfo != null)
                StrayInfo.RemoveFromWorld();

            StrayInfo = new Stray();
        }


        /// <summary>Checks the chance for a WildHorse to spawn if a stable exists on the farm, and spawns one if it should. Used for new save files.</summary>
        internal void NewWildHorse(bool overrideChance = false)
        {
            // If the previous day had a WildHorse, remove it from the map
            if (HorseInfo != null)
                HorseInfo.RemoveFromWorld();

            // Check chance for a WildHorse spawn, and spawn a new WildHorse if there should be one
            if (Randomizer.Next(0, 4) == 0 || overrideChance)
            {
                // Only spawn a wild horse if the player has a stable
                foreach (Building b in Game1.getFarm().buildings)
                    if (b is Stable || overrideChance)
                    {
                        HorseInfo = new WildHorse();
                        break;
                    }
            }
        }


        /// <summary>Returns true if a pet in available for adoption at Marnie's.</summary>
        public bool CanAdoptNow()
        {
            return CanAdopt ? true : false;
        }






        /*****************************
         ** P E T   A D O P T I O N **
         *****************************/

        /// <summary>Places the pet bed in Marnie's</summary>
        internal void PlaceBetBed()
        {
            GameLocation marnies = Game1.getLocationFromName("AnimalShop");
            TileSheet tileSheet = new xTile.Tiles.TileSheet("PetBed", marnies.map, Earth.Helper.Content.GetActualAssetKey("assets/petbed.png"), new xTile.Dimensions.Size(1, 1), new xTile.Dimensions.Size(16, 15));
            marnies.map.AddTileSheet(tileSheet);
            Layer buildingLayer = marnies.map.GetLayer("Buildings");
            buildingLayer.Tiles[17, 15] = new StaticTile(buildingLayer, tileSheet, BlendMode.Additive, 0);
        }


        
        internal void AdoptPet()
        {
            Game1.activeClickableMenu = new NamingMenu(PetNamer, "Name:");
        }
        


        /// <summary>Check to see if the player is attempting to interact with the stray</summary>
        internal void StrayCheck(object sender, ButtonPressedEventArgs e)
        {
            if (StrayInfo != null &&
                e.Button.Equals(SButton.MouseRight) &&
                StrayInfo.PetInstance.withinPlayerThreshold(3))
            {
                if ((int)e.Cursor.Tile.X >= StrayInfo.PetInstance.getLeftMostTileX().X && (int)e.Cursor.Tile.X <= StrayInfo.PetInstance.getRightMostTileX().X &&
                    (int)e.Cursor.Tile.Y >= StrayInfo.PetInstance.getTileY() - 1 && (int)e.Cursor.Tile.Y <= StrayInfo.PetInstance.getTileY() + 1)
                {

                    Game1.activeClickableMenu = new ConfirmationDialog("This is one of the strays that Marnie has taken in. \n\n" +
                        $"The animal is wary, but curious. Will you adopt this {ModEntry.Sanitize(StrayInfo.PetInstance.GetType().Name)}?", (who) =>
                    {
                        if (Game1.activeClickableMenu is StardewValley.Menus.ConfirmationDialog cd)
                            cd.cancel();

                        Game1.activeClickableMenu = new NamingMenu(PetNamer, $"What will you name it?");
                    }, (who) =>
                    {
                        // Exit the naming menu
                        Game1.drawObjectDialogue($"You leave the little one to rest for now. Marnie will take good care of it.");
                    });
                }
            }
        }


        /// <summary>Adopts and names the stray being interacted with. Called in the CheckStray event handler.</summary>
        internal void PetNamer(string petName)
        {
            // Name Pet and add to Adopt & Skin database
            StrayInfo.PetInstance.Name = petName;
            StrayInfo.PetInstance.displayName = petName;
            Earth.AddCreature(StrayInfo.PetInstance, StrayInfo.SkinID);

            // Warp the new Pet to the farmhouse
            StrayInfo.PetInstance.warpToFarmHouse(Game1.player);

            // Pet is no longer a Stray to keep track of
            StrayInfo = null;

            // Exit the naming menu
            Game1.drawObjectDialogue($"{petName} was brought home.");
        }






        /*********************************
         ** H O R S E   A D O P T I O N **
         *********************************/

        /// <summary>Check to see if the player is attempting to interact with the wild horse</summary>
        internal void HorseCheck(object sender, ButtonPressedEventArgs e)
        {
            if (HorseInfo != null && 
                e.Button.Equals(SButton.MouseRight) &&
                HorseInfo.HorseInstance.withinPlayerThreshold(3))
            {
                if ((int)e.Cursor.Tile.X >= HorseInfo.HorseInstance.getLeftMostTileX().X && (int)e.Cursor.Tile.X <= HorseInfo.HorseInstance.getRightMostTileX().X &&
                    (int)e.Cursor.Tile.Y >= HorseInfo.HorseInstance.getTileY() - 1 && (int)e.Cursor.Tile.Y <= HorseInfo.HorseInstance.getTileY() + 1)
                {

                    Game1.activeClickableMenu = new ConfirmationDialog("This appears to be an escaped horse from a neighboring town. \n\nIt looks tired, but friendly. Will you adopt this horse?", (who) =>
                    {
                        if (Game1.activeClickableMenu is StardewValley.Menus.ConfirmationDialog cd)
                            cd.cancel();

                        Game1.activeClickableMenu = new NamingMenu(HorseNamer, "What will you name this horse?");
                    }, (who) =>
                    {
                        // Exit the naming menu
                        Game1.drawObjectDialogue($"You leave the creature to rest for now. It's got a big, bright world ahead of it.");
                    });
                }
            }
        }


        /// <summary>Adopts and names the wild horse being interacted with. Called in the CheckHorse event handler.</summary>
        internal void HorseNamer(string horseName)
        {
            // Name Horse and add to Adopt & Skin database
            HorseInfo.HorseInstance.Name = horseName;
            HorseInfo.HorseInstance.displayName = horseName;
            Earth.AddCreature(HorseInfo.HorseInstance, HorseInfo.SkinID);

            // Horse is no longer a WildHorse to keep track of
            HorseInfo = null;

            // Exit the naming menu
            Game1.drawObjectDialogue($"Adopted {horseName}.");
        }
    }
}