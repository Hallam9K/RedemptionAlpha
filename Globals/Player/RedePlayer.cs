using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.NPCs.Critters;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals.Player
{
    public class RedePlayer : ModPlayer
    {
        public int spiritLevel = 0;
        public int maxSpiritLevel = 3;
        public bool foundHall;

        public override void ResetEffects()
        {
        }
        public override void Initialize()
        {
            foundHall = false;
        }
        public override TagCompound Save()
        {
            var saveS = new List<string>();
            if (foundHall) saveS.Add("foundHall");

            return new TagCompound
            {
                { "saveS", saveS }
            };
        }

        public override void Load(TagCompound tag)
        {
            var saveS = tag.GetList<string>("saveS");
            foundHall = saveS.Contains("foundHall");
        }
    }
}