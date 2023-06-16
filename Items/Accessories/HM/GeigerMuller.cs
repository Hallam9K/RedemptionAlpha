using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.Localization;

namespace Redemption.Items.Accessories.HM
{
    public class GeigerMuller : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Geiger-Muller");
            // Tooltip.SetDefault("Lab issued Geiger counter. The louder it gets, the higher the chance of you getting irradiated.");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.value = Item.buyPrice(0, 20, 50, 0);
            Item.rare = ItemRarityID.LightRed;
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
            string rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status1");
            string rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note1");
            switch (modPlayer.irradiatedLevel)
            {
                case 0:
                    rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status1");
                    rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note1");
                    break;
                case 1:
                    rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status2");
                    rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note1");
                    break;
                case 2:
                    rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status3");
                    rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note2");
                    break;
                case 3:
                    rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status4");
                    rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note2");
                    break;
                case 4:
                    rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status5");
                    rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note3");
                    break;
                case 5:
                    rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status6");
                    rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note4");
                    break;
            }
            string text1 = rad + Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.StatusEnd") + rad2;
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
