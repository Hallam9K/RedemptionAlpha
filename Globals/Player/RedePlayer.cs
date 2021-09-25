using System.Collections.Generic;
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
        public override void SaveData(TagCompound tag)
        {
            var saveS = new List<string>();
            if (foundHall) saveS.Add("foundHall");

            tag["saveS"] = saveS;
        }

        public override void LoadData(TagCompound tag)
        {
            var saveS = tag.GetList<string>("saveS");
            foundHall = saveS.Contains("foundHall");
        }
    }
}