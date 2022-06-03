using Redemption.Biomes;
using Redemption.Items.Donator.Lizzy;
using Redemption.Items.Donator.Uncon;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Furniture.Lab;

namespace Redemption.Globals.Player
{
    public class AbilityPlayer : ModPlayer
    {
        public bool Spiritwalker;
        public override void Initialize()
        {
            Spiritwalker = false;
        }
        public override void SaveData(TagCompound tag)
        {
            var abilityS = new List<string>();
            if (Spiritwalker) abilityS.Add("Spiritwalker");

            tag["abilityS"] = abilityS;
        }
        public override void LoadData(TagCompound tag)
        {
            var abilityS = tag.GetList<string>("abilityS");
            Spiritwalker = abilityS.Contains("Spiritwalker");
        }
    }
}
