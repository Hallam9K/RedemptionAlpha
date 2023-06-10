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
        public static LocalizedText Status1 { get; private set; }
        public static LocalizedText Status2 { get; private set; }
        public static LocalizedText Status3 { get; private set; }
        public static LocalizedText Status4 { get; private set; }
        public static LocalizedText Status5 { get; private set; }
        public static LocalizedText Status6 { get; private set; }
        public static LocalizedText StatusEnd { get; private set; }
        public static LocalizedText Note1 { get; private set; }
        public static LocalizedText Note2 { get; private set; }
        public static LocalizedText Note3 { get; private set; }
        public static LocalizedText Note4 { get; private set; }
        public override void SetStaticDefaults()
        {
            Status1 = this.GetLocalization(nameof(Status1));
            Status2  = this.GetLocalization(nameof(Status2));
            Status3 = this.GetLocalization(nameof(Status3));
            Status4 = this.GetLocalization(nameof(Status4));
            Status5 = this.GetLocalization(nameof(Status5));
            Status6 = this.GetLocalization(nameof(Status6));
            StatusEnd = this.GetLocalization(nameof(StatusEnd));
            Note1 = this.GetLocalization(nameof(Note1));
            Note2 = this.GetLocalization(nameof(Note2));
            Note3 = this.GetLocalization(nameof(Note3));
            Note4 = this.GetLocalization(nameof(Note4));
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
