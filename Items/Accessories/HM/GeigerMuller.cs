using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Accessories.HM
{
    public class GeigerMuller : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Geiger-Muller");
            Tooltip.SetDefault("Lab issued Geiger counter. The louder it gets, the higher the chance of you getting irradiated.");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.value = Item.buyPrice(0, 20, 50, 0);
            Item.rare = ItemRarityID.Lime;
            Item.width = 34;
            Item.height = 28;
            Item.accessory = true;
        }
        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<MullerEffect>().effect = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MullerEffect>().effect = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            Radiation modPlayer = player.RedemptionRad();
            string rad = "No";
            string rad2 = "nothing to note.";
            switch (modPlayer.irradiatedLevel)
            {
                case 0:
                    rad = "No";
                    rad2 = "nothing to note.";
                    break;
                case 1:
                    rad = "Low";
                    rad2 = "nothing to note.";
                    break;
                case 2:
                    rad = "Medium";
                    rad2 = "have teochrome-issued pills on hand just in case.";
                    break;
                case 3:
                    rad = "High";
                    rad2 = "have teochrome-issued pills on hand just in case.";
                    break;
                case 4:
                    rad = "Very high";
                    rad2 = "high chance of irradiation and suffering ARS.";
                    break;
                case 5:
                    rad = "Extreme";
                    rad2 = "Acute Radiation Syndrome detected.";
                    break;
            }
            string text1 = rad + " doses of radiation detected on self, " + rad2;
            TooltipLine line = new(Mod, "text1", text1)
            {
                OverrideColor = Color.LimeGreen
            };
            tooltips.Insert(2, line);
        }
    }

    public class MullerEffect : ModPlayer
    {
        public bool effect;
        public override void ResetEffects()
        {
            effect = false;
        }
    }
}

